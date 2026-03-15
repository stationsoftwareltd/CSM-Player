<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfig
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Me.TreeListCameras = New DevExpress.XtraTreeList.TreeList()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.butSave = New DevExpress.XtraEditors.SimpleButton()
        Me.butLogin = New DevExpress.XtraEditors.SimpleButton()
        Me.tbHeight = New DevExpress.XtraEditors.TextEdit()
        Me.tbWidth = New DevExpress.XtraEditors.TextEdit()
        Me.tbPassword = New DevExpress.XtraEditors.TextEdit()
        Me.tbUsername = New DevExpress.XtraEditors.TextEdit()
        Me.tbPort = New DevExpress.XtraEditors.TextEdit()
        Me.tbNVRIP = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl7 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl6 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl5 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.pictureBoxPreview = New System.Windows.Forms.PictureBox()
        CType(Me.TreeListCameras, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.tbHeight.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbWidth.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbPassword.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbUsername.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbPort.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbNVRIP.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pictureBoxPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TreeListCameras
        '
        Me.TreeListCameras.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeListCameras.Location = New System.Drawing.Point(0, 66)
        Me.TreeListCameras.Name = "TreeListCameras"
        Me.TreeListCameras.Size = New System.Drawing.Size(850, 347)
        Me.TreeListCameras.TabIndex = 3
        '
        'PanelControl1
        '
        Me.PanelControl1.Controls.Add(Me.butSave)
        Me.PanelControl1.Controls.Add(Me.butLogin)
        Me.PanelControl1.Controls.Add(Me.tbHeight)
        Me.PanelControl1.Controls.Add(Me.tbWidth)
        Me.PanelControl1.Controls.Add(Me.tbPassword)
        Me.PanelControl1.Controls.Add(Me.tbUsername)
        Me.PanelControl1.Controls.Add(Me.tbPort)
        Me.PanelControl1.Controls.Add(Me.tbNVRIP)
        Me.PanelControl1.Controls.Add(Me.LabelControl7)
        Me.PanelControl1.Controls.Add(Me.LabelControl6)
        Me.PanelControl1.Controls.Add(Me.LabelControl5)
        Me.PanelControl1.Controls.Add(Me.LabelControl4)
        Me.PanelControl1.Controls.Add(Me.LabelControl3)
        Me.PanelControl1.Controls.Add(Me.LabelControl2)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl1.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(850, 66)
        Me.PanelControl1.TabIndex = 4
        '
        'butSave
        '
        Me.butSave.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.save1
        Me.butSave.Location = New System.Drawing.Point(708, 11)
        Me.butSave.Name = "butSave"
        Me.butSave.Size = New System.Drawing.Size(99, 38)
        Me.butSave.TabIndex = 26
        Me.butSave.Text = "Save"
        '
        'butLogin
        '
        Me.butLogin.ImageOptions.SvgImage = Global.CSM_Player.My.Resources.Resources.security_unlock
        Me.butLogin.Location = New System.Drawing.Point(603, 11)
        Me.butLogin.Name = "butLogin"
        Me.butLogin.Size = New System.Drawing.Size(99, 38)
        Me.butLogin.TabIndex = 25
        Me.butLogin.Text = "Login"
        '
        'tbHeight
        '
        Me.tbHeight.Location = New System.Drawing.Point(528, 29)
        Me.tbHeight.Name = "tbHeight"
        Me.tbHeight.Size = New System.Drawing.Size(69, 20)
        Me.tbHeight.TabIndex = 24
        '
        'tbWidth
        '
        Me.tbWidth.Location = New System.Drawing.Point(453, 29)
        Me.tbWidth.Name = "tbWidth"
        Me.tbWidth.Size = New System.Drawing.Size(69, 20)
        Me.tbWidth.TabIndex = 23
        '
        'tbPassword
        '
        Me.tbPassword.Location = New System.Drawing.Point(328, 29)
        Me.tbPassword.Name = "tbPassword"
        Me.tbPassword.Size = New System.Drawing.Size(119, 20)
        Me.tbPassword.TabIndex = 22
        '
        'tbUsername
        '
        Me.tbUsername.Location = New System.Drawing.Point(203, 29)
        Me.tbUsername.Name = "tbUsername"
        Me.tbUsername.Size = New System.Drawing.Size(119, 20)
        Me.tbUsername.TabIndex = 21
        '
        'tbPort
        '
        Me.tbPort.Location = New System.Drawing.Point(137, 29)
        Me.tbPort.Name = "tbPort"
        Me.tbPort.Size = New System.Drawing.Size(60, 20)
        Me.tbPort.TabIndex = 19
        '
        'tbNVRIP
        '
        Me.tbNVRIP.Location = New System.Drawing.Point(12, 29)
        Me.tbNVRIP.Name = "tbNVRIP"
        Me.tbNVRIP.Size = New System.Drawing.Size(119, 20)
        Me.tbNVRIP.TabIndex = 18
        '
        'LabelControl7
        '
        Me.LabelControl7.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl7.Appearance.Options.UseFont = True
        Me.LabelControl7.Location = New System.Drawing.Point(528, 12)
        Me.LabelControl7.Name = "LabelControl7"
        Me.LabelControl7.Size = New System.Drawing.Size(64, 12)
        Me.LabelControl7.TabIndex = 17
        Me.LabelControl7.Text = "Video Height"
        '
        'LabelControl6
        '
        Me.LabelControl6.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl6.Appearance.Options.UseFont = True
        Me.LabelControl6.Location = New System.Drawing.Point(453, 12)
        Me.LabelControl6.Name = "LabelControl6"
        Me.LabelControl6.Size = New System.Drawing.Size(60, 12)
        Me.LabelControl6.TabIndex = 16
        Me.LabelControl6.Text = "Video Width"
        '
        'LabelControl5
        '
        Me.LabelControl5.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl5.Appearance.Options.UseFont = True
        Me.LabelControl5.Location = New System.Drawing.Point(328, 12)
        Me.LabelControl5.Name = "LabelControl5"
        Me.LabelControl5.Size = New System.Drawing.Size(49, 12)
        Me.LabelControl5.TabIndex = 15
        Me.LabelControl5.Text = "Password"
        '
        'LabelControl4
        '
        Me.LabelControl4.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl4.Appearance.Options.UseFont = True
        Me.LabelControl4.Location = New System.Drawing.Point(203, 12)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(50, 12)
        Me.LabelControl4.TabIndex = 14
        Me.LabelControl4.Text = "Username"
        '
        'LabelControl3
        '
        Me.LabelControl3.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl3.Appearance.Options.UseFont = True
        Me.LabelControl3.Location = New System.Drawing.Point(137, 12)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(22, 12)
        Me.LabelControl3.TabIndex = 13
        Me.LabelControl3.Text = "Port"
        '
        'LabelControl2
        '
        Me.LabelControl2.Appearance.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.LabelControl2.Appearance.Options.UseFont = True
        Me.LabelControl2.Location = New System.Drawing.Point(12, 12)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(80, 12)
        Me.LabelControl2.TabIndex = 11
        Me.LabelControl2.Text = "NVR IP Address"
        '
        'pictureBoxPreview
        '
        Me.pictureBoxPreview.BackColor = System.Drawing.Color.Black
        Me.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pictureBoxPreview.Location = New System.Drawing.Point(0, 413)
        Me.pictureBoxPreview.Name = "pictureBoxPreview"
        Me.pictureBoxPreview.Size = New System.Drawing.Size(850, 362)
        Me.pictureBoxPreview.TabIndex = 5
        Me.pictureBoxPreview.TabStop = False
        '
        'frmConfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(850, 775)
        Me.Controls.Add(Me.TreeListCameras)
        Me.Controls.Add(Me.pictureBoxPreview)
        Me.Controls.Add(Me.PanelControl1)
        Me.IconOptions.Icon = CType(resources.GetObject("frmConfig.IconOptions.Icon"), System.Drawing.Icon)
        Me.Name = "frmConfig"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "HikVision Configuration"
        CType(Me.TreeListCameras, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        Me.PanelControl1.PerformLayout()
        CType(Me.tbHeight.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbWidth.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbPassword.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbUsername.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbPort.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbNVRIP.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pictureBoxPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TreeListCameras As DevExpress.XtraTreeList.TreeList
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LabelControl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents tbPort As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tbNVRIP As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tbHeight As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tbWidth As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tbPassword As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tbUsername As DevExpress.XtraEditors.TextEdit
    Friend WithEvents butLogin As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents butSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents pictureBoxPreview As PictureBox
End Class
