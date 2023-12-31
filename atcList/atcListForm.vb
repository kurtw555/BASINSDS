Imports atcData
Imports atcUtility
Imports MapWinUtility
Imports System.Windows.Forms

Public Class atcListForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()
        InitializeComponent() 'required by Windows Form Designer

        Integer.TryParse(GetSetting("BASINS", "List", "MaxWidth", pMaxWidth), pMaxWidth)
        pFormat = GetSetting("BASINS", "List", "Format", pFormat)
        pExpFormat = GetSetting("BASINS", "List", "ExpFormat", pExpFormat)
        pCantFit = GetSetting("BASINS", "List", "CantFit", pCantFit)
        'Me.Text = GetSetting("BASINS", "List", "Title", Me.Text)
        Integer.TryParse(GetSetting("BASINS", "List", "SignificantDigits", pSignificantDigits), pSignificantDigits)

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
    'Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuAnalysis As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents agdMain As atcControls.atcGrid
    Friend WithEvents mnuAttributeRows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAttributeColumns As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSizeColumnsToContents As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSep1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSep1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSelectAttributes As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFileSelectData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewValues As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFilterNoData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuDateValueFormats As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewValueAttributes As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSaveChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSaveIn As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditAtrributeValues As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEditAddAtrribute As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(atcListForm))
        'Me.MainMenu1 = New System.Windows.Forms.MenuStrip(Me.components)
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip()
        'Me.mnuFile = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSelectData = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSelectAttributes = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSep1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveIn = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFileSave = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditCopy = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditAtrributeValues = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEditAddAtrribute = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAttributeRows = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAttributeColumns = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSep1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSizeColumnsToContents = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewValues = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewValueAttributes = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFilterNoData = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuDateValueFormats = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAnalysis = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.agdMain = New atcControls.atcGrid
        Me.SuspendLayout()
        '
        'MainMenu1
        '
        'Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuAnalysis, Me.mnuHelp})
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuAnalysis, Me.mnuHelp})
        '
        'mnuFile
        '
        'Me.mnuFile.Index = 0
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuFileSelectData, Me.mnuFileSelectAttributes, Me.mnuFileSep1, Me.mnuSaveChanges, Me.mnuSaveIn, Me.mnuFileSave})
        Me.mnuFile.Text = "File"
        '
        'mnuFileSelectData
        '
        'Me.mnuFileSelectData.Index = 0
        Me.mnuFileSelectData.Text = "Select &Data"
        '
        'mnuFileSelectAttributes
        '
        ' Me.mnuFileSelectAttributes.Index = 1
        Me.mnuFileSelectAttributes.Text = "Select &Attributes"
        '
        'mnuFileSep1
        '
        'Me.mnuFileSep1.Index = 2
        Me.mnuFileSep1.Text = "-"
        '
        'mnuSaveChanges
        '
        'Me.mnuSaveChanges.Index = 3
        Me.mnuSaveChanges.Text = "Save Changes"
        '
        'mnuSaveIn
        '
        ' Me.mnuSaveIn.Index = 4
        Me.mnuSaveIn.Text = "Save In..."
        '
        'mnuFileSave
        '
        ' Me.mnuFileSave.Index = 5
        Me.mnuFileSave.ShortcutKeys = System.Windows.Forms.Shortcut.CtrlS
        Me.mnuFileSave.Text = "Save Grid As Text"
        '
        'mnuEdit
        '
        'Me.mnuEdit.Index = 1
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuEditCopy, Me.mnuEditAtrributeValues, Me.mnuEditAddAtrribute})
        Me.mnuEdit.Text = "Edit"
        '
        'mnuEditCopy
        '
        'Me.mnuEditCopy.Index = 0
        Me.mnuEditCopy.ShortcutKeys = System.Windows.Forms.Shortcut.CtrlC
        Me.mnuEditCopy.Text = "Copy"
        '
        'mnuEditAtrributeValues
        '
        'Me.mnuEditAtrributeValues.Index = 1
        Me.mnuEditAtrributeValues.Text = "Allow Editing Attribute Values"
        '
        'mnuEditAddAtrribute
        '
        'Me.mnuEditAddAtrribute.Index = 2
        Me.mnuEditAddAtrribute.Text = "Add Attribute"
        '
        'mnuView
        '
        'Me.mnuView.Index = 2
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.mnuAttributeRows, Me.mnuAttributeColumns, Me.mnuViewSep1, Me.mnuSizeColumnsToContents, Me.mnuViewValues, Me.mnuViewValueAttributes, Me.mnuFilterNoData, Me.mnuDateValueFormats})
        Me.mnuView.Text = "View"
        '
        'mnuAttributeRows
        '
        Me.mnuAttributeRows.Checked = True
        'Me.mnuAttributeRows.Index = 0
        Me.mnuAttributeRows.Text = "Attribute Rows"
        '
        'mnuAttributeColumns
        '
        'Me.mnuAttributeColumns.Index = 1
        Me.mnuAttributeColumns.Text = "Attribute Columns"
        '
        'mnuViewSep1
        '
        'Me.mnuViewSep1.Index = 2
        Me.mnuViewSep1.Text = "-"
        '
        'mnuSizeColumnsToContents
        '
        'Me.mnuSizeColumnsToContents.Index = 3
        Me.mnuSizeColumnsToContents.Text = "Size Columns To Contents"
        '
        'mnuViewValues
        '
        Me.mnuViewValues.Checked = True
        'Me.mnuViewValues.Index = 4
        Me.mnuViewValues.Text = "Time Series Values"
        '
        'mnuViewValueAttributes
        '
        'Me.mnuViewValueAttributes.Index = 5
        Me.mnuViewValueAttributes.Text = "Value Attributes"
        '
        'mnuFilterNoData
        '
        Me.mnuFilterNoData.Checked = True
        'Me.mnuFilterNoData.Index = 6
        Me.mnuFilterNoData.Text = "Filter NoData"
        '
        'mnuDateValueFormats
        '
        'Me.mnuDateValueFormats.Index = 7
        Me.mnuDateValueFormats.Text = "Date and Value Formats..."
        '
        'mnuAnalysis
        '
        ' Me.mnuAnalysis.Index = 3
        Me.mnuAnalysis.Text = "Analysis"
        '
        'mnuHelp
        '
        'Me.mnuHelp.Index = 4
        Me.mnuHelp.ShortcutKeys = System.Windows.Forms.Shortcut.F1
        Me.mnuHelp.Text = "Help"
        '
        'agdMain
        '
        Me.agdMain.AllowHorizontalScrolling = True
        Me.agdMain.AllowNewValidValues = False
        Me.agdMain.CellBackColor = System.Drawing.Color.Empty
        Me.agdMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.agdMain.Fixed3D = False
        Me.agdMain.LineColor = System.Drawing.Color.Empty
        Me.agdMain.LineWidth = 0.0!
        Me.agdMain.Location = New System.Drawing.Point(0, 0)
        Me.agdMain.Name = "agdMain"
        Me.agdMain.Size = New System.Drawing.Size(528, 525)
        Me.agdMain.Source = Nothing
        Me.agdMain.TabIndex = 0
        '
        'atcListForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(528, 525)
        Me.Controls.Add(Me.agdMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenu1 = Me.MainMenu1
        Me.Name = "atcListForm"
        Me.Text = "Time Series List"
        Me.ResumeLayout(False)

    End Sub

#End Region

    'The group of atcTimeseries displayed
    Private WithEvents pDataGroup As atcTimeseriesGroup

    Private pDateFormat As New atcDateFormat(GetSetting("BASINS", "List", "DateFormat"))

    'Value formatting options, can be overridden by timeseries attributes
    Private pMaxWidth As Integer = 10
    Private pFormat As String = "#,##0.########"
    Private pExpFormat As String = "#.#e#"
    Private pCantFit As String = "#"
    Private pSignificantDigits As Integer = 5

    Private pEditedText As String = " (Edited)"
    Private pEditedGroup As New atcTimeseriesGroup

    'Translator class between pDataGroup and agdMain
    Private pSource As atcTimeseriesGridSource
    Private pDisplayAttributes As Generic.List(Of String)
    Private pSwapperSource As atcControls.atcGridSourceRowColumnSwapper
    Private pHeaders As Generic.List(Of String)

    Public HAlignment As atcControls.atcAlignment = atcControls.atcAlignment.HAlignRight

    Public Sub Initialize(Optional ByVal aTimeseriesGroup As atcData.atcTimeseriesGroup = Nothing,
                          Optional ByVal aDisplayAttributes As Generic.List(Of String) = Nothing,
                          Optional ByVal aShowValues As Boolean = True,
                          Optional ByVal aFilterNoData As Boolean = False,
                          Optional ByVal aShowForm As Boolean = True,
                          Optional ByVal aHeaders As Generic.List(Of String) = Nothing)
        If aTimeseriesGroup Is Nothing Then
            pDataGroup = New atcTimeseriesGroup
        Else
            pDataGroup = aTimeseriesGroup
        End If

        If aDisplayAttributes Is Nothing Then
            pDisplayAttributes = atcDataManager.DisplayAttributes
        Else
            pDisplayAttributes = aDisplayAttributes
        End If

        pHeaders = aHeaders
        If pHeaders Is Nothing Then
            pHeaders = New Generic.List(Of String)()
        End If

        If aShowForm Then
            Dim DisplayPlugins As ICollection = atcDataManager.GetPlugins(GetType(atcDataDisplay))
            For Each lDisp As atcDataDisplay In DisplayPlugins
                Dim lMenuText As String = lDisp.Name
                If lMenuText.StartsWith("Analysis::") Then lMenuText = lMenuText.Substring(10)
                '### need to fix
                'mnuAnalysis.DropDownItems.Add(lMenuText, New EventHandler(AddressOf mnuAnalysis_Click))
            Next
        End If

        If pDataGroup.Count = 0 Then 'ask user to specify some timeseries
            pDataGroup = atcDataManager.UserSelectData("", pDataGroup, Nothing, True, True, Me.Icon)
        End If

        If pDataGroup.Count > 0 Then
            If aShowForm Then Me.Show()
            mnuViewValues.Checked = aShowValues
            mnuFilterNoData.Checked = aFilterNoData
            PopulateGrid()
        Else 'user declined to specify timeseries
            Me.Close()
        End If

    End Sub

    Public Property DateFormat() As atcDateFormat
        Get
            Return pDateFormat
        End Get
        Set(ByVal newValue As atcDateFormat)
            pDateFormat = newValue
            If pSource IsNot Nothing Then
                pSource.DateFormat = pDateFormat
                If agdMain IsNot Nothing Then agdMain.Refresh()
            End If
        End Set
    End Property

    Public Sub ValueFormat(Optional ByVal aMaxWidth As Integer = 10, _
                           Optional ByVal aFormat As String = "#,##0.########", _
                           Optional ByVal aExpFormat As String = "#.#e#", _
                           Optional ByVal aCantFit As String = "#", _
                           Optional ByVal aSignificantDigits As Integer = 5)
        pMaxWidth = aMaxWidth
        pFormat = aFormat
        pExpFormat = aExpFormat
        pCantFit = aCantFit
        pSignificantDigits = aSignificantDigits
        If pSource IsNot Nothing Then
            pSource.ValueFormat(pMaxWidth, pFormat, pExpFormat, pCantFit, pSignificantDigits)
            If agdMain IsNot Nothing Then agdMain.Refresh()
        End If
    End Sub

    Private Sub PopulateGrid()
        'with timeseries data, a list of attributes and options define a timeseries grid source
        pSource = New atcTimeseriesGridSource(pDataGroup, pDisplayAttributes,
                                              mnuViewValues.Checked,
                                              mnuFilterNoData.Checked,
                                              pHeaders)
        pSource.AttributeValuesEditable = mnuEditAtrributeValues.Checked
        pSource.DisplayValueAttributes = mnuViewValueAttributes.Checked
        With pSource
            .DateFormat = pDateFormat
            .ValueFormat(pMaxWidth, pFormat, pExpFormat, pCantFit, pSignificantDigits)
        End With

        pSwapperSource = New atcControls.atcGridSourceRowColumnSwapper(pSource)
        pSwapperSource.SwapRowsColumns = mnuAttributeColumns.Checked

        agdMain.Initialize(pSwapperSource)
        If HAlignment = atcControls.atcAlignment.HAlignLeft Then
            'ToDo: figure out how to set cell value to be left justified
            'SetAlignment()
        End If
        'TODO: could SizeAllColumnsToContents return total width?
        agdMain.SizeAllColumnsToContents()

        SizeToGrid()
        agdMain.Refresh()
    End Sub

    Private Sub SetAlignment()
        If pSwapperSource IsNot Nothing Then
            With pSwapperSource
                For lrow As Integer = 0 To .Rows - 1
                    For lcol As Integer = 1 To .Columns - 1
                        .Alignment(lrow, lcol) = HAlignment
                    Next
                Next
            End With
        End If
    End Sub

    Private Function GetIndex(ByVal aName As String) As Integer
        Return CInt(Mid(aName, InStr(aName, "#") + 1))
    End Function

    Private Sub mnuAnalysis_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAnalysis.Click
        atcDataManager.ShowDisplay(sender.Text, pDataGroup, Me.Icon)
    End Sub

    Private Sub pDataGroup_Added(ByVal aAdded As atcCollection) Handles pDataGroup.Added
        If Me.Visible Then PopulateGrid()
        'TODO: could efficiently insert newly added item(s)
    End Sub

    Private Sub agdMain_CellEdited(ByVal aGrid As atcControls.atcGrid, ByVal aRow As Integer, ByVal aColumn As Integer) Handles agdMain.CellEdited
        Dim lTs As atcTimeseries = Nothing
        Dim lIsValue As Boolean = True
        Dim lValueAttDef As atcAttributeDefinition = Nothing
        pSource.CellDataset(aColumn, lTs, lIsValue, lValueAttDef)
        If lTs IsNot Nothing Then
            If Not pEditedGroup.Contains(lTs) Then
                pEditedGroup.Add(lTs)
                agdMain.Refresh()
                If Not Me.Text.EndsWith(pEditedText) Then Me.Text &= pEditedText
            End If
        End If
    End Sub

    'Private Sub pDataGroup_Edited(ByVal aEdited As atcData.atcDataSet) Handles pDataGroup.Edited
    '    If Not Me.Text.EndsWith(pEditedText) Then Me.Text &= pEditedText
    '    If Not pEditedGroup.Contains(aEdited) Then pEditedGroup.Add(aEdited)
    '    agdMain.Refresh()
    'End Sub

    Private Sub pDataGroup_Removed(ByVal aRemoved As atcCollection) Handles pDataGroup.Removed
        If Me.Visible Then PopulateGrid()
        'TODO: could efficiently remove by serial number
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
        If pEditedGroup.Count > 0 Then
            Select Case MapWinUtility.Logger.Msg("Data was edited, save changes?", MsgBoxStyle.YesNoCancel, "BASINS Data")
                Case MsgBoxResult.Cancel
                    e.Cancel = True
                    Return
                Case MsgBoxResult.Yes
                    mnuSaveChanges_Click(Nothing, Nothing)
            End Select
        End If
        pDataGroup = Nothing
        pSource = Nothing
    End Sub

    Private Sub mnuAttributeRows_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAttributeRows.Click
        SwapRowsColumns = False
    End Sub

    Private Sub mnuAttributeColumns_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAttributeColumns.Click
        SwapRowsColumns = True
    End Sub

    Private Sub mnuEditCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditCopy.Click
        Clipboard.SetDataObject(Me.ToString)
    End Sub

    Private Sub mnuFileSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSave.Click
        Dim lSaveDialog As New System.Windows.Forms.SaveFileDialog
        With lSaveDialog
            .Title = "Save Grid As"
            .DefaultExt = ".txt"
            .FileName = ReplaceString(Me.Text, " ", "_") & ".txt"
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If
            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                SaveFileString(.FileName, Me.ToString)
            End If
        End With
    End Sub

    Private Sub mnuFileSelectAttributes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSelectAttributes.Click
        'Dim lst As New atcControls.atcSelectList
        'Dim lAvailable As New Generic.List(Of String)
        'For Each lAttrDef As atcAttributeDefinition In atcDataAttributes.AllDefinitions
        '    If lAttrDef.Displayable Then lAvailable.Add(lAttrDef.Name)
        'Next
        ''Add any current display attributes not in atcDataAttributes.AllDefinitions
        'For Each lAttrName As String In pDisplayAttributes
        '    If Not lAvailable.Contains(lAttrName) Then
        '        lAvailable.Add(lAttrName)
        '    End If
        'Next
        'lAvailable.Sort()
        'If lst.AskUser(lAvailable, pDisplayAttributes) Then
        '    'TODO: set project modified flag?
        '    PopulateGrid()
        'End If
        Dim lSelector As New frmSelectAttributes()
        If lSelector.AskUser(pDataGroup, pDisplayAttributes) Then
            PopulateGrid()
        End If
    End Sub

    Private Sub mnuFileSelectData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileSelectData.Click
        atcDataManager.UserSelectData("", pDataGroup, Nothing, False, True, Me.Icon)
    End Sub

    Private Sub mnuSizeColumnsToContents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSizeColumnsToContents.Click
        agdMain.SizeAllColumnsToContents()
        SizeToGrid()
        agdMain.Refresh()
    End Sub

    Private Sub mnuViewValues_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewValues.Click
        mnuViewValues.Checked = Not mnuViewValues.Checked
        PopulateGrid()
    End Sub

    Private Sub mnuFilterNoData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFilterNoData.Click
        mnuFilterNoData.Checked = Not mnuFilterNoData.Checked
        PopulateGrid()
    End Sub

    Private Sub mnuViewValueAttributes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewValueAttributes.Click
        mnuViewValueAttributes.Checked = Not mnuViewValueAttributes.Checked
        DisplayValueAttributes = mnuViewValueAttributes.Checked
    End Sub

    Public Property DisplayValueAttributes() As Boolean
        Get
            Return pSource.DisplayValueAttributes
        End Get
        Set(ByVal newValue As Boolean)
            mnuViewValueAttributes.Checked = newValue
            pSource.DisplayValueAttributes = newValue
            agdMain.SizeAllColumnsToContents()
            SizeToGrid()
            agdMain.Refresh()
        End Set
    End Property

    'True for attributes in columns, False for attributes in rows
    Public Property SwapRowsColumns() As Boolean
        Get
            Return pSwapperSource.SwapRowsColumns
        End Get
        Set(ByVal newValue As Boolean)
            If pSwapperSource.SwapRowsColumns <> newValue Then
                pSwapperSource.SwapRowsColumns = newValue
                agdMain.SizeAllColumnsToContents()
                SizeToGrid()
                agdMain.Refresh()
            End If
            mnuAttributeColumns.Checked = newValue
            mnuAttributeRows.Checked = Not newValue
        End Set
    End Property

    Public Overrides Function ToString() As String
        Dim lGridText As String = agdMain.ToString()
        With DateFormat
            If .IncludeYears OrElse .IncludeMonths OrElse .IncludeDays OrElse .IncludeHours OrElse .IncludeMinutes OrElse .IncludeSeconds Then
                'There is something in the date field, leave it alone
            Else 'Date field is empty, skip it entirely by removing tabs
                lGridText = lGridText.Replace(vbCrLf & vbTab, vbCrLf)
                If lGridText.StartsWith(vbTab) Then
                    lGridText = lGridText.Substring(1)
                End If
            End If
        End With
        If Me.Text.Trim.Length = 0 Then
            Return lGridText
        Else
            Return Me.Text & vbCrLf & lGridText
        End If
    End Function

    Private Sub ShowHelpForList()
        If System.Reflection.Assembly.GetEntryAssembly.Location.EndsWith("TimeseriesUtility.exe") Then
            ShowHelp("View\List.html")
        ElseIf Application.ProductName = "USGSHydroToolbox" Then
            ShowHelp("Time-Series Tools\List.html")
        Else
            ShowHelp("BASINS Details\Analysis\Time Series Functions\List.html")
        End If
    End Sub

    Private Sub mnuHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelp.Click
        ShowHelpForList()
    End Sub
    Private Sub atcListForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyValue = System.Windows.Forms.Keys.F1 Then
            ShowHelpForList()
        End If
    End Sub

    Private Sub mnuDateValueFormats_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDateValueFormats.Click
        Dim lFrmOptions As New frmOptions
        With lFrmOptions
            .List = Me
            Select Case pDateFormat.DateOrder
                Case atcDateFormat.DateOrderEnum.DayMonthYear : .radioOrderDMY.Checked = True
                Case atcDateFormat.DateOrderEnum.JulianDate : .radioOrderJulian.Checked = True
                Case atcDateFormat.DateOrderEnum.MonthDayYear : .radioOrderMDY.Checked = True
                Case atcDateFormat.DateOrderEnum.YearMonthDay : .radioOrderYMD.Checked = True
            End Select
            .chkYears.Checked = pDateFormat.IncludeYears
            .chkMonths.Checked = pDateFormat.IncludeMonths
            .chkDays.Checked = pDateFormat.IncludeDays
            .chkHours.Checked = pDateFormat.IncludeHours
            .chkMinutes.Checked = pDateFormat.IncludeMinutes
            .chkSeconds.Checked = pDateFormat.IncludeSeconds

            .chk2digitYears.Checked = pDateFormat.TwoDigitYears
            .chkMidnight24.Checked = pDateFormat.Midnight24
            .chkMonthNames.Checked = pDateFormat.MonthNames

            .txtDateSeparator.Text = pDateFormat.DateSeparator
            .txtTimeSeparator.Text = pDateFormat.TimeSeparator

            .txtFormat.Text = pFormat
            .txtExpFormat.Text = pExpFormat
            .txtSignificantDigits.Text = pSignificantDigits
            .txtMaxWidth.Text = pMaxWidth
            .txtCantFit.Text = pCantFit
            .txtTitle.Text = Me.Text

            .SaveState()
            .Icon = Me.Icon
            .Show(Me)
        End With
    End Sub

    Friend Sub SetOptions(ByVal aFrmOptions As frmOptions)
        If aFrmOptions IsNot Nothing Then
            With aFrmOptions
                If .radioOrderDMY.Checked Then pDateFormat.DateOrder = atcDateFormat.DateOrderEnum.DayMonthYear
                If .radioOrderJulian.Checked Then pDateFormat.DateOrder = atcDateFormat.DateOrderEnum.JulianDate
                If .radioOrderMDY.Checked Then pDateFormat.DateOrder = atcDateFormat.DateOrderEnum.MonthDayYear
                If .radioOrderYMD.Checked Then pDateFormat.DateOrder = atcDateFormat.DateOrderEnum.YearMonthDay

                pDateFormat.IncludeYears = .chkYears.Checked
                pDateFormat.IncludeMonths = .chkMonths.Checked
                pDateFormat.IncludeHours = .chkHours.Checked
                pDateFormat.IncludeMinutes = .chkMinutes.Checked
                pDateFormat.IncludeSeconds = .chkSeconds.Checked
                pDateFormat.IncludeDays = .chkDays.Checked

                pDateFormat.TwoDigitYears = .chk2digitYears.Checked
                pDateFormat.Midnight24 = .chkMidnight24.Checked
                pDateFormat.MonthNames = .chkMonthNames.Checked

                pDateFormat.DateSeparator = .txtDateSeparator.Text
                pDateFormat.TimeSeparator = .txtTimeSeparator.Text

                pFormat = .txtFormat.Text
                pExpFormat = .txtExpFormat.Text
                Integer.TryParse(.txtSignificantDigits.Text, pSignificantDigits)
                Integer.TryParse(.txtMaxWidth.Text, pMaxWidth)
                pCantFit = .txtCantFit.Text
                'PopulateGrid()
                With pSource
                    .DateFormat = pDateFormat
                    .ValueFormat(pMaxWidth, pFormat, pExpFormat, pCantFit, pSignificantDigits)
                End With
                Me.Text = .txtTitle.Text
                agdMain.SizeAllColumnsToContents()
                SizeToGrid()
                agdMain.Refresh()

                SaveSetting("BASINS", "List", "DateFormat", pDateFormat.ToString)
                SaveSetting("BASINS", "List", "Title", Me.Text.Replace(pEditedText, ""))
                SaveSetting("BASINS", "List", "MaxWidth", pMaxWidth)
                SaveSetting("BASINS", "List", "Format", pFormat)
                SaveSetting("BASINS", "List", "ExpFormat", pExpFormat)
                SaveSetting("BASINS", "List", "CantFit", pCantFit)
                SaveSetting("BASINS", "List", "SignificantDigits", pSignificantDigits)
            End With
        End If
    End Sub

    Public Sub SizeToGrid()
        Try
            Dim lRequestedHeight As Integer = (Me.Height - agdMain.Height) + pSwapperSource.Rows * agdMain.RowHeight(0)
            Dim lRequestedWidth As Integer = (Me.Width - agdMain.Width)
            For lColumn As Integer = 0 To pSwapperSource.Columns - 1
                lRequestedWidth += agdMain.ColumnWidth(lColumn)
            Next
            Dim lScreenArea As System.Drawing.Rectangle = My.Computer.Screen.WorkingArea

            Width = Math.Min(lScreenArea.Width - 100, lRequestedWidth + 20)
            Height = Math.Min(lScreenArea.Height - 100, lRequestedHeight + 20)
        Catch 'Ignore error if we can't tell how large to make it, or can't rezise
        End Try
    End Sub

    Private Sub mnuSaveChanges_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSaveChanges.Click
        Dim lNotSaved As New atcTimeseriesGroup
        For Each lTs As atcTimeseries In pEditedGroup
            Dim lSaved As Boolean = False
            Dim lSource As atcDataSource = atcDataManager.DataSourceBySpecification(lTs.Attributes.GetValue("Data Source"))
            If lSource IsNot Nothing AndAlso lSource.CanSave Then
                lSaved = lSource.AddDataSet(lTs, atcDataSource.EnumExistAction.ExistReplace)
            End If
            If Not lSaved Then
                lNotSaved.Add(lTs)
            End If
        Next

        'Second chance to save any that could not be saved in their original file
        SaveAs(lNotSaved)

        pEditedGroup.Clear()
        Me.Text = Me.Text.Replace(pEditedText, "")
    End Sub

    Private Sub mnuSaveIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSaveIn.Click
        SaveAs(pDataGroup)
        Me.Text = Me.Text.Replace(pEditedText, "")
    End Sub

    Private Sub SaveAs(ByVal aDataGroup As atcTimeseriesGroup)
        If aDataGroup IsNot Nothing AndAlso aDataGroup.Count > 0 Then
            Dim lFormSave As New frmSaveData
            Dim lSaveSource As atcDataSource = lFormSave.AskUser(aDataGroup)
            If lSaveSource IsNot Nothing AndAlso Not String.IsNullOrEmpty(lSaveSource.Specification) Then
                lSaveSource.AddDataSets(aDataGroup)
            End If
        End If
    End Sub

    Private Sub mnuEditAtrributeValues_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditAtrributeValues.Click
        pSource.AttributeValuesEditable = Not pSource.AttributeValuesEditable
        mnuEditAtrributeValues.Checked = pSource.AttributeValuesEditable
        agdMain.Refresh()
    End Sub

    Private Sub mnuEditAddAtrribute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditAddAtrribute.Click

        Dim lAttributeName As String = InputBox("Attribute Name:", "Add Attribute", "").Trim()
        If lAttributeName.Length > 0 Then
            If Not pDisplayAttributes.Contains(lAttributeName) Then
                Dim lDefinition As atcAttributeDefinition = atcDataAttributes.GetDefinition(lAttributeName, True)
                If Not pDisplayAttributes.Contains(lDefinition.Name) Then
                    pDisplayAttributes.Add(lDefinition.Name)
                    agdMain.Refresh()
                End If
            End If
        End If
    End Sub
End Class