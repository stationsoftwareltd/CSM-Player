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
            MessageBox.Show("Login successful! Loading cameras into the list...", "Test Login", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Trigger your existing TreeList method to prove the connection works
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
        ' --- NEW: LOAD DESCRIPTIONS FROM DATABASE ---
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

        ' 1. Set up the Columns
        TreeListCameras.Columns.Clear()

        Dim colName = TreeListCameras.Columns.Add()
        colName.Caption = "Camera Name"
        colName.VisibleIndex = 0
        colName.OptionsColumn.AllowEdit = False

        Dim colChannel = TreeListCameras.Columns.Add()
        colChannel.Caption = "Channel ID"
        colChannel.VisibleIndex = 1
        colChannel.Visible = False

        ' --- NEW: DESCRIPTION COLUMN ---
        Dim colDesc = TreeListCameras.Columns.Add()
        colDesc.Caption = "Description"
        colDesc.VisibleIndex = 2
        colDesc.OptionsColumn.AllowEdit = True ' This is the only editable column!

        ' 2. Clear old data and initialize the root node
        TreeListCameras.ClearNodes()
        Dim rootNodeData As Object() = {"Hikvision NVR", "Root", ""}

        ' Using the strict casting for the root node as previously noted
        Dim rootNode = TreeListCameras.AppendNode(rootNodeData, CType(Nothing, TreeListNode))

        ' ... (Keep your IP network query logic exactly the same) ...
        ' 4. Query the NVR for the active IP channels
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

            ' 5. Loop through the slots and filter for active cameras
            For i As Integer = 0 To totalIpChannels - 1
                If i < ipParaCfg.struIPDevInfo.Length Then

                    ' This means a camera is configured in this slot
                    If ipParaCfg.struIPDevInfo(i).byEnable = 1 Then
                        Dim channelNum As Integer = startDChan + i

                        ' --- GET THE REAL-TIME NETWORK STATUS ---
                        Dim isOnline As Boolean = False
                        Try
                            isOnline = (ipParaCfg.struStreamMode(i).uGetStream.byUnion(0) = 1)
                        Catch ex As Exception
                            isOnline = True
                        End Try

                        Dim cameraName As String = If(isOnline, $"IP Camera {channelNum}", $"IP Camera {channelNum} (Offline)")

                        ' --- NEW: LOOK UP THE DESCRIPTION ---
                        Dim currentDesc As String = ""
                        If cameraDescriptions.ContainsKey(channelNum) Then
                            currentDesc = cameraDescriptions(channelNum)
                        End If

                        ' Add all three columns to the TreeList (Name, Channel, Description)
                        Dim camNode = TreeListCameras.AppendNode(New Object() {cameraName, channelNum, currentDesc}, rootNode)

                        camNode.Tag = isOnline
                    End If

                End If
            Next
        End If

        Marshal.FreeHGlobal(ptrIpParaCfg)
        TreeListCameras.ExpandAll()
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
        ' We only care if they edited the Description column, and not the main folder node
        If e.Column.Caption = "Description" AndAlso e.Node.ParentNode IsNot Nothing Then

            Dim channelID As Integer = Convert.ToInt32(e.Node.GetValue(1))
            Dim newDescription As String = e.Value.ToString()

            Try
                Using conn As New MySqlConnection(DBConnectionString)
                    conn.Open()

                    ' We check if the row exists first. 
                    ' (If physical_id is a Primary Key, you could use ON DUPLICATE KEY UPDATE instead)
                    Dim checkQuery As String = "SELECT COUNT(*) FROM config_camera WHERE physical_id = @id"
                    Dim exists As Boolean = False

                    Using checkCmd As New MySqlCommand(checkQuery, conn)
                        checkCmd.Parameters.AddWithValue("@id", channelID)
                        exists = (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    End Using

                    Dim saveQuery As String
                    If exists Then
                        saveQuery = "UPDATE config_camera SET description = @desc WHERE physical_id = @id"
                    Else
                        saveQuery = "INSERT INTO config_camera (physical_id, description) VALUES (@id, @desc)"
                    End If

                    Using saveCmd As New MySqlCommand(saveQuery, conn)
                        saveCmd.Parameters.AddWithValue("@id", channelID)
                        saveCmd.Parameters.AddWithValue("@desc", newDescription)
                        saveCmd.ExecuteNonQuery()
                    End Using
                End Using

            Catch ex As Exception
                MessageBox.Show($"Failed to save description to database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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