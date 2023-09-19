Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class atcGraphForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSelectData = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileSaveJson = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileApplyJson = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFilePrint = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditGraph = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuEditCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditCopyMetafile = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewVerticalZoom = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewHorizontalZoom = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewZoomAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuAnalysis = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCoordinates = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCoordinatesOnMenuBar = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenu1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainMenu1
        '
        Me.MainMenu1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuAnalysis, Me.mnuCoordinates, Me.mnuHelp})
        Me.MainMenu1.Location = New System.Drawing.Point(0, 0)
        Me.MainMenu1.Name = "MainMenu1"
        Me.MainMenu1.Size = New System.Drawing.Size(800, 28)
        Me.MainMenu1.TabIndex = 0
        Me.MainMenu1.Text = "MenuStrip1"
        '
        'mnuFile
        '
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFileSelectData, Me.mnuFileSep1, Me.mnuFileSaveJson, Me.mnuFileApplyJson, Me.mnuFileSave, Me.mnuFilePrint})
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(46, 24)
        Me.mnuFile.Text = "File"
        '
        'mnuFileSelectData
        '
        Me.mnuFileSelectData.Name = "mnuFileSelectData"
        Me.mnuFileSelectData.Size = New System.Drawing.Size(173, 26)
        Me.mnuFileSelectData.Text = "Select Data"
        '
        'mnuFileSep1
        '
        Me.mnuFileSep1.Name = "mnuFileSep1"
        Me.mnuFileSep1.Size = New System.Drawing.Size(170, 6)
        '
        'mnuFileSaveJson
        '
        Me.mnuFileSaveJson.Name = "mnuFileSaveJson"
        Me.mnuFileSaveJson.Size = New System.Drawing.Size(173, 26)
        Me.mnuFileSaveJson.Text = "Save Specs"
        '
        'mnuFileApplyJson
        '
        Me.mnuFileApplyJson.Name = "mnuFileApplyJson"
        Me.mnuFileApplyJson.Size = New System.Drawing.Size(173, 26)
        Me.mnuFileApplyJson.Text = "Apply Specs"
        '
        'mnuFileSave
        '
        Me.mnuFileSave.Name = "mnuFileSave"
        Me.mnuFileSave.Size = New System.Drawing.Size(173, 26)
        Me.mnuFileSave.Text = "Save As..."
        '
        'mnuFilePrint
        '
        Me.mnuFilePrint.Name = "mnuFilePrint"
        Me.mnuFilePrint.Size = New System.Drawing.Size(173, 26)
        Me.mnuFilePrint.Text = "Print"
        '
        'mnuEdit
        '
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuEditGraph, Me.ToolStripSeparator1, Me.mnuEditCopy, Me.mnuEditCopyMetafile})
        Me.mnuEdit.Name = "mnuEdit"
        Me.mnuEdit.Size = New System.Drawing.Size(49, 24)
        Me.mnuEdit.Text = "Edit"
        '
        'mnuEditGraph
        '
        Me.mnuEditGraph.Name = "mnuEditGraph"
        Me.mnuEditGraph.Size = New System.Drawing.Size(185, 26)
        Me.mnuEditGraph.Text = "Graph"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(182, 6)
        '
        'mnuEditCopy
        '
        Me.mnuEditCopy.Name = "mnuEditCopy"
        Me.mnuEditCopy.Size = New System.Drawing.Size(185, 26)
        Me.mnuEditCopy.Text = "Copy"
        '
        'mnuEditCopyMetafile
        '
        Me.mnuEditCopyMetafile.Name = "mnuEditCopyMetafile"
        Me.mnuEditCopyMetafile.Size = New System.Drawing.Size(185, 26)
        Me.mnuEditCopyMetafile.Text = "Copy Metafile"
        '
        'mnuView
        '
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuViewVerticalZoom, Me.mnuViewHorizontalZoom, Me.mnuViewZoomAll})
        Me.mnuView.Name = "mnuView"
        Me.mnuView.Size = New System.Drawing.Size(55, 24)
        Me.mnuView.Text = "View"
        '
        'mnuViewVerticalZoom
        '
        Me.mnuViewVerticalZoom.Name = "mnuViewVerticalZoom"
        Me.mnuViewVerticalZoom.Size = New System.Drawing.Size(235, 26)
        Me.mnuViewVerticalZoom.Text = "Vertical Zoom/Pan"
        '
        'mnuViewHorizontalZoom
        '
        Me.mnuViewHorizontalZoom.Name = "mnuViewHorizontalZoom"
        Me.mnuViewHorizontalZoom.Size = New System.Drawing.Size(235, 26)
        Me.mnuViewHorizontalZoom.Text = "Horizontal Zoom/Pan"
        '
        'mnuViewZoomAll
        '
        Me.mnuViewZoomAll.Name = "mnuViewZoomAll"
        Me.mnuViewZoomAll.Size = New System.Drawing.Size(235, 26)
        Me.mnuViewZoomAll.Text = "Zoom to All"
        '
        'mnuAnalysis
        '
        Me.mnuAnalysis.Name = "mnuAnalysis"
        Me.mnuAnalysis.Size = New System.Drawing.Size(76, 24)
        Me.mnuAnalysis.Text = "Analysis"
        '
        'mnuCoordinates
        '
        Me.mnuCoordinates.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCoordinatesOnMenuBar})
        Me.mnuCoordinates.Name = "mnuCoordinates"
        Me.mnuCoordinates.Size = New System.Drawing.Size(103, 24)
        Me.mnuCoordinates.Text = "Coordinates"
        '
        'mnuCoordinatesOnMenuBar
        '
        Me.mnuCoordinatesOnMenuBar.Name = "mnuCoordinatesOnMenuBar"
        Me.mnuCoordinatesOnMenuBar.Size = New System.Drawing.Size(224, 26)
        Me.mnuCoordinatesOnMenuBar.Text = "On Menu Bar"
        '
        'mnuHelp
        '
        Me.mnuHelp.Name = "mnuHelp"
        Me.mnuHelp.Size = New System.Drawing.Size(55, 24)
        Me.mnuHelp.Text = "Help"
        '
        'atcGraphForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.MainMenu1)
        Me.MainMenuStrip = Me.MainMenu1
        Me.Name = "atcGraphForm"
        Me.Text = "Graph"
        Me.MainMenu1.ResumeLayout(False)
        Me.MainMenu1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MainMenu1 As MenuStrip
    Friend WithEvents mnuFile As ToolStripMenuItem
    Friend WithEvents mnuFileSelectData As ToolStripMenuItem
    Friend WithEvents mnuFileSep1 As ToolStripSeparator
    Friend WithEvents mnuFileSaveJson As ToolStripMenuItem
    Friend WithEvents mnuFileApplyJson As ToolStripMenuItem
    Friend WithEvents mnuFileSave As ToolStripMenuItem
    Friend WithEvents mnuFilePrint As ToolStripMenuItem
    Friend WithEvents mnuEdit As ToolStripMenuItem
    Friend WithEvents mnuEditGraph As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents mnuEditCopy As ToolStripMenuItem
    Friend WithEvents mnuEditCopyMetafile As ToolStripMenuItem
    Friend WithEvents mnuView As ToolStripMenuItem
    Friend WithEvents mnuViewVerticalZoom As ToolStripMenuItem
    Friend WithEvents mnuViewHorizontalZoom As ToolStripMenuItem
    Friend WithEvents mnuViewZoomAll As ToolStripMenuItem
    Friend WithEvents mnuAnalysis As ToolStripMenuItem
    Friend WithEvents mnuCoordinates As ToolStripMenuItem
    Friend WithEvents mnuCoordinatesOnMenuBar As ToolStripMenuItem
    Friend WithEvents mnuHelp As ToolStripMenuItem
End Class
