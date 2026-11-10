'Imports System.Data
Imports Microsoft.Data.Sqlite
Imports System.Data
Partial Class SwatInput
    Private pPnd As clsPnd = New clsPnd(Me)
    ReadOnly Property Pnd() As clsPnd
        Get
            Return pPnd
        End Get
    End Property

    Public Class clsPndItem
        Public SUBBASIN As Double
        Public PND_FR As Single
        Public PND_PSA As Double
        Public PND_PVOL As Double
        Public PND_ESA As Double
        Public PND_EVOL As Double
        Public PND_VOL As Double
        Public PND_SED As Double
        Public PND_NSED As Double
        Public PND_K As Single
        Public IFLOD1 As Long
        Public IFLOD2 As Long
        Public NDTARG As Long
        Public PSETLP1 As Single
        Public PSETLP2 As Single
        Public NSETLP1 As Single
        Public NSETLP2 As Single
        Public CHLAP As Single
        Public SECCIP As Single
        Public PND_NO3 As Single
        Public PND_SOLP As Single
        Public PND_ORGN As Single
        Public PND_ORGP As Single
        Public IPND1 As Long
        Public IPND2 As Long
        Public WET_FR As Single
        Public WET_NSA As Double
        Public WET_NVOL As Double
        Public WET_MXSA As Double
        Public WET_MXVOL As Double
        Public WET_VOL As Double
        Public WET_SED As Double
        Public WET_NSED As Double
        Public WET_K As Single
        Public PSETLW1 As Single
        Public PSETLW2 As Single
        Public NSETLW1 As Single
        Public NSETLW2 As Single
        Public CHLAW As Single
        Public SECCIW As Single
        Public WET_NO3 As Single
        Public WET_SOLP As Single
        Public WET_ORGN As Single
        Public WET_ORGP As Single

        Public Sub New(ByVal aSUBBASIN As Double)
            SUBBASIN = aSUBBASIN
        End Sub

        Sub New(ByVal aRow As DataRow)
            PopulateObject(aRow, Me)
        End Sub

        Public Function AddSQL() As String
            Return "INSERT INTO pnd ( SUBBASIN, PND_FR, PND_PSA, PND_PVOL, PND_ESA, PND_EVOL, PND_VOL, PND_SED, PND_NSED, PND_K, IFLOD1, IFLOD2, NDTARG, PSETLP1, PSETLP2, NSETLP1, NSETLP2, CHLAP, SECCIP, PND_NO3, PND_SOLP, PND_ORGN, PND_ORGP, IPND1, IPND2, WET_FR, WET_NSA, WET_NVOL, WET_MXSA, WET_MXVOL, WET_VOL, WET_SED, WET_NSED, WET_K, PSETLW1, PSETLW2, NSETLW1, NSETLW2, CHLAW, SECCIW, WET_NO3, WET_SOLP, WET_ORGN, WET_ORGP  ) " _
                 & "Values ('" & SUBBASIN & "', '" & PND_FR & "', '" & PND_PSA & "', '" & PND_PVOL & "', '" & PND_ESA & "', '" & PND_EVOL & "', '" & PND_VOL & "', '" & PND_SED & "', '" & PND_NSED & "', '" & PND_K & "', '" & IFLOD1 & "', '" & IFLOD2 & "', '" & NDTARG & "', '" & PSETLP1 & "', '" & PSETLP2 & "', '" & NSETLP1 & "', '" & NSETLP2 & "', '" & CHLAP & "', '" & SECCIP & "', '" & PND_NO3 & "', '" & PND_SOLP & "', '" & PND_ORGN & "', '" & PND_ORGP & "', '" & IPND1 & "', '" & IPND2 & "', '" & WET_FR & "', '" & WET_NSA & "', '" & WET_NVOL & "', '" & WET_MXSA & "', '" & WET_MXVOL & "', '" & WET_VOL & "', '" & WET_SED & "', '" & WET_NSED & "', '" & WET_K & "', '" & PSETLW1 & "', '" & PSETLW2 & "', '" & NSETLW1 & "', '" & NSETLW2 & "', '" & CHLAW & "', '" & SECCIW & "', '" & WET_NO3 & "', '" & WET_SOLP & "', '" & WET_ORGN & "', '" & WET_ORGP & "'   )"
        End Function
    End Class

    ''' <summary>
    ''' Gw Input Section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsPnd
        Private pSwatInput As SwatInput
        Private pTableName As String = "pnd"

        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            'based on mwSWATPlugIn.DBLayer.createPndTable
            Try
                DropTable(pTableName, pSwatInput.CnSwatInput)

                Dim lConnection As SqliteConnection = pSwatInput.CnSwatInput
                Dim lCloseWhenDone As Boolean = False
                If lConnection.State <> ConnectionState.Open Then
                    lConnection.Open()
                    lCloseWhenDone = True
                End If

                Dim lCreateSql As String =
                    "CREATE TABLE " & pTableName & " (" &
                    "OID INTEGER PRIMARY KEY AUTOINCREMENT, " &
                    "SUBBASIN REAL, " &
                    "PND_FR REAL, " &
                    "PND_PSA REAL, " &
                    "PND_PVOL REAL, " &
                    "PND_ESA REAL, " &
                    "PND_EVOL REAL, " &
                    "PND_VOL REAL, " &
                    "PND_SED REAL, " &
                    "PND_NSED REAL, " &
                    "PND_K REAL, " &
                    "IFLOD1 INTEGER, " &
                    "IFLOD2 INTEGER, " &
                    "NDTARG INTEGER, " &
                    "PSETLP1 REAL, " &
                    "PSETLP2 REAL, " &
                    "NSETLP1 REAL, " &
                    "NSETLP2 REAL, " &
                    "CHLAP REAL, " &
                    "SECCIP REAL, " &
                    "PND_NO3 REAL, " &
                    "PND_SOLP REAL, " &
                    "PND_ORGN REAL, " &
                    "PND_ORGP REAL, " &
                    "IPND1 INTEGER, " &
                    "IPND2 INTEGER, " &
                    "WET_FR REAL, " &
                    "WET_NSA REAL, " &
                    "WET_NVOL REAL, " &
                    "WET_MXSA REAL, " &
                    "WET_MXVOL REAL, " &
                    "WET_VOL REAL, " &
                    "WET_SED REAL, " &
                    "WET_NSED REAL, " &
                    "WET_K REAL, " &
                    "PSETLW1 REAL, " &
                    "PSETLW2 REAL, " &
                    "NSETLW1 REAL, " &
                    "NSETLW2 REAL, " &
                    "CHLAW REAL, " &
                    "SECCIW REAL, " &
                    "WET_NO3 REAL, " &
                    "WET_SOLP REAL, " &
                    "WET_ORGN REAL, " &
                    "WET_ORGP REAL" &
                    ");"

                Dim lResult As Boolean = ExecuteNonQuery(lCreateSql, lConnection)
                If lCloseWhenDone Then
                    lConnection.Close()
                End If
                Return lResult
            Catch lEx As Exception
                Debug.Print(lEx.Message)
                Return False
            End Try
        End Function

        Public Function Table() As DataTable
            pSwatInput.Status("Reading " & pTableName & " from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM " & pTableName & ";")
        End Function

        Public Sub Add(ByVal aItem As clsPndItem)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Save(Optional ByVal aTable As DataTable = Nothing)
            If aTable Is Nothing Then aTable = Table()

            pSwatInput.Status("Writing " & pTableName & " text ...")

            For Each lRow As DataRow In aTable.Rows
                Dim lSubBasin As String = lRow.Item(1)
                Dim lHruNum As String = lRow.Item(2)
                Dim lTextFilename As String = StringFnameHRUs(lSubBasin, lHruNum) & "." & pTableName

                Dim lSB As New Text.StringBuilder

                lSB.AppendLine(" .Pnd file Subbasin: " & lSubBasin & " " & HeaderString())
                lSB.AppendLine("Pond inputs:")
                lSB.AppendLine(Format(lRow.Item(2), "0.000").PadLeft(16) & "    | PND_FR : Fraction of subbasin area that drains into ponds. The value for PND_FR should be between 0.0 and 1.0. If PND_FR = 1.0, the pond is at the outlet of the subbasin on the main channel")
                lSB.AppendLine(Format(lRow.Item(3), "0.000").PadLeft(16) & "    | PND_PSA: Surface area of ponds when filled to principal spillway [ha]")
                lSB.AppendLine(Format(lRow.Item(4), "0.000").PadLeft(16) & "    | PND_PVOL: Volume of water stored in ponds when filled to the principal spillway [104 m3]")
                lSB.AppendLine(Format(lRow.Item(5), "0.000").PadLeft(16) & "    | PND_ESA: Surface area of ponds when filled to emergency spillway [ha]")
                lSB.AppendLine(Format(lRow.Item(6), "0.000").PadLeft(16) & "    | PND_EVOL: Volume of water stored in ponds when filled to the emergency spillway [104 m3]")
                lSB.AppendLine(Format(lRow.Item(7), "0.000").PadLeft(16) & "    | PND_VOL: Initial volume of water in ponds [104 m3]")
                lSB.AppendLine(Format(lRow.Item(8), "0.000").PadLeft(16) & "    | PND_SED: Initial sediment concentration in pond water [mg/l]")
                lSB.AppendLine(Format(lRow.Item(9), "0.000").PadLeft(16) & "    | PND_NSED: Normal sediment concentration in pond water [mg/l]")
                lSB.AppendLine(Format(lRow.Item(10), "0.000").PadLeft(16) & "    | PND_K: Hydraulic conductivity through bottom of ponds [mm/hr].")
                lSB.AppendLine(Format(lRow.Item(11), "0").PadLeft(16) & "    | IFLOD1: Beginning month of non-flood season")
                lSB.AppendLine(Format(lRow.Item(12), "0").PadLeft(16) & "    | IFLOD2: Ending month of non-flood season")
                lSB.AppendLine(Format(lRow.Item(13), "0.000").PadLeft(16) & "    | NDTARG: Number of days needed to reach target storage from current pond storage")
                lSB.AppendLine(Format(lRow.Item(14), "0.000").PadLeft(16) & "    | PSETLP1: Phosphorus settling rate in pond for months IPND1 through IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(15), "0.000").PadLeft(16) & "    | PSETLP2: Phosphorus settling rate in pond for months other than IPND1-IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(16), "0.000").PadLeft(16) & "    | NSETLP1: Initial dissolved oxygen concentration in the reach [mg O2/l]")
                lSB.AppendLine(Format(lRow.Item(17), "0.000").PadLeft(16) & "    | NSETLP2: Initial dissolved oxygen concentration in the reach [mg O2/l]")
                lSB.AppendLine(Format(lRow.Item(18), "0.000").PadLeft(16) & "    | CHLAP: Chlorophyll a production coefficient for ponds [ ] ")
                lSB.AppendLine(Format(lRow.Item(19), "0.000").PadLeft(16) & "    | SECCIP: Water clarity coefficient for ponds [m]")
                lSB.AppendLine(Format(lRow.Item(20), "0.000").PadLeft(16) & "    | PND_NO3: Initial concentration of NO3-N in pond [mg N/l]")
                lSB.AppendLine(Format(lRow.Item(21), "0.000").PadLeft(16) & "    | PND_SOLP: Initial concentration of soluble P in pond [mg P/L]")
                lSB.AppendLine(Format(lRow.Item(22), "0.000").PadLeft(16) & "    | PND_ORGN: Initial concentration of organic N in pond [mg N/l]")
                lSB.AppendLine(Format(lRow.Item(23), "0.000").PadLeft(16) & "    | PND_ORGP: Initial concentration of organic P in pond [mg P/l]")
                lSB.AppendLine("Inputs used in both ponds and wetlands:")
                lSB.AppendLine(Format(lRow.Item(24), "0").PadLeft(16) & "    | IPND1: Beginning month of mid-year nutrient settling ""season""")
                lSB.AppendLine(Format(lRow.Item(25), "0").PadLeft(16) & "    | IPND2: Ending month of mid-year nutrient settling ""season""")
                lSB.AppendLine("Wetland inputs:")
                lSB.AppendLine(Format(lRow.Item(26), "0.000").PadLeft(16) & "    | WET_FR : Fraction of subbasin area that drains into wetlands")
                lSB.AppendLine(Format(lRow.Item(27), "0.000").PadLeft(16) & "    | WET_NSA: Surface area of wetlands at normal water level [ha]")
                lSB.AppendLine(Format(lRow.Item(28), "0.000").PadLeft(16) & "    | WET_NVOL: Volume of water stored in wetlands when filled to normal water level [104 m3] ")
                lSB.AppendLine(Format(lRow.Item(29), "0.000").PadLeft(16) & "    | WET_MXSA: Surface area of wetlands at maximum water level [ha]")
                lSB.AppendLine(Format(lRow.Item(30), "0.000").PadLeft(16) & "    | WET_MXVOL: Volume of water stored in wetlands when filled to maximum water level [104 m3]")
                lSB.AppendLine(Format(lRow.Item(31), "0.000").PadLeft(16) & "    | WET_VOL: Initial volume of water in wetlands [104 m3]")
                lSB.AppendLine(Format(lRow.Item(32), "0.000").PadLeft(16) & "    | WET_SED: Initial sediment concentration in wetland water [mg/l]")
                lSB.AppendLine(Format(lRow.Item(33), "0.000").PadLeft(16) & "    | WET_NSED: Normal sediment concentration in wetland water [mg/l]")
                lSB.AppendLine(Format(lRow.Item(34), "0.000").PadLeft(16) & "    | WET_K: Hydraulic conductivity of bottom of wetlands [mm/hr]")
                lSB.AppendLine(Format(lRow.Item(35), "0.000").PadLeft(16) & "    | PSETLW1: Phosphorus settling rate in wetland for months IPND1 through IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(36), "0.000").PadLeft(16) & "    | PSETLW2: Phosphorus settling rate in wetlands for months other than IPND1-IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(37), "0.000").PadLeft(16) & "    | NSETLW1: Nitrogen settling rate in wetlands for months IPND1 through IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(38), "0.000").PadLeft(16) & "    | NSETLW2: Nitrogen settling rate in wetlands for months other than IPND1-IPND2 [m/year]")
                lSB.AppendLine(Format(lRow.Item(39), "0.000").PadLeft(16) & "    | CHLAW: Chlorophyll a production coefficient for wetlands [ ]")
                lSB.AppendLine(Format(lRow.Item(40), "0.000").PadLeft(16) & "    | SECCIW: Water clarity coefficient for wetlands [m]")
                lSB.AppendLine(Format(lRow.Item(41), "0.000").PadLeft(16) & "    | WET_NO3: Initial concentration of NO3-N in wetland [mg N/l]")
                lSB.AppendLine(Format(lRow.Item(42), "0.000").PadLeft(16) & "    | WET_SOLP: Initial concentration of soluble P in wetland [mg P/l]")
                lSB.AppendLine(Format(lRow.Item(43), "0.000").PadLeft(16) & "    | WET_ORGN: Initial concentration of organic N in wetland [mg N/l]")
                lSB.AppendLine(Format(lRow.Item(44), "0.000").PadLeft(16) & "    | WET_ORGP: Initial concentration of organic P in wetland [mg P/l]")

                IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\" & lTextFilename, lSB.ToString)
                'ReplaceNonAscii(lTextFilename)
            Next
        End Sub
    End Class
End Class
