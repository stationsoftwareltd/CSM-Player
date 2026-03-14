Imports DevExpress.XtraTreeList.Nodes
Imports HikVisionWrapper
Imports System.Runtime.InteropServices
Imports MySqlConnector

Public Class frmMain
    ' This variable stores your active session ID. 
    ' A value of -1 means you are not logged in.
    Private m_lUserID As Integer = -1
    Private m_lRealHandle As Integer = -1
    Private m_lPlayHandle As Integer = -1
    ' Tracks whether the playback video is currently paused
    Private m_isPaused As Boolean = False
    ' Tracks the playback speed multiplier. 0 = 1x, positive = Fast, negative = Slow.
    Private m_speedIndex As Integer = 0
    ' Tracks if the video is playing in reverse to prevent speed-up stalls
    Private m_isReverse As Boolean = False
    ' Duration of the playback clip in minutes (defaults to 1)
    Private m_lengthMinutes As Integer = 1
    ' Prevents the timer from fighting the user when dragging the slider
    Private m_isDragging As Boolean = False
    ' Prevents the timer from rubber-banding the slider while the network buffers a seek
    Private m_ignoreTimerUntil As DateTime = DateTime.MinValue
    ' Tracks the active download stream
    Private m_lDownloadHandle As Integer = -1
    ' Class-level variables to hold our parsed data
    Private m_dbServer As String = "localhost"
    Private m_dbName As String = "csm" ' Set a default if you wish
    Private m_dbPort As String = "3306"
    Private m_dbUser As String = "user"
    Private m_dbPass As String = ""
    Private m_channelId As String = ""
    Private m_timestamp As String = ""
    Private m_isConfigMode As Boolean = False
    Private m_x As Integer = -1
    Private m_y As Integer = -1


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1. Parse Command Line Flags
        Dim args As String() = Environment.GetCommandLineArgs()

        ' Start loop at index 1 because args(0) is always the executable path
        For i As Integer = 1 To args.Length - 1
            ' Add .Trim() to protect against accidental double-spaces in the arguments
            Dim arg As String = args(i).Trim()
            Dim lowerArg As String = arg.ToLower()

            If lowerArg = "/config" Then
                m_isConfigMode = True
            ElseIf lowerArg.StartsWith("/dbserver=") Then
                m_dbServer = arg.Substring(10)
            ElseIf lowerArg.StartsWith("/dbport=") Then
                m_dbPort = arg.Substring(8)
            ElseIf lowerArg.StartsWith("/dbuser=") Then
                m_dbUser = arg.Substring(8)
            ElseIf lowerArg.StartsWith("/dbpass=") Then
                m_dbPass = arg.Substring(8)
            ElseIf lowerArg.StartsWith("/dbname=") Then
                m_dbName = arg.Substring(8)
            ElseIf lowerArg.StartsWith("/channel=") Then
                m_channelId = arg.Substring(9)
            ElseIf lowerArg.StartsWith("/time=") Then
                m_timestamp = arg.Substring(6)
            ElseIf lowerArg.StartsWith("/length=") Then
                Dim lengthStr As String = arg.Substring(8)
                If Not Integer.TryParse(lengthStr, m_lengthMinutes) Then
                    m_lengthMinutes = 1 ' Fallback to 1 minute if they pass invalid text
                End If
            ElseIf lowerArg.StartsWith("/x=") Then
                Integer.TryParse(arg.Substring(3), m_x)
            ElseIf lowerArg.StartsWith("/y=") Then
                Integer.TryParse(arg.Substring(3), m_y)
            End If
        Next

        Dim connString As String = $"Server={m_dbServer};Port={m_dbPort};Database={m_dbName};Uid={m_dbUser};Pwd={m_dbPass};"

        ' 2. Initialize the SDK
        Dim initSuccess As Boolean = CHCNetSDK.NET_DVR_Init()
        If Not initSuccess Then
            MessageBox.Show("Hikvision SDK Initialization Failed! Check DLLs.")
            Return
        End If

        ' 3. Handle Configuration Mode
        If m_isConfigMode Then
            Dim configScreen As New frmConfig()
            configScreen.DBConnectionString = connString
            configScreen.ShowDialog()

            Me.Close()
            Return
        End If

        ' 4. Validate Required Arguments for Viewer Mode
        If String.IsNullOrEmpty(m_channelId) Then
            MessageBox.Show("Missing required flag (/channel) for viewer mode.", "CSM-Playback Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
            Return
        End If

        ' 5. Build Connection String & Fetch NVR Details (FIXED DATABASE VARIABLE HERE)
        Dim nvrData = GetNvrCredentials(connString)

        If Not nvrData.Found Then
            MessageBox.Show($"Could not find NVR data in the database.")
            Return
        End If

        ' 6. Perform the Login
        PerformLogin(nvrData.IPAddress, nvrData.Port, nvrData.Username, nvrData.Password)

        ' 7. Determine Live View vs Playback
        If String.IsNullOrEmpty(m_timestamp) OrElse m_timestamp = "00000000000000" Then
            StartLiveView(Convert.ToInt32(m_channelId))
        Else
            StartPlaybackView(Convert.ToInt32(m_channelId), m_timestamp)
        End If

        ' 8. Configure Viewer UI 
        Dim finalWidth As Integer = If(nvrData.Width > 0, nvrData.Width, 800)
        Dim finalHeight As Integer = If(nvrData.Height > 0, nvrData.Height, 600)

        Me.ClientSize = New Drawing.Size(finalWidth, finalHeight)

        ' 9. Position the window
        If m_x >= 0 AndAlso m_y >= 0 Then
            ' Explicit coordinates provided (e.g., they clicked a specific icon on the map)
            Me.StartPosition = FormStartPosition.Manual
            Me.Location = New Point(m_x, m_y)
        Else
            ' No coordinates provided (e.g., launched from an incident log or menu)
            Me.StartPosition = FormStartPosition.CenterScreen
        End If

        ' Expand the PictureBox to fill the remaining space
        PictureBox1.Dock = DockStyle.Fill
        PictureBox1.BringToFront()
    End Sub


    Private Sub StartLiveView(channelId As Integer)
        StopAllStreams()

        Dim previewInfo As New CHCNetSDK.NET_DVR_PREVIEWINFO()
        previewInfo.lChannel = channelId
        previewInfo.dwStreamType = 0 ' 0 = Main Stream, 1 = Sub Stream
        previewInfo.dwLinkMode = 0   ' 0 = TCP
        previewInfo.bBlocked = True
        previewInfo.hPlayWnd = PictureBox1.Handle

        m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, previewInfo, Nothing, IntPtr.Zero)

        If m_lRealHandle < 0 Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Failed to start Live View on channel {channelId}. Error Code: {errorCode}")
        End If
    End Sub

    Private Sub StartPlaybackView(channelId As Integer, timestamp As String)
        ' Validate the timestamp format
        If timestamp.Length <> 14 Then
            MessageBox.Show("Invalid timestamp format. Expected 14 digits (YYYYMMDDHHmmss).")
            Return
        End If

        StopAllStreams()

        ' 1. Parse the 14-digit string into a .NET DateTime object
        Dim startDateTime As DateTime = DateTime.ParseExact(m_timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)

        ' 2. Calculate the exact End Time by adding the length in minutes
        Dim endDateTime As DateTime = startDateTime.AddMinutes(m_lengthMinutes)

        ' 3. Populate the Hikvision Start Time Structure
        Dim startTime As New CHCNetSDK.NET_DVR_TIME()
        startTime.dwYear = startDateTime.Year
        startTime.dwMonth = startDateTime.Month
        startTime.dwDay = startDateTime.Day
        startTime.dwHour = startDateTime.Hour
        startTime.dwMinute = startDateTime.Minute
        startTime.dwSecond = startDateTime.Second

        ' 4. Populate the Hikvision End Time Structure
        Dim endTime As New CHCNetSDK.NET_DVR_TIME()
        endTime.dwYear = endDateTime.Year
        endTime.dwMonth = endDateTime.Month
        endTime.dwDay = endDateTime.Day
        endTime.dwHour = endDateTime.Hour
        endTime.dwMinute = endDateTime.Minute
        endTime.dwSecond = endDateTime.Second

        ' 5. Request the specific slice of video
        m_lPlayHandle = CHCNetSDK.NET_DVR_PlayBackByTime(m_lUserID, channelId, startTime, endTime, PictureBox1.Handle)

        If m_lPlayHandle < 0 Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Failed to find playback video. Error Code: {errorCode}")
            Return
        End If

        ' 5. Crucial Step: Start the Video Flow
        ' NET_DVR_PlayBackByTime_V40 still only queues the video. You must send a "Play" command (Command 1).
        Dim PLAY_COMMAND As UInteger = 1
        If Not CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_COMMAND, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Failed to begin playback frames. Error Code: {errorCode}")
        End If

        ' Reset the pause button state for the new video stream
        m_isPaused = False
        butPause.ImageOptions.SvgImage = My.Resources.pause
        ' Force the timer settings through code to guarantee it runs
        timerPlayback.Interval = 1000
        timerPlayback.Enabled = True
        timerPlayback.Start()
    End Sub

    Private Sub StopAllStreams()
        ' Stop Live View if it's running
        If m_lRealHandle >= 0 Then
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle)
            m_lRealHandle = -1
        End If

        ' Stop Playback if it's running
        If m_lPlayHandle >= 0 Then
            CHCNetSDK.NET_DVR_StopPlayBack(m_lPlayHandle)
            m_lPlayHandle = -1
        End If

        ' Stop downloading if it's running
        If m_lDownloadHandle >= 0 Then
            CHCNetSDK.NET_DVR_StopGetFile(m_lDownloadHandle)
            m_lDownloadHandle = -1
        End If

        ' Force the picture box to clear its old frame
        PictureBox1.Invalidate()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Stop the video stream first!
        If m_lRealHandle >= 0 Then
            CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle)
            m_lRealHandle = -1
        End If

        ' Log out of the NVR
        If m_lUserID >= 0 Then
            CHCNetSDK.NET_DVR_Logout(m_lUserID)
            m_lUserID = -1
        End If

        StopAllStreams()

        ' Completely shut down the SDK
        CHCNetSDK.NET_DVR_Cleanup()
    End Sub

    Private Structure NvrDetails
        Public Found As Boolean
        Public IPAddress As String
        Public Port As Short
        Public Username As String
        Public Password As String
        Public Width As Integer   ' New!
        Public Height As Integer  ' New!
    End Structure

    Private Function GetNvrCredentials(connString As String) As NvrDetails
        Dim details As New NvrDetails()
        ' Set a fallback default just in case the DB has 0 or NULL
        details.Width = 800
        details.Height = 600

        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim query As String = "SELECT nvr_ip, nvr_port, nvr_user, nvr_password, cctv_width, cctv_height FROM config_user WHERE pk = 1"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            details.IPAddress = reader("nvr_ip").ToString()
                            details.Port = Convert.ToInt16(reader("nvr_port"))
                            details.Username = reader("nvr_user").ToString()
                            details.Password = reader("nvr_password").ToString()
                            ' Safely parse the width and height
                            If Not IsDBNull(reader("cctv_width")) Then
                                details.Width = Convert.ToInt32(reader("cctv_width"))
                            End If

                            If Not IsDBNull(reader("cctv_height")) Then
                                details.Height = Convert.ToInt32(reader("cctv_height"))
                            End If
                            details.Found = True
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Database Error: {ex.Message}")
        End Try

        Return details
    End Function

    Private Sub PerformLogin(ip As String, port As Short, user As String, pass As String)
        Dim deviceInfo As New CHCNetSDK.NET_DVR_DEVICEINFO_V30()

        m_lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, user, pass, deviceInfo)

        If m_lUserID < 0 Then
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Login failed for {ip}. Error Code: {errorCode}")
        End If
    End Sub

    Private Sub butSnapshot_Click(sender As Object, e As EventArgs) Handles butSnapshot.Click
        ' 1. Ensure the destination directory exists
        Dim targetFolder As String = "C:\CSM\CCTV-Snapshots"
        If Not System.IO.Directory.Exists(targetFolder) Then
            System.IO.Directory.CreateDirectory(targetFolder)
        End If

        ' 2. Generate the filename (yyyymmddhhmmss)
        Dim timestamp As String = DateTime.Now.ToString("yyyyMMddHHmmss")
        Dim filePath As String = System.IO.Path.Combine(targetFolder, $"{timestamp}.bmp")

        Dim success As Boolean = False

        ' 3. Take the snapshot based on which view is currently active
        If m_lRealHandle >= 0 Then
            ' We are in Live View
            success = CHCNetSDK.NET_DVR_CapturePicture(m_lRealHandle, filePath)
        ElseIf m_lPlayHandle >= 0 Then
            ' We are in Playback View
            success = CHCNetSDK.NET_DVR_PlayBackCaptureFile(m_lPlayHandle, filePath)
        Else
            MessageBox.Show("No active video stream to snapshot.", "Snapshot Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 4. Verify the result
        If success Then
            ' Optional: You can remove this message box later if you want silent snapshots
            MessageBox.Show($"Snapshot saved successfully to:" & vbCrLf & filePath, "Snapshot", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
            MessageBox.Show($"Failed to take snapshot. Error Code: {errorCode}", "Snapshot Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub butPause_Click(sender As Object, e As EventArgs) Handles butPause.Click
        If m_lPlayHandle >= 0 Then

            If Not m_isPaused Then
                ' Video is playing -> Send PAUSE command (3)
                Dim PLAY_PAUSE As UInteger = 3
                If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_PAUSE, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                    butPause.ImageOptions.SvgImage = My.Resources.play
                    m_isPaused = True

                End If
            Else
                ' Video is paused -> Send RESUME command (4)
                ' WARNING: Command 4 automatically resets the NVR hardware to 1x speed!
                Dim PLAY_RESTART As UInteger = 4
                If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_RESTART, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                    butPause.ImageOptions.SvgImage = My.Resources.pause
                    m_isPaused = False

                    ' Resync our UI to match the NVR's hardware reset
                    m_speedIndex = 0
                    UpdateSpeedLabel()

                End If
            End If

        End If
    End Sub

    Private Sub butFrame_Click(sender As Object, e As EventArgs) Handles butFrame.Click
        ' Command 8 = Step Forward One Frame
        If m_lPlayHandle >= 0 Then
            Dim PLAY_FRAME As UInteger = 8

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_FRAME, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                ' Since stepping a frame freezes the video, we must update our UI state to match
                If Not m_isPaused Then
                    m_isPaused = True

                    ' Change the pause button to show the Play icon, so the user knows they 
                    ' need to click it to resume normal playback
                    butPause.ImageOptions.SvgImage = My.Resources.play
                End If

            End If
        End If
    End Sub

    Private Sub butSpeedUp_Click(sender As Object, e As EventArgs) Handles butSpeedUp.Click
        ' Safety check: Do nothing if playing in reverse
        If m_isReverse Then Return

        If m_lPlayHandle >= 0 Then
            Dim PLAY_FAST As UInteger = 5

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_FAST, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then
                If m_speedIndex < 4 Then
                    m_speedIndex += 1
                End If

                UpdateSpeedLabel()

                If m_isPaused Then
                    m_isPaused = False
                    butPause.ImageOptions.SvgImage = My.Resources.pause
                End If
            End If
        End If
    End Sub

    Private Sub butSlowDown_Click(sender As Object, e As EventArgs) Handles butSlowDown.Click
        ' Safety check: Do nothing if playing in reverse
        If m_isReverse Then Return

        If m_lPlayHandle >= 0 Then
            Dim PLAY_SLOW As UInteger = 6

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_SLOW, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then
                If m_speedIndex > -4 Then
                    m_speedIndex -= 1
                End If

                UpdateSpeedLabel()

                If m_isPaused Then
                    m_isPaused = False
                    butPause.ImageOptions.SvgImage = My.Resources.pause
                End If
            End If
        End If
    End Sub

    Private Sub butNormalSpeed_Click(sender As Object, e As EventArgs) Handles butNormalSpeed.Click
        ' Command 7 = Return to Normal Speed (1x)
        If m_lPlayHandle >= 0 Then
            Dim PLAY_NORMAL As UInteger = 7

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_NORMAL, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                ' Reset index directly to 0 (1x)
                m_speedIndex = 0
                UpdateSpeedLabel()

                ' If it was paused, clicking normal speed also resumes playback
                If m_isPaused Then
                    m_isPaused = False
                    butPause.ImageOptions.SvgImage = My.Resources.pause
                End If

            End If
        End If
    End Sub

    Private Sub butReverse_Click(sender As Object, e As EventArgs) Handles butReverse.Click
        If m_lPlayHandle >= 0 Then
            Dim PLAY_REVERSE As UInteger = 30

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_REVERSE, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                ' Lock the speed controls
                m_isReverse = True

                m_speedIndex = 0
                UpdateSpeedLabel()

                If m_isPaused Then
                    m_isPaused = False
                    butPause.ImageOptions.SvgImage = My.Resources.pause
                End If
            End If
        End If
    End Sub

    Private Sub butForward_Click(sender As Object, e As EventArgs) Handles butForward.Click
        If m_lPlayHandle >= 0 Then
            Dim PLAY_FORWARD As UInteger = 29

            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_FORWARD, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                ' Unlock the speed controls
                m_isReverse = False

                m_speedIndex = 0
                UpdateSpeedLabel()

                If m_isPaused Then
                    m_isPaused = False
                    butPause.ImageOptions.SvgImage = My.Resources.pause
                End If
            End If
        End If
    End Sub

    Private Sub butDownload_Click(sender As Object, e As EventArgs) Handles butDownload.Click
        ' Only allow downloading if we have a valid user login and are viewing a playback string
        If m_lUserID < 0 OrElse String.IsNullOrEmpty(m_timestamp) Then
            MessageBox.Show("Downloads are only available for historical playback clips.", "Download Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Prevent multiple concurrent downloads from freezing the NVR
        If m_lDownloadHandle >= 0 Then
            MessageBox.Show("A download is already in progress. Please wait.", "Download Active", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 1. Create the target directory
        Dim targetFolder As String = "C:\CSM\CCTV-Clips"
        If Not System.IO.Directory.Exists(targetFolder) Then
            System.IO.Directory.CreateDirectory(targetFolder)
        End If

        ' 2. Generate the filename
        Dim timestamp As String = DateTime.Now.ToString("yyyyMMddHHmmss")
        Dim filePath As String = System.IO.Path.Combine(targetFolder, $"{timestamp}.mp4")

        Try
            ' 3. Rebuild the exact Start and End times
            Dim startDateTime As DateTime = DateTime.ParseExact(m_timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)
            Dim endDateTime As DateTime = startDateTime.AddMinutes(m_lengthMinutes)

            Dim startTime As New CHCNetSDK.NET_DVR_TIME()
            startTime.dwYear = startDateTime.Year
            startTime.dwMonth = startDateTime.Month
            startTime.dwDay = startDateTime.Day
            startTime.dwHour = startDateTime.Hour
            startTime.dwMinute = startDateTime.Minute
            startTime.dwSecond = startDateTime.Second

            Dim endTime As New CHCNetSDK.NET_DVR_TIME()
            endTime.dwYear = endDateTime.Year
            endTime.dwMonth = endDateTime.Month
            endTime.dwDay = endDateTime.Day
            endTime.dwHour = endDateTime.Hour
            endTime.dwMinute = endDateTime.Minute
            endTime.dwSecond = endDateTime.Second

            ' 4. Request the download stream from the NVR
            Dim channelID As Integer = Convert.ToInt32(m_channelId)
            m_lDownloadHandle = CHCNetSDK.NET_DVR_GetFileByTime(m_lUserID, channelID, startTime, endTime, filePath)

            If m_lDownloadHandle < 0 Then
                Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
                MessageBox.Show($"Failed to prepare download. Error Code: {errorCode}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 5. Send the "Play" command to actually begin the background file transfer
            Dim PLAY_START As UInteger = 1
            ' 5. Send the "Play" command to actually begin the background file transfer
            If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lDownloadHandle, PLAY_START, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero) Then

                ' Reveal and reset the progress bar
                progressBarDownload.Position = 0
                progressBarDownload.Visible = True

                ' Start the monitoring timer
                timerDownload.Start()

            Else
                Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
                MessageBox.Show($"Failed to start download stream. Error Code: {errorCode}")

                ' Clean up if it failed
                CHCNetSDK.NET_DVR_StopGetFile(m_lDownloadHandle)
                m_lDownloadHandle = -1
            End If

        Catch ex As Exception
            MessageBox.Show($"Download Exception: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub timerDownload_Tick(sender As Object, e As EventArgs) Handles timerDownload.Tick
        If m_lDownloadHandle >= 0 Then

            ' Ask the NVR how much of the file has been downloaded (0 to 100)
            Dim progress As Integer = CHCNetSDK.NET_DVR_GetDownloadPos(m_lDownloadHandle)

            ' Update the UI if it's a valid percentage
            If progress >= 0 AndAlso progress <= 100 Then
                progressBarDownload.Position = progress
            End If

            ' 100 means success. 200 means a network error occurred.
            If progress = 100 OrElse progress = 200 Then

                ' The file is finished, close the stream
                timerDownload.Stop()
                CHCNetSDK.NET_DVR_StopGetFile(m_lDownloadHandle)
                m_lDownloadHandle = -1

                ' Hide the progress bar again
                progressBarDownload.Visible = False

                ' Optional: Only show an error box if it physically failed
                If progress = 200 Then
                    MessageBox.Show("The download failed due to a network error.", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            End If

        End If
    End Sub

    Private Sub timerPlayback_Tick(sender As Object, e As EventArgs) Handles timerPlayback.Tick
        If m_lPlayHandle >= 0 AndAlso Not m_isDragging Then

            ' Ask the NVR for the exact clock time stamped on the current video frame
            Dim osdTime As New CHCNetSDK.NET_DVR_TIME()

            If CHCNetSDK.NET_DVR_GetPlayBackOsdTime(m_lPlayHandle, osdTime) Then

                ' Safeguard: During the first 1-2 seconds of buffering, the NVR might return 
                ' a blank date (Year 0). We only calculate if the year is valid.
                If osdTime.dwYear > 2000 AndAlso osdTime.dwMonth > 0 AndAlso osdTime.dwDay > 0 Then

                    Try
                        ' 1. Rebuild our original requested start time
                        Dim startDateTime As DateTime = DateTime.ParseExact(m_timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)

                        ' 2. Determine the total duration in seconds (e.g., 1 min = 60 seconds)
                        Dim totalSeconds As Double = m_lengthMinutes * 60.0

                        ' 3. Build a DateTime out of the NVR's current frame time
                        Dim currentDateTime As New DateTime(CType(osdTime.dwYear, Integer),
                                                            CType(osdTime.dwMonth, Integer),
                                                            CType(osdTime.dwDay, Integer),
                                                            CType(osdTime.dwHour, Integer),
                                                            CType(osdTime.dwMinute, Integer),
                                                            CType(osdTime.dwSecond, Integer))

                        ' 4. Calculate exactly how many seconds have played so far
                        Dim elapsedSeconds As Double = (currentDateTime - startDateTime).TotalSeconds

                        ' 5. Do the math to find our percentage!
                        Dim pct As Double = (elapsedSeconds / totalSeconds) * 100

                        ' Lock it between 0 and 100 so the trackbar doesn't crash
                        If pct < 0 Then pct = 0
                        If pct > 100 Then pct = 100

                        ' Update the slider!
                        trackBarProgress.Value = CInt(pct)

                        ' Update the UI Label with the actual video time
                        labTime.Text = currentDateTime.ToString("HH:mm:ss")

                        ' Print it to the output window so we can watch it work
                        Console.WriteLine($"[NVR TIME] Frame: {currentDateTime.ToString("HH:mm:ss")} | Math: {pct:F1}%")

                    Catch ex As Exception
                        ' Silently catch any weird date math issues during video glitches
                    End Try

                End If
            End If

        End If
    End Sub

    Private Sub trackBarProgress_EditValueChanged(sender As Object, e As EventArgs) Handles trackBarProgress.EditValueChanged
        ' ONLY do this math if the user is physically dragging the slider.
        ' (If we don't check this, it will fight the Timer!)
        If m_isDragging Then

            Try
                ' 1. Rebuild our original requested start time
                Dim startDateTime As DateTime = DateTime.ParseExact(m_timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)

                ' 2. Determine the total duration in seconds
                Dim totalSeconds As Double = m_lengthMinutes * 60.0

                ' 3. Calculate what time the slider is currently pointing at
                Dim targetSeconds As Double = (trackBarProgress.Value / 100.0) * totalSeconds
                Dim targetDateTime As DateTime = startDateTime.AddSeconds(targetSeconds)

                ' 4. Instantly update the label so the user sees where they are landing!
                labTime.Text = targetDateTime.ToString("HH:mm:ss")

            Catch ex As Exception
                ' Failsafe for background parsing errors
            End Try

        End If
    End Sub

    Private Sub trackBarProgress_MouseDown(sender As Object, e As MouseEventArgs) Handles trackBarProgress.MouseDown
        ' Pause the timer updates so the slider doesn't fight you while dragging
        m_isDragging = True
    End Sub

    Private Sub trackBarProgress_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBarProgress.MouseUp
        If m_lPlayHandle >= 0 Then

            Try
                ' 1. Calculate the exact target time based on where the user dropped the slider
                Dim startDateTime As DateTime = DateTime.ParseExact(m_timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)
                Dim totalSeconds As Double = m_lengthMinutes * 60.0

                Dim targetSeconds As Double = (trackBarProgress.Value / 100.0) * totalSeconds
                Dim targetDateTime As DateTime = startDateTime.AddSeconds(targetSeconds)

                ' 2. Populate the Hikvision Time Structure
                Dim seekTime As New CHCNetSDK.NET_DVR_TIME()
                seekTime.dwYear = targetDateTime.Year
                seekTime.dwMonth = targetDateTime.Month
                seekTime.dwDay = targetDateTime.Day
                seekTime.dwHour = targetDateTime.Hour
                seekTime.dwMinute = targetDateTime.Minute
                seekTime.dwSecond = targetDateTime.Second

                ' 3. Allocate memory
                Dim size As Integer = System.Runtime.InteropServices.Marshal.SizeOf(seekTime)
                Dim ptrTime As IntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size)
                System.Runtime.InteropServices.Marshal.StructureToPtr(seekTime, ptrTime, False)

                ' 4. Send Command 26 (NET_DVR_PLAYSETTIME)
                Dim PLAY_SETTIME As UInteger = 26

                If CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_SETTIME, ptrTime, CType(size, UInteger), IntPtr.Zero, IntPtr.Zero) Then

                    m_speedIndex = 0
                    UpdateSpeedLabel()

                    ' --- THE FIX: Forcefully unpause the NVR hardware! ---
                    If m_isPaused Then
                        Dim PLAY_RESTART As UInteger = 4
                        CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, PLAY_RESTART, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero)

                        m_isPaused = False
                        butPause.ImageOptions.SvgImage = My.Resources.pause
                    End If

                    ' Give the NVR 2 seconds to load the new video frame before the timer reads it
                    m_ignoreTimerUntil = DateTime.Now.AddSeconds(2)

                Else
                    Dim errorCode As UInteger = CHCNetSDK.NET_DVR_GetLastError()
                    Console.WriteLine($"[SEEK ERROR] Failed to jump to time. Error Code: {errorCode}")
                End If

                ' 5. Free the memory!
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptrTime)

            Catch ex As Exception
                Console.WriteLine($"[SEEK EXCEPTION] {ex.Message}")
            End Try

        End If

        m_isDragging = False
    End Sub

    Private Sub UpdateSpeedLabel()
        If m_speedIndex = 0 Then
            labSpeed.Text = "1x"
        ElseIf m_speedIndex > 0 Then
            ' 1 = 2x, 2 = 4x, 3 = 8x, 4 = 16x
            Dim multiplier As Integer = CInt(Math.Pow(2, m_speedIndex))
            labSpeed.Text = $"{multiplier}x"
        Else
            ' -1 = 1/2x, -2 = 1/4x, -3 = 1/8x, -4 = 1/16x
            Dim divider As Integer = CInt(Math.Pow(2, Math.Abs(m_speedIndex)))
            labSpeed.Text = $"1/{divider}x"
        End If
    End Sub
End Class
