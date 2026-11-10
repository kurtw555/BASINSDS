Imports System.Data
Imports Microsoft.Data.SQLite

Partial Class SwatInput
    Private pMgt As clsMgt = New clsMgt(Me)
    ReadOnly Property Mgt() As clsMgt
        Get
            Return pMgt
        End Get
    End Property
    Public Function calHeatUnits(ByVal aCropName As String, ByVal aX As Single, ByVal aY As Single, ByVal aDataDirectory As String) As Single
        Return calHeatUnits(aCropName, aX, aY, aDataDirectory)
    End Function

    Public Class clsMgtItem1
        Public SUBBASIN As Double
        Public HRU As Double
        Public LANDUSE As String
        Public SOIL As String
        Public SLOPE_CD As String
        Public IGRO As Integer
        Public PLANT_ID As Integer
        Public LAI_INIT As Single
        Public BIO_INIT As Single
        Public PHU_PLT As Single
        Public BIOMIX As Single
        Public CN2 As Single
        Public USLE_P As Single
        Public BIO_MIN As Single
        Public FILTERW As Single
        Public IURBAN As Long
        Public URBLU As Long
        Public IRRSC As Long
        Public IRRNO As Long
        Public FLOWMIN As Single
        Public DIVMAX As Single
        Public FLOWFR As Single
        Public DDRAIN As Single
        Public TDRAIN As Single
        Public GDRAIN As Single
        Public NROT As Integer
        Public HUSC As Integer
        Public ISCROP As Integer

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

        Sub New(ByVal aRow As DataRow)
            PopulateObject(aRow, Me)
        End Sub

        Public Function AddSQL() As String
            Return "INSERT INTO mgt1 ( SUBBASIN , HRU , LANDUSE , SOIL , SLOPE_CD , IGRO , PLANT_ID , LAI_INIT , BIO_INIT , PHU_PLT , BIOMIX , CN2 , USLE_P , BIO_MIN , FILTERW , IURBAN , URBLU , IRRSC , IRRNO , FLOWMIN , DIVMAX , FLOWFR , DDRAIN , TDRAIN , GDRAIN , NROT , HUSC , ISCROP  ) " _
                 & "Values ('" & SUBBASIN & "', '" & HRU & "', '" & LANDUSE & "', '" & SOIL & "', '" & SLOPE_CD & "', '" & IGRO & "', '" & PLANT_ID & "', '" & LAI_INIT & "', '" & BIO_INIT & "', '" & PHU_PLT & "', '" & BIOMIX & "', '" & CN2 & "', '" & USLE_P & "', '" & BIO_MIN & "', '" & FILTERW & "', '" & IURBAN & "', '" & URBLU & "', '" & IRRSC & "', '" & IRRNO & "', '" & FLOWMIN & "', '" & DIVMAX & "', '" & FLOWFR & "', '" & DDRAIN & "', '" & TDRAIN & "', '" & GDRAIN & "', '" & NROT & "', '" & HUSC & "', '" & ISCROP & "'  )"
        End Function

        Public Shared ReadOnly Property Units() As Dictionary(Of String, String)

            Get
                Dim unitsDictionary As New Dictionary(Of String, String)

                unitsDictionary.Add("SUBBASIN", "")
                unitsDictionary.Add("HRU", "")
                unitsDictionary.Add("LANDUSE", "")
                unitsDictionary.Add("SOIL", "")
                unitsDictionary.Add("SLOPE_CD", "")
                unitsDictionary.Add("IGRO", "N/A")
                unitsDictionary.Add("PLANT_ID", "N/A")
                unitsDictionary.Add("LAI_INIT", "N/A")
                unitsDictionary.Add("BIO_INIT", "kg/ha")
                unitsDictionary.Add("PHU_PLT", "none")
                unitsDictionary.Add("BIOMIX", "fraction")
                unitsDictionary.Add("CN2", "none")
                unitsDictionary.Add("USLE_P", "none")
                unitsDictionary.Add("BIO_MIN", "kg/ha")
                unitsDictionary.Add("FILTERW", "m")
                unitsDictionary.Add("IURBAN", "N/A")
                unitsDictionary.Add("URBLU", "N/A")
                unitsDictionary.Add("IRRSC", "N/A")
                unitsDictionary.Add("IRRNO", "N/A")
                unitsDictionary.Add("FLOWMIN", "m3/s")
                unitsDictionary.Add("DIVMAX", "number")
                unitsDictionary.Add("FLOWFR", "fraction")
                unitsDictionary.Add("DDRAIN", "mm")
                unitsDictionary.Add("TDRAIN", "hr")
                unitsDictionary.Add("GDRAIN", "hr")
                unitsDictionary.Add("NROT", "N/A")
                unitsDictionary.Add("HUSC", "")
                unitsDictionary.Add("ISCROP", "")

                Return unitsDictionary

            End Get
        End Property

    End Class

    Public Class clsMgtItem2
        Public SUBBASIN As Double
        Public HRU As Double
        Public LANDUSE As String
        Public SOIL As String
        Public SLOPE_CD As String
        Public CROP As String
        Public YEAR As Integer
        Public MONTH As Integer
        Public DAY As Integer
        Public HUSC As Single
        Public MGT_OP As Integer
        Public HEATUNITS As Single
        Public PLANT_ID As Integer
        Public CURYR_MAT As Integer
        Public LAI_INIT As Single
        Public BIO_INIT As Single
        Public HI_TARG As Single
        Public BIO_TARG As Single
        Public CNOP As Double
        Public IRR_AMT As Single
        Public FERT_ID As Integer
        Public FRT_KG As Single
        Public FRT_SURFACE As Single
        Public PEST_ID As Integer
        Public PST_KG As Single
        Public TILLAGE_ID As Integer
        Public HARVEFF As Single
        Public HI_OVR As Single
        Public GRZ_DAYS As Integer
        Public MANURE_ID As Integer
        Public BIO_EAT As Single
        Public BIO_TRMP As Single
        Public MANURE_KG As Single
        Public WSTRS_ID As Integer
        Public AUTO_WSTRS As Single
        Public AFERT_ID As Integer
        Public AUTO_NSTRS As Single
        Public AUTO_NAPP As Single
        Public AUTO_NYR As Single
        Public AUTO_EFF As Single
        Public AFRT_SURFACE As Single
        Public SWEEPEFF As Single
        Public FR_CURB As Single
        Public IMP_TRIG As Double
        Public FERT_DAYS As Long
        Public CFRT_ID As Long
        Public IFRT_FREQ As Long
        Public CFRT_KG As Single

        Sub New(ByVal aRow As DataRow)
            PopulateObject(aRow, Me)
        End Sub

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
            Return "INSERT INTO mgt2 ( SUBBASIN , HRU , LANDUSE , SOIL , SLOPE_CD , CROP , [YEAR] , [MONTH] , [DAY] , HUSC , MGT_OP , HEATUNITS , PLANT_ID , CURYR_MAT , LAI_INIT , BIO_INIT , HI_TARG , BIO_TARG , CNOP , IRR_AMT , FERT_ID , FRT_KG , FRT_SURFACE , PEST_ID , PST_KG , TILLAGE_ID , HARVEFF , HI_OVR , GRZ_DAYS , MANURE_ID , BIO_EAT , BIO_TRMP , MANURE_KG , WSTRS_ID , AUTO_WSTRS , AFERT_ID , AUTO_NSTRS , AUTO_NAPP , AUTO_NYR , AUTO_EFF , AFRT_SURFACE , SWEEPEFF , FR_CURB , IMP_TRIG , FERT_DAYS , CFRT_ID , IFRT_FREQ , CFRT_KG  ) " _
                 & "Values (" & SUBBASIN & "  ," & HRU & "  ,'" & LANDUSE & "', '" & SOIL & "', '" & SLOPE_CD & "', '" & CROP & "'  ," & YEAR & "  ," & MONTH & "  ," & DAY & "  ," & HUSC & "  ," & MGT_OP & "  ," & HEATUNITS & "  ," & PLANT_ID & "  ," & CURYR_MAT & "  ," & LAI_INIT & "  ," & BIO_INIT & "  ," & HI_TARG & "  ," & BIO_TARG & "  ," & CNOP & "  ," & IRR_AMT & "  ," & FERT_ID & "  ," & FRT_KG & "  ," & FRT_SURFACE & "  ," & PEST_ID & "  ," & PST_KG & "  ," & TILLAGE_ID & "  ," & HARVEFF & "  ," & HI_OVR & "  ," & GRZ_DAYS & "  ," & MANURE_ID & "  ," & BIO_EAT & "  ," & BIO_TRMP & "  ," & MANURE_KG & "  ," & WSTRS_ID & "  ," & AUTO_WSTRS & "  ," & AFERT_ID & "  ," & AUTO_NSTRS & "  ," & AUTO_NAPP & "  ," & AUTO_NYR & "  ," & AUTO_EFF & "  ," & AFRT_SURFACE & "  ," & SWEEPEFF & "  ," & FR_CURB & "  ," & IMP_TRIG & "  ," & FERT_DAYS & "  ," & CFRT_ID & "  ," & IFRT_FREQ & "  ," & CFRT_KG & ")"
        End Function

        Public Function Clone() As clsMgtItem2

            Dim mgt2 As clsMgtItem2
            mgt2 = DirectCast(Me.MemberwiseClone(), clsMgtItem2)
            Return mgt2

        End Function

        Public Shared ReadOnly Property Units() As Dictionary(Of String, String)

            Get
                Dim unitsDictionary As New Dictionary(Of String, String)

                unitsDictionary.Add("SUBBASIN", "")
                unitsDictionary.Add("HRU", "")
                unitsDictionary.Add("LANDUSE", "")
                unitsDictionary.Add("SOIL", "")
                unitsDictionary.Add("SLOPE", "")

                unitsDictionary.Add("CROP", "")
                unitsDictionary.Add("YEAR", "year")
                unitsDictionary.Add("MONTH", "")
                unitsDictionary.Add("DAY", "")

                Return unitsDictionary
            End Get

        End Property


    End Class

    ''' <summary>
    ''' Management (MGT) input section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsMgt
        Private pSwatInput As SwatInput
        Private pTable1Name As String = "mgt1"
        Private pTable2Name As String = "mgt2"

        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            Return Table1Create() And Table2Create()
        End Function

        Public Function Table1Create() As Boolean
            'based on mwSWATPlugIn:DBLayer:createHruTable
            Try
                DropTable(pTable1Name, pSwatInput.CnSwatInput)

                Dim lConnection As SqliteConnection = pSwatInput.CnSwatInput
                Dim lCloseWhenDone As Boolean = False
                If lConnection.State <> ConnectionState.Open Then
                    lConnection.Open()
                    lCloseWhenDone = True
                End If

                Dim lCreateSql As String = _
                    "CREATE TABLE " & pTable1Name & " (" &
                    "OID INTEGER PRIMARY KEY AUTOINCREMENT, " &
                    "SUBBASIN REAL, HRU REAL, LANDUSE TEXT, SOIL TEXT, SLOPE_CD TEXT, " &
                    "IGRO INTEGER, PLANT_ID INTEGER, LAI_INIT REAL, BIO_INIT REAL, PHU_PLT REAL, BIOMIX REAL, CN2 REAL, USLE_P REAL, BIO_MIN REAL, FILTERW REAL, " &
                    "IURBAN INTEGER, URBLU INTEGER, IRRSC INTEGER, IRRNO INTEGER, FLOWMIN REAL, DIVMAX REAL, FLOWFR REAL, DDRAIN REAL, TDRAIN REAL, GDRAIN REAL, " &
                    "NROT INTEGER, HUSC INTEGER, ISCROP INTEGER" &
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

        Public Function Table2Create() As Boolean
            'based on mwSWATPlugIn:DBLayer:createHruTable
            Try
                DropTable(pTable2Name, pSwatInput.CnSwatInput)

                Dim lConnection As SqliteConnection = pSwatInput.CnSwatInput
                Dim lCloseWhenDone As Boolean = False
                If lConnection.State <> ConnectionState.Open Then
                    lConnection.Open()
                    lCloseWhenDone = True
                End If

                Dim lCreateSql As String = _
                    "CREATE TABLE " & pTable2Name & " (" &
                    "OID INTEGER PRIMARY KEY AUTOINCREMENT, " &
                    "SUBBASIN REAL, HRU REAL, LANDUSE TEXT, SOIL TEXT, SLOPE_CD TEXT, CROP TEXT, YEAR INTEGER, MONTH INTEGER, DAY INTEGER, " &
                    "HUSC REAL, MGT_OP INTEGER, HEATUNITS REAL, PLANT_ID INTEGER, CURYR_MAT INTEGER, LAI_INIT REAL, BIO_INIT REAL, HI_TARG REAL, BIO_TARG REAL, CNOP REAL, " &
                    "IRR_AMT REAL, FERT_ID INTEGER, FRT_KG REAL, FRT_SURFACE REAL, PEST_ID INTEGER, PST_KG REAL, TILLAGE_ID INTEGER, HARVEFF REAL, HI_OVR REAL, GRZ_DAYS INTEGER, " &
                    "MANURE_ID INTEGER, BIO_EAT REAL, BIO_TRMP REAL, MANURE_KG REAL, WSTRS_ID INTEGER, AUTO_WSTRS REAL, AFERT_ID INTEGER, AUTO_NSTRS REAL, AUTO_NAPP REAL, AUTO_NYR INTEGER, " &
                    "AUTO_EFF REAL, AFRT_SURFACE REAL, SWEEPEFF REAL, FR_CURB REAL, IMP_TRIG REAL, FERT_DAYS INTEGER, CFRT_ID INTEGER, IFRT_FREQ INTEGER, CFRT_KG REAL" &
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

        Public Function Table1() As DataTable
            pSwatInput.Status("Reading MGT1 from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM mgt1 ORDER BY SUBBASIN, HRU;")
        End Function

        Public Function Table2() As DataTable
            pSwatInput.Status("Reading MGT2 from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM mgt2 ORDER BY SUBBASIN, HRU, [YEAR], [MONTH], [DAY], HUSC;")
        End Function

        Public Sub Add1(ByVal aItem As clsMgtItem1)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Add2(ByVal aItem As clsMgtItem2)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        ''' <summary>
        ''' Save MGT information to set of .mgt text input files
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Save(Optional ByVal aTable1 As DataTable = Nothing, Optional ByVal aTable2 As DataTable = Nothing)
            If aTable1 Is Nothing Then aTable1 = Table1()
            If aTable2 Is Nothing Then aTable2 = Table2()

            pSwatInput.Status("Writing MGT text ...")
            For Each lMgt1Row As DataRow In aTable1.Rows
                Try
                    Dim lSubBasin As String = lMgt1Row.Item(1).ToString.Trim
                    Dim lHruNum As String = lMgt1Row.Item(2).ToString.Trim

                    Dim lSB As New System.Text.StringBuilder
                    '1st line
                    lSB.AppendLine(" .mgt file Subbasin:" & lSubBasin & " HRU:" & lHruNum & " Luse:" & lMgt1Row.Item(3) _
                                 & " Soil: " & lMgt1Row.Item(4) & " Slope: " & lMgt1Row.Item(5) _
                                 & " " & HeaderString())
                    '2. NMGT
                    lSB.AppendLine(Format(0, "0").PadLeft(16) & "    | NMGT:Management code")

                    lSB.AppendLine("Initial Plant Growth Parameters")
                    'IGRO
                    lSB.AppendLine(Format(lMgt1Row.Item(("IGRO")), "0").PadLeft(16) & "    | IGRO: Land cover status: 0-none growing; 1-growing")
                    'PLANT_ID
                    lSB.AppendLine(Format(lMgt1Row.Item(("PLANT_ID")), "0").PadLeft(16) & "    | PLANT_ID: Land cover ID number (IGRO = 1)")
                    'LAI_INIT
                    lSB.AppendLine(Format(lMgt1Row.Item(("LAI_INIT")), "0.00").PadLeft(16) & "    | LAI_INIT: Initial leaf are index (IGRO = 1)")
                    'BIO_INIT
                    lSB.AppendLine(Format(lMgt1Row.Item(("BIO_INIT")), "0.00").PadLeft(16) & "    | BIO_INIT: Initial biomass (kg/ha) (IGRO = 1)")
                    'PHU_PLT
                    lSB.AppendLine(Format(lMgt1Row.Item(("PHU_PLT")), "0.00").PadLeft(16) & "    | PHU_PLT: Number of heat units to bring plant to maturity (IGRO = 1)")

                    lSB.AppendLine("General Management Parameters")
                    'BIOMIX
                    lSB.AppendLine(Format(lMgt1Row.Item(("BIOMIX")), "0.00").PadLeft(16) & "    | BIOMIX: Biological mixing efficiency")
                    'CN2
                    lSB.AppendLine(Format(lMgt1Row.Item(("CN2")), "0.00").PadLeft(16) & "    | CN2: Initial SCS CN II value")
                    'USLE_P
                    lSB.AppendLine(Format(lMgt1Row.Item(("USLE_P")), "0.00").PadLeft(16) & "    | USLE_P: USLE support practice factor")
                    'BIO_MIN
                    lSB.AppendLine(Format(lMgt1Row.Item(("BIO_MIN")), "0.00").PadLeft(16) & "    | BIO_MIN: Minimum biomass for grazing (kg/ha)")
                    'FILTERW
                    lSB.AppendLine(Format(lMgt1Row.Item(("FILTERW")), "0.000").PadLeft(16) & "    | FILTERW: width of edge of field filter strip (m)")

                    lSB.AppendLine("Urban Management Parameters")
                    'IURBAN
                    lSB.AppendLine(Format(lMgt1Row.Item(("IURBAN")), "0").PadLeft(16) & "    | IURBAN: urban simulation code, 0-none, 1-USGS, 2-buildup/washoff")
                    'URBLU
                    lSB.AppendLine(Format(lMgt1Row.Item(("URBLU")), "0").PadLeft(16) & "    | URBLU: urban land type")

                    lSB.AppendLine("Irrigation Management Parameters")
                    'IRRSC
                    lSB.AppendLine(Format(lMgt1Row.Item(("IRRSC")), "0").PadLeft(16) & "    | IRRSC: irrigation code")
                    'IRRNO
                    lSB.AppendLine(Format(lMgt1Row.Item(("IRRNO")), "0").PadLeft(16) & "    | IRRNO: irrigation source location")
                    'FLOWMIN
                    lSB.AppendLine(Format(lMgt1Row.Item(("FLOWMIN")), "0.000").PadLeft(16) & "    | FLOWMIN: min in-stream flow for irr diversions (m^3/s)")
                    'DIVMAX
                    lSB.AppendLine(Format(lMgt1Row.Item(("DIVMAX")), "0.000").PadLeft(16) & "    | DIVMAX: max irrigation diversion from reach (+mm/-10^4m^3)")
                    'FLOWFR
                    lSB.AppendLine(Format(lMgt1Row.Item(("FLOWFR")), "0.000").PadLeft(16) & "    | FLOWFR: : fraction of flow allowed to be pulled for irr")

                    lSB.AppendLine("Tile Drain Management Parameters")

                    'DDRAIN
                    lSB.AppendLine(Format(lMgt1Row.Item(("DDRAIN")), "0.000").PadLeft(16) & "    | DDRAIN: depth to subsurface tile drain (mm)")
                    'TDRAIN
                    lSB.AppendLine(Format(lMgt1Row.Item(("TDRAIN")), "0.000").PadLeft(16) & "    | TDRAIN: time to drain soil to field capacity (hr)")
                    'GDRAIN
                    lSB.AppendLine(Format(lMgt1Row.Item(("GDRAIN")), "0.000").PadLeft(16) & "    | GDRAIN: drain tile lag time (hr)")

                    lSB.AppendLine("Management Operations:")
                    'NROT
                    lSB.AppendLine(Format(lMgt1Row.Item(("NROT")), "0").PadLeft(16) & "    | NROT: number of years of rotation")
                    lSB.AppendLine("Operation Schedule:")

                    If lMgt1Row.Item(("NROT")) > 0 Then 'Print operations schedule if 1 or more operation years
                        '3d line 'from mgt2
                        Dim lBlnHu As Boolean
                        If lMgt1Row.Item(("HUSC")) = 0 Then
                            lBlnHu = True
                        Else
                            lBlnHu = False
                        End If

                        Dim lRowsMgt2() As DataRow = aTable2.Select("SUBBASIN = " & lMgt1Row.Item(1) & " AND HRU = " & lMgt1Row.Item(2), "[YEAR] ASC, [MONTH] ASC, [DAY] ASC, HUSC ASC")
                        If lRowsMgt2.GetUpperBound(0) >= 0 Then
                            Dim lYear As Integer = lRowsMgt2(0).Item(("YEAR"))

                            Dim lLine As String = ""

                            For Each lRowMgt2 As DataRow In lRowsMgt2
                                If lBlnHu = True Then
                                    lLine = Strings.StrDup(7, " ") & Format(lRowMgt2.Item(("HUSC")), "##0.000").PadLeft(8)
                                Else
                                    lLine = Format(lRowMgt2.Item(("MONTH")), "##").PadLeft(3) _
                                          & Format(lRowMgt2.Item(("DAY")), "##").PadLeft(3) & Strings.StrDup(9, " ")
                                End If
                                lLine &= Format(lRowMgt2.Item(("MGT_OP")), "##").PadLeft(3)

                                Dim lMgtOp As Integer = lRowMgt2.Item(("MGT_OP"))
                                Select Case lMgtOp
                                    Case Is = -1 'skipped year
                                        lLine = Strings.StrDup(19, " ") & "0" & Strings.StrDup(76, " ")
                                    Case Is = 1
                                        lLine &= Format(lRowMgt2.Item(("PLANT_ID")), "####").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("CURYR_MAT")), "##").PadLeft(7) _
                                               & Format(lRowMgt2.Item(("HEATUNITS")), "#####0.00000").PadLeft(13) _
                                               & Format(lRowMgt2.Item(("LAI_INIT")), "##0.00").PadLeft(7) _
                                               & Format(lRowMgt2.Item(("BIO_INIT")), "####0.00000").PadLeft(12) _
                                               & Format(lRowMgt2.Item(("HI_TARG")), "0.00").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("BIO_TARG")), "##0.00").PadLeft(7) _
                                               & Format(lRowMgt2.Item(("CNOP")), "#0.00").PadLeft(6)
                                    Case Is = 2 'irrigation
                                        lLine &= Space(13) & Format(lRowMgt2.Item(("IRR_AMT")), "#####0.00000").PadLeft(12)
                                    Case Is = 3 'fertilizer: there are several variables that are not included in SWAT 2002 but are existing in the table i.e. FRMINN, etc. Those are not shown in the interface and it seems to be expected to be on the next version. In this version they are not printed in the files.
                                        lLine &= Format(lRowMgt2.Item(("FERT_ID")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("FRT_KG")), "#####0.00000").PadLeft(20) _
                                               & Format(lRowMgt2.Item(("FRT_SURFACE")), "##0.00").PadLeft(7)
                                    Case Is = 4 'pesticide
                                        lLine &= Format(lRowMgt2.Item(("PEST_ID")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("PST_KG")), "#####0.00000").PadLeft(20)
                                    Case Is = 5 'Harvest and kill operation
                                        lLine &= Format(lRowMgt2.Item(("CNOP")), "#####0.00000").PadLeft(25)
                                    Case Is = 6 'Tillage operation
                                        lLine &= Format(lRowMgt2.Item(("TILLAGE_ID")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("CNOP")), "#####0.00000").PadLeft(20)
                                    Case Is = 7 ' Harvest only operation
                                        lLine &= Format(lRowMgt2.Item(("HARVEFF")), "#####0.00000").PadLeft(25) _
                                               & Format(lRowMgt2.Item(("HI_OVR")), "##0.00").PadLeft(7)
                                    Case Is = 8 'KILL OPERATION

                                    Case Is = 9 ' Grazing operation
                                        lLine &= Format(lRowMgt2.Item(("GRZ_DAYS")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("MANURE_ID")), "##0").PadLeft(4) _
                                               & Format(lRowMgt2.Item(("BIO_EAT")), "#####0.00000").PadLeft(16) _
                                               & Format(lRowMgt2.Item(("BIO_TRMP")), "##0.00").PadLeft(7) _
                                               & Format(lRowMgt2.Item(("MANURE_KG")), "####0.00000").PadLeft(12)
                                    Case Is = 10 'Auto irrigation: AUTO_RNF is a variable showing in AVSWAT but not shown in SWAT2002. It is not included here.
                                        lLine &= Format(lRowMgt2.Item(("WSTRS_ID")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("AUTO_WSTRS")), "#####0.00000").PadLeft(20)
                                    Case Is = 11 'Auto fert option
                                        lLine &= Format(lRowMgt2.Item(("AFERT_ID")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("AUTO_NSTRS")), "#####0.00000").PadLeft(20) _
                                               & Format(lRowMgt2.Item(("AUTO_NAPP")), "##0.00").PadLeft(7) _
                                               & Format(lRowMgt2.Item(("AUTO_NYR")), "####0.00000").PadLeft(12) _
                                               & Format(lRowMgt2.Item(("AUTO_EFF")), "0.00").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("AFRT_SURFACE")), "####0.00").PadLeft(9)
                                    Case Is = 12 'street sweeping operation
                                        lLine &= Format(lRowMgt2.Item(("SWEEPEFF")), "#####0.00000").PadLeft(25) _
                                               & Format(lRowMgt2.Item(("FR_CURB")), "##0.00").PadLeft(7)
                                    Case Is = 13 'realease/impound
                                        lLine &= Format(lRowMgt2.Item(("IMP_TRIG")), "###0").PadLeft(5)
                                    Case Is = 14 'Continuous fertilizer
                                        lLine &= Format(lRowMgt2.Item(("FERT_DAYS")), "###0").PadLeft(5) _
                                               & Format(lRowMgt2.Item(("CFRT_ID")), "##0").PadLeft(4) _
                                               & Format(lRowMgt2.Item(("IFRT_FREQ")), "#0").PadLeft(3) _
                                               & Format(lRowMgt2.Item(("CFRT_KG")), "#####0.0000").PadLeft(13)
                                End Select
                                'SKIP A LINE IN NEW YEAR
                                Dim lNYear As Integer = lRowMgt2.Item(("YEAR"))
                                If lNYear <> lYear Then
                                    lYear = lNYear
                                    If lMgtOp <> -1 Then
                                        lSB.AppendLine(Strings.StrDup(19, " ") & "0" & Strings.StrDup(76, " "))
                                    End If
                                End If
                                lSB.AppendLine(lLine)
                            Next
                        End If
                    End If
                    lSB.AppendLine("")
                    IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\" & StringFnameHRUs(lSubBasin, lHruNum) & ".mgt", lSB.ToString)
                Catch lEx As Exception
                    Debug.Print(lEx.Message)
                End Try
            Next
        End Sub
    End Class
End Class
