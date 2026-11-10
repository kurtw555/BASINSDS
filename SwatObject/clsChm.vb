Imports System.Data
Imports Microsoft.Data.Sqlite

Partial Class SwatInput
    Private pChm As clsChm = New clsChm(Me)
    ReadOnly Property Chm() As clsChm
        Get
            Return pChm
        End Get
    End Property

    Public Class clsChmItem
        Public SUBBASIN As Double
        Public HRU As Double
        Public LANDUSE As String
        Public SOIL As String
        Public SLOPE_CD As String
        Public SOL_NO3(9) As Double
        Public SOL_ORGN(9) As Double
        Public SOL_LABP(9) As Double
        Public SOL_ORGP(9) As Double
        Public PESTNAME(9) As String
        Public PLTPST(9) As Double
        Public SOLPST(9) As Double
        Public PSTENR(9) As Double

        Public Sub New(ByVal aSUBBASIN As Double,
                       ByVal aHRU As Double,
                       ByVal aLANDUSE As String,
                       ByVal aSOIL As String,
                       ByVal aSLOPE_CD As String)
            SUBBASIN = aSUBBASIN
            HRU = aHRU
            LANDUSE = aLANDUSE
            SOIL = aSOIL
            SLOPE_CD = aSLOPE_CD
        End Sub

        Public Function AddSQL() As String
            Dim lSQL As String = "INSERT INTO chm ( SUBBASIN, HRU, LANDUSE, SOIL, SLOPE_CD, " _
                               & GroupOfStrings("SOL_NO3#", 10, ", ") & ", " _
                               & GroupOfStrings("SOL_ORGN#", 10, ", ") & ", " _
                               & GroupOfStrings("SOL_LABP#", 10, ", ") & ", " _
                               & GroupOfStrings("SOL_ORGP#", 10, ", ") & ", " _
                               & GroupOfStrings("PESTNAME#", 10, ", ") & ", " _
                               & GroupOfStrings("PLTPST#", 10, ", ") & ", " _
                               & GroupOfStrings("SOLPST#", 10, ", ") & ", " _
                               & GroupOfStrings("PSTENR#", 10, ", ") _
                               & " ) Values (" _
                               & "'" & SUBBASIN & "', '" _
                               & HRU & "', '" _
                               & LANDUSE & "', '" _
                               & SOIL & "', '" _
                               & SLOPE_CD & "', '" _
                               & ArrayToString(SOL_NO3, "', '") & "', '" _
                               & ArrayToString(SOL_ORGN, "', '") & "', '" _
                               & ArrayToString(SOL_LABP, "', '") & "', '" _
                               & ArrayToString(SOL_ORGP, "', '") & "', '" _
                               & ArrayToString(PESTNAME, "', '") & "', '" _
                               & ArrayToString(PLTPST, "', '") & "', '" _
                               & ArrayToString(SOLPST, "', '") & "', '" _
                               & ArrayToString(PSTENR, "', '") & "' )"
            Return lSQL
        End Function

        Public Shared ReadOnly Property Units() As Dictionary(Of String, String)

            Get

                Dim unitsDictionary As New Dictionary(Of String, String)

                unitsDictionary.Add("SUBBASIN", "")
                unitsDictionary.Add("HRU", "")
                unitsDictionary.Add("LANDUSE", "")
                unitsDictionary.Add("SOIL", "")
                unitsDictionary.Add("SLOPE_CD", "")
                unitsDictionary.Add("SOL_NO3", "mg/kg")
                unitsDictionary.Add("SOL_ORGN", "mg/kg")
                unitsDictionary.Add("SOL_LABP", "mg/kg")
                unitsDictionary.Add("SOL_ORGP", "mg/kg")
                unitsDictionary.Add("PESTNAME", "")
                unitsDictionary.Add("PLTPST", "kg/ha")
                unitsDictionary.Add("SOLPST", "kg/ha")
                unitsDictionary.Add("PSTENR", "kg/ha")

                Return unitsDictionary
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Chm Input Section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsChm
        Private pSwatInput As SwatInput
        Private pTableName As String = "chm"

        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            'based on mwSWATPlugIn.DBLayer.createChmTable
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
                lCreateSql.Append("SUBBASIN REAL, HRU REAL, LANDUSE TEXT, SOIL TEXT, SLOPE_CD TEXT, ")
                AppendRepeatedColumns(lCreateSql, "SOL_NO3", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "SOL_ORGN", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "SOL_LABP", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "SOL_ORGP", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "PESTNAME", 10, "TEXT")
                AppendRepeatedColumns(lCreateSql, "PLTPST", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "SOLPST", 10, "REAL")
                AppendRepeatedColumns(lCreateSql, "PSTENR", 10, "REAL")
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

        Private Sub AppendRepeatedColumns(ByVal aSql As Text.StringBuilder, ByVal aPrefix As String, ByVal aCount As Integer, ByVal aType As String)
            For i As Integer = 1 To aCount
                aSql.Append(aPrefix & i & " " & aType & ", ")
            Next
        End Sub

        Public Function Table() As DataTable
            pSwatInput.Status("Reading " & pTableName & " from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM " & pTableName & " ORDER BY SUBBASIN, HRU;")
        End Function

        Public Sub Add(ByVal aItem As clsChmItem)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Save(Optional ByVal aTable As DataTable = Nothing, Optional ByVal aPestTable As DataTable = Nothing)
            If aTable Is Nothing Then aTable = Table()
            If aPestTable Is Nothing Then aPestTable = QueryDB("SELECT * FROM pest;", pSwatInput.CnSwatParm)

            pSwatInput.Status("Writing " & pTableName & " text ...")

            'fill pest dictionary
            Dim dictPest As New Collections.Generic.Dictionary(Of String, String)
            For Each lRow As DataRow In aPestTable.Rows
                dictPest.Add(lRow.Item("PESTNAME"), lRow.Item("IPNUM"))
            Next

            For Each lRow As DataRow In aTable.Rows
                Dim lSubNum As Integer = lRow.Item(1)
                Dim sSubNum As String = lSubNum
                Dim lHruNum As Integer = lRow.Item(2)
                Dim sHruNum As String = lHruNum

                Dim lTextFilename As String = StringFnameHRUs(sSubNum, lHruNum) & "." & pTableName

                Dim lSB As New Text.StringBuilder
                lSB.AppendLine(" .chm file Subbasin:" & sSubNum & " HRU:" & sHruNum & " Luse:" & lRow.Item(3) _
                             & " Soil: " & lRow.Item(4) & " Slope: " & lRow.Item(5) _
                             & " " & HeaderString())
                lSB.AppendLine("Soil Nutrient Data")
                lSB.AppendLine(" Soil Layer               :           1           2           3           4           5           6           7           8           9          10")

                lSB.Append(" Soil NO3 [mg/kg]         :") : Append10TextColumns(lSB, lRow, "SOL_NO3")  '4th line
                lSB.Append(" Soil organic N [mg/kg]   :") : Append10TextColumns(lSB, lRow, "SOL_ORGN") '5th Line
                lSB.Append(" Soil labile P [mg/kg]    :") : Append10TextColumns(lSB, lRow, "SOL_LABP") '6th Line
                lSB.Append(" Soil organic P [mg/kg]   :") : Append10TextColumns(lSB, lRow, "SOL_ORGP") '7th Line

                lSB.AppendLine()
                lSB.AppendLine("Soil Pesticide Data")
                lSB.AppendLine(" Pesticide  Pst on plant    Pst in 1st soil layer Pst enrichment")
                lSB.AppendLine("   #           [kg/ha]           [kg/ha]           [kg/ha]")
                For i As Integer = 1 To 10
                    Dim lPestName As Object = lRow.Item("PESTNAME" & i)
                    If lPestName Is System.DBNull.Value OrElse lPestName.ToString.Length = 0 Then
                        lSB.AppendLine("   0" & "0.00".PadLeft(18) & "0.00".PadLeft(18) & "0.00".PadLeft(18))
                    Else
                        lSB.AppendLine(dictPest.Item(lRow.Item("PESTNAME" & i)).PadLeft(4) _
                                     & Format(lRow.Item("PLTPST" & i), "0.00").PadLeft(18) _
                                     & Format(lRow.Item("SOLPST" & i), "0.00").PadLeft(18) _
                                     & Format(lRow.Item("PSTENR" & i), "0.00").PadLeft(18))
                    End If
                Next

                IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\" & lTextFilename, lSB.ToString)
                'ReplaceNonAscii(lTextFilename)
            Next
        End Sub

        Private Sub Append10TextColumns(ByVal aSB As Text.StringBuilder, ByVal aRow As DataRow, ByVal aSection As String)
            For i As Integer = 1 To 10
                aSB.Append(Format(aRow.Item(aSection & i), "0.00").PadLeft(12))
            Next
            aSB.AppendLine()
        End Sub

    End Class
End Class
