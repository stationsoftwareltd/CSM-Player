<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.progressBarDownload = New DevExpress.XtraEditors.ProgressBarControl()
        Me.butDownload = New DevExpress.XtraEditors.SimpleButton()
        Me.labTime = New DevExpress.XtraEditors.LabelControl()
        Me.trackBarProgress = New DevExpress.XtraEditors.TrackBarControl()
        Me.labSpeed = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.butForward = New DevExpress.XtraEditors.SimpleButton()
        Me.butReverse = New DevExpress.XtraEditors.SimpleButton()
        Me.butSpeedUp = New DevExpress.XtraEditors.SimpleButton()
        Me.butNormalSpeed = New DevExpress.XtraEditors.SimpleButton()
        Me.butSlowDown = New DevExpress.XtraEditors.SimpleButton()
        Me.butSnapshot = New DevExpress.XtraEditors.SimpleButton()
        Me.butAudio = New DevExpress.XtraEditors.SimpleButton()
        Me.butFrame = New DevExpress.XtraEditors.SimpleButton()
        Me.butPause = New DevExpress.XtraEditors.SimpleButton()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.timerPlayback = New System.Windows.Forms.Timer(Me.components)
        Me.timerDownload = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.progressBarDownload.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBarProgress, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBarProgress.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelControl1
        '
        Me.PanelControl1.Controls.Add(Me.progressBarDownload)
        Me.PanelControl1.Controls.Add(Me.butDownload)
        Me.PanelControl1.Controls.Add(Me.labTime)
        Me.PanelControl1.Controls.Add(Me.trackBarProgress)
        Me.PanelControl1.Controls.Add(Me.labSpeed)
        Me.PanelControl1.Controls.Add(Me.LabelControl2)
        Me.PanelControl1.Controls.Add(Me.LabelControl1)
        Me.PanelControl1.Controls.Add(Me.butForward)
        Me.PanelControl1.Controls.Add(Me.butReverse)
        Me.PanelControl1.Controls.Add(Me.butSpeedUp)
        Me.PanelControl1.Controls.Add(Me.butNormalSpeed)
        Me.PanelControl1.Controls.Add(Me.butSlowDown)
        Me.PanelControl1.Controls.Add(Me.butSnapshot)
        Me.PanelControl1.Controls.Add(Me.butAudio)
        Me.PanelControl1.Controls.Add(Me.butFrame)
        Me.PanelControl1.Controls.Add(Me.butPause)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl1.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(784, 112)
        Me.PanelControl1.TabIndex = 1
        '
        'progressBarDownload
        '
        Me.progressBarDownload.Location = New System.Drawing.Point(480, 31)
        Me.progressBarDownload.Name = "progressBarDownload"
        Me.progressBarDownload.Size = New System.Drawing.Size(174, 16)
        Me.progressBarDownload.TabIndex = 15
        Me.progressBarDownload.Visible = False
        '
        'butDownload
        '
        Me.butDownload.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.download
        Me.butDownload.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butDownload.Location = New System.Drawing.Point(442, 24)
        Me.butDownload.Name = "butDownload"
        Me.butDownload.Size = New System.Drawing.Size(32, 32)
        Me.butDownload.TabIndex = 14
        '
        'labTime
        '
        Me.labTime.Appearance.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.labTime.Appearance.Options.UseFont = True
        Me.labTime.Location = New System.Drawing.Point(426, 73)
        Me.labTime.Name = "labTime"
        Me.labTime.Size = New System.Drawing.Size(70, 18)
        Me.labTime.TabIndex = 13
        Me.labTime.Text = "00:00:00"
        '
        'trackBarProgress
        '
        Me.trackBarProgress.EditValue = Nothing
        Me.trackBarProgress.Location = New System.Drawing.Point(12, 62)
        Me.trackBarProgress.Name = "trackBarProgress"
        Me.trackBarProgress.Properties.LabelAppearance.Options.UseTextOptions = True
        Me.trackBarProgress.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.trackBarProgress.Properties.Maximum = 100
        Me.trackBarProgress.Size = New System.Drawing.Size(408, 50)
        Me.trackBarProgress.TabIndex = 12
        '
        'labSpeed
        '
        Me.labSpeed.Appearance.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.labSpeed.Appearance.Options.UseFont = True
        Me.labSpeed.Location = New System.Drawing.Point(296, 31)
        Me.labSpeed.Name = "labSpeed"
        Me.labSpeed.Size = New System.Drawing.Size(19, 18)
        Me.labSpeed.TabIndex = 11
        Me.labSpeed.Text = "x1"
        '
        'LabelControl2
        '
        Me.LabelControl2.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl2.Appearance.Options.UseFont = True
        Me.LabelControl2.Location = New System.Drawing.Point(362, 6)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(46, 12)
        Me.LabelControl2.TabIndex = 10
        Me.LabelControl2.Text = "Direction"
        '
        'LabelControl1
        '
        Me.LabelControl1.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl1.Appearance.Options.UseFont = True
        Me.LabelControl1.Location = New System.Drawing.Point(173, 7)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(105, 12)
        Me.LabelControl1.TabIndex = 9
        Me.LabelControl1.Text = "---------Speed ---------"
        '
        'butForward
        '
        Me.butForward.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.play
        Me.butForward.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butForward.Location = New System.Drawing.Point(388, 24)
        Me.butForward.Name = "butForward"
        Me.butForward.Size = New System.Drawing.Size(32, 32)
        Me.butForward.TabIndex = 8
        '
        'butReverse
        '
        Me.butReverse.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.reverse
        Me.butReverse.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butReverse.Location = New System.Drawing.Point(350, 24)
        Me.butReverse.Name = "butReverse"
        Me.butReverse.Size = New System.Drawing.Size(32, 32)
        Me.butReverse.TabIndex = 7
        '
        'butSpeedUp
        '
        Me.butSpeedUp.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.forward
        Me.butSpeedUp.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butSpeedUp.Location = New System.Drawing.Point(249, 24)
        Me.butSpeedUp.Name = "butSpeedUp"
        Me.butSpeedUp.Size = New System.Drawing.Size(32, 32)
        Me.butSpeedUp.TabIndex = 6
        '
        'butNormalSpeed
        '
        Me.butNormalSpeed.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.play
        Me.butNormalSpeed.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butNormalSpeed.Location = New System.Drawing.Point(211, 24)
        Me.butNormalSpeed.Name = "butNormalSpeed"
        Me.butNormalSpeed.Size = New System.Drawing.Size(32, 32)
        Me.butNormalSpeed.TabIndex = 5
        '
        'butSlowDown
        '
        Me.butSlowDown.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.rewind
        Me.butSlowDown.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butSlowDown.Location = New System.Drawing.Point(173, 24)
        Me.butSlowDown.Name = "butSlowDown"
        Me.butSlowDown.Size = New System.Drawing.Size(32, 32)
        Me.butSlowDown.TabIndex = 4
        '
        'butSnapshot
        '
        Me.butSnapshot.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.snapshot
        Me.butSnapshot.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butSnapshot.Location = New System.Drawing.Point(126, 24)
        Me.butSnapshot.Name = "butSnapshot"
        Me.butSnapshot.Size = New System.Drawing.Size(32, 32)
        Me.butSnapshot.TabIndex = 3
        '
        'butAudio
        '
        Me.butAudio.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.audio
        Me.butAudio.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butAudio.Location = New System.Drawing.Point(88, 24)
        Me.butAudio.Name = "butAudio"
        Me.butAudio.Size = New System.Drawing.Size(32, 32)
        Me.butAudio.TabIndex = 2
        '
        'butFrame
        '
        Me.butFrame.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.frame
        Me.butFrame.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butFrame.Location = New System.Drawing.Point(50, 24)
        Me.butFrame.Name = "butFrame"
        Me.butFrame.Size = New System.Drawing.Size(32, 32)
        Me.butFrame.TabIndex = 1
        '
        'butPause
        '
        Me.butPause.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.pause
        Me.butPause.ImageOptions.SvgImageSize = New System.Drawing.Size(24, 24)
        Me.butPause.Location = New System.Drawing.Point(12, 24)
        Me.butPause.Name = "butPause"
        Me.butPause.Size = New System.Drawing.Size(32, 32)
        Me.butPause.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 112)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(784, 518)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'timerPlayback
        '
        Me.timerPlayback.Enabled = True
        Me.timerPlayback.Interval = 1000
        '
        'timerDownload
        '
        Me.timerDownload.Enabled = True
        Me.timerDownload.Interval = 1000
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 630)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.PanelControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CSM Playback"
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        Me.PanelControl1.PerformLayout()
        CType(Me.progressBarDownload.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBarProgress.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBarProgress, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents butPause As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butReverse As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butSpeedUp As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butNormalSpeed As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butSlowDown As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butSnapshot As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butAudio As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butFrame As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents butForward As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents labSpeed As DevExpress.XtraEditors.LabelControl
    Friend WithEvents trackBarProgress As DevExpress.XtraEditors.TrackBarControl
    Friend WithEvents timerPlayback As Timer
    Friend WithEvents labTime As DevExpress.XtraEditors.LabelControl
    Friend WithEvents butDownload As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents timerDownload As Timer
    Friend WithEvents progressBarDownload As DevExpress.XtraEditors.ProgressBarControl
End Class
