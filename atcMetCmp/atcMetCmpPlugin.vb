Imports atcdata
Imports atcUtility
Imports MapWinUtility

Imports System.Reflection

Public Class atcMetCmpPlugin
    Inherits atcData.atcTimeseriesSource

    Private Shared pAvailableOperations As atcDataAttributes = Nothing
    Private Shared pName As String = "Timeseries::Meteorologic Generation"
    Friend Shared pIcon As System.Drawing.Icon = Nothing
 
    Public Overrides ReadOnly Property Name() As String
        Get
            Return "Timeseries::Meteorologic Generation"
        End Get
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Meteorologic Generation"
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return Name
        End Get
    End Property

    Public Overrides ReadOnly Property CanOpen() As Boolean
        Get
            Return True
        End Get
    End Property

    'The only element of aArgs is an atcDataGroup or atcTimeseries
    'The attribute(s) will be set to the result(s) of calculation(s)
    Public Overrides Function Open(ByVal aOperationName As String, _
                          Optional ByVal aArgs As atcDataAttributes = Nothing) As Boolean
        Dim lTMinTSer As atcTimeseries = Nothing
        Dim lTMaxTSer As atcTimeseries = Nothing
        Dim lSRadTSer As atcTimeseries = Nothing
        Dim lWindTSer As atcTimeseries = Nothing
        Dim lDlyTSer As atcTimeseries = Nothing
        Dim lObsTimeTSer As atcTimeseries = Nothing
        Dim lLatitude As Double
        Dim lCTS(12) As Double
        Dim lDegF As Boolean
        Dim lObsTime As Integer
        Dim lAttDef As atcAttributeDefinition
        Dim lOk As Boolean

        Dim lOpns As atcDataAttributes = AvailableOperations()
        DataSets.Clear() 'assume we don't want any old datasets (maybe add arg to define this???)
        Select Case aOperationName
            Case "Solar Radiation"
                Dim lCldTSer As atcTimeseries = Nothing
                If aArgs Is Nothing Then
                    Dim lForm As New frmCmpSol
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lCldTSer, lLatitude)
                Else
                    lCldTSer = aArgs.GetValue("DCLD")
                    lLatitude = aArgs.GetValue("Latitude")
                    lOk = True
                End If
                If lOk AndAlso lCldTSer IsNot Nothing AndAlso _
                  lLatitude >= MetComputeLatitudeMin AndAlso lLatitude <= MetComputeLatitudeMax Then
                    Dim lMetCmpTS As atcTimeseries = SolarRadiationFromCloudCover(lCldTSer, Me, lLatitude)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Cloud Cover from Solar"
                If aArgs Is Nothing Then
                    'TODO: Add interface to Cloud Cover from Solar
                    Logger.Msg("No user interface available for <Cloud Cover from Solar>.  Use a script for now", "Cloud Cover from Solar")
                    'Dim lForm As New frmCmpSol
                    'lOk = lForm.AskUser(DataManager, lCldTSer, lLatitude)
                Else
                    lSRadTSer = aArgs.GetValue("SRAD")
                    lLatitude = aArgs.GetValue("Latitude")
                    lOk = True
                End If
                If lOk AndAlso lSRadTSer IsNot Nothing AndAlso _
                  lLatitude >= MetComputeLatitudeMin AndAlso lLatitude <= MetComputeLatitudeMax Then
                    Dim lMetCmpTS As atcTimeseries = CloudCoverTimeseriesFromSolar(lSRadTSer, Me, lLatitude)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Jensen PET"
                Dim lCTX As Double
                If aArgs Is Nothing Then
                    Dim lForm As New frmCmpJPET
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lTMinTSer, lTMaxTSer, lSRadTSer, lDegF, lCTX, lCTS)
                Else
                    lTMinTSer = aArgs.GetValue("TMIN")
                    lTMaxTSer = aArgs.GetValue("TMAX")
                    lSRadTSer = aArgs.GetValue("SRAD")
                    lDegF = aArgs.GetValue("Degrees F")
                    lCTX = aArgs.GetValue("Constant Coefficient")
                    lCTS = aArgs.GetValue("Jensen Monthly Coefficients")
                    lOk = True
                End If
                If lOk Then
                    lAttDef = atcDataAttributes.GetDefinition("Jensen Monthly Coefficients")
                    For i As Integer = 1 To 12
                        If lCTS(i) > lAttDef.Max Or lCTS(i) < lAttDef.Min Then
                            lOk = False
                            Exit For
                        End If
                    Next
                    lAttDef = atcDataAttributes.GetDefinition("Constant Coefficient")
                    If lOk AndAlso lTMinTSer IsNot Nothing AndAlso lTMaxTSer IsNot Nothing AndAlso _
                                   lSRadTSer IsNot Nothing AndAlso lCTX >= lAttDef.Min AndAlso lCTX <= lAttDef.Max Then
                        Dim lMetCmpTS As atcTimeseries = CmpJen(lTMinTSer, lTMaxTSer, lSRadTSer, Me, lDegF, lCTX, lCTS)
                        MyBase.DataSets.Add(lMetCmpTS)
                    End If
                End If
            Case "Hamon PET"
                If aArgs Is Nothing Then
                    Dim lForm As New frmCmpHPET
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lTMinTSer, lTMaxTSer, lDegF, lLatitude, lCTS)
                Else
                    lTMinTSer = aArgs.GetValue("TMIN")
                    lTMaxTSer = aArgs.GetValue("TMAX")
                    lDegF = aArgs.GetValue("Degrees F")
                    lLatitude = aArgs.GetValue("Latitude")
                    lCTS = aArgs.GetValue("Hamon Monthly Coefficients")
                    lOk = True
                End If
                If lOk Then
                    Dim lHM As atcDefinedValue = lOpns.GetDefinedValue("Hamon PET")
                    If lHM Is Nothing Then
                        lAttDef = atcDataAttributes.GetDefinition("Hamon Monthly Coefficients")
                    Else
                        lAttDef = lHM.Arguments.GetValue("Hamon Monthly Coefficients")
                    End If
                    For i As Integer = 1 To 12
                        If lCTS(i) > lAttDef.Max Or lCTS(i) < lAttDef.Min Then
                            lOk = False
                            Exit For
                        End If
                    Next
                    If lOk AndAlso lTMaxTSer IsNot Nothing AndAlso lTMinTSer IsNot Nothing AndAlso _
                       lLatitude >= MetComputeLatitudeMin AndAlso lLatitude <= MetComputeLatitudeMax Then
                        Dim lMetCmpTS As atcTimeseries = PanEvaporationTimeseriesComputedByHamon(lTMinTSer, lTMaxTSer, Me, lDegF, lLatitude, lCTS)
                        MyBase.DataSets.Add(lMetCmpTS)
                    End If
                End If
            Case "Penman Pan Evaporation"
                Dim lDewPTSer As atcTimeseries = Nothing
                If aArgs Is Nothing Then
                    Dim lForm As New frmCmpPenman
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lTMinTSer, lTMaxTSer, lSRadTSer, lDewPTSer, lWindTSer)
                Else
                    lTMinTSer = aArgs.GetValue("TMIN")
                    lTMaxTSer = aArgs.GetValue("TMAX")
                    lSRadTSer = aArgs.GetValue("SRAD")
                    lDewPTSer = aArgs.GetValue("DEWP")
                    lWindTSer = aArgs.GetValue("TWND")
                    lOk = True
                End If
                If lOk AndAlso lTMinTSer IsNot Nothing AndAlso lTMaxTSer IsNot Nothing AndAlso _
                               lSRadTSer IsNot Nothing AndAlso lDewPTSer IsNot Nothing AndAlso lWindTSer IsNot Nothing Then
                    Dim lMetCmpTS As atcTimeseries = PanEvaporationTimeseriesComputedByPenman(lTMinTSer, lTMaxTSer, lSRadTSer, lDewPTSer, lWindTSer, Me)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Wind Travel"
                If aArgs Is Nothing Then
                    Dim ltsgroup As atcTimeseriesGroup = atcDataManager.UserSelectData("Select Wind Speed data for computing " & aOperationName, Nothing, Nothing, True, True, pIcon)
                    If ltsgroup IsNot Nothing AndAlso ltsgroup.Count > 0 Then lWindTSer = ltsgroup(0)
                Else
                    lWindTSer = aArgs.GetValue("WIND")
                End If
                If lWindTSer IsNot Nothing Then
                    Dim lMetCmpTS As atcTimeseries = WindTravelFromWindSpeed(lWindTSer, Me)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Cloud Cover"
                Dim lPctSunTSer As atcTimeseries = Nothing
                If aArgs Is Nothing Then
                    Dim ltsgroup As atcTimeseriesGroup = atcDataManager.UserSelectData("Select Percent Sun data for computing " & aOperationName, Nothing, Nothing, True, True, pIcon)
                    If ltsgroup IsNot Nothing AndAlso ltsgroup.Count > 0 Then lPctSunTSer = ltsgroup(0)
                Else
                    lPctSunTSer = aArgs.GetValue("PSUN")
                End If
                If Not lPctSunTSer Is Nothing Then
                    Dim lMetCmpTS As atcTimeseries = CmpCld(lPctSunTSer, Me)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Solar Radiation (Disaggregate)"
                If aArgs Is Nothing Then
                    Dim lForm As New frmDisSol
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lDlyTSer, lLatitude)
                Else
                    lDlyTSer = aArgs.GetValue("SRAD")
                    lLatitude = aArgs.GetValue("Latitude")
                    lOk = True
                End If
                If lOk AndAlso lDlyTSer IsNot Nothing AndAlso _
                  lLatitude >= MetComputeLatitudeMin AndAlso lLatitude <= MetComputeLatitudeMax Then
                    Dim lMetCmpTS As atcTimeseries = DisSolPet(lDlyTSer, Me, 1, lLatitude)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Evapotranspiration"
                If aArgs Is Nothing Then
                    Dim lForm As New frmDisSol
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lForm.Text = "Disaggregate Evapotranspiration"
                    lForm.lblTSer.Text = "Specify Daily Evapotranspiration Timeseries"
                    lOk = lForm.AskUser(lDlyTSer, lLatitude)
                Else
                    lDlyTSer = aArgs.GetValue("DEVT")
                    lLatitude = aArgs.GetValue("Latitude")
                    lOk = True
                End If
                If lOk AndAlso lDlyTSer IsNot Nothing AndAlso _
                  lLatitude >= MetComputeLatitudeMin AndAlso lLatitude <= MetComputeLatitudeMax Then
                    Dim lMetCmpTS As atcTimeseries = DisSolPet(lDlyTSer, Me, 2, lLatitude)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Temperature"
                If aArgs Is Nothing Then
                    Dim lForm As New frmDisTemp
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lTMinTSer, lTMaxTSer, lObsTime)
                    'build obs time TSer with constant value from aObsTime argument
                    If lOk Then
                        lObsTimeTSer = lTMinTSer.Clone
                        For i As Integer = 1 To lObsTimeTSer.numValues
                            lObsTimeTSer.Value(i) = lObsTime
                        Next
                        lObsTimeTSer.Attributes.SetValue("Scenario", "CONST-" & lObsTime)
                        lObsTimeTSer.Attributes.SetValue("Constituent", lTMinTSer.Attributes.GetValue("Constituent") & "-OBS")
                    End If
                Else
                    lTMinTSer = aArgs.GetValue("TMIN")
                    lTMaxTSer = aArgs.GetValue("TMAX")
                    lObsTimeTSer = aArgs.GetValue("Observation Hour Timeseries")
                    lOk = True
                End If
                If lOk AndAlso lTMinTSer IsNot Nothing AndAlso lTMinTSer IsNot Nothing AndAlso lObsTimeTSer IsNot Nothing Then
                    Dim lMetCmpTS As atcTimeseries = DisaggTemp(lTMinTSer, lTMaxTSer, Me, lObsTimeTSer)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Wind (Disaggregate)"
                Dim lHrDist(24) As Double
                Dim lHrSum As Double = 0
                If aArgs Is Nothing Then
                    Dim lForm As New frmDisWind
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lWindTSer, lHrDist)
                Else
                    lWindTSer = aArgs.GetValue("TWND")
                    lHrDist = aArgs.GetValue("Hourly Distribution")
                    lOk = True
                End If
                If lOk Then
                    lAttDef = atcDataAttributes.GetDefinition("Hourly Distribution")
                    For i As Integer = 1 To 24
                        If lHrDist(i) > lAttDef.Max Or lHrDist(i) < lAttDef.Min Then
                            lOk = False
                            Exit For
                        Else
                            lHrSum += lHrDist(i)
                        End If
                    Next
                    If lOk AndAlso lWindTSer IsNot Nothing AndAlso Math.Abs(lHrSum - 1) < 0.001 Then
                        Dim lMetCmpTS As atcTimeseries = DisWnd(lWindTSer, Me, lHrDist)
                        MyBase.DataSets.Add(lMetCmpTS)
                    End If
                End If
            Case "Precipitation"
                Dim lHrTSers As atcTimeseriesGroup = Nothing
                Dim lTol As Double
                Dim lSummFile As String = ""
                If aArgs Is Nothing Then
                    Dim lForm As New frmDisPrec
                    If pIcon IsNot Nothing Then lForm.Icon = pIcon
                    lOk = lForm.AskUser(lDlyTSer, lHrTSers, lObsTime, lTol, lSummFile)
                    If lOk Then
                        'build obs time TSer with constant value from aObsTime argument
                        lObsTimeTSer = lDlyTSer.Clone
                        For i As Integer = 1 To lObsTimeTSer.numValues
                            lObsTimeTSer.Value(i) = lObsTime
                        Next
                        lObsTimeTSer.Attributes.SetValue("Scenario", "CONST-" & lObsTime)
                        lObsTimeTSer.Attributes.SetValue("Constituent", lDlyTSer.Attributes.GetValue("Constituent") & "-OBS")
                    End If
                Else
                    lDlyTSer = aArgs.GetValue("DPRC")
                    lHrTSers = aArgs.GetValue("HPCP")
                    lObsTimeTSer = aArgs.GetValue("Observation Hour Timeseries")
                    lTol = aArgs.GetValue("Data Tolerance")
                    lSummFile = aArgs.GetValue("Summary File")
                    lOk = True
                End If
                lAttDef = atcDataAttributes.GetDefinition("Data Tolerance")
                If lOk AndAlso lHrTSers IsNot Nothing AndAlso lObsTimeTSer IsNot Nothing AndAlso _
                       lTol >= lAttDef.Min AndAlso lTol <= lAttDef.Max Then
                    Dim lMetCmpTS As atcTimeseries = DisaggPrecip(lDlyTSer, Me, lHrTSers, lObsTimeTSer, lTol, lSummFile)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
            Case "Dewpoint"
                Dim lDewPTSer As atcTimeseries = Nothing
                Dim lATmpTSer As atcTimeseries = Nothing
                If aArgs Is Nothing Then
                    'TODO: Add interface to Dewpoint Disaggregation
                    Logger.Msg("No user interface available for <Dewpoint Disaggration>.  Use a script for now", "Dewpoint Disaggration")
                    '          Dim lForm As New frmDisTemp
                    '         lOk = lForm.AskUser(DataManager, lTMinTSer, lTMaxTSer, lObsTime)
                Else
                    lDewPTSer = aArgs.GetValue("Dewpoint")
                    lATmpTSer = aArgs.GetValue("ATMP")
                    lOk = True
                End If
                If lOk AndAlso lDewPTSer IsNot Nothing AndAlso lATmpTSer IsNot Nothing Then
                    Dim lMetCmpTS As atcTimeseries = DisDewPoint(lDewPTSer, Me, lATmpTSer)
                    MyBase.DataSets.Add(lMetCmpTS)
                End If
        End Select

        If MyBase.DataSets.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

#If GISProvider = "DotSpatial" Then
    <CLSCompliant(False)> _
    Public Sub Initialize()
        Dim lAvlOps As atcDataAttributes = AvailableOperations()
    End Sub
#Else
    <CLSCompliant(False)> _
    Public Overrides Sub Initialize(ByVal aMapWin As MapWindow.Interfaces.IMapWin, _
                                    ByVal aParentHandle As Integer)
        MyBase.Initialize(aMapWin, aParentHandle)
        Dim lAvlOps As atcDataAttributes = AvailableOperations()
    End Sub
#End If

    Public Overrides Function ToString() As String
        Return Name.Replace("Timeseries::", "").Replace("Seasonal - ", "")
    End Function

    Public Overrides ReadOnly Property AvailableOperations() As atcDataAttributes
        Get
            If pAvailableOperations Is Nothing Then
                pAvailableOperations = New atcDataAttributes
                Dim lArguments As atcDataAttributes
                Dim defTimeSeriesOne As New atcAttributeDefinition
                With defTimeSeriesOne
                    .Name = "Timeseries"
                    .Description = "One time series"
                    .Editable = True
                    .TypeString = "atcTimeseries"
                End With

                Dim defTMinTS As New atcAttributeDefinition
                defTMinTS = defTimeSeriesOne.Clone("TMIN", "Daily Minimum Temperature")
                Dim defTMaxTS As New atcAttributeDefinition
                defTMaxTS = defTimeSeriesOne.Clone("TMAX", "Daily Maximum Temperature")

                Dim defLat As New atcAttributeDefinition
                With defLat
                    .Name = "Latitude"
                    .Description = "Latitude in decimal degrees"
                    .DefaultValue = 0
                    .Editable = True
                    .TypeString = "Double"
                End With

                Dim lSolar As New atcAttributeDefinition
                With lSolar
                    .Name = "Solar Radiation"
                    .Category = "Computations"
                    .Description = "Generate Solar Radiation from Cloud Cover"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTimeSeriesOne.Clone("DCLD", "Daily Cloud Cover (values = 0 - 10)"), Nothing)
                lArguments.SetValue(defLat, Nothing)

                pAvailableOperations.SetValue(lSolar, Nothing, lArguments)

                Dim lCloud As New atcAttributeDefinition
                With lCloud
                    .Name = "Cloud Cover from Solar"
                    .Category = "Computations"
                    .Description = "Generate Cloud Cover from Solar Radiation"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defSRadTS As New atcAttributeDefinition
                defSRadTS = defTimeSeriesOne.Clone("SRAD", "Daily Solar Radiation (Langleys)")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defSRadTS, Nothing)
                lArguments.SetValue(defLat, Nothing)

                pAvailableOperations.SetValue(lCloud, Nothing, lArguments)

                Dim defDegF As New atcAttributeDefinition
                With defDegF
                    .Name = "Degrees F"
                    .Description = "Temperature in Degrees F, True or False"
                    .DefaultValue = True
                    .Editable = True
                    .TypeString = "Boolean"
                End With

                Dim defHMonCoeff As New atcAttributeDefinition
                With defHMonCoeff
                    .Name = "Hamon Monthly Coefficients"
                    .Description = "Coefficients for Hamon PET computation"
                    .DefaultValue = New Double() {0, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055, 0.0055}
                    .Max = 1
                    .Min = 0
                    .Editable = True
                    .TypeString = "Double(12)"
                End With

                Dim lHamon As New atcAttributeDefinition
                With lHamon
                    .Name = "Hamon PET"
                    .Category = "Computations"
                    .Description = "Generate PET using Hamon's algorithm"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTMinTS, Nothing)
                lArguments.SetValue(defTMaxTS, Nothing)
                lArguments.SetValue(defDegF, Nothing)
                lArguments.SetValue(defLat, Nothing)
                lArguments.SetValue(defHMonCoeff, defHMonCoeff)

                pAvailableOperations.SetValue(lHamon, Nothing, lArguments)

                Dim defJMonCoeff As New atcAttributeDefinition
                With defJMonCoeff
                    .Name = "Jensen Monthly Coefficients"
                    .Description = "Coefficients for Jensen PET computation"
                    .DefaultValue = New Double() {0, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012, 0.012}
                    .Max = 1
                    .Min = 0
                    .Editable = True
                    .TypeString = "Double(12)"
                End With

                Dim lJensen As New atcAttributeDefinition
                With lJensen
                    .Name = "Jensen PET"
                    .Category = "Computations"
                    .Description = "Generate PET using Jensen's algorithm"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defConstCoeff As New atcAttributeDefinition
                With defConstCoeff
                    .Name = "Constant Coefficient"
                    .Description = "Constant Coefficient CTX used in Jensen PET computation"
                    .DefaultValue = 23
                    .Max = 27
                    .Min = 10
                    .Editable = True
                    .TypeString = "Double"
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTMinTS, Nothing)
                lArguments.SetValue(defTMaxTS, Nothing)
                lArguments.SetValue(defSRadTS, Nothing)
                lArguments.SetValue(defDegF, Nothing)
                lArguments.SetValue(defJMonCoeff, Nothing)
                lArguments.SetValue(defConstCoeff, Nothing)

                pAvailableOperations.SetValue(lJensen, Nothing, lArguments)

                Dim lPenman As New atcAttributeDefinition
                With lPenman
                    .Name = "Penman Pan Evaporation"
                    .Category = "Computations"
                    .Description = "Generate Pan Evaporation using Penman's algorithm"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defDewPTS As New atcAttributeDefinition
                defDewPTS = defTimeSeriesOne.Clone("DEWP", "Daily Dewpoint Temperature")
                Dim defTWindTS As New atcAttributeDefinition
                defTWindTS = defTimeSeriesOne.Clone("TWND", "Daily Wind Movement (miles)")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTMinTS, Nothing)
                lArguments.SetValue(defTMaxTS, Nothing)
                lArguments.SetValue(defSRadTS, Nothing)
                lArguments.SetValue(defDewPTS, Nothing)
                lArguments.SetValue(defTWindTS, Nothing)

                pAvailableOperations.SetValue(lPenman, Nothing, lArguments)

                Dim lWind As New atcAttributeDefinition
                With lWind
                    .Name = "Wind Travel"
                    .Category = "Computations"
                    .Description = "Generate Daily Wind Travel using Average Daily Wind Speed"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defWindTS As New atcAttributeDefinition
                defWindTS = defTimeSeriesOne.Clone("WIND", "Daily Average Wind Speed (mph)")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defWindTS, Nothing)
                pAvailableOperations.SetValue(lWind, Nothing, lArguments)

                Dim lCldCov As New atcAttributeDefinition
                With lCldCov
                    .Name = "Cloud Cover"
                    .Category = "Computations"
                    .Description = "Generate Daily Cloud Cover using Daily Percent Sunshine"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defPSunTS As New atcAttributeDefinition
                defPSunTS = defTimeSeriesOne.Clone("PSUN", "Daily Percent Sunshine")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defPSunTS, Nothing)
                pAvailableOperations.SetValue(lCldCov, Nothing, lArguments)

                Dim lDisSolar As New atcAttributeDefinition
                With lDisSolar
                    .Name = "Solar Radiation (Disaggregate)"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily Solar Radiation to Hourly"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defSRadTS, Nothing)
                lArguments.SetValue(defLat, Nothing)
                pAvailableOperations.SetValue(lDisSolar, Nothing, lArguments)

                Dim lDisEvap As New atcAttributeDefinition
                With lDisEvap
                    .Name = "Evapotranspiration"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily Evapotranspiration to Hourly"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defEvapTS As New atcAttributeDefinition
                defEvapTS = defTimeSeriesOne.Clone("DEVT", "Daily Evapotranspiration (in)")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defEvapTS, Nothing)
                lArguments.SetValue(defLat, Nothing)
                pAvailableOperations.SetValue(lDisEvap, Nothing, lArguments)

                Dim defObsTimeTS As New atcAttributeDefinition
                defObsTimeTS = defTimeSeriesOne.Clone("Observation Hour Time Series", "Time Series of Daily Observation times (1-24)")

                Dim lDisTemp As New atcAttributeDefinition
                With lDisTemp
                    .Name = "Temperature"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily TMIN/TMAX to Hourly Temperature"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                'Dim defObsTime As New atcAttributeDefinition
                'With defObsTime
                '    .Name = "Observation Time"
                '    .Description = "Hour (1 - 24) that Daily TMin/TMax observations were made"
                '    .DefaultValue = 24
                '    .Max = 24
                '    .Min = 1
                '    .Editable = True
                '    .TypeString = "Integer"
                'End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTMinTS, Nothing)
                lArguments.SetValue(defTMaxTS, Nothing)
                lArguments.SetValue(defObsTimeTS, Nothing)
                pAvailableOperations.SetValue(lDisTemp, Nothing, lArguments)

                Dim lDisWind As New atcAttributeDefinition
                With lDisWind
                    .Name = "Wind (Disaggregate)"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily Wind to Hourly"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defHrDist As New atcAttributeDefinition
                With defHrDist
                    .Name = "Hourly Distribution"
                    .Description = "Hourly Distribution for Wind Travel Disaggregation"
                    .DefaultValue = New Double() {0, 0.034, 0.034, 0.034, 0.034, 0.034, 0.034, 0.034, 0.035, 0.037, 0.041, 0.046, 0.05, 0.053, 0.054, 0.058, 0.057, 0.056, 0.05, 0.043, 0.04, 0.038, 0.035, 0.035, 0.034}
                    .Max = 1
                    .Min = 0
                    .Editable = True
                    .TypeString = "Double(24)"
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defTWindTS, Nothing)
                lArguments.SetValue(defHrDist, Nothing)
                pAvailableOperations.SetValue(lDisWind, Nothing, lArguments)

                Dim lDisPrec As New atcAttributeDefinition
                With lDisPrec
                    .Name = "Precipitation"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily Precip to Hourly based on nearby Hourly data"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defDPrecTS As New atcAttributeDefinition
                defDPrecTS = defTimeSeriesOne.Clone("DPRC", "Daily Precipitation")

                Dim defHrPrec As New atcAttributeDefinition
                With defHrPrec
                    .Name = "HPCP"
                    .Description = "Hourly Precipitation stations to use for Disaggregating Daily Precip"
                    .Editable = True
                    .TypeString = "atcDataGroup"
                End With

                Dim defTolerance As New atcAttributeDefinition
                With defTolerance
                    .Name = "Data Tolerance"
                    .Description = "Allowable ratio (as %) between Daily and Hourly values to use Hourly values for disaggregation"
                    .DefaultValue = 90
                    .Max = 100
                    .Min = 0
                    .Editable = True
                    .TypeString = "Double"
                End With

                Dim defSummFile As New atcAttributeDefinition
                With defSummFile
                    .Name = "Summary File"
                    .Description = "MetCmp Precipitation Disaggregation Summary File"
                    .DefaultValue = "DisPrecip.sum"
                    .Editable = True
                    .TypeString = "String"
                End With

                lArguments = New atcDataAttributes
                lArguments.SetValue(defDPrecTS, Nothing)
                lArguments.SetValue(defHrPrec, Nothing)
                lArguments.SetValue(defObsTimeTS, Nothing)
                lArguments.SetValue(defTolerance, Nothing)
                lArguments.SetValue(defSummFile, Nothing)
                pAvailableOperations.SetValue(lDisPrec, Nothing, lArguments)

                Dim lDisDewPoint As New atcAttributeDefinition
                With lDisDewPoint
                    .Name = "Dewpoint"
                    .Category = "Disaggregations"
                    .Description = "Disaggregate Daily Dewpoint Temp to Hourly"
                    .Editable = False
                    .TypeString = "atcTimeseries"
                    .Calculator = Me
                End With

                Dim defATmpTS As New atcAttributeDefinition
                defATmpTS = defTimeSeriesOne.Clone("ATMP", "Hourly Air Temperature")

                lArguments = New atcDataAttributes
                lArguments.SetValue(defDewPTS, Nothing)
                lArguments.SetValue(defATmpTS, Nothing)
                pAvailableOperations.SetValue(lDisDewPoint, Nothing, lArguments)
            End If

            Return pAvailableOperations
        End Get
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal aIcon As System.Drawing.Icon)
        If aIcon IsNot Nothing Then
            pIcon = aIcon
        End If
    End Sub
End Class