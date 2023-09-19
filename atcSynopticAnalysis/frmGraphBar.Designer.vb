<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGraphBar
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGraphBar))
        Me.zgc = New ZedGraph.ZedGraphControl
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSave = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFilePrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditGraph = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditSep1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditCopy = New System.Windows.Forms.ToolStripMenuItem
        Me.SuspendLayout()
        '
        'zgc
        '
        Me.zgc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.zgc.Location = New System.Drawing.Point(0, 0)
        Me.zgc.Name = "zgc"
        Me.zgc.ScrollMaxX = 0
        Me.zgc.ScrollMaxY = 0
        Me.zgc.ScrollMaxY2 = 0
        Me.zgc.ScrollMinX = 0
        Me.zgc.ScrollMinY = 0
        Me.zgc.ScrollMinY2 = 0
        Me.zgc.Size = New System.Drawing.Size(425, 397)
        Me.zgc.TabIndex = 0
        '
        'MainMenu1
        '
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFile, Me.mnuEdit})
        '
        'mnuFile
        '
        Me.mnuFile.MergeIndex = 0
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFileSave, Me.mnuFilePrint})
        Me.mnuFile.Text = "File"
        '
        'mnuFileSave
        '
        Me.mnuFileSave.MergeIndex = 0
        Me.mnuFileSave.Text = "Save"
        '
        'mnuFilePrint
        '
        Me.mnuFilePrint.MergeIndex = 1
        Me.mnuFilePrint.Text = "Print"
        '
        'mnuEdit
        '
        Me.mnuEdit.MergeIndex = 1
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuEditGraph, Me.mnuEditSep1, Me.mnuEditCopy})
        Me.mnuEdit.Text = "Edit"
        '
        'mnuEditGraph
        '
        Me.mnuEditGraph.MergeIndex = 0
        Me.mnuEditGraph.Text = "Graph"
        Me.mnuEditGraph.Visible = False
        '
        'mnuEditSep1
        '
        Me.mnuEditSep1.MergeIndex = 1
        Me.mnuEditSep1.Text = "-"
        '
        'mnuEditCopy
        '
        Me.mnuEditCopy.MergeIndex = 2
        Me.mnuEditCopy.ShortcutKeys = System.Windows.Forms.Shortcut.CtrlC
        Me.mnuEditCopy.Text = "Copy"
        '
        'frmGraphBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(425, 397)
        Me.Controls.Add(Me.zgc)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        'Me.Menu = Me.MainMenu1
        Me.Name = "frmGraphBar"
        Me.Text = "Graph"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents zgc As ZedGraph.ZedGraphControl
    Friend WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFilePrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditGraph As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditSep1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditCopy As System.Windows.Forms.ToolStripMenuItem
End Class
