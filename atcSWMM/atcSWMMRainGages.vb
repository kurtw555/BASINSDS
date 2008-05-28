Imports System.Collections.ObjectModel
Imports System.IO
Imports MapWinUtility
Imports atcUtility
Imports System.Text

Public Class RainGages
    Inherits KeyedCollection(Of String, RainGage)
    Protected Overrides Function GetKeyForItem(ByVal aRainGage As RainGage) As String
        Dim lKey As String = aRainGage.Name
        Return lKey
    End Function

    Public SWMMProject As SWMMProject

    Public Overrides Function ToString() As String
        Dim lSB As New StringBuilder

        lSB.Append("[RAINGAGES]" & vbCrLf & _
                   ";;               Rain      Recd.  Snow   Data       Source           Station    Rain  Start     " & vbCrLf & _
                   ";;Name           Type      Freq.  Catch  Source     Name             ID         Units Date      " & vbCrLf & _
                   ";;----------------------------------------------------------------------------------------------" & vbCrLf)

        For Each lRaingage As RainGage In Me
            With lRaingage
                lSB.Append(StrPad(.Name, 16, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(.Form, 9, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(.Interval, 6, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(Format(.SnowCatchFactor, "0.0"), 6, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(.Type, 10, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(.Name & ":P", 16, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(" ", 10, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(.Units, 5, " ", False))
                lSB.Append(" ")
                lSB.Append(vbCrLf)
            End With
        Next

        Return lSB.ToString
    End Function

    Public Function CoordinatesToString() As String
        Dim lSB As New StringBuilder
        lSB.Append("[SYMBOLS]" & vbCrLf & _
                   ";;Gage           X-Coord            Y-Coord           " & vbCrLf & _
                   ";;-------------- ------------------ ------------------" & vbCrLf)

        For Each lRaingage As RainGage In Me
            With lRaingage
                lSB.Append(StrPad(.Name, 16, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(Format(.XPos, "0.000"), 18, " ", False))
                lSB.Append(" ")
                lSB.Append(StrPad(Format(.YPos, "0.000"), 18, " ", False))
                lSB.Append(vbCrLf)
            End With
        Next

        Return lSB.ToString
    End Function

    Public Function TimeSeriesHeaderToString() As String
        Dim lSB As New StringBuilder
        lSB.Append("[TIMESERIES]" & vbCrLf & _
                   ";;Name           Date       Time       Value     " & vbCrLf & _
                   ";;-------------- ---------- ---------- ----------")
        Return lSB.ToString
    End Function

    Public Function TimeSeriesToString() As String
        Dim lSB As New StringBuilder
        lSB.Append(";RAINFALL" & vbCrLf)

        For Each lRaingage As RainGage In Me
            lSB.Append(Me.SWMMProject.TimeSeriesToString(lRaingage.TimeSeries, lRaingage.Name & ":P"))
        Next

        Return lSB.ToString
    End Function
End Class

Public Class RainGage
    Public Name As String
    Public Form As String = "INTENSITY" 'intensity (or volume or cumulative)
    Public Interval As String = "1:00"
    Public SnowCatchFactor As Double = 1.0
    Public Type As String = "TIMESERIES" 'timeseries (or file)
    Public TimeSeries As atcData.atcTimeseries
    Public Units As String = "IN" 'in (or mm)
    Public YPos As Double = 0.0
    Public XPos As Double = 0.0
End Class
