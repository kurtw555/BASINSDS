<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWinHSPF
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWinHSPF))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FunctionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ReachEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LandUseEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.InputDataEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.PollutantToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.btnControl = New System.Windows.Forms.Button
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.FunctionsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(4, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(862, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.EditToolStripMenuItem.Text = "Edit"
        '
        'FunctionsToolStripMenuItem
        '
        Me.FunctionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ReachEditorToolStripMenuItem, Me.LandUseEditorToolStripMenuItem, Me.InputDataEditorToolStripMenuItem, Me.PollutantToolStripMenuItem})
        Me.FunctionsToolStripMenuItem.Name = "FunctionsToolStripMenuItem"
        Me.FunctionsToolStripMenuItem.Size = New System.Drawing.Size(65, 20)
        Me.FunctionsToolStripMenuItem.Text = "Functions"
        '
        'ReachEditorToolStripMenuItem
        '
        Me.ReachEditorToolStripMenuItem.Name = "ReachEditorToolStripMenuItem"
        Me.ReachEditorToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.ReachEditorToolStripMenuItem.Text = "Reach Editor"
        '
        'LandUseEditorToolStripMenuItem
        '
        Me.LandUseEditorToolStripMenuItem.Name = "LandUseEditorToolStripMenuItem"
        Me.LandUseEditorToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.LandUseEditorToolStripMenuItem.Text = "Land Use Editor"
        '
        'InputDataEditorToolStripMenuItem
        '
        Me.InputDataEditorToolStripMenuItem.Name = "InputDataEditorToolStripMenuItem"
        Me.InputDataEditorToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.InputDataEditorToolStripMenuItem.Text = "Input Data Editor"
        '
        'PollutantToolStripMenuItem
        '
        Me.PollutantToolStripMenuItem.Name = "PollutantToolStripMenuItem"
        Me.PollutantToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.PollutantToolStripMenuItem.Text = "Pollutant Selection"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'btnControl
        '
        Me.btnControl.Image = CType(resources.GetObject("btnControl.Image"), System.Drawing.Image)
        Me.btnControl.ImageKey = "(none)"
        Me.btnControl.Location = New System.Drawing.Point(0, 33)
        Me.btnControl.Name = "btnControl"
        Me.btnControl.Size = New System.Drawing.Size(53, 49)
        Me.btnControl.TabIndex = 1
        Me.btnControl.UseVisualStyleBackColor = True
        '
        'frmWinHSPF
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(862, 507)
        Me.Controls.Add(Me.btnControl)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmWinHSPF"
        Me.Text = "Hydrological Simulation Program - Fortran (HSPF)"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FunctionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReachEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LandUseEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InputDataEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PollutantToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnControl As System.Windows.Forms.Button

End Class
