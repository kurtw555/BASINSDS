Imports System.Data
Imports Microsoft.Data.SQLite
Partial Class SwatInput
    Private pWgn As clsWgn = New clsWgn(Me)
    ReadOnly Property Wgn() As clsWgn
        Get
            Return pWgn
        End Get
    End Property

    Public Class clsWgnItem
        Public SUBBASIN As Integer
        Public STATION As String
        Public WLATITUDE As Single
        Public WLONGITUDE As Single
        Public WELEV As Single
        Public RAIN_YRS As Single
        Public TMPMX(11) As Single
        Public TMPMN(11) As Single
        Public TMPSTDMX(11) As Single
        Public TMPSTDMN(11) As Single
        Public PCPMM(11) As Single
        Public PCPSTD(11) As Single
        Public PCPSKW(11) As Single
        Public PR_W1_(11) As Single
        Public PR_W2_(11) As Single
        Public PCPD(11) As Single
        Public RAINHHMX(11) As Single
        Public SOLARAV(11) As Single
        Public DEWPT(11) As Single
        Public WNDAV(11) As Single

        Public Sub New(ByVal aSUBBASIN As Integer)
            SUBBASIN = aSUBBASIN
        End Sub

        Public Sub New(ByVal aSUBBASIN As Integer, ByVal aDataRow As DataRow)
            SUBBASIN = aSUBBASIN
            With aDataRow
                STATION = .Item("STATION")
                WLATITUDE = .Item("WLATITUDE")
                WLONGITUDE = .Item("WLONGITUDE")
                WELEV = .Item("WELEV")
                RAIN_YRS = .Item("RAIN_YRS")
                For lMonth As Integer = 1 To 12
                    TMPMX(lMonth - 1) = .Item("TMPMX" & lMonth)
                    TMPMN(lMonth - 1) = .Item("TMPMN" & lMonth)
                    TMPSTDMX(lMonth - 1) = .Item("TMPSTDMX" & lMonth)
                    TMPSTDMN(lMonth - 1) = .Item("TMPSTDMN" & lMonth)
                    PCPMM(lMonth - 1) = .Item("PCPMM" & lMonth)
                    PCPSTD(lMonth - 1) = .Item("PCPSTD" & lMonth)
                    PCPSKW(lMonth - 1) = .Item("PCPSKW" & lMonth)
                    PR_W1_(lMonth - 1) = .Item("PR_W1_" & lMonth)
                    PR_W2_(lMonth - 1) = .Item("PR_W2_" & lMonth)
                    PCPD(lMonth - 1) = .Item("PCPD" & lMonth)
                    RAINHHMX(lMonth - 1) = .Item("RAINHHMX" & lMonth)
                    SOLARAV(lMonth - 1) = .Item("SOLARAV" & lMonth)
                    DEWPT(lMonth - 1) = .Item("DEWPT" & lMonth)
                    WNDAV(lMonth - 1) = .Item("WNDAV" & lMonth)
                Next
            End With
        End Sub

        Public Function AddSQL() As String
            Return "INSERT INTO wgn( SUBBASIN, STATION, WLATITUDE, WLONGITUDE, WELEV, RAIN_YRS, " _
                    & GroupOfStrings("TMPMX#", 12, ", ") & ", " _
                    & GroupOfStrings("TMPMN#", 12, ", ") & ", " _
                    & GroupOfStrings("TMPSTDMX#", 12, ", ") & ", " _
                    & GroupOfStrings("TMPSTDMN#", 12, ", ") & ", " _
                    & GroupOfStrings("PCPMM#", 12, ", ") & ", " _
                    & GroupOfStrings("PCPSTD#", 12, ", ") & ", " _
                    & GroupOfStrings("PCPSKW#", 12, ", ") & ", " _
                    & GroupOfStrings("PR_W1_#", 12, ", ") & ", " _
                    & GroupOfStrings("PR_W2_#", 12, ", ") & ", " _
                    & GroupOfStrings("PCPD#", 12, ", ") & ", " _
                    & GroupOfStrings("RAINHHMX#", 12, ", ") & ", " _
                    & GroupOfStrings("SOLARAV#", 12, ", ") & ", " _
                    & GroupOfStrings("DEWPT#", 12, ", ") & ", " _
                    & GroupOfStrings("WNDAV#", 12, ", ") _
                    & " ) Values ('" & SUBBASIN & "', '" & STATION & "', '" & WLATITUDE & "', '" & WLONGITUDE & "', '" & WELEV & "', '" & RAIN_YRS & "', '" _
                    & ArrayToString(TMPMX, "', '") & "', '" _
                    & ArrayToString(TMPMN, "', '") & "', '" _
                    & ArrayToString(TMPSTDMX, "', '") & "', '" _
                    & ArrayToString(TMPSTDMN, "', '") & "', '" _
                    & ArrayToString(PCPMM, "', '") & "', '" _
                    & ArrayToString(PCPSTD, "', '") & "', '" _
                    & ArrayToString(PCPSKW, "', '") & "', '" _
                    & ArrayToString(PR_W1_, "', '") & "', '" _
                    & ArrayToString(PR_W2_, "', '") & "', '" _
                    & ArrayToString(PCPD, "', '") & "', '" _
                    & ArrayToString(RAINHHMX, "', '") & "', '" _
                    & ArrayToString(SOLARAV, "', '") & "', '" _
                    & ArrayToString(DEWPT, "', '") & "', '" _
                    & ArrayToString(WNDAV, "', '") & "' )"
        End Function
    End Class

    ''' <summary>
    ''' WGN Input Section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsWgn
        Private pSwatInput As SwatInput
        Private pTableName As String = "wgn"

        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            'based on mwSWATPlugIn:DBLayer:createRTE
            Try
                DropTable(pTableName, pSwatInput.CnSwatInput)

                Dim lConnection As SqliteConnection = pSwatInput.CnSwatInput
                Dim lCloseWhenDone As Boolean = False
                If lConnection.State <> ConnectionState.Open Then
                    lConnection.Open()
                    lCloseWhenDone = True
                End If

                Dim lCreateSql As New Text.StringBuilder
                lCreateSql.Append("CREATE TABLE " & pTableName & " (")
                lCreateSql.Append("OID INTEGER PRIMARY KEY AUTOINCREMENT, ")
                lCreateSql.Append("SUBBASIN INTEGER, STATION TEXT, WLATITUDE REAL, WLONGITUDE REAL, WELEV REAL, RAIN_YRS REAL, ")
                Append12DBColumnsReal(lCreateSql, "TMPMX")
                Append12DBColumnsReal(lCreateSql, "TMPMN")
                Append12DBColumnsReal(lCreateSql, "TMPSTDMX")
                Append12DBColumnsReal(lCreateSql, "TMPSTDMN")
                Append12DBColumnsReal(lCreateSql, "PCPMM")
                Append12DBColumnsReal(lCreateSql, "PCPSTD")
                Append12DBColumnsReal(lCreateSql, "PCPSKW")
                Append12DBColumnsReal(lCreateSql, "PR_W1_")
                Append12DBColumnsReal(lCreateSql, "PR_W2_")
                Append12DBColumnsReal(lCreateSql, "PCPD")
                Append12DBColumnsReal(lCreateSql, "RAINHHMX")
                Append12DBColumnsReal(lCreateSql, "SOLARAV")
                Append12DBColumnsReal(lCreateSql, "DEWPT")
                Append12DBColumnsReal(lCreateSql, "WNDAV")
                lCreateSql.Length -= 2
                lCreateSql.Append(");")

                Dim lResult As Boolean = ExecuteNonQuery(lCreateSql.ToString(), lConnection)
                If lCloseWhenDone Then
                    lConnection.Close()
                End If
                Return lResult
            Catch lEx As Exception
                Debug.Print(lEx.Message)
                Return False
            End Try
        End Function

        Private Sub Append12DBColumnsReal(ByVal aSql As Text.StringBuilder, ByVal aSection As String)
            For i As Integer = 1 To 12
                aSql.Append(aSection & i & " REAL, ")
            Next
        End Sub

        Public Function Table() As DataTable
            pSwatInput.Status("Reading " & pTableName & " from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM " & pTableName & ";")
        End Function

        Public Sub Add(ByVal aItem As clsWgnItem)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Save(Optional ByVal aTable As DataTable = Nothing)
            If aTable Is Nothing Then aTable = Table()
            pSwatInput.Status("Writing " & pTableName & " text ...")

            For Each lRow As DataRow In aTable.Rows
                Dim lSubBasin As String = lRow.Item("SUBBASIN")

                Dim lSB As New Text.StringBuilder
                lSB.AppendLine(" .Wgn file Subbasin: " & lSubBasin _
                             & " STATION NAME:" & lRow.Item(("STATION")) & " " _
                             & HeaderString())
                lSB.AppendLine("  LATITUDE =" & Format(lRow.Item(("WLATITUDE")), "0.00").PadLeft(7) _
                             & " LONGITUDE =" & Format(lRow.Item(("WLONGITUDE")), "0.00").PadLeft(7))
                lSB.AppendLine("  ELEV [m] =" & Format(lRow.Item(("WELEV")), "0.00").PadLeft(7))
                lSB.AppendLine("  RAIN_YRS =" & Format(lRow.Item(("RAIN_YRS")), "0.00").PadLeft(7))
                Append12TextColumns(lSB, lRow, "TMPMX")
                Append12TextColumns(lSB, lRow, "TMPMN")
                Append12TextColumns(lSB, lRow, "TMPSTDMX")
                Append12TextColumns(lSB, lRow, "TMPSTDMN")
                Append12TextColumns(lSB, lRow, "PCPMM")
                Append12TextColumns(lSB, lRow, "PCPSTD")
                Append12TextColumns(lSB, lRow, "PCPSKW")
                Append12TextColumns(lSB, lRow, "PR_W1_")
                Append12TextColumns(lSB, lRow, "PR_W2_")
                Append12TextColumns(lSB, lRow, "PCPD")
                Append12TextColumns(lSB, lRow, "RAINHHMX")
                Append12TextColumns(lSB, lRow, "SOLARAV")
                Append12TextColumns(lSB, lRow, "DEWPT")
                Append12TextColumns(lSB, lRow, "WNDAV")

                IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\" & StringFname(lSubBasin, pTableName), lSB.ToString)
            Next
        End Sub

        Private Sub Append12TextColumns(ByVal aSB As Text.StringBuilder, ByVal aRow As DataRow, ByVal aSection As String)
            For i As Integer = 1 To 12
                aSB.Append(Format(aRow.Item(aSection & i), "0.00").PadLeft(6))
            Next
            aSB.AppendLine()
        End Sub
    End Class
End Class
