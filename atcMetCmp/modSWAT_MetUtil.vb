﻿Imports atcUtility

''' <summary>
''' Excerpted and translated routines from SWAT 
''' </summary>
''' <remarks>function and subroutine names are taken from SWAT, variable names updated to meet BASINS conventions</remarks>
Module modSWAT_MetUtil
    Function RelativeHumidity(ByVal aTemperatureAver As Double, ByVal aPrecipitation As Double, _
                              ByVal aTemperatureMonthMax As Double, ByVal aTemperatureMonthMin As Double, _
                              ByVal aDewpointMonthAver As Double, ByVal aWetDaysMonthFraction As Double, _
                              ByVal aRelativeHumidityDistribution As TriangularDistribution) As Double
        Dim lTemperatureMonthAver As Double = (aTemperatureMonthMax + aTemperatureMonthMin) / 2
        Dim lRelativeHumidityMonthAver As Double = SaturationVaporPressure(aDewpointMonthAver) / SaturationVaporPressure(lTemperatureMonthAver)
        Dim lTemp As Double = 0.9 * aWetDaysMonthFraction
        Dim lRelativeHumidityMonthAverAdjWetDry As Double = (lRelativeHumidityMonthAver - lTemp) / (1 - lTemp)
        If lRelativeHumidityMonthAverAdjWetDry < 0.05 Then
            lRelativeHumidityMonthAverAdjWetDry = 0.5 * lRelativeHumidityMonthAver
        End If
        If aPrecipitation > 0.0 Then
            lRelativeHumidityMonthAverAdjWetDry = (lRelativeHumidityMonthAverAdjWetDry * 0.1) + 0.9
        End If
        lTemp = lRelativeHumidityMonthAverAdjWetDry - 1
        Dim lRelativeHumidityMonthMax As Double = lRelativeHumidityMonthAverAdjWetDry - (lTemp * Math.Exp(lTemp))
        Dim lRelativeHumidityMonthMin As Double = lRelativeHumidityMonthAverAdjWetDry * (1.0 - Math.Exp(-lRelativeHumidityMonthAverAdjWetDry))
        Dim lRelativeHumidity As Double = aRelativeHumidityDistribution.Generate(lRelativeHumidityMonthMin, _
                                                                                 lRelativeHumidityMonthAverAdjWetDry, _
                                                                                 lRelativeHumidityMonthMax)
        Return lRelativeHumidity
    End Function

    ''' <summary>
    ''' generates daily solar radiation 
    ''' </summary>
    ''' <param name="aPrecip">precipitation for the day(mm H2O)</param>
    ''' <param name="aFracWetDays">proportion of wet days in a month</param>
    ''' <param name="aSolarRadiationAver">average daily solar radiation for the month(MJ/m^2/day)</param>
    ''' <param name="aSolarRadiationMax">maximum possible radiation for the day in HRU(MJ/m^2)</param>
    ''' <param name="aPrecImpactParm">parameter which predicts impact of precip on daily solar radiation</param>
    ''' <returns>solar radiation for the day (MJ/m^2)</returns>
    ''' <remarks>based on SWAT 2005 routine 'slrgen'</remarks>
    Function SolarRadiation(ByVal aPrecip As Double, ByVal aFracWetDays As Double, _
                            ByVal aSolarRadiationAver As Double, ByVal aSolarRadiationMax As Double, _
                            ByVal aPrecImpactParm As Double) As Double

        Dim lRav As Double  'modified monthly average solar radiation
        lRav = aSolarRadiationAver / (1.0 - (0.5 * aFracWetDays))
        If (aPrecip > 0.0) Then
            lRav *= 0.5
        End If

        Dim lRx As Double 'intermediate calc result
        lRx = aSolarRadiationMax - lRav

        Dim lSolarRadiation As Double = lRav + (aPrecImpactParm * lRx / 4.0)
        If (lSolarRadiation <= 0.0) Then
            lSolarRadiation = 0.05 * aSolarRadiationMax
        End If
        Return lSolarRadiation
    End Function

    ''' <summary>
    ''' calculate saturation vapor pressure for a given air temperature
    ''' </summary>
    ''' <param name="aTemperatureAir">air temperature (deg C)</param>
    ''' <returns>saturation vapor pressure (kPa)</returns>
    ''' <remarks>based on SWAT 2005 routine 'ee'</remarks>
    Function SaturationVaporPressure(ByVal aTemperatureAir As Double) As Double
        Dim lSaturationVaporPressure As Double = 0.0
        If ((aTemperatureAir + 237.3) > 0.0000000001) Then
            lSaturationVaporPressure = ((16.78 * aTemperatureAir) - 116.9) / (aTemperatureAir + 237.3)
            lSaturationVaporPressure = Math.Exp(lSaturationVaporPressure)
        End If
        Return lSaturationVaporPressure
    End Function

    ''' <summary>
    ''' generate daily wind speed
    ''' </summary>
    ''' <param name="aWindMonthlyAverage">average wind speed for the month(m/s)</param>
    ''' <param name="aWindSpeedDistribution">wind speed distribution</param>
    ''' <returns>daily wind speed(m/s)</returns>
    ''' <remarks>based on SWAT 2005 routine 'wndgen'</remarks>
    Function Wind(ByVal aWindMonthlyAverage As Double, ByVal aWindSpeedDistribution As UniformDistribution) As Double
        Dim lRandomNumber As Double = aWindSpeedDistribution.PreviousValue
        aWindSpeedDistribution.Generate()
        Dim lWind As Double = aWindMonthlyAverage * (-Math.Log(lRandomNumber)) ^ 0.3
        Return lWind
    End Function
End Module

Public Class SwatWeatherStations
    Inherits PointLocations
 
    Private Const pFileName As String = "AllStns.txt"
    Protected Overrides Function Delimeter() As String
        Return " "
    End Function

    Public Sub New()
        MyBase.New(pFileName, "atcMetCmp")
        MapWinUtility.Logger.Dbg("  Built SwatWeatherStations with Count " & Count)
    End Sub

    Public Overrides Sub AddLocation(ByVal aTable As atcUtility.atcTable)
        Me.Add(New SwatWeatherStation(aTable))
    End Sub

    Protected Overrides Function InternalFilename() As String
        Return pFileName
    End Function
End Class

Public Class SwatWeatherStation
    Inherits PointLocation

    Public State As String
    Public NameKey As String
    Public Name As String
    Public LatSinR As Double
    Public LatCosR As Double
    Public Elev As Double
    Public PrecDaysPerMonth As Double
    Public PrecAv(12) As Double
    Public AirTempMaxAv(12) As Double '9-20
    Public AirTempMinAv(12) As Double '21-32
    Public Prob_WetAfterDry(12) As Double  '93-104
    Public Prob_WetAfterWet(12) As Double  '105-116
    Public ProportionWet(12) As Double 'calc
    Public SolarAv(12) As Double '141-152
    Public DewpointAv(12) As Double '153-164
    Public WindAv(12) As Double '165-176

    ''' <summary>Fill specific details about point from record in table</summary>
    ''' <param name="aTable">table containing locations</param>
    ''' <remarks>uses current record from table</remarks>
    Public Sub New(ByVal aTable As atcUtility.atcTable)
        MyBase.New(aTable, 4, 5, 6, " ")
        Dim lFieldValues() As String = Split(aTable.CurrentRecordAsDelimitedString, ",")
        State = lFieldValues(0)
        NameKey = lFieldValues(1)
        Name = lFieldValues(2).Replace("_", " ")
        If lFieldValues(3) = 690 Then
            Debug.Print("ProblemSpot")
        End If
        LatSinR = Math.Sin(Latitude * DegreesToRadians)
        LatCosR = Math.Cos(Latitude * DegreesToRadians)
        Elev = lFieldValues(6)
        'If Id = 203 Then 'Hartsfield
        '    Debug.Print("Stop")
        'End If
        For lMonthIndex As Integer = 1 To 12
            AirTempMaxAv(lMonthIndex) = lFieldValues(7 + lMonthIndex)
            AirTempMinAv(lMonthIndex) = lFieldValues(19 + lMonthIndex)
            Prob_WetAfterDry(lMonthIndex) = lFieldValues(91 + lMonthIndex)
            Prob_WetAfterWet(lMonthIndex) = lFieldValues(103 + lMonthIndex)
            ProportionWet(lMonthIndex) = lFieldValues(115 + lMonthIndex)
            SolarAv(lMonthIndex) = lFieldValues(139 + lMonthIndex)
            DewpointAv(lMonthIndex) = lFieldValues(151 + lMonthIndex)
            WindAv(lMonthIndex) = lFieldValues(163 + lMonthIndex)

            Dim lDaysPerMonth As Integer = DayMon(2000, lMonthIndex)

            If Prob_WetAfterWet(lMonthIndex) <= Prob_WetAfterDry(lMonthIndex) OrElse _
               Prob_WetAfterDry(lMonthIndex) <= 0 Then
                Prob_WetAfterDry(lMonthIndex) = 0.75 * ProportionWet(lMonthIndex) / lDaysPerMonth
                Prob_WetAfterWet(lMonthIndex) = 0.25 + Prob_WetAfterDry(lMonthIndex)
            Else
                ProportionWet(lMonthIndex) = lDaysPerMonth * Prob_WetAfterDry(lMonthIndex) / (1 - Prob_WetAfterWet(lMonthIndex) + Prob_WetAfterDry(lMonthIndex))
            End If
            If ProportionWet(lMonthIndex) < 0.0 Then
                ProportionWet(lMonthIndex) = 0.001
            End If
            ProportionWet(lMonthIndex) = ProportionWet(lMonthIndex) / lDaysPerMonth
        Next
    End Sub

    ''' <summary></summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Key() As String
        Return NameKey
    End Function

    Public Overrides Function Description() As String
        Return Name
    End Function
End Class

