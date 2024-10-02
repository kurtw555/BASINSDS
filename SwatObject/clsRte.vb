Imports System.Data
Imports System.Data.SQLite
Partial Class SwatInput
    Private pRte As clsRte = New clsRte(Me)
    ReadOnly Property Rte() As clsRte
        Get
            Return pRte
        End Get
    End Property

    Public Class clsRteItem
        Public SUBBASIN As Long
        Public CH_W2 As Single
        Public CH_D As Single
        Public CH_S2 As Single
        Public CH_L2 As Single
        Public CH_N2 As Single
        Public CH_K2 As Single
        Public CH_EROD As Single
        Public CH_COV As Single
        Public CH_WDR As Single
        Public ALPHA_BNK As Single

        Public Sub New(ByVal aSUBBASIN As Long)
            SUBBASIN = aSUBBASIN
        End Sub

        Sub New(ByVal aRow As DataRow)
            PopulateObject(aRow, Me)
        End Sub

        Public Function AddSQL() As String
            Return "INSERT INTO rte ( SUBBASIN , CH_W2 , CH_D , CH_S2 , CH_L2 , CH_N2 , CH_K2 , CH_EROD , CH_COV , CH_WDR , ALPHA_BNK   ) " _
                 & "Values ('" & SUBBASIN & "', '" & CH_W2 & "', '" & CH_D & "', '" & CH_S2 & "', '" _
                 & CH_L2 & "', '" & CH_N2 & "', '" & CH_K2 & "', '" & CH_EROD & "', '" & CH_COV & "', '" _
                 & CH_WDR & "', '" & ALPHA_BNK & "'  );"
        End Function
    End Class

    ''' <summary>
    ''' RTE Input Section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsRte
        Private pSwatInput As SwatInput
        Private pTableName As String = "rte"

        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            'based on mwSWATPlugIn:DBLayer:createRTE
            Try
                DropTable(pTableName, pSwatInput.CnSwatInput)

                'Open the connection
                Dim lConnection As SqliteConnection = pSwatInput.OpenSqliteConnection()

                'Open the Catalog
                Dim lCatalog As New ADOX.Catalog
                lCatalog.ActiveConnection = lConnection

                'Create the table
                Dim lTable As New ADOX.Table
                lTable.Name = pTableName

                Dim lKeyColumn As New ADOX.Column
                With lKeyColumn
                    .Name = "OID"
                    .Type = ADOX.DataTypeEnum.adInteger
                    .ParentCatalog = lCatalog
                    .Properties("AutoIncrement").Value = True
                End With

                With lTable.Columns
                    .Append(lKeyColumn)
                    .Append("SUBBASIN", ADOX.DataTypeEnum.adInteger, 4)
                    .Append("CH_W2", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_D", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_S2", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_L2", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_N2", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_K2", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_EROD", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_COV", ADOX.DataTypeEnum.adSingle)
                    .Append("CH_WDR", ADOX.DataTypeEnum.adSingle)
                    .Append("ALPHA_BNK", ADOX.DataTypeEnum.adSingle)
                End With

                lTable.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, lKeyColumn.Name)
                lCatalog.Tables.Append(lTable)
                lTable = Nothing
                lCatalog = Nothing
                lConnection.Close()
                lConnection = Nothing
                Return True
            Catch lEx As ApplicationException
                Debug.Print(lEx.Message)
                Return False
            End Try
        End Function

        Public Function Table() As DataTable
            pSwatInput.Status("Reading " & pTableName & " from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM " & pTableName & ";")
        End Function

        Public Sub Add(ByVal aItem As clsRteItem)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Save(Optional ByVal aTable As DataTable = Nothing)
            If aTable Is Nothing Then aTable = Table()
            pSwatInput.Status("Writing " & pTableName & " text ...")

            For Each lRow As DataRow In aTable.Rows

                Dim lSubBasin As String = lRow.Item("SUBBASIN")

                Dim lSB As New System.Text.StringBuilder
                '1st line
                lSB.AppendLine(" .rte file Subbasin: " & lSubBasin & " " & HeaderString())
                '---2. CHW2
                lSB.AppendLine(Format(lRow.Item(2), "0.000").PadLeft(14) & "    | CHW2 : Main channel width [m]")
                '---3. CHD
                lSB.AppendLine(Format(lRow.Item(3), "0.000").PadLeft(14) & "    | CHD : Main channel depth [m]")
                '---4. CH_S2
                lSB.AppendLine(Format(lRow.Item(4), "0.000").PadLeft(14) & "    | CH_S2 : Main channel slope [m/m]")
                '---5. CH_L2
                lSB.AppendLine(Format(lRow.Item(5), "0.000").PadLeft(14) & "    | CH_L2 : Main channel length [km]")
                '---6. CH_N2
                lSB.AppendLine(Format(lRow.Item(6), "0.000").PadLeft(14) & "    | CH_N2 : Manning's nvalue for main channel")
                '---7. CH_K2
                lSB.AppendLine(Format(lRow.Item(7), "0.000").PadLeft(14) & "    | CH_K2 : Effective hydraulic conductivity [mm/hr]")
                '---8. CH_EROD
                lSB.AppendLine(Format(lRow.Item(8), "0.000").PadLeft(14) & "    | CH_EROD: Channel erodibility factor")
                '---9. CH_COV
                lSB.AppendLine(Format(lRow.Item(9), "0.000").PadLeft(14) & "    | CH_COV : Channel cover factor")
                '---10. CH_WDR
                lSB.AppendLine(Format(lRow.Item(10), "0.000").PadLeft(14) & "    | CH_WDR : Channel width:depth ratio [m/m]")
                '---11. CH_WDR
                lSB.AppendLine(Format(lRow.Item(11), "0.000").PadLeft(14) & "    | ALPHA_BNK : Baseflow alpha factor for bank storage [days]")

                IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\" & StringFname(lSubBasin, pTableName), lSB.ToString)
            Next

        End Sub
    End Class
End Class
