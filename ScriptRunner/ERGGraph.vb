Imports atcUtility
Imports atcData
Imports atcTimeseriesCSV
Imports atcWDM
Imports atcGraph
Imports atcList
Imports HspfSupport
Imports MapWindow.Interfaces
Imports ZedGraph
Imports MapWinUtility
Imports System

Module ERGGraph
    'black
    Private Const pTestPath As String = "C:\ERG_SteamElectric\Black"
    Private pLocation As Integer = 39 '1 '97 '39
    Private pStartYearForBaselinePlot As Integer = 1999
    Private pStartYearForSimulation As Integer = 1982
    Private pComplianceDate As Integer = 2019
    Private pSiteName As String = "Black"
    'etowah
    'Private Const pTestPath As String = "C:\ERG_SteamElectric\Etowah"
    ''Private Const pTestPath As String = "C:\ERG_SteamElectric\Etowah\RevisedBackground"
    'Private pLocation As Integer = 18 '1 '50 '18
    'Private pStartYearForBaselinePlot As Integer = 2004
    'Private pStartYearForSimulation As Integer = 1982
    'Private pComplianceDate As Integer = 2021
    'Private pSiteName As String = "Etowah"

    Public Sub ScriptMain(ByRef aMapWin As IMapWin)
        'DoBaselineHistoricGraphsMultipleLocations()
        'DoERGGraphs()
        'ComputeThreeMonthRollingAverages()
        DoERGCompositeGraph()
    End Sub

    Private Sub DoERGGraphs()

        Dim lCsvName As String = ""

        Dim pRunNames As New atcCollection
        pRunNames.Add("AsBaselineHistoric")
        pRunNames.Add("CdBaselineHistoric")
        pRunNames.Add("CuBaselineHistoric")
        pRunNames.Add("NiBaselineHistoric")
        pRunNames.Add("PbBaselineHistoric")
        pRunNames.Add("SeBaselineHistoric")
        pRunNames.Add("TlBaselineHistoric")
        pRunNames.Add("ZnBaselineHistoric")

        lCsvName = "Total_Concentration.csv"

        Dim lTimeseriesGroup As New atcTimeseriesGroup
        Dim lTimeseriesCsv As New atcTimeseriesCSV.atcTimeseriesCSV

        ChDriveDir(pTestPath & "\OutputPlots")
        'ChDriveDir(pTestPath & "\OutputPlotsBenthic")
        'ChDriveDir(pTestPath & "\OutputPlotsDownstream")


        Dim lSDate(5) As Integer : lSDate(0) = pStartYearForBaselinePlot : lSDate(1) = 1 : lSDate(2) = 1
        Dim lSDateJ As Double = Date2J(lSDate)
        Dim lEDate(5) As Integer : lEDate(0) = 2020 : lEDate(1) = 12 : lEDate(2) = 31
        Dim lEdatej As Double = Date2J(lEDate)

        'do baseline historic plots
        For Each lRunName As String In pRunNames

            Dim lOutFileName As String = lRunName & ".png"

            If Not FileExists(lOutFileName) Then

                'if start date file doesn't exist, copy from the first run name folder 
                If Not FileExists(pTestPath & "\" & lRunName & "\" & "Total_Concentration.csv.start") Then
                    FileCopy(pTestPath & "\" & pRunNames(0) & "\" & "Total_Concentration.csv.start", pTestPath & "\" & lRunName & "\" & "Total_Concentration.csv.start")
                End If

                Dim lCsvFileName As String = pTestPath & "\" & lRunName & "\" & lCsvName
                If lTimeseriesCsv.Open(lCsvFileName) Then

                    Dim lTimSer1 As atcTimeseries = lTimeseriesCsv.DataSets.ItemByKey(pLocation)

                    lTimSer1.Attributes.SetValue("YAxis", "Left")
                    lTimeseriesGroup.Add(SubsetByDate(lTimSer1, _
                                                lSDateJ, _
                                                lEdatej, Nothing))

                    GraphTimeseriesBaselineHistoric(lTimeseriesGroup, lRunName, lOutFileName)

                    'while we've got it open, also output average total conc for each year
                    Dim lAverageAnnualFileName As String = lRunName & "AnnualAverage.txt"
                    Dim lHeader As String = lRunName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                    WriteAverageAnnualFile(lAverageAnnualFileName, lTimSer1, pStartYearForSimulation, lHeader)

                    lTimeseriesCsv.Clear()
                    lTimeseriesGroup.Clear()
                Else
                    Logger.Msg("Unable to Open " & lCsvFileName)
                End If
            End If
        Next

        Dim pMetalNames As New atcCollection
        pMetalNames.Add("As")
        pMetalNames.Add("Cd")
        pMetalNames.Add("Cu")
        pMetalNames.Add("Ni")
        pMetalNames.Add("Pb")
        pMetalNames.Add("Se")
        pMetalNames.Add("Tl")
        pMetalNames.Add("Zn")

        lSDate(0) = pComplianceDate : lSDate(1) = 1 : lSDate(2) = 1
        lSDateJ = Date2J(lSDate)
        lEDate(0) = pComplianceDate + 9 : lEDate(1) = 12 : lEDate(2) = 31
        lEdatej = Date2J(lEDate)

        Dim lTimeseriesOptionsGroup As New atcTimeseriesGroup
        Dim lTimeseriesCsvOption As New atcTimeseriesCSV.atcTimeseriesCSV

        'do post-compliance option plots
        For Each lMetalName As String In pMetalNames
            Dim lOutFileName As String = lMetalName & "Options.png"

            If Not FileExists(lOutFileName) Then
                Dim lOptionName As String = ""

                lOptionName = lMetalName & "Baseline"
                If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                    'baseline folder exists

                    'if start date file doesn't exist, copy from the first run name folder 
                    If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start") Then
                        FileCopy(pTestPath & "\SeBaseline\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start")
                    End If

                    Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                    If lTimeseriesCsvOption.Open(lCsvFileName) Then
                        Dim lTimSerX As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(pLocation)
                        lTimSerX.Attributes.SetValue("YAxis", "Left")
                        lTimSerX.Attributes.SetValue("Constituent", "Baseline")
                        lTimeseriesGroup.Add(SubsetByDate(lTimSerX, _
                                                    lSDateJ, _
                                                    lEdatej, Nothing))

                        'while we've got it open, also output average total conc for each year
                        Dim lAverageAnnualFileName As String = lOptionName & "AnnualAverage.txt"
                        Dim lHeader As String = lOptionName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                        WriteAverageAnnualFile(lAverageAnnualFileName, lTimSerX, pComplianceDate, lHeader)

                        lTimeseriesCsvOption.Clear()
                    Else
                        Logger.Msg("Unable to Open " & lCsvFileName)
                    End If
                End If

                lOptionName = lMetalName & "OptionA"
                If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                    'folder exists

                    'if start date file doesn't exist, copy from the first run name folder 
                    If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start") Then
                        FileCopy(pTestPath & "\SeBaseline\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start")
                    End If

                    Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                    If lTimeseriesCsvOption.Open(lCsvFileName) Then
                        Dim lTimSerA As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(pLocation)
                        lTimSerA.Attributes.SetValue("YAxis", "Left")
                        lTimSerA.Attributes.SetValue("Constituent", "Option A")
                        lTimeseriesGroup.Add(SubsetByDate(lTimSerA, _
                                                    lSDateJ, _
                                                    lEdatej, Nothing))

                        'while we've got it open, also output average total conc for each year
                        Dim lAverageAnnualFileName As String = lOptionName & "AnnualAverage.txt"
                        Dim lHeader As String = lOptionName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                        WriteAverageAnnualFile(lAverageAnnualFileName, lTimSerA, pComplianceDate, lHeader)

                        lTimeseriesCsvOption.Clear()
                    Else
                        Logger.Msg("Unable to Open " & lCsvFileName)
                    End If
                End If

                lOptionName = lMetalName & "OptionB"
                If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                    'folder exists

                    'if start date file doesn't exist, copy from the first run name folder 
                    If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start") Then
                        FileCopy(pTestPath & "\SeBaseline\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start")
                    End If

                    Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                    If lTimeseriesCsvOption.Open(lCsvFileName) Then
                        Dim lTimSerB As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(pLocation)
                        lTimSerB.Attributes.SetValue("YAxis", "Left")
                        lTimSerB.Attributes.SetValue("Constituent", "Option B")
                        lTimeseriesGroup.Add(SubsetByDate(lTimSerB, _
                                                    lSDateJ, _
                                                    lEdatej, Nothing))

                        'while we've got it open, also output average total conc for each year
                        Dim lAverageAnnualFileName As String = lOptionName & "AnnualAverage.txt"
                        Dim lHeader As String = lOptionName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                        WriteAverageAnnualFile(lAverageAnnualFileName, lTimSerB, pComplianceDate, lHeader)

                        lTimeseriesCsvOption.Clear()
                    Else
                        Logger.Msg("Unable to Open " & lCsvFileName)
                    End If
                End If

                lOptionName = lMetalName & "OptionC"
                If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                    'folder exists

                    'if start date file doesn't exist, copy from the first run name folder 
                    If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start") Then
                        FileCopy(pTestPath & "\SeBaseline\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start")
                    End If

                    Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                    If lTimeseriesCsvOption.Open(lCsvFileName) Then
                        Dim lTimSerC As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(pLocation)
                        lTimSerC.Attributes.SetValue("YAxis", "Left")
                        lTimSerC.Attributes.SetValue("Constituent", "Option C")
                        lTimeseriesGroup.Add(SubsetByDate(lTimSerC, _
                                                    lSDateJ, _
                                                    lEdatej, Nothing))

                        'while we've got it open, also output average total conc for each year
                        Dim lAverageAnnualFileName As String = lOptionName & "AnnualAverage.txt"
                        Dim lHeader As String = lOptionName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                        WriteAverageAnnualFile(lAverageAnnualFileName, lTimSerC, pComplianceDate, lHeader)

                        lTimeseriesCsvOption.Clear()
                    Else
                        Logger.Msg("Unable to Open " & lCsvFileName)
                    End If
                End If

                lOptionName = lMetalName & "OptionD"
                If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                    'folder exists

                    'if start date file doesn't exist, copy from the first run name folder 
                    If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start") Then
                        FileCopy(pTestPath & "\SeBaseline\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start")
                    End If

                    Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                    If lTimeseriesCsvOption.Open(lCsvFileName) Then
                        Dim lTimSerD As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(pLocation)
                        lTimSerD.Attributes.SetValue("YAxis", "Left")
                        lTimSerD.Attributes.SetValue("Constituent", "Option D")
                        lTimeseriesGroup.Add(SubsetByDate(lTimSerD, _
                                                    lSDateJ, _
                                                    lEdatej, Nothing))

                        'while we've got it open, also output average total conc for each year
                        Dim lAverageAnnualFileName As String = lOptionName & "AnnualAverage.txt"
                        Dim lHeader As String = lOptionName & " Annual Average from " & lCsvName & " at Seg " & pLocation
                        WriteAverageAnnualFile(lAverageAnnualFileName, lTimSerD, pComplianceDate, lHeader)

                        lTimeseriesCsvOption.Clear()
                    Else
                        Logger.Msg("Unable to Open " & lCsvFileName)
                    End If
                End If

                GraphTimeseriesOptions(lTimeseriesGroup, lMetalName, lOutFileName, "Computed Tot Conc Ug/L", "")
                lTimeseriesGroup.Clear()
            End If
        Next

    End Sub

    Sub GraphTimeseriesBaselineHistoric(ByVal aDataGroup As atcTimeseriesGroup, ByVal aRunName As String, ByVal aOutFile As String)
        Dim lZgc As ZedGraphControl = CreateZgc()
        Dim lGrapher As New clsGraphTime(aDataGroup, lZgc)
        Dim lPane As GraphPane = lZgc.MasterPane.PaneList(0)
        Dim lCurve As ZedGraph.LineItem

        For i As Integer = 0 To lPane.CurveList.Count - 1
            lCurve = lPane.CurveList.Item(i)
            If i = 0 Then
                lCurve.Color = Drawing.Color.Blue
            ElseIf i = 1 Then
                lCurve.Color = Drawing.Color.Green
            End If
        Next i

        lPane.YAxis.Scale.Min = 0

        If lPane.CurveList.Count = 1 Then
            lPane.XAxis.Title.Text = aRunName.Substring(0, 2) & " at Segment " & pLocation
        Else
            lPane.XAxis.Title.Text = aRunName.Substring(0, 2) & " at Segments " & pLocation & " and " & (pLocation - 1).ToString
        End If
        lZgc.Width = 1200

        lZgc.SaveIn(aOutFile)
        lZgc.Dispose()
    End Sub

    Sub GraphTimeseriesOptions(ByVal aDataGroup As atcTimeseriesGroup, ByVal aRunName As String, ByVal aOutFile As String, ByVal aUnits As String, ByVal aSiteName As String)
        Dim lZgc As ZedGraphControl = CreateZgc()
        Dim lGrapher As New clsGraphTime(aDataGroup, lZgc)
        Dim lPane As GraphPane = lZgc.MasterPane.PaneList(0)
        Dim lCurve As ZedGraph.LineItem

        For i As Integer = 0 To lPane.CurveList.Count - 1
            lCurve = lPane.CurveList.Item(i)
            If lCurve.Label.Text = "Computed Baseline" Then
                lCurve.Color = Drawing.Color.Blue
                lCurve.Label.Text = "Baseline"
            ElseIf lCurve.Label.Text = "Computed Option A" Then
                lCurve.Color = Drawing.Color.Green
                lCurve.Label.Text = "Option A"
            ElseIf lCurve.Label.Text = "Computed Option B" Then
                lCurve.Color = Drawing.Color.Red
                lCurve.Label.Text = "Option B"
            ElseIf lCurve.Label.Text = "Computed Option C" Then
                lCurve.Color = Drawing.Color.Cyan
                lCurve.Label.Text = "Option C"
            ElseIf lCurve.Label.Text = "Computed Option D" Then
                lCurve.Color = Drawing.Color.Magenta
                lCurve.Label.Text = "Option D"
            End If
        Next i

        If aOutFile.Contains("Composite") Then
            lCurve = lPane.CurveList.Item(0)
            lCurve.Color = Drawing.Color.Blue
            lCurve = lPane.CurveList.Item(1)
            lCurve.Color = Drawing.Color.Purple
            lCurve = lPane.CurveList.Item(2)
            lCurve.Color = Drawing.Color.LightGray

            lZgc.Width = 1800
            lPane.Legend.IsVisible = False
            lPane.XAxis.Title.Text = ""
            lPane.YAxis.Type = ZedGraph.AxisType.Log
            'lPane.YAxis.Scale.Min = 0.001

            'add up to seven additional solid lines
            Dim lNationalModelBaseline As Double = 0.0
            Dim lNationalModelOptionD As Double = 0.0
            Dim lAquaBenchmarkAcute As Double = 0.0
            Dim lAquaBenchmarkChronic As Double = 0.0
            Dim lHealthBenchmarkWO As Double = 0.0
            Dim lHealthBenchmarkO As Double = 0.0
            Dim lMCL As Double = 0.0

            'site specific data 
            If aSiteName = "Black" Then
                If aRunName = "As" Then
                    lNationalModelBaseline = 0.00003109
                    lNationalModelOptionD = 0.0000216
                ElseIf aRunName = "Cd" Then
                    lNationalModelBaseline = 0.0001746
                    lNationalModelOptionD = 0.000007179
                ElseIf aRunName = "Cu" Then
                    lNationalModelBaseline = 0.00004359
                    lNationalModelOptionD = 0.000008495
                ElseIf aRunName = "Pb" Then
                    lNationalModelBaseline = 0.00001324
                    lNationalModelOptionD = 0.000005569
                ElseIf aRunName = "Ni" Then
                    lNationalModelBaseline = 0.001391
                    lNationalModelOptionD = 0.00002862
                ElseIf aRunName = "Se" Then
                    lNationalModelBaseline = 0.00181
                    lNationalModelOptionD = 0.00002911
                ElseIf aRunName = "Tl" Then
                    lNationalModelBaseline = 0.00005413
                    lNationalModelOptionD = 0.00001535
                ElseIf aRunName = "Zn" Then
                    lNationalModelBaseline = 0.002195
                    lNationalModelOptionD = 0.00007315
                End If
            ElseIf aSiteName = "Etowah" Then
                If aRunName = "As" Then
                    lNationalModelBaseline = 0.00000728
                    lNationalModelOptionD = 0.000003614
                ElseIf aRunName = "Cd" Then
                    lNationalModelBaseline = 0.0000704
                    lNationalModelOptionD = 0.000002608
                ElseIf aRunName = "Cu" Then
                    lNationalModelBaseline = 0.00001636
                    lNationalModelOptionD = 0.000002341
                ElseIf aRunName = "Pb" Then
                    lNationalModelBaseline = 0.000005056
                    lNationalModelOptionD = 0.000002101
                ElseIf aRunName = "Ni" Then
                    lNationalModelBaseline = 0.0005551
                    lNationalModelOptionD = 0.000003906
                ElseIf aRunName = "Se" Then
                    lNationalModelBaseline = 0.0007252
                    lNationalModelOptionD = 0.000003546
                ElseIf aRunName = "Tl" Then
                    lNationalModelBaseline = 0.00002092
                    lNationalModelOptionD = 0.000006077
                ElseIf aRunName = "Zn" Then
                    lNationalModelBaseline = 0.0008717
                    lNationalModelOptionD = 0.00001238
                End If
            End If

            If aRunName = "As" Then
                lPane.YAxis.Title.Text = "Total Arsenic (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 0.000018
                lHealthBenchmarkO = 0.00014
                lMCL = 0.01
            ElseIf aRunName = "Cd" Then
                lPane.YAxis.Title.Text = "Total Cadmium (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 0.0
                lHealthBenchmarkO = 0.0
                lMCL = 0.0
            ElseIf aRunName = "Cu" Then
                lPane.YAxis.Title.Text = "Total Copper (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 1.3
                lHealthBenchmarkO = 0.0
                lMCL = 1.3
            ElseIf aRunName = "Ni" Then
                lPane.YAxis.Title.Text = "Total Nickel (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 0.61
                lHealthBenchmarkO = 4.6
                lMCL = 0.0
            ElseIf aRunName = "Pb" Then
                lPane.YAxis.Title.Text = "Total Lead (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 0.0
                lHealthBenchmarkO = 0.0
                lMCL = 0.0
            ElseIf aRunName = "Se" Then
                lPane.YAxis.Title.Text = "Total Selenium (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.005
                lHealthBenchmarkWO = 0.17
                lHealthBenchmarkO = 4.2
                lMCL = 0.05
            ElseIf aRunName = "Tl" Then
                lPane.YAxis.Title.Text = "Total Thallium (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 0.00024
                lHealthBenchmarkO = 0.00047
                lMCL = 0.0
            ElseIf aRunName = "Zn" Then
                lPane.YAxis.Title.Text = "Total Zinc (ug/L)"
                lAquaBenchmarkAcute = 0.0
                lAquaBenchmarkChronic = 0.0
                lHealthBenchmarkWO = 7.4
                lHealthBenchmarkO = 26
                lMCL = 0.0
            End If
            'now add them
            If lNationalModelBaseline > 0.0 Then
                Dim lLine1 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lNationalModelBaseline * 1000, Drawing.Drawing2D.DashStyle.Dash)
                lLine1.Color = Drawing.Color.Blue
                lLine1.Line.Width = 10
            End If
            If lNationalModelOptionD > 0.0 Then
                Dim lLine2 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lNationalModelOptionD * 1000, Drawing.Drawing2D.DashStyle.Dash)
                lLine2.Color = Drawing.Color.Purple
                lLine2.Line.Width = 10
            End If
            If lAquaBenchmarkAcute > 0.0 Then
                Dim lLine3 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lAquaBenchmarkAcute * 1000, Drawing.Drawing2D.DashStyle.Dot)
                lLine3.Color = Drawing.Color.Brown
                lLine3.Line.Width = 10
            End If
            If lAquaBenchmarkChronic > 0.0 Then
                Dim lLine4 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lAquaBenchmarkChronic * 1000, Drawing.Drawing2D.DashStyle.Dot)
                lLine4.Color = Drawing.Color.Green
                lLine4.Line.Width = 10
            End If
            If lHealthBenchmarkWO > 0.0 Then
                Dim lLine5 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lHealthBenchmarkWO * 1000, Drawing.Drawing2D.DashStyle.DashDot)
                lLine5.Color = Drawing.Color.Orange
                lLine5.Line.Width = 2
            End If
            If lHealthBenchmarkO > 0.0 Then
                Dim lLine6 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lHealthBenchmarkO * 1000, Drawing.Drawing2D.DashStyle.DashDot)
                lLine6.Color = Drawing.Color.Red
                lLine6.Line.Width = 2
            End If
            If lMCL > 0.0 Then
                Dim lLine7 As ZedGraph.LineItem = AddLineMine(lPane, 0.0, lMCL * 1000, Drawing.Drawing2D.DashStyle.DashDotDot)
                lLine7.Color = Drawing.Color.Pink
                lLine7.Line.Width = 10
            End If

        Else
            lZgc.Width = 1200
            lPane.XAxis.Title.Text = aRunName.Substring(0, 2) & " at Segment " & pLocation
            lPane.YAxis.Scale.Min = 0
            lPane.YAxis.Title.Text = aUnits
        End If

        lZgc.SaveIn(aOutFile)
        lZgc.Dispose()
    End Sub

    Sub WriteAverageAnnualFile(ByVal aAverageAnnualFileName As String, ByVal aTimSer As atcTimeseries, ByVal aStartYear As Integer, ByVal aHeader As String)
        Dim aAnnualTimSer As atcTimeseries = atcData.modTimeseriesMath.Aggregate(aTimSer, atcTimeUnit.TUYear, 1, atcTran.TranAverSame)
        Dim lWrite As New IO.StreamWriter(aAverageAnnualFileName, False)
        Dim lDate(5) As Integer

        Dim lStr As String = aHeader
        lWrite.WriteLine(lStr)

        For lIndex As Integer = 1 To aAnnualTimSer.numValues
            J2Date(aAnnualTimSer.Dates.Value(lIndex), lDate)
            If lDate(0) >= aStartYear Then
                lStr = lDate(0).ToString & " " & DoubleToString(aAnnualTimSer.Value(lIndex))
                lWrite.WriteLine(lStr)
            End If
        Next
        lWrite.Close()
    End Sub

    Private Sub ComputeThreeMonthRollingAverages()
        ChDriveDir(pTestPath & "\OutputPlots")
        Dim lCsvName As String = "Dissolved_Concentration.csv"

        Dim lSDate(5) As Integer : lSDate(0) = pComplianceDate : lSDate(1) = 1 : lSDate(2) = 1
        Dim lSDateJ As Double = Date2J(lSDate)

        Dim pMetalNames As New atcCollection
        'pMetalNames.Add("As")
        'pMetalNames.Add("Cd")
        'pMetalNames.Add("Cu")
        'pMetalNames.Add("Ni")
        'pMetalNames.Add("Pb")
        pMetalNames.Add("Se")
        'pMetalNames.Add("Tl")
        'pMetalNames.Add("Zn")

        Dim pOptions As New atcCollection
        pOptions.Add("Baseline")
        pOptions.Add("OptionA")
        pOptions.Add("OptionB")
        pOptions.Add("OptionC")
        pOptions.Add("OptionD")

        Dim lTimeseriesCsvOption As New atcTimeseriesCSV.atcTimeseriesCSV
        Dim lTimeseriesGroup As New atcTimeseriesGroup

        'do post-compliance three month rolling averages
        For Each lMetalName As String In pMetalNames
            For Each lOption As String In pOptions
                Dim lOptionName As String = ""
                lOptionName = lMetalName & lOption
                Dim lThreeMonthAverageFileName As String = lOptionName & "ThreeMonthRollingAverage.csv"

                If Not FileExists(lThreeMonthAverageFileName) Then
                    If FileExists(pTestPath & "\" & lOptionName & "\", True) Then
                        'option folder exists

                        'if start date file doesn't exist, copy from the total conc 
                        If Not FileExists(pTestPath & "\" & lOptionName & "\" & "Dissolved_Concentration.csv.start") Then
                            FileCopy(pTestPath & "\" & lOptionName & "\" & "Total_Concentration.csv.start", pTestPath & "\" & lOptionName & "\" & "Dissolved_Concentration.csv.start")
                        End If

                        Dim lCsvFileName As String = pTestPath & "\" & lOptionName & "\" & lCsvName
                        If lTimeseriesCsvOption.Open(lCsvFileName) Then

                            Dim lHeader As String = lOptionName & " Three Month Rolling Average from " & lCsvName

                            For lLoc As Integer = 1 To pLocation
                                Dim lTimSerX As atcTimeseries = lTimeseriesCsvOption.DataSets.ItemByKey(lLoc)
                                lTimeseriesGroup.Add(lTimSerX)
                            Next

                            WriteThreeMonthAverageFile(lThreeMonthAverageFileName, lTimeseriesGroup, pComplianceDate, lHeader)

                            lTimeseriesCsvOption.Clear()
                            lTimeseriesGroup.Clear()
                        Else
                            Logger.Msg("Unable to Open " & lCsvFileName)
                        End If
                    End If

                End If
            Next
        Next
    End Sub

    Sub WriteThreeMonthAverageFile(ByVal aThreeMonthFileName As String, ByVal aTimSerGroup As atcTimeseriesGroup, ByVal aStartYear As Integer, ByVal aHeader As String)
        Dim aTimSer1 As atcTimeseries = aTimSerGroup(0)
        Dim lWrite As New IO.StreamWriter(aThreeMonthFileName, False)
        Dim lDate(5) As Integer

        Dim lStr As String = aHeader
        lWrite.WriteLine(lStr)
        lStr = "Year, Month "
        For lIndex As Integer = 1 To aTimSerGroup.Count
            lStr &= ", " & lIndex.ToString
        Next
        lWrite.WriteLine(lStr)

        'make a first pass to find the start of each desired month
        Dim lMonthStarts As New atcCollection
        For lIndex As Integer = 1 To aTimSer1.numValues
            J2Date(aTimSer1.Dates.Value(lIndex), lDate)
            If lDate(0) >= aStartYear Then
                If lDate(2) = 1 Then
                    lMonthStarts.Add(lIndex)
                End If
            End If
        Next

        Dim lStartingIndex As Integer = 0
        Dim lEndingIndex As Integer = 0
        For lMonth As Integer = 0 To lMonthStarts.Count - 3
            lStartingIndex = lMonthStarts(lMonth)
            If lMonth = lMonthStarts.Count - 3 Then
                'last month
                lEndingIndex = aTimSer1.numValues
            Else
                lEndingIndex = lMonthStarts(lMonth + 3) - 1
            End If
            'get average of values between start and ending index and write
            J2Date(aTimSer1.Dates.Value(lEndingIndex), lDate)
            Dim lYr As Integer = lDate(0)
            Dim lMo As Integer = lDate(1)
            lStr = lYr.ToString & ", " & lMo.ToString
            For Each lTimSer As atcTimeseries In aTimSerGroup
                Dim lThreeMonthSum As Double = 0.0
                For lIndexX As Integer = lStartingIndex To lEndingIndex
                    lThreeMonthSum += lTimSer.Value(lIndexX)
                Next
                Dim lValue As String = Format((lThreeMonthSum / (lEndingIndex - lStartingIndex + 1)), "0.####")
                lStr = lStr & ", " & lValue
            Next
            lWrite.WriteLine(lStr)
        Next
        lWrite.Close()
    End Sub

    Private Sub DoBaselineHistoricGraphsMultipleLocations()

        Dim lCsvName As String = ""

        Dim pRunNames As New atcCollection
        pRunNames.Add("AsBaselineHistoric")
        pRunNames.Add("CdBaselineHistoric")
        pRunNames.Add("CuBaselineHistoric")
        pRunNames.Add("NiBaselineHistoric")
        pRunNames.Add("PbBaselineHistoric")
        pRunNames.Add("SeBaselineHistoric")
        pRunNames.Add("TlBaselineHistoric")
        pRunNames.Add("ZnBaselineHistoric")

        lCsvName = "Total_Concentration.csv"

        Dim lTimeseriesGroup As New atcTimeseriesGroup
        Dim lTimeseriesCsv As New atcTimeseriesCSV.atcTimeseriesCSV

        ChDriveDir(pTestPath & "\OutputPlots")

        Dim lSDate(5) As Integer : lSDate(0) = pStartYearForBaselinePlot : lSDate(1) = 1 : lSDate(2) = 1
        Dim lSDateJ As Double = Date2J(lSDate)
        Dim lEDate(5) As Integer : lEDate(0) = 2020 : lEDate(1) = 12 : lEDate(2) = 31
        Dim lEdatej As Double = Date2J(lEDate)

        'do baseline historic plots
        For Each lRunName As String In pRunNames

            Dim lOutFileName As String = lRunName & "IRWandNextDownstream.png"

            If Not FileExists(lOutFileName) Then

                'if start date file doesn't exist, copy from the first run name folder 
                If Not FileExists(pTestPath & "\" & lRunName & "\" & "Total_Concentration.csv.start") Then
                    FileCopy(pTestPath & "\" & pRunNames(0) & "\" & "Total_Concentration.csv.start", pTestPath & "\" & lRunName & "\" & "Total_Concentration.csv.start")
                End If

                Dim lCsvFileName As String = pTestPath & "\" & lRunName & "\" & lCsvName
                If lTimeseriesCsv.Open(lCsvFileName) Then

                    Dim lTimSer1 As atcTimeseries = lTimeseriesCsv.DataSets.ItemByKey(pLocation)

                    lTimSer1.Attributes.SetValue("YAxis", "Left")
                    lTimeseriesGroup.Add(SubsetByDate(lTimSer1, _
                                                lSDateJ, _
                                                lEdatej, Nothing))

                    Dim lTimSer2 As atcTimeseries = lTimeseriesCsv.DataSets.ItemByKey(pLocation - 1)

                    lTimSer2.Attributes.SetValue("YAxis", "Left")
                    lTimeseriesGroup.Add(SubsetByDate(lTimSer2, _
                                                lSDateJ, _
                                                lEdatej, Nothing))

                    GraphTimeseriesBaselineHistoric(lTimeseriesGroup, lRunName, lOutFileName)

                    lTimeseriesCsv.Clear()
                    lTimeseriesGroup.Clear()
                Else
                    Logger.Msg("Unable to Open " & lCsvFileName)
                End If
            End If
        Next
    End Sub

    Private Sub DoERGCompositeGraph()

        Dim lCsvName As String = ""

        Dim pMetalNames As New atcCollection
        'pMetalNames.Add("As")
        'pMetalNames.Add("Cd")
        'pMetalNames.Add("Cu")
        'pMetalNames.Add("Ni")
        'pMetalNames.Add("Pb")
        'pMetalNames.Add("Se")
        pMetalNames.Add("Tl")
        'pMetalNames.Add("Zn")

        lCsvName = "Total_Concentration.csv"

        Dim lTimeseriesGroup As New atcTimeseriesGroup

        ChDriveDir(pTestPath & "\OutputPlots")

        For Each lMetal As String In pMetalNames

            Dim lOutFileName As String = pSiteName & lMetal.Substring(0, 2) & "Composite" & ".png"
            
            'get baseline historic data
            Dim lTimeseriesCsv1 As New atcTimeseriesCSV.atcTimeseriesCSV
            Dim lRunName As String = lMetal & "BaselineHistoric"
            Dim lCsvFileName As String = pTestPath & "\" & lRunName & "\" & lCsvName
            If lTimeseriesCsv1.Open(lCsvFileName) Then

                Dim lTimSer1 As atcTimeseries = lTimeseriesCsv1.DataSets.ItemByKey(pLocation)

                lTimSer1.Attributes.SetValue("YAxis", "Left")

                Dim lSDate(5) As Integer : lSDate(0) = pComplianceDate - 10 : lSDate(1) = 1 : lSDate(2) = 1
                Dim lSDateJ As Double = Date2J(lSDate)
                Dim lEDate(5) As Integer : lEDate(0) = pComplianceDate - 1 : lEDate(1) = 12 : lEDate(2) = 31
                Dim lEdatej As Double = Date2J(lEDate)

                lTimeseriesGroup.Add(SubsetByDate(lTimSer1, _
                                            lSDateJ, _
                                            lEdatej, Nothing))
                lTimeseriesCsv1.Clear()
            Else
                Logger.Msg("Unable to Open " & lCsvFileName)
            End If

            'get option d post-compliance data
            Dim lTimeseriesCsv3 As New atcTimeseriesCSV.atcTimeseriesCSV
            lRunName = lMetal & "OptionD"
            lCsvFileName = pTestPath & "\" & lRunName & "\" & lCsvName
            If lTimeseriesCsv3.Open(lCsvFileName) Then

                Dim lTimSerD As atcTimeseries = lTimeseriesCsv3.DataSets.ItemByKey(pLocation)

                lTimSerD.Attributes.SetValue("YAxis", "Left")

                Dim lSDate(5) As Integer : lSDate(0) = pComplianceDate : lSDate(1) = 1 : lSDate(2) = 1
                Dim lSDateJ As Double = Date2J(lSDate)
                Dim lEDate(5) As Integer : lEDate(0) = pComplianceDate + 9 : lEDate(1) = 12 : lEDate(2) = 31
                Dim lEdatej As Double = Date2J(lEDate)

                lTimeseriesGroup.Add(SubsetByDate(lTimSerD, _
                                            lSDateJ, _
                                            lEdatej, Nothing))
                lTimeseriesCsv3.Clear()
            Else
                Logger.Msg("Unable to Open " & lCsvFileName)
            End If

            'get baseline post-compliance data
            Dim lTimeseriesCsv2 As New atcTimeseriesCSV.atcTimeseriesCSV
            lRunName = lMetal & "Baseline"
            lCsvFileName = pTestPath & "\" & lRunName & "\" & lCsvName
            If lTimeseriesCsv2.Open(lCsvFileName) Then

                Dim lTimSer2 As atcTimeseries = lTimeseriesCsv2.DataSets.ItemByKey(pLocation)

                lTimSer2.Attributes.SetValue("YAxis", "Left")

                Dim lSDate(5) As Integer : lSDate(0) = pComplianceDate : lSDate(1) = 1 : lSDate(2) = 1
                Dim lSDateJ As Double = Date2J(lSDate)
                Dim lEDate(5) As Integer : lEDate(0) = pComplianceDate + 9 : lEDate(1) = 12 : lEDate(2) = 31
                Dim lEdatej As Double = Date2J(lEDate)

                lTimeseriesGroup.Add(SubsetByDate(lTimSer2, _
                                            lSDateJ, _
                                            lEdatej, Nothing))
                lTimeseriesCsv2.Clear()
            Else
                Logger.Msg("Unable to Open " & lCsvFileName)
            End If
            
            GraphTimeseriesOptions(lTimeseriesGroup, lMetal, lOutFileName, "Computed Tot Conc Ug/L", pSiteName)

            lTimeseriesGroup.Clear()

        Next

    End Sub

    Public Function AddLineMine(ByRef aPane As ZedGraph.GraphPane, _
                            ByVal aACoef As Double, _
                            ByVal aBCoef As Double, _
                   Optional ByVal aLineStyle As Drawing.Drawing2D.DashStyle = Drawing.Drawing2D.DashStyle.Solid, _
                   Optional ByVal aTag As String = Nothing) As LineItem
        With aPane
            Dim lXValues(2) As Double
            Dim lYValues(2) As Double
            Dim lStep As Double = (.XAxis.Scale.Max - .XAxis.Scale.Min) / lXValues.GetUpperBound(0)
            For lIndex As Integer = 0 To lXValues.GetUpperBound(0)
                lXValues(lIndex) = .XAxis.Scale.Min + (lStep * lIndex)
                lYValues(lIndex) = (aACoef * lXValues(lIndex)) + aBCoef
            Next
            Dim lCurve As LineItem = .AddCurve("", lXValues, lYValues, Drawing.Color.Blue, SymbolType.None)
            lCurve.Line.Style = aLineStyle
            lCurve.Tag = aTag
            Return lCurve
        End With
    End Function
End Module