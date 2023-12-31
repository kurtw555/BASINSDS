Option Strict Off
Option Explicit On

Imports MapWinUtility.Strings
Imports System.Windows.Forms

Friend Class frmSelectScript
    Inherits System.Windows.Forms.Form
    'Copyright 2010 by AQUA TERRA Consultants

    Public SelectedScript As String
    Public ButtonPressed As String
    Private pDataFilename As String
    Private pCurrentRow As Integer = 1
    Private CanReadBackColor As Drawing.Color = Drawing.Color.LightGreen 'Drawing.Color.FromArgb(11861940) 'RGB(180, 255, 180)
    Private NotReadableBackColor As Drawing.Color = Drawing.Color.Red

    Private Sub agdScripts_MouseDownCell(ByVal aGrid As atcControls.atcGrid, ByVal aRow As Integer, ByVal aColumn As Integer) Handles agdScripts.MouseDownCell
        SetSelectedRow(aRow)
    End Sub

    Private Sub frmKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyValue = System.Windows.Forms.Keys.F1 Then
            btnHelp_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        If Application.ProductName = "USGSHydroToolbox" Then
            atcUtility.ShowHelp("Tutorials/Read Data with Script.html")
        Else
            atcUtility.ShowHelp("BASINS Details/Plug-ins/Time-Series Plug-ins/Read Data with Script.html")
        End If
        'atcUtility.ShowHelp("SW Toolbox Details/Plug-ins/Time-Series Plug-ins/Read Data with Script.html")
        'atcUtility.ShowHelp("GW Toolbox Details/Plug-ins/Time-Series Plug-ins/Read Data with Script.html")
    End Sub

    Private Sub SetSelectedRow(ByVal aRow As Integer)
        pCurrentRow = aRow
        SelectedScript = agdScripts.Source.CellValue(aRow, 1)
        Dim lLastRow As Integer = agdScripts.Source.Rows - 1
        Dim lLastColumn As Integer = agdScripts.Source.Columns - 1
        For lRow As Integer = 1 To lLastRow
            For lColumn As Integer = 1 To lLastColumn
                If lRow = aRow Then
                    agdScripts.Source.CellSelected(lRow, lColumn) = True
                Else
                    agdScripts.Source.CellSelected(lRow, lColumn) = False
                End If
            Next
        Next
        agdScripts.Refresh()
        EnableButtons()
    End Sub

    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
        ButtonPressed = cmdCancel.Text
        Me.Hide()
    End Sub

    Private Sub cmdFind_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdFind.Click
        Dim bgColor As Drawing.Color
        ButtonPressed = cmdFind.Text
        dlgOpenFileOpen.Filter = "Wizard Script Files (*.ws)|*.ws|All Files (*.*)|*.*"
        dlgOpenFileOpen.DefaultExt = "ws"
        dlgOpenFileOpen.Title = "Open Script File"
        dlgOpenFileOpen.ShowDialog()
        Dim ScriptFilename, ScriptDescription As String
        Dim Script As clsATCscriptExpression
        If dlgOpenFileOpen.FileName <> "" Then
            ScriptFilename = dlgOpenFileOpen.FileName
            Script = ScriptFromString(WholeFileString(ScriptFilename))
            If Script Is Nothing Then
                ScriptDescription = Err.Description
                bgColor = NotReadableBackColor
            Else
                ScriptDescription = Script.SubExpression(0).Printable
                Script = Nothing
                SaveSetting("ATCTimeseriesImport", "Scripts", ScriptFilename, ScriptDescription)
                bgColor = TestScriptColor(ScriptFilename)
            End If
            With agdScripts.Source
                Dim lCurRow As Integer

                For lCurRow = 0 To .Rows - 1
                    If .CellValue(lCurRow, 1) = ScriptFilename Then
                        'Already have this script in the grid, skip adding new row
                        GoTo SetProperties
                    End If
                Next
                lCurRow = .Rows
                .Rows = .Rows + 1
SetProperties:
                .CellValue(lCurRow, 0) = ScriptDescription
                .CellColor(lCurRow, 0) = bgColor
                .CellValue(lCurRow, 1) = ScriptFilename
                'TODO: scroll down so this row is visible, translate from old grid code below:
                'While Not .get_RowIsVisible(agdScripts.rows)
                '    .TopRow = .TopRow + 1
                'End While
                SetSelectedRow(lCurRow)
            End With
            EnableButtons()
        End If
    End Sub

    Private Sub cmdHelp_Click()
        MsgBox("Select a script that will recognize the data you are importing. " & vbCr & "If no appropriate script is listed, select a similar one " & vbCr & "and click 'Edit' to create a new script based on it." & vbCr & "'Run' interprets the selected script and imports your data." & vbCr & "'Edit' reads the selected script and presents an interface for customizing it." & vbCr & "      Note: some complex scripts use features that can not yet be edited in the graphical " & vbCr & "      interface. These scripts may be edited manually as text files before pressing 'Run'. " & vbCr & "'Find' browses your disk for new scripts that are not in the list." & vbCr & "'Forget' removes the selected script from the list, but leaves it on disk." & vbCr & "'Debug' runs the selected script one step at a time." & vbCr & "'Cancel' closes this window without importing any data" & vbCr & "Green scripts have tested the current file and can probably read it." & vbCr & "Pink scripts have tested the current file and probably can't read it." & vbCr & "Red scripts contain errors or cannot be found on disk." & vbCr & "Other scripts are unable to test files for readability.", MsgBoxStyle.OkOnly, "Help for Script Selection")
    End Sub

    Private Sub cmdDelete_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdDelete.Click
        If pCurrentRow > 1 AndAlso agdScripts.Source.CellValue(pCurrentRow, 1) <> "" Then
            If MsgBox("About to forget script:" & vbCr & "Description: " & agdScripts.Source.CellValue(pCurrentRow, 0) & vbCr & "Filename: " & agdScripts.Source.CellValue(pCurrentRow, 1), MsgBoxStyle.YesNo, "Confirm Forget") = MsgBoxResult.Yes Then
                DeleteSetting("ATCTimeseriesImport", "Scripts", agdScripts.Source.CellValue(pCurrentRow, 1))
                LoadGrid(pDataFilename) 'This is inefficient, but easier than copying all the .textmatrix and .cellbackcolor
                agdScripts.Refresh()
            End If
        End If
        EnableButtons()
    End Sub

    Private Sub cmdRun_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdRun.Click
        ButtonPressed = cmdRun.Text
        Me.Hide()
    End Sub

    'Private Sub cmdTest_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
    '    ButtonPressed = cmdTest.Text
    '    Me.Hide()
    'End Sub

    Private Sub cmdWizard_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdWizard.Click
        ButtonPressed = cmdWizard.Text
        Me.Hide()
    End Sub

    Public Sub LoadGrid(Optional ByRef DataFilename As String = "")
        Dim MySettings As String(,)
        Dim intSettings As Integer
        Dim bgColor As Drawing.Color
        Dim CanReadRow As Integer
        Dim RowsFilled As Integer

        pDataFilename = DataFilename
        Dim lSource As New atcControls.atcGridSource

        With lSource
            MySettings = GetAllSettings("ATCTimeseriesImport", "Scripts")
            .Columns = 2
            If Not MySettings Is Nothing Then
                .Rows = UBound(MySettings, 1) - LBound(MySettings, 1) + 3 'the number of scripts found plus title line and first blank line
            End If

            .CellValue(0, 0) = "Description"
            .CellValue(0, 1) = "Script File"
            CanReadRow = 0
            .CellValue(1, 0) = "Blank Script"
            .CellValue(1, 1) = ""
            RowsFilled = 1
            If MySettings Is Nothing Then
                MsgBox("Use the Find button to locate scripts." & vbCr & "Look for the Scripts directory where this program is installed.", MsgBoxStyle.OkOnly, "No Scripts Found Yet")
            Else
                For intSettings = LBound(MySettings, 1) To UBound(MySettings, 1)
                    'Set filename in second column
                    Dim lRow As Integer = RowsFilled + 1
                    If IO.File.Exists(MySettings(intSettings, 0)) Then
                        'Set filename in column 1
                        .CellValue(lRow, 1) = MySettings(intSettings, 0)

                        'Set description in column 0
                        .CellValue(lRow, 0) = MySettings(intSettings, 1).Trim("""")

                        'Set background of cell based on whether this script can read data file
                        bgColor = TestScriptColor(MySettings(intSettings, 0))
                        .CellColor(lRow, 1) = bgColor
                        .CellColor(lRow, 0) = bgColor

                        If bgColor = CanReadBackColor Then CanReadRow = lRow
                        RowsFilled += 1
                    End If
                Next intSettings
            End If
            .Rows = RowsFilled + 1 'only retain non-missing scripts
            .FixedRows = 1
        End With
        agdScripts.Initialize(lSource)
        agdScripts.SizeAllColumnsToContents(agdScripts.Width)
        agdScripts.ColumnWidth(1) = agdScripts.ClientSize.Width - agdScripts.ColumnWidth(0) - 15
        If CanReadRow > 0 Then
            SetSelectedRow(CanReadRow)
        End If
        EnableButtons()
    End Sub

    Private Function TestScriptColor(ByRef ScriptFilename As String) As Drawing.Color
        Dim Script As clsATCscriptExpression
        Dim TestResult As String
        Dim ScriptString As String
        Dim lColor As Drawing.Color = NotReadableBackColor
        Try
            With agdScripts
                lColor = .CellBackColor
                If pDataFilename <> "" Then
                    If IO.File.Exists(ScriptFilename) Then
                        ScriptString = WholeFileString(ScriptFilename)
                        If InStr(ScriptString, "(Test ") > 0 Then
                            Script = ScriptFromString(ScriptString)
                            If Script IsNot Nothing Then
                                TestResult = ScriptTest(Script, pDataFilename)
                                Select Case TestResult
                                    Case "0" : lColor = NotReadableBackColor ' No, this script can not read this data file
                                    Case "1" : lColor = CanReadBackColor ' Yes, this script can read this data file
                                    Case Else : MsgBox("Script '" & ScriptFilename & "' test says: " & vbCr _
                                                      & TestResult & vbCr & "(Expected 0 or 1)", MsgBoxStyle.OkOnly, "Script Test")
                                End Select
                            End If
                        End If
                    End If
                End If
            End With
        Catch
        End Try
        Return lColor
    End Function

    Private Sub frmSelectScript_Resize(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Resize
        'If VB6.PixelsToTwipsY(Height) > 600 Then agdScripts.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(Height) - 576)
        'If VB6.PixelsToTwipsX(Width) > 450 Then
        '	fraButtons.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(Width) - VB6.PixelsToTwipsX(fraButtons.Width) - 192)
        '	If VB6.PixelsToTwipsX(fraButtons.Left) > 300 Then agdScripts.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(fraButtons.Left) - 228)
        'End If
    End Sub

    Private Sub EnableButtons()
        cmdRun.Enabled = (Me.SelectedScript IsNot Nothing)
        cmdDelete.Enabled = cmdRun.Enabled
        'cmdTest.Enabled = cmdRun.Enabled
    End Sub

End Class