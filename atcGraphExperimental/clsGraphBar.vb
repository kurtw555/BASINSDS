﻿Imports System.Drawing

Imports atcData
Imports atcUtility
Imports MapWinUtility
Imports ZedGraph

Public Class clsGraphBar
    Inherits clsGraphBoxWhisker

    <CLSCompliant(False)>
    Public Sub New(ByVal aDataGroup As atcTimeseriesGroup, ByVal aZedGraphControl As ZedGraphControl, Optional aDelaySetup As Boolean = False)
        MyBase.New(aDataGroup, aZedGraphControl)
        If Not aDelaySetup Then
            SetUpGraph(True)
        End If
    End Sub

    Public Overrides Sub SetUpGraph(Optional OutputToFile As Boolean = False)
        If Datasets.Count = 0 Then
            Exit Sub
        End If

        'Set up the chart
        pZgc.GraphPane.BarSettings.Type = BarType.Overlay
        pZgc.GraphPane.XAxis.IsVisible = False
        pZgc.GraphPane.Legend.IsVisible = ShowLegend()
        If String.Compare(Title(), "no title", True) = 0 Then
        ElseIf String.IsNullOrEmpty(Title()) Then
            Dim lcon As String = Datasets(0).Attributes.GetValue("Constituent", "Value")
            pZgc.GraphPane.Title.Text = lcon & " Ranges"
        Else
            pZgc.GraphPane.Title.Text = Title()
        End If
        Dim listDataArrays As New Generic.List(Of Double())
        For I As Integer = 0 To Datasets.Count - 1
            listDataArrays.Add(Datasets(I).Values)
        Next
        If XLabels Is Nothing OrElse XLabels.Count <> Datasets.Count Then
            XLabels = New Generic.List(Of String)
            Dim lbl As String = ""
            For I As Integer = 0 To Datasets.Count - 1
                lbl = Datasets(I).Attributes.GetValue("Location", "QTY" & I.ToString())
                If Not String.IsNullOrEmpty(lbl) AndAlso lbl.Length > 8 Then
                    lbl = lbl.Substring(0, 8)
                End If
                XLabels.Add(lbl)
            Next
        End If
        'Dim lPane As GraphPane = MyBase.pZgc.MasterPane.PaneList(0)
        'lPane.Legend.IsVisible = False
        With pZgc.GraphPane.XAxis
            '.Scale.MaxAuto = False
            If String.IsNullOrEmpty(XTitle()) Then
                With Datasets(0).Attributes
                    Dim lScen As String = .GetValue("scenario")
                    Dim lLoc As String = .GetValue("location")
                    Dim lCons As String = .GetValue("constituent")
                    Dim lUnit As String = .GetValue("Units")
                    Dim lTimeUnit As String = TimeUnitText(Datasets(0))
                    'Dim lCurveColor As Color = GetMatchingColor(lScen & ":" & lLoc & ":" & lCons)
                    XTitle = lTimeUnit & " " & lCons & " at " & lLoc
                End With
            End If
            .Title.Text = XTitle()
        End With

        With pZgc.GraphPane.YAxis
            If YUseLog() Then
                .Type = AxisType.Log
            Else
                .Type = AxisType.Linear
            End If
            '.Scale.MaxAuto = False
            '.Scale.MinAuto = False
            '.MinSpace = 80
            If String.IsNullOrEmpty(YTitle()) Then
                With Datasets(0).Attributes
                    Dim lCons As String = .GetValue("constituent")
                    Dim lUnit As String = .GetValue("Units")
                    Dim lTimeUnit As String = TimeUnitText(Datasets(0))
                    YTitle = lCons & " " & lUnit
                End With
            End If
            .Title.Text = YTitle() 'lTimeseriesY.ToString
        End With
        'ScaleAxis(Datasets, pZgc.GraphPane.YAxis)
        BarPlot(listDataArrays, XLabels())
        If Double.IsNaN(XLabelBaseline) Then
            With pZgc.GraphPane.YAxis
                If .Type = AxisType.Linear Then
                    XLabelBaseline = .Scale.Min - (.Scale.MajorStep * 10)
                ElseIf .Type = AxisType.Log Then
                    XLabelBaseline = .Scale.Min - (.Scale.MajorStep * 10)
                End If
                '.Scale.MinAuto = False
                '.Scale.Min = XLabelBaseline - .Scale.MajorStep * 10
            End With
        End If
        pZgc.GraphPane.BarSettings.ClusterScaleWidthAuto = False
        pZgc.GraphPane.BarSettings.MinClusterGap = 0.2
        pZgc.GraphPane.BarSettings.MinBarGap = 0.2
        'With pZgc.GraphPane.XAxis
        '    .Type = AxisType.Text
        '    .Scale.TextLabels = pXLabels.ToArray()
        '    .Scale.FontSpec.Angle = -90
        'End With
        pZgc.GraphPane.AxisChange()
        'pZgc.GraphPane.YAxis.Scale.MinGrace = Math.Abs(XLabelBaseline) + pZgc.GraphPane.YAxis.Scale.MajorStep * 5

        If ShowLegend Then
            pZgc.GraphPane.Legend.IsHStack = True
            pZgc.GraphPane.Legend.Border.Color = Color.LightGray
        Else
            'use labels
            Dim lAngle As Single = -90
            If Not Single.IsNaN(XLabelAngle) AndAlso XLabelAngle <= 90 AndAlso XLabelAngle >= -90 Then
                lAngle = XLabelAngle
            End If
            Dim labelw As TextObj = Nothing
            Dim lBufferWid As Integer = -999
            For I As Integer = 0 To Datasets.Count - 1
                Dim label As TextObj = New TextObj(XLabels(I), I, 1.01, CoordType.XScaleYChartFraction, AlignH.Left, AlignV.Center)
                label.ZOrder = ZOrder.A_InFront
                label.FontSpec.Border.IsVisible = False
                label.FontSpec.Angle = XLabelAngle
                If ShowXLabelColor AndAlso DataColors IsNot Nothing AndAlso DataColors.Count = Datasets.Count Then
                    label.FontSpec.FontColor = DataColors(I)
                Else
                    'label.FontSpec.FontColor = Color.Black
                End If
                If System.Windows.Forms.TextRenderer.MeasureText(XLabels(I), label.FontSpec.GetFont(1.0)).Width > lBufferWid Then
                    lBufferWid = System.Windows.Forms.TextRenderer.MeasureText(XLabels(I), label.FontSpec.GetFont(1.0)).Width
                    labelw = label
                End If
                pZgc.GraphPane.GraphObjList.Add(label)
            Next
            If labelw IsNot Nothing Then
                pZgc.GraphPane.Margin.Bottom = System.Windows.Forms.TextRenderer.MeasureText(labelw.Text, labelw.FontSpec.GetFont(1.0)).Width + 15
            End If
        End If
        pZgc.GraphPane.AxisChange()

        If OutputToFile Then
            If IO.Directory.Exists(IO.Path.GetDirectoryName(OutputFile)) Then
                pZgc.GraphPane.GetImage().Save(OutputFile)
            End If
        End If
        pZgc.Refresh()
    End Sub

    Private Sub BarPlot(ByVal data As Generic.List(Of Double()), ByVal names As Generic.List(Of String))
    End Sub
End Class
