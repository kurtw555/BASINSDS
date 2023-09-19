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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDisplayFrequencyGrid))
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSaveGrid = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSaveReport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSaveViewNDay = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileExportResults = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditCopy = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewColumns = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewRows = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSep1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewHigh = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewLow = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSep2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSizeColumnsToContents = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAnalysis = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.agdMain = New atcControls.atcGrid
        Me.lblNote = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'MainMenu1
        '
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuAnalysis, Me.mnuHelp})
        '
        'mnuFile
        '
        Me.mnuFile.MergeIndex = 0
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFileSaveGrid, Me.mnuFileSaveReport, Me.mnuFileSaveViewNDay, Me.mnuFileExportResults})
        Me.mnuFile.Text = "File"
        '
        'mnuFileSaveGrid
        '
        Me.mnuFileSaveGrid.MergeIndex = 0
        Me.mnuFileSaveGrid.ShortcutKeys = System.Windows.Forms.Shortcut.CtrlS
        Me.mnuFileSaveGrid.Text = "Save Grid"
        '
        'mnuFileSaveReport
        '
        Me.mnuFileSaveReport.MergeIndex = 1
        Me.mnuFileSaveReport.Text = "Save Report"
        '
        'mnuFileSaveViewNDay
        '
        Me.mnuFileSaveViewNDay.MergeIndex = 2
        Me.mnuFileSaveViewNDay.Text = "Save/View N-Day"
        '
        'mnuFileExportResults
        '
        Me.mnuFileExportResults.MergeIndex = 3
        Me.mnuFileExportResults.Text = "Export Results"
        '
        'mnuEdit
        '
        Me.mnuEdit.MergeIndex = 1
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuEditCopy})
        Me.mnuEdit.Text = "Edit"
        '
        'mnuEditCopy
        '
        Me.mnuEditCopy.MergeIndex = 0
        Me.mnuEditCopy.ShortcutKeys = System.Windows.Forms.Shortcut.CtrlC
        Me.mnuEditCopy.Text = "Copy"
        '
        'mnuView
        '
        Me.mnuView.MergeIndex = 2
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuViewColumns, Me.mnuViewRows, Me.mnuViewSep1, Me.mnuViewHigh, Me.mnuViewLow, Me.mnuViewSep2, Me.mnuSizeColumnsToContents})
        Me.mnuView.Text = "View"
        '
        'mnuViewColumns
        '
        Me.mnuViewColumns.Checked = True
        Me.mnuViewColumns.MergeIndex = 0
        Me.mnuViewColumns.Text = "Columns"
        '
        'mnuViewRows
        '
        Me.mnuViewRows.MergeIndex = 1
        Me.mnuViewRows.Text = "Rows"
        '
        'mnuViewSep1
        '
        Me.mnuViewSep1.MergeIndex = 2
        Me.mnuViewSep1.Text = "-"
        '
        'mnuViewHigh
        '
        Me.mnuViewHigh.Checked = True
        Me.mnuViewHigh.MergeIndex = 3
        Me.mnuViewHigh.Text = "High"
        '
        'mnuViewLow
        '
        Me.mnuViewLow.MergeIndex = 4
        Me.mnuViewLow.Text = "Low"
        '
        'mnuViewSep2
        '
        Me.mnuViewSep2.MergeIndex = 5
        Me.mnuViewSep2.Text = "-"
        '
        'mnuSizeColumnsToContents
        '
        Me.mnuSizeColumnsToContents.MergeIndex = 6
        Me.mnuSizeColumnsToContents.Text = "Size Columns To Contents"
        '
        'mnuAnalysis
        '
        Me.mnuAnalysis.MergeIndex = 3
        Me.mnuAnalysis.Text = "Analysis"
        '
        'mnuHelp
        '
        Me.mnuHelp.MergeIndex = 4
        Me.mnuHelp.ShortcutKeys = System.Windows.Forms.Shortcut.F1
        Me.mnuHelp.ShowShortcutKeys = False
        Me.mnuHelp.Text = "Help"
        '
        'agdMain
        '
        Me.agdMain.AllowHorizontalScrolling = True
        Me.agdMain.AllowNewValidValues = False
        Me.agdMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.agdMain.CellBackColor = System.Drawing.Color.Empty
        Me.agdMain.Fixed3D = False
        Me.agdMain.LineColor = System.Drawing.Color.Empty
        Me.agdMain.LineWidth = 0.0!
        Me.agdMain.Location = New System.Drawing.Point(0, 0)
        Me.agdMain.Name = "agdMain"
        Me.agdMain.Size = New System.Drawing.Size(720, 545)
        Me.agdMain.Source = Nothing
        Me.agdMain.TabIndex = 0
        '
        'lblNote
        '
        Me.lblNote.AutoSize = True
        Me.lblNote.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblNote.Location = New System.Drawing.Point(0, 528)
        Me.lblNote.Name = "lblNote"
        Me.lblNote.Size = New System.Drawing.Size(598, 17)
        Me.lblNote.TabIndex = 1
        Me.lblNote.Text = "Note: Could not complete analysis for all values. Review source data for missing " & _
            "or zero flows."
        Me.lblNote.Visible = False
        '
        'frmDisplayFrequencyGrid
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.ClientSize = New System.Drawing.Size(720, 545)
        Me.Controls.Add(Me.lblNote)
        Me.Controls.Add(Me.agdMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        'Me.Menu = Me.MainMenu1
        Me.Name = "frmDisplayFrequencyGrid"
        Me.Text = "Frequency Statistics"
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
