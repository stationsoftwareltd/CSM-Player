Imports System.Runtime.InteropServices
Imports DevExpress.XtraTreeList.Nodes
Imports HikVisionWrapper
Imports MySqlConnector

Public Class frmConfig
    Private m_lUserID As Integer = -1
    ' Tracks the live preview stream on the config screen
    Private m_lRealHandlePreview As Integer = -1
    Public Property DBConnectionString As String

    ' --- 1. LOAD THE DATA ON STARTUP ---
    Private Sub frmConfig_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text &= $" - v{My.Application.Info.Version.ToString()}"
        Try
            Using conn As New MySqlConnection(DBConnectionString)
                conn.Open()

                Dim query As String = "SELECT nvr_ip, nvr_port, nvr_user, nvr_password, cctv_width, cctv_height FROM config_user WHERE pk = 1"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Populate the textboxes with the existing data
                            tbNVRIP.Text = reader("nvr_ip").ToString()
                            tbPort.Text = reader("nvr_port").ToString()
                            tbUsername.Text = reader("nvr_user").ToString()
                            tbPassword.Text = reader("nvr_password").ToString()

                            If Not IsDBNull(reader("cctv_width")) Then tbWidth.Text = reader("cctv_width").ToString()
                            If Not IsDBNull(reader("cctv_height")) Then tbHeight.Text = reader("cctv_height").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Failed to load existing config: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' --- 2. TEST THE NVR LOGIN ---
    Private Sub butLogin_Click(sender As Object, e As EventArgs) Handles butLogin.Click
        ' If we are already logged in from a previous click, log out first
        If m_lUserID >= 0 Then
            CHCNetSDK.NET_DVR_Logout(m_lUserID)
            m_lUserID = -1
        End If

        ' Clear the TreeList so it starts fresh
        TreeListCameras.ClearNodes()

        ' Grab the details from the textboxes
        Dim ip As String = tbNVRIP.Text
        Dim port As Short
        If Not Short.TryParse(tbPort.Text, port) Then
            MessageBox.Show("Please enter a valid Port Number.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim user As String = tbUsername.Text
        Dim pass As String = tbPassword.Text

        ' Attempt the login
        Dim deviceInfo As New CHCNetSDK.NET_DVR_DEVICEINFO_V30()
        m_lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, user, pass, deviceInfo)

        If m_lUserID < 0 Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Login failed! Error Code: {errorCode}", "Test Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            LoadCameras(deviceInfo)
        End If
    End Sub

    ' --- 3. SAVE THE CONFIGURATION ---
    Private Sub butSave_Click(sender As Object, e As EventArgs) Handles butSave.Click
        Try
            Using conn As New MySqlConnection(DBConnectionString)
                conn.Open()

                ' We use parameterized queries to prevent SQL injection and parsing errors
                Dim query As String = "UPDATE config_user SET nvr_ip=@ip, nvr_port=@port, nvr_user=@user, nvr_password=@pass, cctv_width=@width, cctv_height=@height WHERE pk = 1"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ip", tbNVRIP.Text)
                    cmd.Parameters.AddWithValue("@port", tbPort.Text)
                    cmd.Parameters.AddWithValue("@user", tbUsername.Text)
                    cmd.Parameters.AddWithValue("@pass", tbPassword.Text)

                    ' Safely parse the dimensions (fallback to 800x600 if they typed letters)
                    Dim width As Integer = 800
                    Integer.TryParse(tbWidth.Text, width)
                    cmd.Parameters.AddWithValue("@width", width)

                    Dim height As Integer = 600
                    Integer.TryParse(tbHeight.Text, height)
                    cmd.Parameters.AddWithValue("@height", height)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Configuration saved successfully to the database!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Failed to save config: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadCameras(ByRef deviceInfo As CHCNetSDK.NET_DVR_DEVICEINFO_V30)
        ' --- 1. LOAD DESCRIPTIONS FROM DATABASE ---
        Dim cameraDescriptions As New Dictionary(Of Integer, String)()
        Try
            Using conn As New MySqlConnection(DBConnectionString)
                conn.Open()
                Dim query As String = "SELECT physical_id, description FROM config_camera"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim id As Integer = Convert.ToInt32(reader("physical_id"))
                            Dim desc As String = reader("description").ToString()
                            cameraDescriptions(id) = desc
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Could not load descriptions: " & ex.Message)
        End Try

        ' --- NEW: 2. LOAD GATE ASSIGNMENTS FROM DATABASE ---
        Dim g1in As Integer = 0
        Dim g1out As Integer = 0
        Dim g2in As Integer = 0
        Dim g2out As Integer = 0

        Try
            Using conn As New MySqlConnection(DBConnectionString)
                conn.Open()
                ' Pull the saved channels from the config_net2 table
                Dim queryConfig As String = "SELECT gate1in, gate1out, gate2in, gate2out FROM config_net2 LIMIT 1"
                Using cmd As New MySqlCommand(queryConfig, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            If Not IsDBNull(reader("gate1in")) Then g1in = Convert.ToInt32(reader("gate1in"))
                            If Not IsDBNull(reader("gate1out")) Then g1out = Convert.ToInt32(reader("gate1out"))
                            If Not IsDBNull(reader("gate2in")) Then g2in = Convert.ToInt32(reader("gate2in"))
                            If Not IsDBNull(reader("gate2out")) Then g2out = Convert.ToInt32(reader("gate2out"))
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Could not load net2 gate assignments: " & ex.Message)
        End Try

        ' 3. Set up the Columns
        TreeListCameras.Columns.Clear()

        Dim colName = TreeListCameras.Columns.Add()
        colName.Caption = "Camera Name"
        colName.VisibleIndex = 0
        colName.OptionsColumn.AllowEdit = False

        Dim colChannel = TreeListCameras.Columns.Add()
        colChannel.Caption = "Channel ID"
        colChannel.VisibleIndex = 1
        colChannel.Visible = False

        Dim colDesc = TreeListCameras.Columns.Add()
        colDesc.Caption = "Description"
        colDesc.VisibleIndex = 2
        colDesc.OptionsColumn.AllowEdit = True

        ' --- NEW: 4. SETUP THE GATE DROPDOWN COLUMN ---
        Dim colGate = TreeListCameras.Columns.Add()
        colGate.Caption = "Gate Assignment"
        colGate.VisibleIndex = 3
        colGate.Visible = True
        colGate.OptionsColumn.AllowEdit = True

        ' Create the combo box for the column
        Dim repoGateCombo As New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        repoGateCombo.Items.AddRange({"Not assigned", "Gate 1 (IN)", "Gate 1 (OUT)", "Gate 2 (IN)", "Gate 2 (OUT)"})
        repoGateCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        TreeListCameras.RepositoryItems.Add(repoGateCombo)
        colGate.ColumnEdit = repoGateCombo

        ' 5. Clear old data and initialize the root node (Note the 4 elements now!)
        TreeListCameras.ClearNodes()
        Dim rootNodeData As Object() = {"Hikvision NVR", "Root", "", ""}

        ' Keeping the strict casting for the root node as required
        Dim rootNode = TreeListCameras.AppendNode(rootNodeData, CType(Nothing, TreeListNode))

        ' 6. Query the NVR for the active IP channels
        Dim ipParaCfg As New CHCNetSDK.NET_DVR_IPPARACFG_V40()
        Dim size As Integer = Marshal.SizeOf(ipParaCfg)
        Dim ptrIpParaCfg As IntPtr = Marshal.AllocHGlobal(size)
        Marshal.StructureToPtr(ipParaCfg, ptrIpParaCfg, False)
        Dim bytesReturned As UInteger = 0

        Dim success As Boolean = CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, 1062, 0, ptrIpParaCfg, CType(size, UInteger), bytesReturned)

        If success Then
            ipParaCfg = CType(Marshal.PtrToStructure(ptrIpParaCfg, GetType(CHCNetSDK.NET_DVR_IPPARACFG_V40)), CHCNetSDK.NET_DVR_IPPARACFG_V40)

            Dim startDChan As Integer = deviceInfo.byStartDChan
            If startDChan < 33 Then startDChan = 33
            Dim totalIpChannels As Integer = deviceInfo.byIPChanNum + (CInt(deviceInfo.byHighDChanNum) * 256)

            ' 7. Loop through the slots and filter for active cameras
            For i As Integer = 0 To totalIpChannels - 1
                If i < ipParaCfg.struIPDevInfo.Length Then
                    If ipParaCfg.struIPDevInfo(i).byEnable = 1 Then
                        Dim channelNum As Integer = startDChan + i

                        ' Get Online Status
                        Dim isOnline As Boolean = False
                        Try
                            isOnline = (ipParaCfg.struStreamMode(i).uGetStream.byUnion(0) = 1)
                        Catch ex As Exception
                            isOnline = True
                        End Try

                        Dim cameraName As String = If(isOnline, $"IP Camera {channelNum}", $"IP Camera {channelNum} (Offline)")

                        ' Look up the Description
                        Dim currentDesc As String = ""
                        If cameraDescriptions.ContainsKey(channelNum) Then
                            currentDesc = cameraDescriptions(channelNum)
                        End If

                        ' --- NEW: Look up the Gate Assignment ---
                        Dim gateStatus As String = "Not assigned"
                        If channelNum = g1in Then gateStatus = "Gate 1 (IN)"
                        If channelNum = g1out Then gateStatus = "Gate 1 (OUT)"
                        If channelNum = g2in Then gateStatus = "Gate 2 (IN)"
                        If channelNum = g2out Then gateStatus = "Gate 2 (OUT)"

                        ' Add all FOUR columns to the TreeList
                        Dim camNode = TreeListCameras.AppendNode(New Object() {cameraName, channelNum, currentDesc, gateStatus}, rootNode)
                        camNode.Tag = isOnline
                    End If
                End If
            Next
        End If

        Marshal.FreeHGlobal(ptrIpParaCfg)
        TreeListCameras.ExpandAll()
        TreeListCameras.BestFitColumns()
    End Sub

    Private Sub StopLivePreview()
        If m_lRealHandlePreview >= 0 Then
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandlePreview)
            m_lRealHandlePreview = -1
        End If

        ' Force the picture box to clear the old frozen frame
        pictureBoxPreview.Invalidate()
    End Sub

    Private Sub StartLivePreview(channelId As Integer)
        StopLivePreview()

        Dim previewInfo As New CHCNetSDK.NET_DVR_PREVIEWINFO()
        previewInfo.lChannel = channelId
        previewInfo.dwStreamType = 1 ' 1 = Sub Stream (Faster, lower resolution preview)
        previewInfo.dwLinkMode = 0   ' 0 = TCP
        previewInfo.bBlocked = True
        previewInfo.hPlayWnd = pictureBoxPreview.Handle

        m_lRealHandlePreview = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, previewInfo, Nothing, IntPtr.Zero)

        If m_lRealHandlePreview < 0 Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            Console.WriteLine($"[PREVIEW ERROR] Failed to start stream. Code: {errorCode}")
        End If
    End Sub

    Private Sub TreeListCameras_CellValueChanged(sender As Object, e As DevExpress.XtraTreeList.CellValueChangedEventArgs) Handles TreeListCameras.CellValueChanged
        If e.Node.ParentNode Is Nothing Then Return ' Ignore the root node

        Dim channelID As Integer = Convert.ToInt32(e.Node.GetValue(1))

        ' --- HANDLE DESCRIPTION CHANGES ---
        If e.Column.Caption = "Description" Then
            Dim newDescription As String = e.Value.ToString()
            Try
                Using conn As New MySqlConnection(DBConnectionString)
                    conn.Open()
                    Dim checkQuery As String = "SELECT COUNT(*) FROM config_camera WHERE physical_id = @id"
                    Dim exists As Boolean = False

                    Using checkCmd As New MySqlCommand(checkQuery, conn)
                        checkCmd.Parameters.AddWithValue("@id", channelID)
                        exists = (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    End Using

                    Dim saveQuery As String = If(exists,
                        "UPDATE config_camera SET description = @desc WHERE physical_id = @id",
                        "INSERT INTO config_camera (physical_id, description) VALUES (@id, @desc)")

                    Using saveCmd As New MySqlCommand(saveQuery, conn)
                        saveCmd.Parameters.AddWithValue("@id", channelID)
                        saveCmd.Parameters.AddWithValue("@desc", newDescription)
                        saveCmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show($"Failed to save description: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' --- NEW: HANDLE GATE ASSIGNMENT CHANGES ---
        ElseIf e.Column.Caption = "Gate Assignment" Then
            Dim selectedGate As String = e.Value.ToString()

            Try
                Using conn As New MySqlConnection(DBConnectionString)
                    conn.Open()

                    ' 1. Wipe previous assignments for this specific camera
                    ' This ensures a camera isn't accidentally assigned to multiple gates at once
                    Dim sqlClear As String = "UPDATE config_net2 SET gate1in = 0 WHERE gate1in = @id; " &
                                             "UPDATE config_net2 SET gate1out = 0 WHERE gate1out = @id; " &
                                             "UPDATE config_net2 SET gate2in = 0 WHERE gate2in = @id; " &
                                             "UPDATE config_net2 SET gate2out = 0 WHERE gate2out = @id;"
                    Using cmdClear As New MySqlCommand(sqlClear, conn)
                        cmdClear.Parameters.AddWithValue("@id", channelID)
                        cmdClear.ExecuteNonQuery()
                    End Using

                    ' 2. Save the new assignment
                    If selectedGate <> "Not assigned" Then
                        Dim targetField As String = ""
                        Select Case selectedGate
                            Case "Gate 1 (IN)" : targetField = "gate1in"
                            Case "Gate 1 (OUT)" : targetField = "gate1out"
                            Case "Gate 2 (IN)" : targetField = "gate2in"
                            Case "Gate 2 (OUT)" : targetField = "gate2out"
                        End Select

                        If targetField <> "" Then
                            ' Update the single config_net2 record
                            Dim sqlUpdate As String = $"UPDATE config_net2 SET {targetField} = @id"
                            Using cmdUpdate As New MySqlCommand(sqlUpdate, conn)
                                cmdUpdate.Parameters.AddWithValue("@id", channelID)
                                cmdUpdate.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show($"Failed to save gate assignment: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub TreeListCameras_NodeCellStyle(sender As Object, e As DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs) Handles TreeListCameras.NodeCellStyle
        ' Only apply colors to the actual camera nodes, not the main root folder
        If e.Node IsNot Nothing AndAlso e.Node.ParentNode IsNot Nothing Then

            ' Read the status we tucked away in the Tag
            If e.Node.Tag IsNot Nothing Then
                Dim isOnline As Boolean = CBool(e.Node.Tag)

                If isOnline Then
                    ' Online = Soft Green background with standard text
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(200, 255, 200)
                    e.Appearance.ForeColor = System.Drawing.Color.Black
                Else
                    ' Offline = White background with grayed-out text
                    e.Appearance.BackColor = System.Drawing.Color.White
                    e.Appearance.ForeColor = System.Drawing.Color.Gray
                End If
            End If

        End If
    End Sub

    Private Sub TreeListCameras_FocusedNodeChanged(sender As Object, e As DevExpress.XtraTreeList.FocusedNodeChangedEventArgs) Handles TreeListCameras.FocusedNodeChanged
        ' Ignore clicks on the main "Hikvision NVR" root folder
        If e.Node Is Nothing OrElse e.Node.ParentNode Is Nothing Then
            StopLivePreview()
            Return
        End If

        ' Only attempt to play if our Tag confirms the camera is online
        If e.Node.Tag IsNot Nothing AndAlso CBool(e.Node.Tag) = True Then

            Dim channelID As Integer
            If Integer.TryParse(e.Node.GetValue(1).ToString(), channelID) Then
                StartLivePreview(channelID)
            End If

        Else
            ' Camera is offline, ensure the preview box is stopped and cleared
            StopLivePreview()
        End If
    End Sub

    Private Sub frmConfig_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        StopLivePreview()

        If m_lUserID >= 0 Then
            CHCNetSDK.NET_DVR_Logout(m_lUserID)
            m_lUserID = -1
        End If
    End Sub
End Class