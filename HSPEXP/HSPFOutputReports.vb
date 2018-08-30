Imports atcUtility
Imports atcData
Imports atcGraph
Imports HspfSupport
Imports atcUCI
Imports MapWinUtility 'this has to be downloaded separately from http://svn.mapwindow.org/svnroot/MapWindow4Dev/Bin/
Imports System.Collections.Specialized
Imports System.IO 'added by Becky to get directory exists function
Imports System.Xml
Imports System.Data

Module HSPFOutputReports
    Private pTestPath As String
    Private pBaseName As String 'this is the base part of the file name (i.e., without .uci, .wdm, .exs) - it MUST be used to name everything
    Private pOutputLocations As New atcCollection

    Private pGraphSaveFormat As String
    Private pGraphSaveWidth As Integer
    Private pGraphSaveHeight As Integer
    Private pGraphAnnual As Boolean = True
    Private pCurveStepType As String = "NonStep"
    Private pConstituents As New atcCollection
    Private pWaterYears As Boolean = False

    Private pMakeAreaReports As Boolean 'flag to indicate user wants subwatershed & land use reports created
    Friend pHSPFExe As String '= FindFile("Please locate WinHspfLt.exe", IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly.Location), "WinHSPFLt", "WinHspfLt.exe"))
    Private pRunUci As Boolean = False 'Anurag added this option if the user wants this program to run the uci as well
    Private pModelQAQC As Boolean = False
    Private SDateJ, EDateJ As Double
    Private loutfoldername As String
    Private MultiSimulation As Boolean = False
    Private pListModelParameters As Boolean = False
    Private ASDate, AEDate As Date

    Private Sub Initialize()
        If Logger.ProgressStatus Is Nothing OrElse Not (TypeOf (Logger.ProgressStatus) Is MonitorProgressStatus) Then
            'Start running status monitor to give better progress and status indication during long-running processes
            Dim pStatusMonitor As New MonitorProgressStatus
            If pStatusMonitor.StartMonitor(FindFile("Find Status Monitor", "StatusMonitor.exe"),
                                            IO.Directory.GetCurrentDirectory,
                                            System.Diagnostics.Process.GetCurrentProcess.Id) Then
                'put our status monitor (StatusMonitor.exe) between the Logger and the default MW status monitor
                pStatusMonitor.InnerProgressStatus = Logger.ProgressStatus
                Logger.ProgressStatus = pStatusMonitor
                Logger.Status("LABEL TITLE HSPEXP+")
                Logger.Status("PROGRESS TIME OFF") 'Disable time-to-completion estimation
                Logger.Status("")
            Else
                pStatusMonitor.StopMonitor()
                pStatusMonitor = Nothing
            End If
        End If

        Logger.Status("HSPEXP+ started", True)
        pOutputLocations.Clear()

        pGraphSaveFormat = ".png"
        pGraphSaveWidth = 1300
        pGraphSaveHeight = 768
        pModelQAQC = StartUp.chkModelQAQC.Checked


        pRunUci = StartUp.chkRunHSPF.Checked
        pMakeAreaReports = StartUp.chkAreaReports.Checked

        Dim lTestName As String = IO.Path.GetFileNameWithoutExtension(StartUp.cmbUCIPath.Text)
        Logger.Status("Beginning analysis of " & lTestName, True)

        If StartUp.chkMultiSim.Checked Then
            MultiSimulation = True
        End If
        pConstituents.Clear()
        If StartUp.chkWaterBalance.Checked Then
            pConstituents.Add("Water")
        End If
        If StartUp.chkSedimentBalance.Checked Then
            pConstituents.Add("Sediment")
        End If

        If StartUp.chkTotalNitrogen.Checked Then
            pConstituents.Add("TN")
        End If

        If StartUp.chkTotalPhosphorus.Checked Then
            pConstituents.Add("TP")
        End If
        If StartUp.chkBODBalance.Checked Then
            pConstituents.Add("BOD-Labile")
        End If

        If StartUp.chkDO.Checked Then
            pConstituents.Add("DO")
        End If

        If StartUp.chkHeat.Checked Then
            pConstituents.Add("Heat")
        End If

        If StartUp.chkGQUAL1.Checked Then
            pConstituents.Add(StartUp.chkGQUAL1.Text & "-1")
        End If
        If StartUp.chkGQUAL2.Checked Then
            pConstituents.Add(StartUp.chkGQUAL2.Text & "-2")
        End If
        If StartUp.chkGQUAL3.Checked Then
            pConstituents.Add(StartUp.chkGQUAL3.Text & "-3")
        End If
        If StartUp.chkGQUAL4.Checked Then
            pConstituents.Add(StartUp.chkGQUAL4.Text & "-4")
        End If
        If StartUp.chkGQUAL5.Checked Then
            pConstituents.Add(StartUp.chkGQUAL5.Text & "-5")
        End If
        If StartUp.chkGQUAL6.Checked Then
            pConstituents.Add(StartUp.chkGQUAL6.Text & "-6")
        End If
        If StartUp.chkGQUAL7.Checked Then
            pConstituents.Add(StartUp.chkGQUAL7.Text & "-7")
        End If

        'set up the timeseries attributes for statistics
        atcTimeseriesStatistics.atcTimeseriesStatistics.InitializeShared()

        pTestPath = StartUp.cmbUCIPath.Text
        pBaseName = lTestName
        pTestPath = Mid(pTestPath, 1, Len(pTestPath) - Len(pBaseName) - 4)

        For Each lRCH As String In StartUp.txtRCH.Text.Split(","c)
            If IsNumeric(lRCH) Then
                pOutputLocations.Add("R:" & CInt(lRCH)) ' the Cint should get rid of leading spaces and zeros 
            End If
        Next
        StartUp.Hide()
        Logger.StartToFile(pTestPath & "LogFile.txt", , False)
        Logger.Status("Run characteristics read", True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="aMapWin"></param>
    ''' <param name="aHspfUci"></param>
    Public Sub ScriptMain(ByRef aMapWin As Object, ByVal aHspfUci As atcUCI.HspfUci)
        Initialize()
        ChDriveDir(pTestPath)
        Logger.Dbg("CurrentFolder " & My.Computer.FileSystem.CurrentDirectory)
        Logger.Status("HSPEXP+ is running.")
        Try
            Using lProgress As New ProgressLevel
                SDateJ = StartUp.DateTimePicker1.Value.ToOADate()
                EDateJ = StartUp.DateTimePicker2.Value.ToOADate() + 1

                Dim lEchoFileisinFilesBlock As Boolean = False
                Dim lHspfEchoFileName As String = pTestPath & "hspfecho.out" 'Get the default name of echo file
                Dim echoFileInfo As System.IO.FileInfo
                For i As Integer = 0 To aHspfUci.FilesBlock.Count
                    If aHspfUci.FilesBlock.Value(i).Typ = "MESSU" Then
                        lHspfEchoFileName = AbsolutePath(aHspfUci.FilesBlock.Value(i).Name.Trim, CurDir()) 'Update echo file name if it is referenced in the Files block
                        Exit For
                    End If
                Next

                If MultiSimulation Then
                    SubMultiSim(pHSPFExe, pBaseName, pTestPath, SDateJ, EDateJ, lHspfEchoFileName)
                    Logger.Msg("Multi Simulation Manager Process Complete", vbOKOnly)
                    OpenFile(pTestPath)
                    End

                End If

                pListModelParameters = False
                If pListModelParameters Then
                    ListReachParametersForAllUCIFiles(pTestPath)
                End If

#Region "HSPF Run"
                If pRunUci = True Then
                    Logger.Status(Now & " Running HSPF Simulation of " & pBaseName & ".uci", True)
                    Dim lExitCode As Integer
                    ChDriveDir(PathNameOnly(pHSPFExe))
                    lExitCode = LaunchProgram(pHSPFExe, pTestPath, "-1 -1 " & pBaseName & ".uci") 'Run HSPF program
                    ChDriveDir(pTestPath)

                    If lExitCode = -1 Then
                        Throw New ApplicationException("WinHSPFLt could not run, Analysis cannot continue")
                        Exit Sub
                    End If
                End If
                Logger.Status(Now & " HSPF Simulation of " & pBaseName & ".uci" & " finished.", True)
#End Region
                'build collection of operation types to report
                Dim lOperationTypes As New atcCollection
                lOperationTypes.Add("P:", "PERLND")
                lOperationTypes.Add("I:", "IMPLND")
                lOperationTypes.Add("R:", "RCHRES")

                Dim lStr As String = ""
                Dim lRunMade As String = ""

                If IO.File.Exists(lHspfEchoFileName) Then
                    echoFileInfo = New System.IO.FileInfo(lHspfEchoFileName)
                    lRunMade = echoFileInfo.LastWriteTime.ToString
                Else
                    Logger.Msg("The ECHO file is not available for this model. Please check if model ran successfully last time", vbCritical)
                    End
                    Return
                End If

#Region "Read Echo File to decide if Model Ran"
                Dim HSPFRan As Boolean = False
                Using echoFileReader As StreamReader = File.OpenText(lHspfEchoFileName)
                    While Not echoFileReader.EndOfStream
                        Dim nextLine As String = echoFileReader.ReadLine()
                        If Not nextLine.ToUpper.Contains("END OF JOB") Then
                            HSPFRan = False
                        Else
                            HSPFRan = True
                        End If
                    End While
                End Using

                If HSPFRan = False Then
                    Logger.Dbg("ECHO file says that run was terminated last time. HSPEXP+ will exit!")
                    Dim ans As Integer
                    ans = MsgBox("ECHO File contains a message that the run was terminated last time. HSPEXP+ will quit. Please make sure that UCI" &
                                 " file runs properly!")
                    OpenFile(lHspfEchoFileName)
                    End
                End If
#End Region
                loutfoldername = pTestPath

                Dim lDateString As String = Format(Year(lRunMade), "00") & Format(Month(lRunMade), "00") &
                                    Format(Microsoft.VisualBasic.DateAndTime.Day(lRunMade), "00") & Format(Hour(lRunMade), "00") & Format(Minute(lRunMade), "00")
                loutfoldername = pTestPath & "Reports_" & lDateString & "\"
                Directory.CreateDirectory(loutfoldername)
                File.Copy(pTestPath & pBaseName & ".uci", loutfoldername & pBaseName & ".uci", overwrite:=True)
                'A folder name is given that has the basename and the time when the run was made.

#Region "Start QA/QC Report"

                Dim QAQCReportFile As New Text.StringBuilder

                'QAQCReportFile = My.Computer.FileSystem.OpenTextFileWriter(loutfoldername & "\ModelQAQCReport.htm", False)
                If pModelQAQC Then
                    QAQCReportFile.AppendLine("<html>")
                    QAQCReportFile.AppendLine(GeneralModelInfo(aHspfUci))
                    QAQCReportFile.AppendLine(HSPFParmValues(aHspfUci, lRunMade))

                    'Area Report should become part of QAQC report now
                    Dim alocations As New atcCollection
                    For Each lRCHRES As HspfOperation In aHspfUci.OpnBlks("RCHRES").Ids
                        Dim lDownstreamReachID As Integer = lRCHRES.DownOper("RCHRES")
                        If lDownstreamReachID = 0 Then
                            alocations.Add("R:" & lRCHRES.Id)
                        End If
                    Next

                    'Dim lReport As atcReport.ReportText = HspfSupport.AreaReport(aHspfUci, lRunMade, lOperationTypes, alocations, True, loutfoldername & "/AreaReports/")

                End If

#End Region

#Region "AutomatedGraph and Regan Graph Generation"

                If StartUp.chkAdditionalgraphs.Checked Then
                    Try
                        ChDriveDir(pTestPath)
                        MakeAutomatedGraphs(SDateJ, EDateJ, loutfoldername, pTestPath)
                    Catch exGraph As Exception
                        Logger.Msg("Exception while making graphs: " & exGraph.ToString)
                    End Try
                End If

                If StartUp.chkReganGraphs.Checked Then
                    ReganGraphs(aHspfUci, SDateJ, EDateJ, loutfoldername)
                End If

#End Region
#Region "Area Report Generation"
                'If pMakeAreaReports Then
                '    Dim alocations As New atcCollection
                '    For Each lRCHRES As HspfOperation In aHspfUci.OpnBlks("RCHRES").Ids

                '        alocations.Add("R:" & lRCHRES.Id)

                '    Next
                '    Logger.Status(Now & " Producing Area Reports.", True)
                '    Logger.Dbg(Now & " Producing land use and area reports")
                '    'Now the area reports are generated for all the reaches in the UCI file.
                '    Dim lReport As atcReport.ReportText = HspfSupport.AreaReport(aHspfUci, lRunMade, lOperationTypes, alocations, True, loutfoldername & "/AreaReports/")
                '    lReport.MetaData.Insert(lReport.MetaData.ToString.IndexOf("Assembly"), lReport.AssemblyMetadata(System.Reflection.Assembly.GetExecutingAssembly) & vbCrLf)
                '    SaveFileString(loutfoldername & "/AreaReports/AreaReport.txt", lReport.ToString)
                'End If
#End Region

#Region "Hydrology Calibration"
                If StartUp.chkExpertStats.Checked = True Then
                    Dim lExpertSystemFileNames As New NameValueCollection
                    AddFilesInDir(lExpertSystemFileNames, IO.Directory.GetCurrentDirectory, False, "*.exs")
                    If lExpertSystemFileNames.Count < 1 Then 'Becky added this if-then to warn the user if no EXS files exist
                        MsgBox("No basins specifications file (*.exs) file found in directory " & IO.Directory.GetCurrentDirectory &
                               "!  Statistics, summaries, and graphs cannot be computed. EXS file can be generated using WinHSPF3.1 or later.",
                               vbOKOnly, "No Specification File!")
                        Logger.Dbg(Now & " No basins specifications file found, no statistics computed")
                    End If

                    Dim lExpertSystem As HspfSupport.atcExpertSystem

                    For Each lExpertSystemFileName As String In lExpertSystemFileNames
                        Logger.Status(Now & " Calculating Expert Statistics for the file " & lExpertSystemFileName, True)
                        Try
                            Logger.Dbg(Now & " Calculating run statistics.")
                            lExpertSystem = New HspfSupport.atcExpertSystem(aHspfUci, lExpertSystemFileName, SDateJ, EDateJ)
                            Dim lHydrologyWDMFileName As String = lExpertSystem.ExpertWDMFileName
                            lStr = lExpertSystem.Report(lRunMade)

                            SaveFileString(loutfoldername & "ExpertSysStats-" & IO.Path.GetFileNameWithoutExtension(lExpertSystemFileName) & ".txt", lStr)

                            'Becky added these to output advice
                            Logger.Dbg(Now & " Creating advice to save in " & pBaseName & ".*.txt")
                            For lSiteIndex As Integer = 1 To lExpertSystem.Sites.Count
                                Dim lAdviceStr As String = "Advice for Calibration Run " & pBaseName & vbCrLf & Now & vbCrLf & vbCrLf
                                lExpertSystem.CalcAdvice(lAdviceStr, lSiteIndex)
                                Dim lSiteNam As String = lExpertSystem.Sites(lSiteIndex - 1).Name
                                SaveFileString(loutfoldername & pBaseName & "." & lSiteNam & "advice.txt", lAdviceStr)
                            Next

                            Dim lCons As String = "Flow"
                            For Each lSite As HexSite In lExpertSystem.Sites
                                Dim lSiteName As String = lSite.Name
                                Dim lArea As Double = lSite.Area
                                Dim lSimTSerInchesOriginal As atcTimeseries = SubsetByDate(lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(0)), lExpertSystem.SDateJ, lExpertSystem.EDateJ, Nothing)
                                Dim lSimTSerInches As atcTimeseries = Aggregate(lSimTSerInchesOriginal, atcTimeUnit.TUDay, 1, atcTran.TranSumDiv)
                                lSimTSerInches.Attributes.SetValue("Units", "Flow (inches)")
                                Dim lSimTSer As atcTimeseries = InchesToCfs(lSimTSerInches, lArea)
                                lSimTSer.Attributes.SetValue("Units", "Flow (cfs)")
                                lSimTSer.Attributes.SetValue("YAxis", "Left")
                                lSimTSer.Attributes.SetValue("StepType", pCurveStepType)
                                Dim lObsTSerOriginal As atcTimeseries = SubsetByDate(lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(1)), lExpertSystem.SDateJ, lExpertSystem.EDateJ, Nothing)
                                Dim lObsTSer As atcTimeseries = Aggregate(lObsTSerOriginal, atcTimeUnit.TUDay, 1, atcTran.TranAverSame)
                                lObsTSer.Attributes.SetValue("Units", "Flow (cfs)")
                                lObsTSer.Attributes.SetValue("YAxis", "Left")
                                lObsTSer.Attributes.SetValue("StepType", pCurveStepType)
                                Dim lObsTSerInches As atcTimeseries = CfsToInches(lObsTSer, lArea)
                                lObsTSerInches.Attributes.SetValue("Units", "Flow (inches)")
                                'Anurag changed the code so that the original time series for observed flow in cfs 
                                'and simulated flow volume in 'inches could be at a smaller time step. Later on 
                                'Anurag did the same for simulated precipitation.  This way shorter time period time series
                                'could be used for storm graphs if the user is interested.

                                Dim lPrecDsn As Integer = lSite.DSN(5)
                                Dim lPrecTserOriginal As atcTimeseries = SubsetByDate(lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lPrecDsn), lExpertSystem.SDateJ, lExpertSystem.EDateJ, Nothing)
                                lPrecTserOriginal.Attributes.SetValue("Units", "inches")
                                Dim lPrecTser As atcTimeseries = Aggregate(lPrecTserOriginal, atcTimeUnit.TUDay, 1, atcTran.TranSumDiv)

                                Logger.Dbg(Now & " Calculating monthly summary for " & lSiteName)
                                'pProgressBar.pbProgress.Increment(5)
                                Dim lTSerBroken As atcTimeseries = lSimTSer.Clone

                                Dim PercentMissingObservedData As Double = 0.0

                                If lObsTSerInches.Attributes.GetDefinedValue("Count Missing").Value > 0 Then
                                    PercentMissingObservedData = lObsTSerInches.Attributes.GetDefinedValue("Count Missing").Value * 100 / lObsTSerInches.Attributes.GetDefinedValue("Count").Value
                                    For i As Integer = 1 To lObsTSerInches.numValues
                                        If Double.IsNaN(lObsTSerInches.Value(i)) Then
                                            lSimTSerInches.Value(i) = Double.NaN
                                            lTSerBroken.Value(i) = Double.NaN

                                        End If
                                    Next

                                End If

                                lStr = HspfSupport.MonthlyAverageCompareStats.Report(aHspfUci,
                                                                                     lCons, lSiteName,
                                                                                     "inches",
                                                                                     lSimTSerInches, lObsTSerInches,
                                                                                     lRunMade,
                                                                                     lExpertSystem.SDateJ,
                                                                                     lExpertSystem.EDateJ,
                                                                                     PercentMissingObservedData)
                                Dim lOutFileName As String = loutfoldername & "MonthlyAverage" & lCons & "Stats-" & lSiteName & ".txt"
                                SaveFileString(lOutFileName, lStr)

                                Logger.Dbg(Now & " Calculating annual summary for " & lSiteName)
                                lStr = HspfSupport.AnnualCompareStats.Report(aHspfUci,
                                                                             lCons, lSiteName,
                                                                             "inches",
                                                                             lPrecTser, lSimTSerInches, lObsTSerInches,
                                                                             lRunMade,
                                                                             lExpertSystem.SDateJ,
                                                                             lExpertSystem.EDateJ,
                                                                             PercentMissingObservedData)
                                lOutFileName = loutfoldername & "Annual" & lCons & "Stats-" & lSiteName & ".txt"
                                SaveFileString(lOutFileName, lStr)

                                Logger.Dbg(Now & " Calculating daily summary for " & lSiteName)
                                'pProgressBar.pbProgress.Increment(6)
                                lStr = HspfSupport.DailyMonthlyCompareStats.Report(aHspfUci,
                                                                                   lCons, lSiteName,
                                                                                   lTSerBroken, lObsTSer,
                                                                                   lRunMade,
                                                                                   lExpertSystem.SDateJ,
                                                                                   lExpertSystem.EDateJ,
                                                                                   PercentMissingObservedData)
                                lOutFileName = loutfoldername & "DailyMonthly" & lCons & "Stats-" & lSiteName & ".txt"
                                SaveFileString(lOutFileName, lStr)


                                Logger.Status(Now & " Preparing Graphs", True)
                                Dim lTimeSeries As New atcTimeseriesGroup
                                Logger.Dbg(Now & " Creating nonstorm graphs")
                                lTimeSeries.Add("Observed", lObsTSer)
                                lTimeSeries.Add("Simulated", lSimTSer)
                                If PercentMissingObservedData > 0 Then
                                    lTimeSeries.Add("SimulatedBroken", lTSerBroken)
                                End If
                                lTimeSeries.Add("Precipitation", lPrecTser)
                                lTimeSeries.Add("LZS", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(9)))
                                lTimeSeries.Add("UZS", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(8)))
                                lTimeSeries.Add("PotET", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(6)))
                                lTimeSeries.Add("ActET", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(7)))
                                lTimeSeries.Add("Baseflow", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(4)))
                                lTimeSeries.Add("Interflow", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(3)))
                                lTimeSeries.Add("Surface", lExpertSystem.ExpertWDMDataSource.DataSets.ItemByKey(lSite.DSN(2)))
                                GraphAll(lExpertSystem.SDateJ, lExpertSystem.EDateJ,
                                             lCons, lSiteName,
                                             lTimeSeries,
                                             pGraphSaveFormat,
                                             pGraphSaveWidth,
                                             pGraphSaveHeight,
                                             pGraphAnnual, loutfoldername,
                                            True, True,
                                             True, PercentMissingObservedData)
                                lTimeSeries.Clear()


                                Logger.Dbg(Now & " Creating storm graphs")
                                lSimTSer = InchesToCfs(lSimTSerInchesOriginal, lArea)

                                lTimeSeries.Add("Observed", lObsTSerOriginal)
                                lTimeSeries.Add("Simulated", lSimTSer)
                                lTimeSeries.Add("Prec", lPrecTserOriginal)

                                lTimeSeries(0).Attributes.SetValue("Units", "cfs")
                                lTimeSeries(0).Attributes.SetValue("StepType", pCurveStepType)
                                lTimeSeries(1).Attributes.SetValue("Units", "cfs")
                                lTimeSeries(1).Attributes.SetValue("StepType", pCurveStepType)
                                lTimeSeries(2).Attributes.SetValue("YAxis", "Aux")
                                IO.Directory.CreateDirectory(loutfoldername & "\Storms\")
                                GraphStorms(lTimeSeries, 2, loutfoldername & "Storms\" & lSiteName, pGraphSaveFormat, pGraphSaveWidth, pGraphSaveHeight, lExpertSystem, True)
                                lTimeSeries.Dispose()



                            Next

                            lExpertSystem = Nothing

                        Catch lEx As ApplicationException
                            If lEx.Message.Contains("rogram will quit") Then
                                Logger.Msg(lEx.Message)
                                End
                            End If
                            Logger.Dbg(lEx.Message)
                        End Try
                    Next lExpertSystemFileName
                End If
#End Region

#Region "Water Quality"
                If pConstituents.Count > 0 Then
                    'Dim lHspfBinDataSource As New atcDataSource
                    Dim lOpenHspfBinDataSource As New atcDataSource

                    Logger.Dbg(Now & " Opening the binary output files.")

                    'Dim lLocations As New atcCollection
                    For i As Integer = 0 To aHspfUci.FilesBlock.Count
                        If aHspfUci.FilesBlock.Value(i).Typ = "BINO" Then
                            Dim lHspfBinFileName As String = AbsolutePath(aHspfUci.FilesBlock.Value(i).Name.Trim, CurDir())
                            lOpenHspfBinDataSource = atcDataManager.DataSourceBySpecification(lHspfBinFileName)
                            If lOpenHspfBinDataSource Is Nothing Then
                                If atcDataManager.OpenDataSource(lHspfBinFileName) Then
                                    lOpenHspfBinDataSource = atcDataManager.DataSourceBySpecification(lHspfBinFileName)
                                End If
                            End If
                        End If
                    Next i

                    For Each lConstituent As String In pConstituents
                        Dim lConstProperties As New List(Of ConstituentProperties)
                        Logger.Dbg("------ Begin summary for " & lConstituent & " -----------------")
                        Logger.Status("Begin summary for " & lConstituent)
                        Dim lConstituentName As String = ""
                        Dim lActiveSections As New List(Of String)
                        Dim CheckQUALID As Boolean = False
                        Dim lGQALID As Integer = 0
                        Select Case lConstituent
                            Case "Water"
                                lConstituentName = "WAT"
                                lActiveSections.Add("PWATER")
                                lActiveSections.Add("IWATER")
                                lActiveSections.Add("HYDR")
                            Case "Sediment"
                                lConstituentName = "SED"
                                lActiveSections.Add("SEDMNT")
                                lActiveSections.Add("SOLIDS")
                                lActiveSections.Add("SEDTRN")
                            Case "DO"
                                lConstituentName = "DO"
                                lActiveSections.Add("PWTGAS")
                                lActiveSections.Add("IWTGAS")
                                lActiveSections.Add("OXRX")
                            Case "Heat"
                                lConstituentName = "Heat"
                                lActiveSections.Add("PWTGAS")
                                lActiveSections.Add("IWTGAS")
                                lActiveSections.Add("HTRCH")
                            Case "TN"
                                lConstituentName = "TN"
                                lConstProperties = Utility.LocateConstituentNames(aHspfUci, lConstituent)
                                If lConstProperties Is Nothing Then
                                    End
                                End If
                                lActiveSections.Add("NITR")
                                lActiveSections.Add("PQUAL")
                                lActiveSections.Add("IQUAL")
                                lActiveSections.Add("NUTRX")
                                lActiveSections.Add("PLANK")

                            Case "TP"
                                lConstituentName = "TP"
                                lConstProperties = Utility.LocateConstituentNames(aHspfUci, lConstituent)
                                If lConstProperties Is Nothing Then
                                    End
                                End If
                                lActiveSections.Add("NITR")
                                lActiveSections.Add("PHOS")
                                lActiveSections.Add("PQUAL")
                                lActiveSections.Add("IQUAL")
                                lActiveSections.Add("NUTRX")
                                lActiveSections.Add("PLANK")

                            Case "BOD-Labile"
                                lConstituentName = "BOD-Labile"
                                lConstProperties = Utility.LocateConstituentNames(aHspfUci, lConstituent)
                                If lConstProperties Is Nothing Then
                                    End
                                End If
                                lActiveSections.Add("PQUAL")
                                lActiveSections.Add("IQUAL")
                                lActiveSections.Add("OXRX")
                                lActiveSections.Add("NUTRX")
                                lActiveSections.Add("PLANK")
                            Case Else
                                lGQALID = Right(lConstituent, 1)
                                lConstituentName = SafeSubstring(lConstituent, 0, lConstituent.Length - 2)
                                lConstProperties = Utility.LocateConstituentNames(aHspfUci, lConstituentName, lGQALID)
                                If lConstProperties Is Nothing Then
                                    End
                                End If
                                lActiveSections.Add("PQUAL")
                                lActiveSections.Add("IQUAL")
                                lActiveSections.Add("GQUAL")

                        End Select

                        Dim lScenarioResults As New atcDataSource

                        If lScenarioResults.DataSets.Count = 0 Then
                            For Each activeSection As String In lActiveSections
                                lScenarioResults.DataSets.Add(atcDataManager.DataSets.FindData("Section", activeSection))
                            Next
                        End If


                        If lScenarioResults.DataSets.Count > 0 Then

                            Dim lReportCons As New atcReport.ReportText
                            lReportCons = Nothing
                            Dim lOutFileName As String = ""

                            Dim LandLoadingReportForConstituents As DataTable = LandLoadingReports(loutfoldername, lScenarioResults, aHspfUci, pBaseName, lRunMade, lConstituentName, lConstProperties, SDateJ, EDateJ, lGQALID)
                            If pModelQAQC Then
                                QAQCReportFile.AppendLine("<h2>" & lConstituent & " Balance Analysis</h2>")
                                QAQCReportFile.AppendLine(LoadingRateComparison(lConstituentName, LandLoadingReportForConstituents))
                            End If

                            ReachBudgetReports(loutfoldername, lScenarioResults, aHspfUci, pBaseName, lRunMade, lConstituentName, lConstProperties, SDateJ, EDateJ, lGQALID)
                            Logger.Status(Now & " Generating Reports for " & lConstituent)
                            Logger.Dbg(Now & " Generating Reports for " & lConstituent)
                            lReportCons = Nothing

                            If lConstituent = "TN" OrElse lConstituent = "TP" OrElse
                                lConstituent = "Sediment" OrElse lConstituent = "Water" Then

                                With HspfSupport.ConstituentBudget.Report(aHspfUci, lConstituent, lOperationTypes, pBaseName,
                                                                      lScenarioResults, pOutputLocations, lRunMade, SDateJ, EDateJ, lConstProperties)
                                    lReportCons = .Item1
                                    lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_Per_RCH_Ann_Avg_Budget.txt"
                                    If lReportCons IsNot Nothing Then SaveFileString(lOutFileName, lReportCons.ToString)

                                    'lReportCons = Nothing
                                    'lReportCons = .Item2
                                    'lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_Per_RCH_Per_LU_Ann_Avg_NPS_Lds.txt"
                                    'SaveFileString(lOutFileName, lReportCons.ToString)
                                    lReportCons = Nothing
                                    lReportCons = .Item3

                                    lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_LoadAllocation.txt"
                                    If lReportCons IsNot Nothing Then SaveFileString(lOutFileName, lReportCons.ToString)
                                    lReportCons = Nothing

                                    lReportCons = .Item4
                                    If pOutputLocations.Count > 0 Then
                                        lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_LoadAllocation_Locations.txt"
                                        SaveFileString(lOutFileName, lReportCons.ToString)
                                    End If
                                    lReportCons = Nothing
                                    'lReportCons = .Item5
                                    'lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_LoadingRates.txt"
                                    'SaveFileString(lOutFileName, lReportCons.ToString)
                                    'lReportCons = Nothing

                                    If .Item6 IsNot Nothing AndAlso .Item6.Keys.Count > 0 Then
                                        For Each location As String In .Item6.Keys
                                            CreateGraph_BarGraph(.Item6.ItemByKey(location), loutfoldername & lConstituentName & "_" & pBaseName & "_" & location & "_LoadingAllocation.png")
                                        Next location
                                    End If


                                End With
                                'Logger.Dbg(Now & " Calculating Annual Constituent Balance for " & lConstituent)



                                lReportCons = HspfSupport.ConstituentBalance.Report(aHspfUci, lConstituent, lOperationTypes, pBaseName,
                                lScenarioResults, lRunMade, SDateJ, EDateJ, lConstProperties)
                                lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_Per_OPN_Per_Year.txt"

                                SaveFileString(lOutFileName, lReportCons.ToString)

                                'Logger.Dbg("Summary at " & lLocations.Count & " locations")
                                'constituent balance


                                lReportCons = HspfSupport.WatershedConstituentBalance.Report(aHspfUci, lConstituent, lOperationTypes, pBaseName,
                                lScenarioResults, lRunMade, SDateJ, EDateJ, lConstProperties)
                                lOutFileName = loutfoldername & lConstituentName & "_" & pBaseName & "_Grp_By_OPN_LU_Ann_Avg.txt"

                                SaveFileString(lOutFileName, lReportCons.ToString)

                                If pOutputLocations.Count > 0 Then 'subwatershed constituent balance 
                                    HspfSupport.WatershedConstituentBalance.ReportsToFiles _
                                       (aHspfUci, lConstituent, lOperationTypes, pBaseName,
                                        lScenarioResults, pOutputLocations, lRunMade, SDateJ, EDateJ,
                                        lConstProperties, loutfoldername, True)
                                    'now pivoted version
                                    'HspfSupport.WatershedConstituentBalance.ReportsToFiles _
                                    '   (lHspfUci, lConstituent, lOperationTypes, pBaseName, _
                                    '    lHspfBinDataSource, pOutputLocations, lRunMade, _
                                    '    lOutFolderName, True, True)
                                End If
                            End If


                        Else
                            Logger.Dbg("The HBN file didn't have any data for the constituent " & lConstituent & "  therefore the balance reports for " &
                            lConstituent & " will not be generated. Make sure that HSPF run completed last time.")
                            Dim ans As Integer
                            ans = MsgBox("HBN files do not have any data.  Constituent Balance reports will not be generated. " &
                                         "Did uci file run properly last time?")
                        End If
                        For Each lTimeSeries As atcTimeseries In lScenarioResults.DataSets
                            lTimeSeries.ValuesNeedToBeRead = True
                        Next
                    Next lConstituent

                End If
#End Region
                If pModelQAQC Then
                    QAQCReportFile.AppendLine("</html>")
                    'Try
                    File.WriteAllText(pTestPath & "\ModelQAQC.htm", QAQCReportFile.ToString())
                    'Catch
                    'If file is open give warning and then close its

                    'End Try
                End If
                Logger.Status(Now & " Output Written to " & loutfoldername)
                Logger.Dbg("Reports Written in " & loutfoldername)

                'pProgressBar.pbProgress.Increment(39)

                Logger.Dbg(Now & " HSPEXP+ Complete")
                Logger.Msg("HSPEXP+ is complete")

                OpenFile(loutfoldername)
            End Using
        Catch ex As Exception
            'Skip to the end if Cancel was chosen in felu            

            Logger.Msg(ex.ToString, MsgBoxStyle.Critical, "HSPEXP+ did not complete successfully.")


        End Try

        Logger.Status("")
        atcDataManager.Clear()
        StartUp.Show()
        'Call Application.Exit()

    End Sub


    ''' <summary>
    ''' This function looks at each parameter limit in the XML list of parameters and compares them to the values in the UCI file. If the values are not within the 
    ''' limits, it is mentioned in the report.
    ''' </summary>
    ''' <param name="aUCI"></param>
    ''' <param name="aRunMade"></param>
    ''' <returns></returns>
    Private Function HSPFParmValues(ByVal aUCI As HspfUci, ByVal aRunMade As String) As String ' , ByVal ParameterValues As DataTable)
        Dim HSPFParmTable As XmlDocument = New XmlDocument()
        'Dim xmlFileIsThere As Boolean = FileExists(FindFile("", "HSPFParmValues.xml"))
        Dim TableName As String = ""
        Dim ParameterName As String = ""
        Dim MaxValue As Double = 0
        Dim MinValue As Double = 0
        Dim OperationType As String = ""
        Dim ParameterInfo As String = ""

        HSPFParmTable.LoadXml(My.Resources.HSPFParmValues) '    (Resources.IResourceReader("HSPFParmValues.xml")) '(FindFile("", "HSPFParmValues.xml"))
        ParameterInfo &= "<h2>Model Parameter Value Analysis</h2>"
        Dim ParameterLimitIssue As String = "<ul>"

        Dim nodes As XmlNodeList = HSPFParmTable.DocumentElement.SelectNodes("Parm")

        For Each node As XmlNode In nodes
            OperationType = node.SelectSingleNode("OPNTYPE").InnerText
            TableName = node.SelectSingleNode("TABLE").InnerText
            ParameterName = node.SelectSingleNode("ParameterName").InnerText
            MaxValue = node.SelectSingleNode("Max").InnerText
            MinValue = node.SelectSingleNode("Min").InnerText
            Dim lMessageCountPerParameter As Integer = 0
            For Each loperation As HspfOperation In aUCI.OpnBlks(OperationType).Ids
                For Each lTable As HspfTable In loperation.Tables
                    If lTable.Name = TableName Then

                        For Each parm As HspfParm In lTable.Parms
                            Dim lprintLine As Boolean = False

                            If parm.Name = ParameterName Then
                                If parm.Value > MaxValue OrElse parm.Value < MinValue Then
                                    ParameterLimitIssue = "<li>The value of parameter " & parm.Name & " in the Table " & TableName & " for " &
                                OperationType & loperation.Id & " (" & parm.Value & ") is outside the typical limit of " & MinValue & " - " & MaxValue & ".</li>" & vbCrLf
                                    lprintLine = True
                                End If
                            End If
                            If lprintLine = True Then
                                lMessageCountPerParameter += 1
                                If lMessageCountPerParameter > 5 Then
                                    ParameterInfo &= "<p>There have been five instances of parameter " & parm.Name &
                                                         " in the table " & TableName & " outside of limits. It will not be listed anymore.</p>" & vbCrLf
                                Else
                                    ParameterInfo &= ParameterLimitIssue
                                End If
                            Else

                            End If
                        Next parm
                    End If
                Next lTable
            Next loperation
        Next
        ParameterInfo &= "</ul>"
        Return ParameterInfo
    End Function
    ''' <summary>
    ''' This function outputs heading of the QA/QC report and some general information.
    ''' </summary>
    ''' <param name="aUCI"></param>
    ''' <returns></returns>
    Private Function GeneralModelInfo(ByVal aUCI As HspfUci) As String
        Dim GeneralModelInfoText As String = ""
        GeneralModelInfoText = "<h1>HSPF Model QA QC Report.</h1>" & vbCrLf
        GeneralModelInfoText &= "<p>The model file name is " & aUCI.Name.ToString & "."

        GeneralModelInfoText &= "The model simulation period spans from " & aUCI.GlobalBlock.SDate(0) & "/" & aUCI.GlobalBlock.SDate(1) & "/" & aUCI.GlobalBlock.SDate(2) &
             " to " & aUCI.GlobalBlock.EDate(0) & "/" & aUCI.GlobalBlock.EDate(1) & "/" & aUCI.GlobalBlock.EDate(2) & ".</p>" & vbCrLf



        Return GeneralModelInfoText
    End Function
    ''' <summary>
    ''' This function outputs area report of terminal reaches and model calibration reaches.
    ''' </summary>
    ''' <param name="aUCI"></param>
    ''' <returns></returns>
    Private Function ModelAreaReport(ByVal aUCI As HspfUci) As String
        Dim ModelAreaReportTable As String = ""
        Dim alocations As New atcCollection
        For Each lRCHRES As HspfOperation In aUCI.OpnBlks("RCHRES").Ids
            Dim lDownstreamReachID As Integer = lRCHRES.DownOper("RCHRES")
            If lDownstreamReachID = 0 Then
                alocations.Add("R:" & lRCHRES.Id)
            End If
        Next
        'Need to add calibration reaches


        Return ModelAreaReportTable
    End Function
    Private Function LoadingRateComparison(ByVal aConstituentName As String, ByVal aLandLoadingConstReport As DataTable) As String
        Dim LoadingRateComments As New Text.StringBuilder
        Dim newColumn As DataColumn
        newColumn = New DataColumn()
        newColumn.DataType = Type.GetType("System.String")
        newColumn.ColumnName = "genLandUse"
        newColumn.Caption = "Generalized Land Use"
        aLandLoadingConstReport.Columns.Add(newColumn)
        Dim UCILandUse As String = ""
        Dim ListofLandUsesInUCI As New List(Of String)
        'Dim SelectExpressionSumAnnual As String = "OpTypeNumber Like 'P:%' And Year = 'SumAnnual'"
        'Dim foundRows() As DataRow = aLandLoadingConstReport.Select(SelectExpressionSumAnnual)
        For Each row As DataRow In aLandLoadingConstReport.Rows
            UCILandUse = row("OpDesc")
            row("genLandUse") = FindGeneralLandUse(UCILandUse)
            If Not ListofLandUsesInUCI.Contains(row("genLandUse")) And Not row("genLandUse") = "Unknown" Then
                ListofLandUsesInUCI.Add(row("genLandUse"))
            End If
        Next
        Dim lSelectExpression As String = ""

        LoadingRateComments.AppendLine("<p>Following non-typical behaviours were noticed in the model.</p>")
        LoadingRateComments.AppendLine("<ul>")
        Dim IsWetlandALanduse As Boolean = False
        If ListofLandUsesInUCI.Contains("Wetland") Then IsWetlandALanduse = True
        For Each landuse As String In ListofLandUsesInUCI
            Select Case aConstituentName
                Case "WAT"
                    LoadingRateComments.AppendLine(CheckIrrigation(landuse, aLandLoadingConstReport))
                    LoadingRateComments.AppendLine(CheckETIssues(landuse, aLandLoadingConstReport, ListofLandUsesInUCI, IsWetlandALanduse))
                    LoadingRateComments.AppendLine(CheckRunoff(landuse, aLandLoadingConstReport, ListofLandUsesInUCI))
                Case "TN"
                Case "SED"


            End Select

        Next
        LoadingRateComments.AppendLine("</ul>")
        Return LoadingRateComments.ToString
    End Function
    ''' <summary>
    ''' This Function takes the UCI Landuse and finds a corresponding general land use based on a CSV file.
    ''' </summary>
    ''' <param name="aUCILandUse"></param>
    ''' <returns></returns>
    Private Function FindGeneralLandUse(ByVal aUCILandUse As String) As String
        Dim GeneralLandUse As String = "Unknown"
        If aUCILandUse.ToLower.Contains("forest") Then
            GeneralLandUse = "Forest"
            Return GeneralLandUse
        ElseIf aUCILandUse.ToLower.Contains("crop") OrElse aUCILandUse.ToLower.Contains("agric") Then
            GeneralLandUse = "Ag/Other"
            Return GeneralLandUse
        ElseIf aUCILandUse.ToLower.Contains("urban") Then
            GeneralLandUse = "Urban"
            Return GeneralLandUse
        End If
        Using strReader As New IO.StringReader(My.Resources.LandUseNames_Mappings)
            Using MyReader As New FileIO.TextFieldParser(strReader)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim CurrentRow As String()
                While Not MyReader.EndOfData
                    CurrentRow = MyReader.ReadFields
                    If CurrentRow(0) = aUCILandUse Then
                        Return CurrentRow(1)
                    End If
                End While
            End Using
        End Using
        Return GeneralLandUse
    End Function
    Private Function CheckIrrigation(ByVal aLanduse As String, ByVal aLandLoadingConstReport As DataTable) As String
        If aLanduse = "Ag/Other" Then Return ""
        Dim IrrigationStatement As String = ""
        Dim IrrigationApp As Double = 0.0
        Dim lSelectExpression As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & aLanduse & "' And Year = 'SumAnnual'"
        Try
            IrrigationApp = aLandLoadingConstReport.Compute("AVG(IRRAPP6)", lSelectExpression)
        Catch

        End Try
        If IrrigationApp > 0 Then
            Return "<li>" & aLanduse & " land use has irrigation application.</li>" & vbCrLf
        Else
            Return ""
        End If

    End Function

    Private Function CheckRunoff(ByVal aLanduse As String, ByVal aLandLoadingConstReport As DataTable, ByVal aListofLandUsesinUCI As List(Of String)) As String
        Dim lSelectExpression As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & aLanduse & "' And Year = 'SumAnnual'"
        Dim TotalOutFlow As Double = aLandLoadingConstReport.Compute("AVG(TotalOutflow)", lSelectExpression)
        Dim TotalSurfaceRunoff As Double = aLandLoadingConstReport.Compute("AVG(SURO)", lSelectExpression)
        Dim TotalBaseFlow As Double = aLandLoadingConstReport.Compute("AVG(IFWO)", lSelectExpression) + aLandLoadingConstReport.Compute("AVG(AGWO)", lSelectExpression)
        Dim CheckRunoffStatement As New Text.StringBuilder
        If TotalSurfaceRunoff > TotalBaseFlow Then
            CheckRunoffStatement.AppendLine("<li>Surface runoff is greater than baseflow for " & aLanduse & ".</li>")
        End If

        Select Case aLanduse
            Case "Forest"
                For Each landuse2 As String In aListofLandUsesinUCI
                    If landuse2 = "Forest" Then Continue For
                    Dim SelectExpression2 As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & landuse2 & "' And Year = 'SumAnnual'"
                    Dim TotalOutflow2 As Double = aLandLoadingConstReport.Compute("AVG(TotalOutflow)", SelectExpression2)
                    Dim TotalSurfaceRunoff2 As Double = aLandLoadingConstReport.Compute("AVG(SURO)", SelectExpression2)
                    Dim TotalBaseFlow2 As Double = aLandLoadingConstReport.Compute("AVG(IFWO)", SelectExpression2) + aLandLoadingConstReport.Compute("AVG(AGWO)", SelectExpression2)
                    Select Case True
                        Case landuse2 = "Wetland" AndAlso TotalOutflow2 > TotalOutFlow
                            CheckRunoffStatement.AppendLine("<li>Wetland has greater total outflow than Forest.</li>")
                        Case landuse2 <> "Wetland" AndAlso TotalOutflow2 < TotalOutFlow
                            CheckRunoffStatement.AppendLine("<li>Forest has greater total outflow than " & landuse2 & ".</li>")
                        Case landuse2 = "Wetland" AndAlso TotalSurfaceRunoff2 > TotalSurfaceRunoff
                            CheckRunoffStatement.AppendLine("<li>Wetland has greater surface runoff than Forest.</li>")
                        Case landuse2 <> "Wetland" AndAlso TotalSurfaceRunoff2 < TotalSurfaceRunoff
                            CheckRunoffStatement.AppendLine("<li>Forest has greater surface runoff than " & landuse2 & ".</li>")
                    End Select
                Next

            Case "Wetland"
                For Each landuse2 As String In aListofLandUsesinUCI
                    If landuse2 = "Forest" OrElse landuse2 = "Wetland" Then Continue For
                    Dim SelectExpression2 As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & landuse2 & "' And Year = 'SumAnnual'"
                    Dim TotalOutflow2 As Double = aLandLoadingConstReport.Compute("AVG(TotalOutflow)", SelectExpression2)
                    Dim TotalSurfaceRunoff2 As Double = aLandLoadingConstReport.Compute("AVG(SURO)", SelectExpression2)
                    Dim TotalBaseFlow2 As Double = aLandLoadingConstReport.Compute("AVG(IFWO)", SelectExpression2) + aLandLoadingConstReport.Compute("AVG(AGWO)", SelectExpression2)
                    If TotalOutFlow > TotalOutflow2 Then
                        CheckRunoffStatement.AppendLine("<li>Wetland has greater total outflow than " & landuse2 & ".</li>")
                    End If
                Next
        End Select
        CheckRunoffStatement.AppendLine("</ul>")
        Return CheckRunoffStatement.ToString
    End Function

    Private Function CheckETIssues(ByVal aLanduse As String, ByVal aLandLoadingConstReport As DataTable,
                                   ByVal aListofLandUsesinUCI As List(Of String),
                                   ByVal WetlandLUExists As Boolean) As String
        Dim CheckETIssuesStatement As New Text.StringBuilder
        Dim lSelectExpression As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & aLanduse & "' And Year = 'SumAnnual'"
        Dim TotalET As Double = aLandLoadingConstReport.Compute("AVG(TAET)", lSelectExpression)
        Dim PotET As Double = aLandLoadingConstReport.Compute("AVG(PET)", lSelectExpression)
        Dim InterceptionET As Double = aLandLoadingConstReport.Compute("AVG(CEPE)", lSelectExpression)
        Dim UpperZoneET As Double = aLandLoadingConstReport.Compute("AVG(UZET)", lSelectExpression)
        Dim LowerZoneET As Double = aLandLoadingConstReport.Compute("AVG(LZET)", lSelectExpression)
        Dim GroundWaterET As Double = aLandLoadingConstReport.Compute("AVG(AGWET)", lSelectExpression)
        Dim BaseflowET As Double = aLandLoadingConstReport.Compute("AVG(BASET)", lSelectExpression)

        If aLanduse <> "Wetland" AndAlso WetlandLUExists AndAlso GroundWaterET > 0 Then
            CheckETIssuesStatement.AppendLine("<li>Groundwater is being lost through evapotranspiration in " & aLanduse & " even though there is a separate Wetland land use.</li>")
        End If

        Select Case True

            Case aLanduse = "Forest"
                For Each landuse2 As String In aListofLandUsesinUCI
                    If landuse2 = "Forest" Then Continue For
                    Dim lSelectExpression2 As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & landuse2 & "' And Year = 'SumAnnual'"
                    'Comparing Total ET
                    If landuse2 = "Wetland" AndAlso TotalET > aLandLoadingConstReport.Compute("AVG(TAET)", lSelectExpression2) Then
                        CheckETIssuesStatement.AppendLine("<li>Forest has more ET than Wetland.</li>")
                    ElseIf landuse2 <> "Wetland" AndAlso TotalET < aLandLoadingConstReport.Compute("AVG(TAET)", lSelectExpression2) Then
                        CheckETIssuesStatement.AppendLine("<li>" & landuse2 & " has more ET than Forest.</li>")
                    End If
                    'Comparing Interception ET loss
                    If landuse2 <> "Wetland" AndAlso InterceptionET < aLandLoadingConstReport.Compute("AVG(CEPE)", lSelectExpression2) Then
                        CheckETIssuesStatement.AppendLine("<li>" & landuse2 & " has more interception loss than Forest.</li>")
                    End If
                Next landuse2


            Case aLanduse = "Wetland"
                For Each landuse2 As String In aListofLandUsesinUCI
                    If landuse2 = "Forest" OrElse landuse2 = "Wetland" Then Continue For
                    Dim lSelectExpression2 As String = "OpTypeNumber Like 'P:%' And genLandUse = '" & landuse2 & "' And Year = 'SumAnnual'"
                    If TotalET < aLandLoadingConstReport.Compute("AVG(TAET)", lSelectExpression2) Then
                        CheckETIssuesStatement.AppendLine("<li>" & landuse2 & " has more ET than Wetland.</li>")
                    End If
                    If InterceptionET < aLandLoadingConstReport.Compute("AVG(CEPE)", lSelectExpression2) Then
                        CheckETIssuesStatement.AppendLine("<li>" & landuse2 & " has more interception loss than Wetland.</li>")
                    End If



                Next landuse2
        End Select


        CheckETIssuesStatement.AppendLine("</ul>")

        Return CheckETIssuesStatement.ToString

    End Function
    Private Function CheckTotalSedimentErosion(ByVal aLanduse As String, ByVal aLandLoadingConstReport As DataTable) As String
        Dim CheckTotalSedErosion As String = ""






        Return CheckTotalSedErosion
    End Function
End Module




