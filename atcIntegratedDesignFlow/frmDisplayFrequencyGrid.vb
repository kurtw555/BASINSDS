Imports atcData
Imports atcUtility
Imports MapWinUtility
Imports System.Windows.Forms

Friend Class frmDisplayFrequencyGrid
    Inherits System.Windows.Forms.Form
    Private pInitializing As Boolean
    Public WithEvents SWSTATform As frmSWSTAT
    Public WithEvents SWSTATformmod As frmSWSTATmod
    'Appears frmSWSTATDFlowBatch has been replaced by frmsWSTAT for batch development
    'Public WithEvents SWSTATDFlowBatchfrm As frmSWSTATDFlowBatch

    'The group of atcTimeseries displayed
    Private WithEvents pDataGroup As atcTimeseriesGroup

    Private pSource As atcFrequencyGridSource
    Private pSwapperSource As atcControls.atcGridSourceRowColumnSwapper
    Private pNday() As Double
    Friend WithEvents mnuFileExportResults As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblNote As System.Windows.Forms.Label
    Private pReturns() As Double
    Private pConditions As atcDataAttributes

#Region " Windows Form Designer generated code "

    Public Sub New(ByVal aDataGroup As atcData.atcTimeseriesGroup,
                   ByVal aHigh As Boolean,
                   ByVal aNday() As Double,
                   ByVal aReturns() As Double,
                   Optional ByVal aShowForm As Boolean = True,
                   Optional ByVal aConditions As atcDataAttributes = Nothing)
        MyBase.New()
        pInitializing = True
        Me.Visible = False
        pDataGroup = aDataGroup
        pNday = aNday
        pReturns = aReturns
        pConditions = aConditions

        InitializeComponent() 'required by Windows Form Designer

        If aShowForm Then
            Dim DisplayPlugins As ICollection = atcDataManager.GetPlugins(GetType(atcDataDisplay))
            For Each lDisp As atcDataDisplay In DisplayPlugins
                Dim lMenuText As String = lDisp.Name
                If lMenuText.StartsWith("Analysis::") Then lMenuText = lMenuText.Substring(10)
                mnuAnalysis.DropDownItems.Add(lMenuText, Nothing, New EventHandler(AddressOf mnuAnalysis_Click))
            Next
        End If

        pSource = Nothing 'Get rid of obsolete source before changing HighDisplay to avoid refresh trouble
        Me.HighDisplay = aHigh
        If pInitializing Then
            pInitializing = False
            If aShowForm Then Me.Show()
        End If
        PopulateGrid()
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuAnalysis As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents agdMain As atcControls.atcGrid
    Friend WithEvents mnuView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewColumns As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewRows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewHigh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewLow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSaveGrid As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSizeColumnsToContents As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSep1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSep2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSaveReport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSaveViewNDay As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDisplayFrequencyGrid))
        MainMenu1 = New MenuStrip()
        mnuFile = New ToolStripMenuItem()
        mnuFileSaveGrid = New ToolStripMenuItem()
        mnuFileSaveReport = New ToolStripMenuItem()
        mnuFileSaveViewNDay = New ToolStripMenuItem()
        mnuFileExportResults = New ToolStripMenuItem()
        mnuEdit = New ToolStripMenuItem()
        mnuEditCopy = New ToolStripMenuItem()
        mnuView = New ToolStripMenuItem()
        mnuViewColumns = New ToolStripMenuItem()
        mnuViewRows = New ToolStripMenuItem()
        mnuViewSep1 = New ToolStripMenuItem()
        mnuViewHigh = New ToolStripMenuItem()
        mnuViewLow = New ToolStripMenuItem()
        mnuViewSep2 = New ToolStripMenuItem()
        mnuSizeColumnsToContents = New ToolStripMenuItem()
        mnuAnalysis = New ToolStripMenuItem()
        mnuHelp = New ToolStripMenuItem()
        agdMain = New atcControls.atcGrid()
        lblNote = New Label()
        MainMenu1.SuspendLayout()
        SuspendLayout()
        ' 
        ' MainMenu1
        ' 
        MainMenu1.Items.AddRange(New ToolStripItem() {mnuFile, mnuEdit, mnuView, mnuAnalysis, mnuHelp})
        MainMenu1.Location = New System.Drawing.Point(0, 0)
        MainMenu1.Name = "MainMenu1"
        MainMenu1.Size = New System.Drawing.Size(200, 24)
        MainMenu1.TabIndex = 0
        ' 
        ' mnuFile
        ' 
        mnuFile.DropDownItems.AddRange(New ToolStripItem() {mnuFileSaveGrid, mnuFileSaveReport, mnuFileSaveViewNDay, mnuFileExportResults})
        mnuFile.MergeIndex = 0
        mnuFile.Name = "mnuFile"
        mnuFile.Size = New System.Drawing.Size(37, 20)
        mnuFile.Text = "File"
        ' 
        ' mnuFileSaveGrid
        ' 
        mnuFileSaveGrid.MergeIndex = 0
        mnuFileSaveGrid.Name = "mnuFileSaveGrid"
        mnuFileSaveGrid.ShortcutKeys = Keys.Control Or Keys.S
        mnuFileSaveGrid.Size = New System.Drawing.Size(165, 22)
        mnuFileSaveGrid.Text = "Save Grid"
        ' 
        ' mnuFileSaveReport
        ' 
        mnuFileSaveReport.MergeIndex = 1
        mnuFileSaveReport.Name = "mnuFileSaveReport"
        mnuFileSaveReport.Size = New System.Drawing.Size(165, 22)
        mnuFileSaveReport.Text = "Save Report"
        ' 
        ' mnuFileSaveViewNDay
        ' 
        mnuFileSaveViewNDay.MergeIndex = 2
        mnuFileSaveViewNDay.Name = "mnuFileSaveViewNDay"
        mnuFileSaveViewNDay.Size = New System.Drawing.Size(165, 22)
        mnuFileSaveViewNDay.Text = "Save/View N-Day"
        ' 
        ' mnuFileExportResults
        ' 
        mnuFileExportResults.MergeIndex = 3
        mnuFileExportResults.Name = "mnuFileExportResults"
        mnuFileExportResults.Size = New System.Drawing.Size(165, 22)
        mnuFileExportResults.Text = "Export Results"
        ' 
        ' mnuEdit
        ' 
        mnuEdit.DropDownItems.AddRange(New ToolStripItem() {mnuEditCopy})
        mnuEdit.MergeIndex = 1
        mnuEdit.Name = "mnuEdit"
        mnuEdit.Size = New System.Drawing.Size(39, 20)
        mnuEdit.Text = "Edit"
        ' 
        ' mnuEditCopy
        ' 
        mnuEditCopy.MergeIndex = 0
        mnuEditCopy.Name = "mnuEditCopy"
        mnuEditCopy.ShortcutKeys = Keys.Control Or Keys.C
        mnuEditCopy.Size = New System.Drawing.Size(144, 22)
        mnuEditCopy.Text = "Copy"
        ' 
        ' mnuView
        ' 
        mnuView.DropDownItems.AddRange(New ToolStripItem() {mnuViewColumns, mnuViewRows, mnuViewSep1, mnuViewHigh, mnuViewLow, mnuViewSep2, mnuSizeColumnsToContents})
        mnuView.MergeIndex = 2
        mnuView.Name = "mnuView"
        mnuView.Size = New System.Drawing.Size(44, 20)
        mnuView.Text = "View"
        ' 
        ' mnuViewColumns
        ' 
        mnuViewColumns.Checked = True
        mnuViewColumns.CheckState = CheckState.Checked
        mnuViewColumns.MergeIndex = 0
        mnuViewColumns.Name = "mnuViewColumns"
        mnuViewColumns.Size = New System.Drawing.Size(211, 22)
        mnuViewColumns.Text = "Columns"
        ' 
        ' mnuViewRows
        ' 
        mnuViewRows.MergeIndex = 1
        mnuViewRows.Name = "mnuViewRows"
        mnuViewRows.Size = New System.Drawing.Size(211, 22)
        mnuViewRows.Text = "Rows"
        ' 
        ' mnuViewSep1
        ' 
        mnuViewSep1.MergeIndex = 2
        mnuViewSep1.Name = "mnuViewSep1"
        mnuViewSep1.Size = New System.Drawing.Size(211, 22)
        mnuViewSep1.Text = "-"
        ' 
        ' mnuViewHigh
        ' 
        mnuViewHigh.Checked = True
        mnuViewHigh.CheckState = CheckState.Checked
        mnuViewHigh.MergeIndex = 3
        mnuViewHigh.Name = "mnuViewHigh"
        mnuViewHigh.Size = New System.Drawing.Size(211, 22)
        mnuViewHigh.Text = "High"
        ' 
        ' mnuViewLow
        ' 
        mnuViewLow.MergeIndex = 4
        mnuViewLow.Name = "mnuViewLow"
        mnuViewLow.Size = New System.Drawing.Size(211, 22)
        mnuViewLow.Text = "Low"
        ' 
        ' mnuViewSep2
        ' 
        mnuViewSep2.MergeIndex = 5
        mnuViewSep2.Name = "mnuViewSep2"
        mnuViewSep2.Size = New System.Drawing.Size(211, 22)
        mnuViewSep2.Text = "-"
        ' 
        ' mnuSizeColumnsToContents
        ' 
        mnuSizeColumnsToContents.MergeIndex = 6
        mnuSizeColumnsToContents.Name = "mnuSizeColumnsToContents"
        mnuSizeColumnsToContents.Size = New System.Drawing.Size(211, 22)
        mnuSizeColumnsToContents.Text = "Size Columns To Contents"
        ' 
        ' mnuAnalysis
        ' 
        mnuAnalysis.MergeIndex = 3
        mnuAnalysis.Name = "mnuAnalysis"
        mnuAnalysis.Size = New System.Drawing.Size(62, 20)
        mnuAnalysis.Text = "Analysis"
        ' 
        ' mnuHelp
        ' 
        mnuHelp.MergeIndex = 4
        mnuHelp.Name = "mnuHelp"
        mnuHelp.ShortcutKeys = Keys.F1
        mnuHelp.ShowShortcutKeys = False
        mnuHelp.Size = New System.Drawing.Size(44, 20)
        mnuHelp.Text = "Help"
        ' 
        ' agdMain
        ' 
        agdMain.AllowHorizontalScrolling = True
        agdMain.AllowNewValidValues = False
        agdMain.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        agdMain.CellBackColor = Drawing.Color.Empty
        agdMain.Fixed3D = False
        agdMain.LineColor = Drawing.Color.Empty
        agdMain.LineWidth = 0F
        agdMain.Location = New System.Drawing.Point(0, 0)
        agdMain.Name = "agdMain"
        agdMain.Size = New System.Drawing.Size(720, 545)
        agdMain.Source = Nothing
        agdMain.TabIndex = 0
        ' 
        ' lblNote
        ' 
        lblNote.AutoSize = True
        lblNote.Dock = DockStyle.Bottom
        lblNote.Location = New System.Drawing.Point(0, 530)
        lblNote.Name = "lblNote"
        lblNote.Size = New System.Drawing.Size(500, 15)
        lblNote.TabIndex = 1
        lblNote.Text = "Note: Could not complete analysis for all values. Review source data for missing or zero flows."
        lblNote.Visible = False
        ' 
        ' frmDisplayFrequencyGrid
        ' 
        AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        ClientSize = New System.Drawing.Size(720, 545)
        Controls.Add(lblNote)
        Controls.Add(agdMain)
        Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
        Name = "frmDisplayFrequencyGrid"
        Text = "Frequency Statistics"
        MainMenu1.ResumeLayout(False)
        MainMenu1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub

#End Region

    Private Sub PopulateGrid()
        If Not pInitializing Then
            Dim lContinue As Boolean = True
            pSource = New atcFrequencyGridSource(pDataGroup, pNday, pReturns, pConditions)
            'If pSource.Columns < 3 Then
            '    lContinue = UserSpecifyAttributes()
            '    If lContinue Then
            '        pSource = New atcFrequencyGridSource(pDataGroup)
            '    End If
            'End If

            If lContinue Then
                pSource.High = mnuViewHigh.Checked

                pSwapperSource = New atcControls.atcGridSourceRowColumnSwapper(pSource)
                pSwapperSource.SwapRowsColumns = mnuViewRows.Checked

                agdMain.Initialize(pSwapperSource)
                agdMain.SizeAllColumnsToContents()

                SizeToGrid()

                agdMain.Refresh()
                Dim lCouldNotComputeAny As Boolean = False
                For lRow As Integer = 0 To pSource.Rows - 1
                    For lColumn As Integer = 0 To pSource.Columns - 1
                        If pSource.CellValue(lRow, lColumn).Equals(atcFrequencyGridSource.CouldNotComputeText) Then
                            lCouldNotComputeAny = True
                            Exit For
                        End If
                    Next
                    If lCouldNotComputeAny Then Exit For
                Next
                If lCouldNotComputeAny Then
                    lblNote.Visible = True
                    agdMain.Height = Me.ClientRectangle.Height - agdMain.Top - lblNote.Height * 1.5
                Else
                    lblNote.Visible = False
                    agdMain.Height = Me.ClientRectangle.Height - agdMain.Top
                End If

            Else 'user cancelled Frequency Grid specs form
                Me.Close()
            End If
        End If
    End Sub

    Private Sub mnuAnalysis_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAnalysis.Click
        If sender.Text = "Graph" Then
            Try
                If SWSTATform Is Nothing Then
                    Dim lGraphPlugin As New atcGraph.atcGraphPlugin
                    Dim lGraphForm As atcGraph.atcGraphForm = lGraphPlugin.Show(pDataGroup, "Frequency")
                Else
                    SWSTATform.DoFrequencyGraph()
                End If
            Catch ex As Exception
                MapWinUtility.Logger.Msg("Create frequency graph from main SWSTAT form" & vbCrLf & ex.Message, "Unable to create frequency graph")
            End Try
        Else
            atcDataManager.ShowDisplay(sender.Text, pDataGroup, Me.Icon)
        End If
    End Sub

    Private Sub mnuEditCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditCopy.Click
        Clipboard.SetDataObject(Me.ToString)
    End Sub

    Private Sub mnuFileSaveGrid_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSaveGrid.Click
        Dim lSaveDialog As New System.Windows.Forms.SaveFileDialog
        With lSaveDialog
            .Title = "Save Grid As"
            .DefaultExt = ".txt"
            .FileName = ReplaceString(Me.Text, " ", "_") & "_grid.txt"
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If
            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                SaveFileString(.FileName, Me.ToString)
            End If
        End With
    End Sub

    Private Sub mnuFileSaveReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSaveReport.Click
        Dim lSaveDialog As New System.Windows.Forms.SaveFileDialog
        With lSaveDialog
            .Title = "Save Frequency Report As"
            .DefaultExt = ".txt"
            .FileName = GetNewFileName(SafeFilename(ReplaceString(Me.Text, " ", "_") & "_report"), ".txt")
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If
            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                SaveFileString(.FileName, CreateReport)
                OpenFile(.FileName)
            End If
        End With
    End Sub

    Public Function CreateReport(Optional ByVal aExpFmt As Boolean = False) As String
        Return pSource.CreateReport(aExpFmt)
    End Function

    Private Sub pDataGroup_Added(ByVal aAdded As atcCollection) Handles pDataGroup.Added
        PopulateGrid()
        'TODO: could efficiently insert newly added item(s)
    End Sub

    Private Sub pDataGroup_Removed(ByVal aRemoved As atcCollection) Handles pDataGroup.Removed
        PopulateGrid()
        'TODO: could efficiently remove by serial number
    End Sub

    Private Sub mnuViewColumns_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewColumns.Click
        SwapRowsColumns = False
    End Sub

    Private Sub mnuViewRows_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewRows.Click
        SwapRowsColumns = True
    End Sub

    Private Sub mnuViewHigh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuViewHigh.Click
        HighDisplay = True
    End Sub

    Private Sub mnuViewLow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuViewLow.Click
        HighDisplay = False
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Text & vbCrLf & agdMain.ToString
    End Function

    'True for rows and columns to be swapped, false for normal orientation
    Public Property SwapRowsColumns() As Boolean
        Get
            Return pSwapperSource.SwapRowsColumns
        End Get
        Set(ByVal newValue As Boolean)
            If pSwapperSource.SwapRowsColumns <> newValue Then
                pSwapperSource.SwapRowsColumns = newValue
                SizeToGrid()
                agdMain.Refresh()
            End If
            mnuViewRows.Checked = newValue
            mnuViewColumns.Checked = Not newValue
        End Set
    End Property

    Public Property HighDisplay() As Boolean
        Get
            Return pSource.High
        End Get
        Set(ByVal newValue As Boolean)
            mnuViewHigh.Checked = newValue
            mnuViewLow.Checked = Not newValue
            Me.Text = "Frequency Statistics"
            If Not pSource Is Nothing AndAlso pSource.High <> newValue Then
                pSource.High = newValue
                agdMain.SizeAllColumnsToContents()
                SizeToGrid()
                agdMain.Refresh()
            End If
        End Set
    End Property

    Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
        pDataGroup = Nothing
        pSource = Nothing
    End Sub

    Private Sub mnuSizeColumnsToContents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSizeColumnsToContents.Click
        agdMain.SizeAllColumnsToContents()
        SizeToGrid()
        agdMain.Refresh()
    End Sub

    Private Sub mnuHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelp.Click
        If Application.ProductName = "USGSHydroToolbox" Then
            ShowHelp("SW-Tools.html")
        Else
            ShowHelp("BASINS Details\Analysis\USGS Surface Water Statistics.html")
        End If
    End Sub

    Private Sub mnuFileSaveViewNDay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSaveViewNDay.Click
        atcDataManager.UserSelectDisplay("N-Day timeseries", pSource.AllNday)
    End Sub

    Public Sub SizeToGrid()
        Try
            Dim lRequestedHeight As Integer = Me.Height - agdMain.Height + pSwapperSource.Rows * agdMain.RowHeight(0)
            Dim lRequestedWidth As Integer = Me.Width - agdMain.Width
            For lColumn As Integer = 0 To pSwapperSource.Columns - 1
                lRequestedWidth += agdMain.ColumnWidth(lColumn)
            Next
            Dim lScreenArea As System.Drawing.Rectangle = My.Computer.Screen.WorkingArea

            Width = Math.Min(lScreenArea.Width - 100, lRequestedWidth + 20)
            Height = Math.Min(lScreenArea.Height - 100, lRequestedHeight + 20)

        Catch 'Ignore error if we can't tell how large to make it, or can't rezise
        End Try
    End Sub

    Private Sub mnuFileExportResults_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuFileExportResults.Click
        Dim lSaveDialog As New System.Windows.Forms.SaveFileDialog
        With lSaveDialog
            .Title = "Export Frequency Results As"
            .DefaultExt = ".txt"
            .FileName = ReplaceString(Me.Text, " ", "_") & "_export.txt"
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If
            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                SaveFileString(.FileName, pSource.CreateReport(True))
                OpenFile(.FileName)
            End If
        End With

    End Sub
End Class
