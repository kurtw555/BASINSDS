Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Imports atcUtility
Imports ZedGraph

Public Class frmGraphEditor

    Public Event Apply()

    Private pPane As GraphPane
    Private pPaneAux As GraphPane
    Private pZgc As ZedGraphControl
    Private pDateFormat As atcDateFormat
    Private pSettingControls As Integer = 0

    ''' <summary>
    ''' Show this form for customizing the specified ZedGraphControl
    ''' </summary>
    ''' <param name="aZgc">graph control to edit</param>
    ''' <remarks></remarks>
    <CLSCompliant(False)> _
    Public Sub Edit(ByVal aZgc As ZedGraphControl)
        Dim lAutoApply As Boolean = chkAutoApply.Checked
        chkAutoApply.Checked = False

        If pDateFormat Is Nothing Then
            pDateFormat = New atcDateFormat
            With pDateFormat
                .IncludeHours = False
                .IncludeMinutes = False
                .IncludeSeconds = False
            End With
        End If

        pZgc = aZgc
        pPane = aZgc.MasterPane.PaneList(aZgc.MasterPane.PaneList.Count - 1)
        If aZgc.MasterPane.PaneList.Count > 1 Then
            pPaneAux = aZgc.MasterPane.PaneList(0)
        End If
        'Me.cboCurveStep.Items.Add("Smooth") 'Smooth looks pretty but makes the data look smoother than it really is
        Me.cboCurveStep.Items.AddRange(System.Enum.GetNames(GetType(ZedGraph.StepType)))
        Me.cboCurveStyle.Items.AddRange(System.Enum.GetNames(GetType(System.Drawing.Drawing2D.DashStyle)))
        Dim lCustomPos As Integer = Me.cboCurveStyle.Items.IndexOf("Custom")
        If lCustomPos > -1 Then Me.cboCurveStyle.Items.RemoveAt(lCustomPos)
        Me.cboCurveSymbolType.Items.AddRange(System.Enum.GetNames(GetType(ZedGraph.SymbolType)))

        SetComboFromCurves()
        SetComboFromTexts()
        SetControlsFromPane()

        RemoveHandler pZgc.ZoomEvent, AddressOf pZgc_ZoomEvent
        RemoveHandler pZgc.MouseClick, AddressOf pZgc_MouseClick

        AddHandler pZgc.ZoomEvent, AddressOf pZgc_ZoomEvent
        AddHandler pZgc.MouseClick, AddressOf pZgc_MouseClick

        Me.Show()
        chkAutoApply.Checked = lAutoApply
    End Sub

    Private Sub frmGraphEditor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler pZgc.ZoomEvent, AddressOf pZgc_ZoomEvent
        RemoveHandler pZgc.MouseClick, AddressOf pZgc_MouseClick
    End Sub

    Private Sub SetComboFromCurves()
        pSettingControls += 1
        comboWhichCurve.Items.Clear()
        AddCurvesToComboWhichCurve(pPane)
        AddCurvesToComboWhichCurve(pPaneAux)        
        pSettingControls -= 1
    End Sub

    Private Sub AddCurvesToComboWhichCurve(ByVal aPane As GraphPane)
        If aPane IsNot Nothing AndAlso aPane.CurveList IsNot Nothing Then
            For Each lCurve As CurveItem In aPane.CurveList
                If lCurve.Label.Text.Length > 0 Then
                    comboWhichCurve.Items.Add(lCurve.Label.Text)
                ElseIf Not lCurve.Tag Is Nothing Then
                    comboWhichCurve.Items.Add(lCurve.Tag)
                Else
                    comboWhichCurve.Items.Add("curve " & comboWhichCurve.Items.Count + 1)
                End If
            Next
        End If
    End Sub

    Private Sub SetComboFromTexts()
        pSettingControls += 1
        comboWhichText.Items.Clear()
        comboWhichText.Text = ""
        For Each lItem As ZedGraph.GraphObj In pPane.GraphObjList
            If lItem.GetType.Name = "TextObj" Then
                Dim lText As TextObj = lItem
                comboWhichText.Items.Add(lText.Text)
            End If
        Next
        If comboWhichText.Items.Count > 0 Then
            comboWhichText.SelectedIndex = comboWhichText.Items.Count - 1
        End If
        pSettingControls -= 1
    End Sub

    Private Sub SetControlsFromPane()
        pSettingControls += 1
        If comboWhichCurve.Items.Count > 0 Then comboWhichCurve.SelectedIndex = 0
        SetControlsFromAxis(AxisFromCombo())
        Select Case pPane.XAxis.Type
            Case AxisType.Exponent, AxisType.Linear, AxisType.Log
                grpLineEquation.Visible = True
                grpRegression.Visible = True
            Case Else
                grpLineEquation.Visible = False
                grpRegression.Visible = False
        End Select
        chkLegendOutline.Checked = pPane.Legend.Border.IsVisible
        txtLegendFontColor.BackColor = pPane.Legend.FontSpec.FontColor

        If pPaneAux IsNot Nothing Then
            chkLegendOutlineAux.Checked = pPaneAux.Legend.Border.IsVisible
            txtLegendFontColorAux.BackColor = pPaneAux.Legend.FontSpec.FontColor
        End If

        pSettingControls -= 1
    End Sub

    Private Function AxisFromCombo() As Axis
        If Not pPane Is Nothing Then
            chkHoldOpposite.Visible = False
            If radioAxisBottom.Checked Then
                Return pPane.XAxis
            ElseIf radioAxisLeft.Checked Then
                If pPane.YAxis.IsVisible And pPane.Y2Axis.IsVisible Then
                    chkHoldOpposite.Text = "Hold Right"
                    chkHoldOpposite.Visible = True
                End If
                Return pPane.YAxis
            ElseIf radioAxisRight.Checked Then
                If pPane.YAxis.IsVisible And pPane.Y2Axis.IsVisible Then
                    chkHoldOpposite.Text = "Hold Left"
                    chkHoldOpposite.Visible = True
                End If
                Return pPane.Y2Axis
            ElseIf radioAxisAux.Checked Then
                If pPaneAux Is Nothing Then
                    If MsgBox("Add Auxiliary Graph Now?", MsgBoxStyle.YesNo, "No Auxiliary Graph") = MsgBoxResult.Yes Then
                        EnableAuxAxis(pZgc.MasterPane, True, 0.2)
                        pPaneAux = pZgc.MasterPane.PaneList(0)
                        pPane = pZgc.MasterPane.PaneList(1)
                        Return pPaneAux.YAxis
                    End If
                Else
                    Return pPaneAux.YAxis
                End If
            End If
        End If
        Return Nothing
    End Function

    Private Sub SetControlsFromAxis(ByVal aAxis As Axis)
        pSettingControls += 1
        If Not aAxis Is Nothing Then
            txtAxisLabel.Text = aAxis.Title.Text
            radioAxisTime.Enabled = False
            radioAxisLinear.Enabled = False
            radioAxisLogarithmic.Enabled = False
            radioAxisProbability.Enabled = False
            Select Case aAxis.Type
                '### Fix
                'Case AxisType.DateDual
                '   panelProbability.Visible = False
               ' radioAxisTime.Checked = True
                Case AxisType.Linear
                    panelProbability.Visible = False
                    radioAxisLinear.Enabled = True
                    radioAxisLogarithmic.Enabled = True
                    radioAxisLinear.Checked = True
                Case AxisType.Log
                    panelProbability.Visible = False
                    radioAxisLinear.Enabled = True
                    radioAxisLogarithmic.Enabled = True
                    radioAxisLogarithmic.Checked = True
                    '### Fix
                    'Case AxisType.Probability
                    '    panelProbability.Visible = True
                    '    radioAxisProbability.Checked = True
                    '    panelAxisType.Visible = False
                    '    Dim lProbScale As ZedGraph.ProbabilityScale = aAxis.Scale
                    '    Select Case lProbScale.LabelStyle
                    '        Case ProbabilityScale.ProbabilityLabelStyle.Percent
                    '            radioProbablilityPercent.Checked = True
                    '        Case ProbabilityScale.ProbabilityLabelStyle.Fraction
                    '            radioProbablilityFraction.Checked = True
                    '        Case ProbabilityScale.ProbabilityLabelStyle.ReturnInterval
                    '            radioProbablilityReturnPeriod.Checked = True
                    '    End Select
                    '    txtProbabilityDeviations.Text = DoubleToString(lProbScale.standardDeviations)
            End Select
            SetControlsMinMax(aAxis)
            chkRangeReverse.Checked = aAxis.Scale.IsReverse
            chkAxisMajorGridVisible.Checked = aAxis.MajorGrid.IsVisible
            txtAxisMajorGridColor.BackColor = aAxis.MajorGrid.Color
            chkAxisMajorTicsVisible.Checked = aAxis.MajorTic.IsInside
            chkAxisMinorGridVisible.Checked = aAxis.MinorGrid.IsVisible
            txtAxisMinorGridColor.BackColor = aAxis.MinorGrid.Color
            chkAxisMinorTicsVisible.Checked = aAxis.MinorTic.IsInside
        End If
        pSettingControls -= 1
    End Sub

    Private Sub SetControlsMinMax(ByVal aAxis As Axis)
        pSettingControls += 1
        If Not aAxis Is Nothing Then
            If radioAxisTime.Checked Then
                txtAxisDisplayMinimum.Text = pDateFormat.JDateToString(aAxis.Scale.Min)
                txtAxisDisplayMaximum.Text = pDateFormat.JDateToString(aAxis.Scale.Max)
            ElseIf radioAxisLinear.Checked Then
                txtAxisDisplayMinimum.Text = DoubleToString(aAxis.Scale.Min)
                txtAxisDisplayMaximum.Text = DoubleToString(aAxis.Scale.Max)
            ElseIf radioAxisLogarithmic.Checked Then
                txtAxisDisplayMinimum.Text = DoubleToString(aAxis.Scale.Min)
                txtAxisDisplayMaximum.Text = DoubleToString(aAxis.Scale.Max)
            ElseIf radioAxisProbability.Checked Then
                txtAxisDisplayMinimum.Text = DoubleToString(aAxis.Scale.Min)
                txtAxisDisplayMaximum.Text = DoubleToString(aAxis.Scale.Max)
            End If
        End If
        pSettingControls -= 1
    End Sub

    Private Sub SetAxisFromControls(ByVal aAxis As Axis)
        On Error Resume Next 'Set whatever we can legally set
        If Not aAxis Is Nothing Then
            With aAxis
                Dim lTemp As Double
                If radioAxisTime.Checked Then
                    'parse min/max date from textboxes
                    aAxis.Scale.Min = StringToJdate(txtAxisDisplayMinimum.Text, True)
                    aAxis.Scale.Max = StringToJdate(txtAxisDisplayMaximum.Text, False)
                ElseIf radioAxisLinear.Checked Then
                    If aAxis.Type <> AxisType.Linear Then .Type = AxisType.Linear
                ElseIf radioAxisLogarithmic.Checked Then
                    If aAxis.Type <> AxisType.Log Then .Type = AxisType.Log
                    'ElseIf radioAxisProbability.Checked Then
                    '    Dim lProbScale As ZedGraph.ProbabilityScale = aAxis.Scale
                    '    If radioProbablilityPercent.Checked Then
                    '        lProbScale.LabelStyle = ProbabilityScale.ProbabilityLabelStyle.Percent
                    '    ElseIf radioProbablilityFraction.Checked Then
                    '        lProbScale.LabelStyle = ProbabilityScale.ProbabilityLabelStyle.Fraction
                    '    ElseIf radioProbablilityReturnPeriod.Checked Then
                    '        lProbScale.LabelStyle = ProbabilityScale.ProbabilityLabelStyle.ReturnInterval
                    '    End If
                    '    If Double.TryParse(txtProbabilityDeviations.Text, lTemp) Then
                    '        lProbScale.standardDeviations = lTemp
                    '    End If
                    'lProbScale.Exceedance = chkProbabilityExceedance.Checked
                End If

                'If Double.TryParse(txtAxisDisplayMinimum.Text, lTemp) Then
                '    .Scale.MinAuto = False
                '    .Scale.Min = lTemp
                'End If
                'If Double.TryParse(txtAxisDisplayMaximum.Text, lTemp) Then
                '    .Scale.MaxAuto = False
                '    .Scale.Max = lTemp
                'End If

                .Scale.IsReverse = chkRangeReverse.Checked
                Dim lNewMin As Double
                Dim lNewMax As Double
                If Double.TryParse(txtAxisDisplayMinimum.Text, lNewMin) AndAlso _
                   Double.TryParse(txtAxisDisplayMaximum.Text, lNewMax) Then
                    SetYAxisScales(aAxis, lNewMin, lNewMax, chkHoldOpposite.Checked)
                End If

                .MajorGrid.Color = txtAxisMajorGridColor.BackColor
                .MajorGrid.IsVisible = chkAxisMajorGridVisible.Checked

                .MinorGrid.Color = txtAxisMinorGridColor.BackColor
                .MinorGrid.IsVisible = chkAxisMinorGridVisible.Checked

                .MajorTic.IsInside = chkAxisMajorTicsVisible.Checked
                .MinorTic.IsInside = chkAxisMinorTicsVisible.Checked

                If aAxis Is pPane.XAxis OrElse pPaneAux IsNot Nothing AndAlso aAxis Is pPaneAux.XAxis OrElse aAxis Is pPane.YAxis AndAlso Not pPane.Y2Axis.IsVisible Then
                    .MajorTic.IsOpposite = .MajorTic.IsInside
                    .MinorTic.IsOpposite = .MinorTic.IsInside
                Else
                    .MajorTic.IsOpposite = False
                    .MinorTic.IsOpposite = False
                End If

                If pPaneAux Is Nothing OrElse aAxis IsNot pPaneAux.XAxis Then
                    .Title.Text = txtAxisLabel.Text
                    If aAxis Is pPane.XAxis AndAlso pPaneAux IsNot Nothing Then
                        SetAxisFromControls(pPaneAux.XAxis)
                    End If
                End If
            End With
        End If
    End Sub

    Private Sub SetYAxisScales(ByVal aYAxis As Axis, ByVal aMin As Double, ByVal aMax As Double, Optional aHoldOpposite As Boolean = False)
        Dim lOppositeYAxis As Axis = Nothing
        If aYAxis IsNot Nothing Then
            Dim lName As String = aYAxis.GetType().Name
            If lName.ToLower() = "yaxis" Then
                lOppositeYAxis = pPane.Y2Axis
            ElseIf lName.ToLower() = "y2axis" Then
                lOppositeYAxis = pPane.YAxis
            End If
        Else
            Exit Sub
        End If
        Dim lIsReversed As Boolean
        Dim lYAxisMin0 As Double = GetNaN()
        Dim lYAxisMax0 As Double = GetNaN()
        With aYAxis
            lYAxisMin0 = .Scale.Min
            lYAxisMax0 = .Scale.Max
            If Not Double.IsNaN(aMin) Then
                .Scale.MinAuto = False
                .Scale.Min = aMin
            End If
            If Not Double.IsNaN(aMax) Then
                .Scale.MaxAuto = False
                .Scale.Max = aMax
            End If
            lIsReversed = .Scale.IsReverse
        End With
        If lOppositeYAxis IsNot Nothing Then
            If Not aHoldOpposite Then
                Dim lChangeFractionMin As Double = (aMin - lYAxisMin0) / (lYAxisMax0 - lYAxisMin0)
                Dim lChangeFractionMax As Double = (aMax - lYAxisMax0) / (lYAxisMax0 - lYAxisMin0)
                With lOppositeYAxis.Scale
                    Dim lIsReversedOp As Boolean = .IsReverse
                    Dim lYAxisMinOp0 As Double = .Min
                    Dim lYAxisMaxOp0 As Double = .Max
                    .MinAuto = False
                    .MaxAuto = False
                    If (lIsReversed And Not lIsReversedOp) OrElse (Not lIsReversed And lIsReversedOp) Then
                        .Max -= lChangeFractionMin * (lYAxisMaxOp0 - lYAxisMinOp0)
                        .Min -= lChangeFractionMax * (lYAxisMaxOp0 - lYAxisMinOp0)
                    Else
                        .Min += lChangeFractionMin * (lYAxisMaxOp0 - lYAxisMinOp0)
                        .Max += lChangeFractionMax * (lYAxisMaxOp0 - lYAxisMinOp0)
                    End If
                End With
            End If
        End If
    End Sub

    Private Function CurrentCurve() As CurveItem
        If comboWhichCurve.SelectedIndex >= 0 Then
            Dim lPaneCurves As Integer = 0
            If pPane IsNot Nothing Then lPaneCurves = pPane.CurveList.Count
            If comboWhichCurve.SelectedIndex < lPaneCurves Then
                Return pPane.CurveList(comboWhichCurve.SelectedIndex)
            ElseIf pPaneAux IsNot Nothing AndAlso comboWhichCurve.SelectedIndex < lPaneCurves + pPaneAux.CurveList.Count Then
                Return pPaneAux.CurveList(comboWhichCurve.SelectedIndex - lPaneCurves)
            End If
        End If
        Return Nothing
    End Function

    Private Sub SetControlsFromSelectedCurve()
        pSettingControls += 1
        SetControlsFromCurve(CurrentCurve)
        pSettingControls -= 1
    End Sub

    Private Sub SetControlsFromCurve(ByVal aCurve As LineItem)
        pSettingControls += 1
        If Not aCurve Is Nothing Then
            On Error Resume Next
            txtCurveLabel.Text = aCurve.Label.Text
            If pPaneAux IsNot Nothing AndAlso pPaneAux.CurveList.Contains(aCurve) Then
                radioCurveYaxisAuxiliary.Checked = True
            Else
                If aCurve.IsY2Axis Then
                    radioCurveYaxisRight.Checked = True
                Else
                    radioCurveYaxisLeft.Checked = True
                End If
            End If

            txtCurveColor.BackColor = aCurve.Color
            chkCurveLineVisible.Checked = aCurve.Line.IsVisible
            txtCurveWidth.Text = aCurve.Line.Width.ToString()

            chkCurveSymbolVisible.Checked = aCurve.Symbol.IsVisible
            chkCurveSymbolFillVisible.Checked = aCurve.Symbol.Fill.IsVisible
            txtCurveSymbolSize.Text = aCurve.Symbol.Size
            If aCurve.Line.IsSmooth Then
                cboCurveStep.Text = "Smooth"
            Else
                cboCurveStep.Text = System.Enum.GetName(GetType(ZedGraph.StepType), aCurve.Line.StepType)
            End If
            cboCurveStyle.Text = System.Enum.GetName(GetType(System.Drawing.Drawing2D.DashStyle), aCurve.Line.Style)
            cboCurveSymbolType.Text = System.Enum.GetName(GetType(ZedGraph.SymbolType), aCurve.Symbol.Type)
        End If
        pSettingControls -= 1
    End Sub

    Private Sub SetCurveFromControls(ByVal aCurve As LineItem)
        If Not aCurve Is Nothing Then
            On Error Resume Next
            Dim lInt As Integer
            aCurve.Label.Text = txtCurveLabel.Text
            If radioCurveYaxisAuxiliary.Checked Then 'move to aux pane
                If pPaneAux Is Nothing Then
                    EnableAuxAxis(pZgc.MasterPane, True, 0.2)
                    pPaneAux = pZgc.MasterPane.PaneList(0)
                    pPane = pZgc.MasterPane.PaneList(1)
                End If
                If Not pPaneAux.CurveList.Contains(aCurve) Then
                    pPaneAux.CurveList.Add(aCurve)
                    pPane.CurveList.Remove(aCurve)
                End If
            Else
                If pPaneAux IsNot Nothing AndAlso pPaneAux.CurveList.Contains(aCurve) Then
                    pPaneAux.CurveList.Remove(aCurve)
                    pPane.CurveList.Add(aCurve)
                End If
            End If
            aCurve.IsY2Axis = radioCurveYaxisRight.Checked

            aCurve.Color = txtCurveColor.BackColor
            aCurve.Line.IsVisible = chkCurveLineVisible.Checked
            If Integer.TryParse(txtCurveWidth.Text, lInt) Then aCurve.Line.Width = lInt

            aCurve.Symbol.IsVisible = chkCurveSymbolVisible.Checked
            aCurve.Symbol.Fill.IsVisible = chkCurveSymbolFillVisible.Checked
            If Integer.TryParse(txtCurveSymbolSize.Text, lInt) Then aCurve.Symbol.Size = lInt
            If cboCurveStep.Text = "Smooth" Then
                aCurve.Line.IsSmooth = True
            Else
                aCurve.Line.IsSmooth = False
                aCurve.Line.StepType = System.Enum.Parse(GetType(ZedGraph.StepType), cboCurveStep.Text)
            End If
            aCurve.Line.Style = System.Enum.Parse(GetType(System.Drawing.Drawing2D.DashStyle), cboCurveStyle.Text)
            aCurve.Symbol.Type = System.Enum.Parse(GetType(ZedGraph.SymbolType), cboCurveSymbolType.Text)
            'Select Case cboCurveSymbolType.Text
            '    Case "Square" : aCurve.Symbol.Type = SymbolType.Square
            '    Case "Diamond" : aCurve.Symbol.Type = SymbolType.Diamond
            '    Case "Triangle" : aCurve.Symbol.Type = SymbolType.Triangle
            '    Case "Circle" : aCurve.Symbol.Type = SymbolType.Circle
            '    Case "XCross" : aCurve.Symbol.Type = SymbolType.XCross
            '    Case "Plus" : aCurve.Symbol.Type = SymbolType.Plus
            '    Case "Star" : aCurve.Symbol.Type = SymbolType.Star
            '    Case "TriangleDown" : aCurve.Symbol.Type = SymbolType.TriangleDown
            '    Case "HDash" : aCurve.Symbol.Type = SymbolType.HDash
            '    Case "VDash" : aCurve.Symbol.Type = SymbolType.VDash
            '    Case "None" : aCurve.Symbol.Type = SymbolType.None
            'End Select
        End If
    End Sub

    Private Sub SetLegendFromControls()
        With pPane.Legend
            .Border.IsVisible = chkLegendOutline.Checked
            .FontSpec.FontColor = txtLegendFontColor.BackColor
            '.IsVisible = comboLegendLocation.Text <> "None"
            'Select Case comboLegendLocation.Text
            '    Case "Bottom" : .Position = LegendPos.Bottom
            '    Case "BottomCenter" : .Position = LegendPos.BottomCenter
            '    Case "BottomFlushLeft" : .Position = LegendPos.BottomFlushLeft
            '    Case "Float" : .Position = LegendPos.Float
            '    Case "InsideBotLeft" : .Position = LegendPos.InsideBotLeft
            '    Case "InsideBotRight" : .Position = LegendPos.InsideBotRight
            '    Case "InsideTopLeft" : .Position = LegendPos.InsideTopLeft
            '    Case "InsideTopRight" : .Position = LegendPos.InsideTopRight
            '    Case "Left" : .Position = LegendPos.Left
            '    Case "Right" : .Position = LegendPos.Right
            '    Case "Top" : .Position = LegendPos.Top
            '    Case "TopCenter" : .Position = LegendPos.TopCenter
            '    Case "TopFlushLeft" : .Position = LegendPos.TopFlushLeft
            'End Select
        End With
        If pPaneAux IsNot Nothing Then
            With pPaneAux.Legend
                .Border.IsVisible = chkLegendOutlineAux.Checked
                .FontSpec.FontColor = txtLegendFontColorAux.BackColor
            End With
        End If
    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click
        ApplyAll()
    End Sub

    Private Sub ApplyAll()
        If pSettingControls = 0 Then
            SetAxisFromControls(AxisFromCombo())
            SetLegendFromControls()
            Dim lSelectedCurveIndex As Integer = comboWhichCurve.SelectedIndex
            If lSelectedCurveIndex >= 0 Then
                Dim lPaneCurves As Integer = 0
                If pPane IsNot Nothing Then lPaneCurves = pPane.CurveList.Count
                If comboWhichCurve.SelectedIndex < lPaneCurves Then
                    SetCurveFromControls(pPane.CurveList(comboWhichCurve.SelectedIndex))
                ElseIf pPaneAux IsNot Nothing AndAlso comboWhichCurve.SelectedIndex < lPaneCurves + pPaneAux.CurveList.Count Then
                    SetCurveFromControls(pPaneAux.CurveList(comboWhichCurve.SelectedIndex - lPaneCurves))
                End If
                SetComboFromCurves()
                comboWhichCurve.SelectedIndex = lSelectedCurveIndex
            End If

            Dim lLeftCount As Integer = 0
            Dim lRightCount As Integer = 0
            For Each lCurve As CurveItem In pPane.CurveList
                If lCurve.IsY2Axis Then lRightCount += 1 Else lLeftCount += 1
            Next

            'Make sure special groundwater axes do not disappear by checking Title.Text
            pPane.YAxis.IsVisible = (lLeftCount > 0 OrElse pPane.YAxis.Title.Text.StartsWith("Depth"))
            pPane.Y2Axis.IsVisible = (lRightCount > 0 OrElse pPane.Y2Axis.Title.Text.StartsWith("Groundwater"))

            If txtText.Text.Length > 0 Then
                Dim lText As TextObj = FindTextObject(txtText.Text)
                If lText Is Nothing Then
                    AddTextFromControls()
                End If
            End If

            If pPaneAux IsNot Nothing Then
                If pPaneAux.CurveList.Count = 0 Then
                    EnableAuxAxis(pZgc.MasterPane, False, 0.2)
                Else
                    Dim lAuxFraction As Double
                    If Double.TryParse(txtAuxSize.Text, lAuxFraction) Then
                        If lAuxFraction > 0.99 Then lAuxFraction /= 100
                    Else
                        lAuxFraction = 0.2
                    End If
                    EnableAuxAxis(pZgc.MasterPane, True, lAuxFraction)
                End If
            End If

            RaiseEvent Apply()
        End If
    End Sub

    Private Sub cboWhichAxis_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SetControlsFromSelectedCurve()
    End Sub

    Private Sub txtColor_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles txtAxisMinorGridColor.Click, txtAxisMajorGridColor.Click, txtCurveColor.Click, txtLegendFontColor.Click, txtLegendFontColorAux.Click, txtTextColor.Click
        ChooseTextBoxBackColor(sender)
    End Sub

    Private Sub ChooseTextBoxBackColor(ByVal aTextBox As TextBox)
        Dim lColorDialog As ColorDialog = New ColorDialog()
        lColorDialog.Color = aTextBox.BackColor
        If lColorDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            aTextBox.BackColor = lColorDialog.Color
            If chkAutoApply.Checked Then ApplyAll()
        End If
    End Sub

    Private Sub chkAny_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
        chkAxisMajorGridVisible.CheckedChanged,
        chkAxisMajorTicsVisible.CheckedChanged,
        chkAxisMinorGridVisible.CheckedChanged,
        chkAxisMinorTicsVisible.CheckedChanged,
        chkCurveLineVisible.CheckedChanged,
        chkCurveSymbolVisible.CheckedChanged,
        chkCurveSymbolFillVisible.CheckedChanged,
        chkLegendOutline.CheckedChanged,
        chkLegendOutlineAux.CheckedChanged,
        chkRangeReverse.CheckedChanged

        If chkAutoApply.Checked Then ApplyAll()
    End Sub

    Private Sub txtLabel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
            txtAxisLabel.TextChanged, txtCurveLabel.TextChanged
        If chkAutoApply.Checked Then ApplyAll()
    End Sub

    Private Sub txtNumeric_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
              txtCurveWidth.TextChanged, txtCurveSymbolSize.TextChanged, txtProbabilityDeviations.TextChanged
        If chkAutoApply.Checked AndAlso IsNumeric(sender.Text) Then ApplyAll()
    End Sub

    Private Sub radioAxisLocation_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
            radioAxisBottom.CheckedChanged, radioAxisLeft.CheckedChanged, radioAxisRight.CheckedChanged, radioAxisAux.CheckedChanged
        SetControlsFromAxis(AxisFromCombo())
    End Sub

    Private Sub radioGeneral_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
            radioAxisLinear.CheckedChanged, radioAxisLogarithmic.CheckedChanged,
            radioCurveYaxisLeft.CheckedChanged, radioCurveYaxisRight.CheckedChanged, radioCurveYaxisAuxiliary.CheckedChanged,
            radioProbablilityFraction.CheckedChanged, radioProbablilityPercent.CheckedChanged, radioProbablilityReturnPeriod.CheckedChanged ', chkProbabilityExceedance.CheckedChanged

        Dim lRadio As RadioButton = sender
        If lRadio.Checked Then 'Only run for the newly checked one, not also for unchecked
            If radioAxisProbability.Checked Then
                If radioProbablilityPercent.Checked Then
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Fraction", "Percent")
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Return Interval", "Percent")
                ElseIf radioProbablilityFraction.Checked Then
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Percent", "Fraction")
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Return Interval", "Fraction")
                ElseIf radioProbablilityReturnPeriod.Checked Then
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Percent", "Return Interval")
                    txtAxisLabel.Text = ReplaceStringNoCase(txtAxisLabel.Text, "Fraction", "Return Interval")
                End If
            End If

            If chkAutoApply.Checked Then
                ApplyAll()
            End If
        End If
    End Sub

    Private Sub chkAutoApply_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAutoApply.CheckedChanged
        If chkAutoApply.Checked Then
            ApplyAll()
        End If
    End Sub

    Private Sub comboWhichCurve_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comboWhichCurve.SelectedIndexChanged
        SetControlsFromSelectedCurve()
    End Sub

    Private Sub cboCurveAxis_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If chkAutoApply.Checked Then ApplyAll()
    End Sub

    Private Sub pZgc_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Select Case tabsCategory.SelectedTab.Text
            Case "Legend"
                With pPane.Legend
                    .Position = LegendPos.Float
                    .Location = New Location((e.X - pPane.Rect.Left) / pZgc.Width,
                                             (e.Y - pPane.Rect.Top) / pPane.Rect.Height,
                                             CoordType.PaneFraction)
                    .IsVisible = True
                End With
                RaiseEvent Apply()
            Case "Text"
                If txtText.Text.Length > 0 Then
                    Dim lText As TextObj = FindTextObject(txtText.Text)
                    If lText Is Nothing Then
                        AddTextFromControls()
                        lText = FindTextObject(txtText.Text)
                    End If
                    If lText IsNot Nothing Then
                        lText.Location = New Location((e.X - pPane.Rect.Left) / pZgc.Width,
                                                      (e.Y - pPane.Rect.Top) / pPane.Rect.Height,
                                                      CoordType.PaneFraction)
                        RaiseEvent Apply()
                    End If
                End If
        End Select
    End Sub

    Private Sub pZgc_ZoomEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal oldState As ZedGraph.ZoomState, ByVal newState As ZedGraph.ZoomState)
        SetControlsMinMax(AxisFromCombo())
    End Sub

    Private Sub AddedLineItem()
        SetComboFromCurves()
        tabsCategory.SelectTab(tabCurves)
        comboWhichCurve.SelectedIndex = comboWhichCurve.Items.Count - 1
        RaiseEvent Apply()
    End Sub

    Private Sub btnLineEquationAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLineEquationAdd.Click
        Dim lNewLine As LineItem = AddLine(pPane, txtLineAcoef.Text, txtLineBcoef.Text, Drawing2D.DashStyle.Solid, "Y = " & txtLineAcoef.Text & " X + " & txtLineBcoef.Text)
        AddedLineItem()
    End Sub

    Private Sub btnLineConstantYAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLineConstantYAdd.Click
        Dim lNewLine As LineItem = AddLine(pPane, 0, txtLineYconstant.Text, Drawing2D.DashStyle.Solid, "Y = " & txtLineYconstant.Text)
        AddedLineItem()
    End Sub

    Private Sub cboCurveAny_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCurveStep.SelectedIndexChanged, cboCurveStyle.SelectedIndexChanged, cboCurveSymbolType.SelectedIndexChanged
        If chkAutoApply.Checked Then ApplyAll()
    End Sub

    Private Sub txtAxisDisplayMinimum_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAxisDisplayMinimum.TextChanged
        If chkAutoApply.Checked AndAlso IsNumeric(txtAxisDisplayMinimum.Text) Then
            Dim lTemp As Double
            If Double.TryParse(txtAxisDisplayMinimum.Text, lTemp) Then
                Dim lAxis As Axis = AxisFromCombo()
                If Not lAxis Is Nothing AndAlso lAxis.Scale.Min <> lTemp AndAlso lTemp < lAxis.Scale.Max Then
                    lAxis.Scale.Min = lTemp
                    RaiseEvent Apply()
                End If
            End If
        End If
    End Sub

    Private Sub txtAxisDisplayMaximum_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAxisDisplayMaximum.TextChanged
        If chkAutoApply.Checked AndAlso IsNumeric(txtAxisDisplayMaximum.Text) Then
            Dim lTemp As Double
            If Double.TryParse(txtAxisDisplayMaximum.Text, lTemp) Then
                Dim lAxis As Axis = AxisFromCombo()
                If Not lAxis Is Nothing AndAlso lAxis.Scale.Max <> lTemp AndAlso lTemp > lAxis.Scale.Min Then
                    lAxis.Scale.Max = lTemp
                    RaiseEvent Apply()
                End If
            End If
        End If
    End Sub

    'Private Sub comboLegendLocation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    If chkAutoApply.Checked Then
    '        SetLegendFromControls()
    '        RaiseEvent Apply()
    '    End If
    'End Sub

    Private Sub btnTextAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextAdd.Click
        If comboWhichText.Items.Contains(txtText.Text) Then 'Text exists, but was removed (now hidden)
            Dim lText As TextObj = FindTextObject(txtText.Text)
            lText.IsVisible = True
        Else
            AddTextFromControls()
        End If
        RaiseEvent Apply()
    End Sub

    Private Sub AddTextFromControls()
        Dim lText As New TextObj(txtText.Text, 0.5, 0.2)
        lText.Location.CoordinateFrame = CoordType.PaneFraction
        lText.FontSpec.FontColor = txtTextColor.BackColor
        lText.FontSpec.IsBold = True
        lText.FontSpec.Size = 16
        lText.FontSpec.Border.IsVisible = False
        lText.FontSpec.Fill.IsVisible = False
        lText.Location.AlignH = AlignH.Left
        lText.Location.AlignV = AlignV.Top
        pPane.GraphObjList.Add(lText)
        SetComboFromTexts()
        comboWhichText.Text = lText.Text
    End Sub

    Private Sub comboWhichText_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comboWhichText.SelectedIndexChanged
        txtText.Text = comboWhichText.Text
        Dim lText As TextObj = FindTextObject(comboWhichText.Text)
        If Not lText Is Nothing Then
            txtTextColor.BackColor = lText.FontSpec.FontColor
        End If
    End Sub

    Private Function FindTextObject(ByVal aText As String) As TextObj
        For Each lItem As ZedGraph.GraphObj In pPane.GraphObjList
            If lItem.GetType.Name = "TextObj" Then
                Dim lText As TextObj = lItem
                If String.Compare(lText.Text, aText, True) = 0 Then Return lText
            End If
        Next
        Return Nothing
    End Function

    Private Sub btnTextRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextRemove.Click
        Dim lText As TextObj = FindTextObject(comboWhichText.Text)
        If Not lText Is Nothing Then
            'pPane.GraphObjList.Remove(lText)
            'just hide the text text object, don't remove it altogether (can then add it back if needed)
            pPane.GraphObjList(pPane.GraphObjList.IndexOf(lText)).IsVisible = False
            SetComboFromTexts()
            If chkAutoApply.Checked Then RaiseEvent Apply()
        End If
    End Sub

    Private Sub txtText_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtText.KeyUp
        Dim lText As TextObj = FindTextObject(comboWhichText.Text)
        If Not lText Is Nothing AndAlso lText.Text <> txtText.Text Then
            lText.Text = txtText.Text
            SetComboFromTexts()
            If chkAutoApply.Checked Then RaiseEvent Apply()
        End If
    End Sub

    Private Sub btnAxisFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAxisFont.Click
        If UserEditFontSpec(AxisFromCombo.Title.FontSpec) Then
            If chkAutoApply.Checked Then RaiseEvent Apply()
        End If
    End Sub

    Private Sub btnScaleFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScaleFont.Click
        If UserEditFontSpec(AxisFromCombo.Scale.FontSpec) Then
            If chkAutoApply.Checked Then RaiseEvent Apply()
        End If
    End Sub

    Private Sub btnLegendFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLegendFont.Click
        If UserEditFontSpec(pPane.Legend.FontSpec) Then
            If chkAutoApply.Checked Then RaiseEvent Apply()
        End If
    End Sub

    Private Sub btnLegendFontAux_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLegendFontAux.Click
        If pPaneAux Is Nothing Then
            MsgBox("There is no aux axis on this graph")
        Else
            If UserEditFontSpec(pPaneAux.Legend.FontSpec) Then
                If chkAutoApply.Checked Then RaiseEvent Apply()
            End If
        End If
    End Sub

    Private Sub btnTextFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextFont.Click
        Dim lText As TextObj = FindTextObject(comboWhichText.Text)
        If lText IsNot Nothing Then
            If UserEditFontSpec(lText.FontSpec) Then
                If chkAutoApply.Checked Then RaiseEvent Apply()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Use a FontDialog to edit a ZedGraph FontSpec
    ''' </summary>
    ''' <param name="aFontSpec"></param>
    ''' <returns>True if font was edited, false if user cancelled</returns>
    Private Function UserEditFontSpec(ByRef aFontSpec As FontSpec) As Boolean
        Dim cdlg As New System.Windows.Forms.FontDialog
        With cdlg
            .Font = aFontSpec.GetFont(1)
            .AllowSimulations = True
            .ShowEffects = True
            .ShowColor = True
            .Color = aFontSpec.FontColor
        End With
        If cdlg.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            With cdlg.Font
                Dim lBorder As ZedGraph.Border = aFontSpec.Border.Clone
                Dim lAngle As Single = aFontSpec.Angle
                aFontSpec = New FontSpec(
                    .FontFamily.Name, .SizeInPoints * 1.333333, cdlg.Color,
                    .Bold, .Italic, .Underline,
                    aFontSpec.Fill.Color, aFontSpec.Fill.Brush, aFontSpec.Fill.Type)
                aFontSpec.Border = lBorder
                aFontSpec.Angle = lAngle
            End With
            Return True
        End If
        Return False
    End Function

    Private Sub btnLineRegressionAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLineRegressionAdd.Click
        Dim lForm As atcGraphForm = pZgc.ParentForm
        Dim lGrapher As clsGraphScatter = lForm.Grapher
        lGrapher.AddFitLine()
        AddedLineItem()
    End Sub

    Private Sub btnNewColors_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewColors.Click
        Dim lCurrentCurve As CurveItem = CurrentCurve()
        For Each lPane As GraphPane In pZgc.MasterPane.PaneList

            For Each lCurve As CurveItem In lPane.CurveList
                lCurve.Color = Color.FromArgb(255, _
                               64 + CInt(Rnd() * 128), _
                               64 + CInt(Rnd() * 128), _
                               64 + CInt(Rnd() * 128))
                If Object.ReferenceEquals(lCurrentCurve, lCurve) Then
                    txtCurveColor.BackColor = lCurve.Color
                End If
            Next

            'Grayscale from nearly white (first curve) to all the way black (last curve)
            'Dim lNumCurves As Integer = lPane.CurveList.Count
            'Dim lCurveIndex As Integer = 1
            'For Each lCurve As CurveItem In lPane.CurveList
            '    Dim lGrayLevel As Integer = 255 - 255 * lCurveIndex / lNumCurves
            '    lCurve.Color = Color.FromArgb(255, lGrayLevel, lGrayLevel, lGrayLevel)
            '    If Object.ReferenceEquals(lCurrentCurve, lCurve) Then
            '        txtCurveColor.BackColor = lCurve.Color
            '    End If
            '    lCurveIndex += 1
            'Next

        Next
        If chkAutoApply.Checked Then ApplyAll()
    End Sub

End Class