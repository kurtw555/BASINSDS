Imports System.Data
Imports Microsoft.Data.Sqlite
'Imports Microsoft.Data.SQLite


Partial Class SwatInput
    Private pBsn As clsBsn = New clsBsn(Me)
    ReadOnly Property Bsn() As clsBsn
        Get
            Return pBsn
        End Get
    End Property

    Public Class clsBsnItem
        Public SFTMP As Double
        Public SMTMP As Double
        Public SMFMX As Double
        Public SMFMN As Double
        Public TIMP As Double
        Public SNOCOVMX As Double
        Public SNO50COV As Double
        Public IPET As Long
        Public ESCO As Double
        Public EPCO As Double
        Public EVLAI As Double
        Public FFCB As Double
        Public IEVENT As Long
        Public ICRK As Long
        Public SURLAG As Double
        Public ADJ_PKR As Double
        Public PRF As Double
        Public SPCON As Double
        Public SPEXP As Double
        Public RCN As Double
        Public CMN As Double
        Public N_UPDIS As Double
        Public P_UPDIS As Double
        Public NPERCO As Double
        Public PPERCO As Double
        Public PHOSKD As Double
        Public PSP As Double
        Public RSDCO As Double
        Public PERCOP As Double
        Public ISUBWQ As Long
        Public WDPQ As Double
        Public WGPQ As Double
        Public WDLPQ As Double
        Public WGLPQ As Double
        Public WDPS As Double
        Public WGPS As Double
        Public WDLPS As Double
        Public WGLPS As Double
        Public BACTKDQ As Double
        Public THBACT As Double
        Public WOF_P As Double
        Public WOF_LP As Double
        Public WDPF As Double
        Public WGPF As Double
        Public WDLPF As Double
        Public WGLPF As Double
        Public IRTE As Long
        Public MSK_COL1 As Double
        Public MSK_COL2 As Double
        Public MSK_X As Double
        Public IDEG As Long
        Public IWQ As Long
        Public TRNSRCH As Double
        Public EVRCH As Double
        Public IRTPEST As Long
        Public ICN As Long
        Public CNCOEF As Double
        Public CDN As Double
        Public SDNCO As Double
        Public BACT_SWF As Double
        Public BACTMX As Double
        Public BACTMINLP As Double
        Public BACTMINP As Double
        Public WDLPRCH As Double
        Public WDPRCH As Double
        Public WDLPRES As Double
        Public WDPRES As Double
        Public TB_ADJ As Double
        Public DEPIMP_BSN As Double
        Public DDRAIN_BSN As Double
        Public TDRAIN_BSN As Double
        Public GDRAIN_BSN As Double
        Public CN_FROZ As Double
        Public ISED_DET As Double
        Public ETFILE As String

        Public Sub New()

        End Sub

        Sub New(ByVal aRow As DataRow)
            PopulateObject(aRow, Me)
        End Sub

        Public Function AddSQL() As String
            Return "INSERT INTO bsn ( " & atcUtility.FieldNames(Me.GetType, ", ") _
                       & " ) Values ('" & atcUtility.FieldValues(Me, "', '") & "'  )"
        End Function

        Public Shared ReadOnly Property Units() As Dictionary(Of String, String)

            Get
                Dim unitsDictionary As New Dictionary(Of String, String)



                unitsDictionary.Add("SFTMP", "C")
                unitsDictionary.Add("SMTMP", "C")
                unitsDictionary.Add("SMFMX", "mm H2O/degC-day")
                unitsDictionary.Add("SMFMN", "mm H2O/degC-day")
                unitsDictionary.Add("TIMP", "")
                unitsDictionary.Add("SNOCOVMX", "mm")
                unitsDictionary.Add("SNO50COV", "")
                unitsDictionary.Add("IPET", "")
                unitsDictionary.Add("ESCO", "")
                unitsDictionary.Add("EPCO", "")
                unitsDictionary.Add("EVLAI", "m2/m2")
                unitsDictionary.Add("FFCB", "")
                unitsDictionary.Add("IEVENT", "")
                unitsDictionary.Add("ICRK", "")
                unitsDictionary.Add("SURLAG", "days")
                unitsDictionary.Add("ADJ_PKR", "")
                unitsDictionary.Add("PRF", " ")
                unitsDictionary.Add("SPCON", "")
                unitsDictionary.Add("SPEXP", "")
                unitsDictionary.Add("RCN", "mg N/l")
                unitsDictionary.Add("CMN", "")
                unitsDictionary.Add("N_UPDIS", "")
                unitsDictionary.Add("P_UPDIS", "")
                unitsDictionary.Add("NPERCO", "")
                unitsDictionary.Add("PPERCO", "")
                unitsDictionary.Add("PHOSKD", "")
                unitsDictionary.Add("PSP", "")
                unitsDictionary.Add("RSDCO", "")
                unitsDictionary.Add("PERCOP", "")
                unitsDictionary.Add("ISUBWQ", "")
                unitsDictionary.Add("WDPQ", "1/day")
                unitsDictionary.Add("WGPQ", "1/day")
                unitsDictionary.Add("WDLPQ", "1/day")
                unitsDictionary.Add("WGLPQ", "1/day")
                unitsDictionary.Add("WDPS", "1/day")
                unitsDictionary.Add("WGPS", "1/day")
                unitsDictionary.Add("WDLPS", "1/day")
                unitsDictionary.Add("WGLPS", "1/day")
                unitsDictionary.Add("BACTKDQ", "")
                unitsDictionary.Add("THBACT", "")
                unitsDictionary.Add("WOF_P", "")
                unitsDictionary.Add("WOF_LP", "")
                unitsDictionary.Add("WDPF", "")
                unitsDictionary.Add("WGPF", "")
                unitsDictionary.Add("WDLPF", "")
                unitsDictionary.Add("WGLPF", "")
                unitsDictionary.Add("IRTE", "")
                unitsDictionary.Add("MSK_COL1", "")
                unitsDictionary.Add("MSK_COL2", "")
                unitsDictionary.Add("MSK_X", "")
                unitsDictionary.Add("IDEG", "")
                unitsDictionary.Add("IWQ", "")
                unitsDictionary.Add("TRNSRCH", "")
                unitsDictionary.Add("EVRCH", "")
                unitsDictionary.Add("IRTPEST", "")
                unitsDictionary.Add("ICN", "")
                unitsDictionary.Add("CNCOEF", "")
                unitsDictionary.Add("CDN", "")
                unitsDictionary.Add("SDNCO", "")
                unitsDictionary.Add("BACT_SWF", "")
                unitsDictionary.Add("BACTMX", "")
                unitsDictionary.Add("BACTMINLP", "")
                unitsDictionary.Add("BACTMINP", "")
                unitsDictionary.Add("WDLPRCH", "")
                unitsDictionary.Add("WDPRCH", "")
                unitsDictionary.Add("WDLPRES", "")
                unitsDictionary.Add("WDPRES", "")
                unitsDictionary.Add("TB_ADJ", "")
                unitsDictionary.Add("DEPIMP_BSN", "")
                unitsDictionary.Add("DDRAIN_BSN", "")
                unitsDictionary.Add("TDRAIN_BSN", "")
                unitsDictionary.Add("GDRAIN_BSN", "")
                unitsDictionary.Add("CN_FROZ", "")
                unitsDictionary.Add("ISED_DET", "")
                unitsDictionary.Add("ETFILE", "")

                Return unitsDictionary

            End Get

        End Property

    End Class

    ''' <summary>
    ''' Basin (BSN) Input Section
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsBsn
        Private pSwatInput As SwatInput
        Private pTableName As String = "bsn"


        Friend Sub New(ByVal aSwatInput As SwatInput)
            pSwatInput = aSwatInput
        End Sub

        Public Function TableCreate() As Boolean
            'based on mwSWATPlugIn.DBLayer.createBsnTable
            Try
                DropTable(pTableName, pSwatInput.CnSwatInput)

                Dim lConnection As SqliteConnection = pSwatInput.CnSwatInput
                Dim lCloseWhenDone As Boolean = False
                If lConnection.State <> ConnectionState.Open Then
                    lConnection.Open()
                    lCloseWhenDone = True
                End If

                Dim lCreateSql As String = _
                    "CREATE TABLE " & pTableName & " (" & _
                    "OBJECTID INTEGER PRIMARY KEY AUTOINCREMENT, " & _
                    "SFTMP REAL, SMTMP REAL, SMFMX REAL, SMFMN REAL, TIMP REAL, SNOCOVMX REAL, SNO50COV REAL, IPET INTEGER, ESCO REAL, EPCO REAL, EVLAI REAL, FFCB REAL, IEVENT INTEGER, ICRK INTEGER, SURLAG REAL, ADJ_PKR REAL, PRF REAL, SPCON REAL, SPEXP REAL, RCN REAL, CMN REAL, N_UPDIS REAL, P_UPDIS REAL, NPERCO REAL, PPERCO REAL, PHOSKD REAL, PSP REAL, RSDCO REAL, PERCOP REAL, ISUBWQ INTEGER, WDPQ REAL, WGPQ REAL, WDLPQ REAL, WGLPQ REAL, WDPS REAL, WGPS REAL, WDLPS REAL, WGLPS REAL, BACTKDQ REAL, THBACT REAL, WOF_P REAL, WOF_LP REAL, WDPF REAL, WGPF REAL, WDLPF REAL, WGLPF REAL, IRTE INTEGER, MSK_COL1 REAL, MSK_COL2 REAL, MSK_X REAL, IDEG INTEGER, IWQ INTEGER, TRNSRCH INTEGER, EVRCH REAL, IRTPEST INTEGER, ICN INTEGER, CNCOEF REAL, CDN REAL, SDNCO REAL, BACT_SWF REAL, BACTMX REAL, BACTMINLP REAL, BACTMINP REAL, WDLPRCH REAL, WDPRCH REAL, WDLPRES INTEGER, WDPRES INTEGER, TB_ADJ INTEGER, DEPIMP_BSN REAL, DDRAIN_BSN INTEGER, TDRAIN_BSN INTEGER, GDRAIN_BSN REAL, CN_FROZ REAL, ISED_DET REAL, ETFILE TEXT" & _
                    ");"

                ExecuteNonQuery(lCreateSql, lConnection)

                If lCloseWhenDone Then
                    lConnection.Close()
                End If
                Return True
            Catch lEx As Exception
                Debug.Print(lEx.Message)
                Return False
            End Try
        End Function

        Public Function Table() As DataTable
            pSwatInput.Status("Reading " & pTableName & " from database ...")
            Return pSwatInput.QueryInputDB("SELECT * FROM " & pTableName & ";")
        End Function

        Public Function Item() As clsBsnItem
            Return Item(Nothing)
        End Function
        Public Function Item(ByVal aTable As DataTable) As clsBsnItem
            If aTable Is Nothing Then aTable = Table()
            Return New clsBsnItem(aTable.Rows(0))
        End Function

        Public Sub Add(ByVal aItem As clsBsnItem)
            ExecuteNonQuery(aItem.AddSQL, pSwatInput.CnSwatInput)
        End Sub

        Public Sub Save(Optional ByVal aTable As DataTable = Nothing)
            If aTable Is Nothing Then aTable = Table()
            pSwatInput.Status("Writing " & pTableName & " text ...")

            Dim lRow As DataRow = aTable.Rows(0)
            Dim lItem As New clsBsnItem(lRow)
            Dim lSB As New System.Text.StringBuilder
            With lItem
                lSB.AppendLine("Basin data" & Space(10) & " ." & pTableName & " file " & HeaderString())
                lSB.AppendLine("Modeling Options: Land Area")
                lSB.AppendLine("Water Balance:")
                lSB.AppendLine(MakeString(.SFTMP, 3, 4, 8) & "| SFTMP : Snowfall temperature [ºC]")
                lSB.AppendLine(MakeString(.SMTMP, 3, 4, 8) & "| SMTMP : Snow melt base temperature [ºC]")
                lSB.AppendLine(MakeString(.SMFMX, 3, 4, 8) & "| SMFMX : Melt factor for snow on June 21 [mm H2O/ºC-day]")
                lSB.AppendLine(MakeString(.SMFMN, 3, 4, 8) & "| SMFMN : Melt factor for snow on December 21 [mm H2O/ºC-day]")
                lSB.AppendLine(MakeString(.TIMP, 3, 4, 8) & "| TIMP : Snow pack temperature lag factor")
                lSB.AppendLine(MakeString(.SNOCOVMX, 3, 4, 8) & "| SNOCOVMX : Minimum snow water content that corresponds to 100% snow cover [mm]")
                lSB.AppendLine(MakeString(.SNO50COV, 3, 4, 8) & "| SNO50COV : Fraction of snow volume represented by SNOCOVMX that corresponds to 50% snow cover")
                lSB.AppendLine(MakeString(.IPET, 0, 4, 4) & "| IPET: PET method: 0=priest-t, 1=pen-m, 2=har, 3=read into model")
                lSB.AppendLine(.ETFILE.PadLeft(16) & "    | PETFILE: name of potential ET input file")
                lSB.AppendLine(MakeString(.ESCO, 3, 4, 8) & "| ESCO: soil evaporation compensation factor")
                lSB.AppendLine(MakeString(.EPCO, 3, 4, 8) & "| EPCO: plant water uptake compensation factor")
                lSB.AppendLine(MakeString(.EVLAI, 3, 4, 8) & "| EVLAI : Leaf area index at which no evaporation occurs from water surface [m2/m2]")
                lSB.AppendLine(MakeString(.FFCB, 3, 4, 8) & "| FFCB : Initial soil water storage expressed as a fraction of field capacity water content")
                lSB.AppendLine("Surface Runoff:")
                lSB.AppendLine(MakeString(.IEVENT, 0, 4, 4) & "| IEVENT: rainfall/runoff code: 0=daily rainfall/CN")
                lSB.AppendLine(MakeString(.ICRK, 0, 4, 4) & "| ICRK: crack flow code: 1=model crack flow in soil")
                lSB.AppendLine(MakeString(.SURLAG, 3, 4, 8) & "| SURLAG : Surface runoff lag time [days]")
                lSB.AppendLine(MakeString(.ADJ_PKR, 3, 4, 8) & "| ADJ_PKR : Peak rate adjustment factor for sediment routing in the subbasin (tributary channels)")
                lSB.AppendLine(MakeString(.PRF, 3, 4, 8) & "| PRF : Peak rate adjustment factor for sediment routing in the main channel")
                lSB.AppendLine(MakeString(.SPCON, 3, 4, 8) & "| SPCON : Linear parameter for calculating the maximum amount of sediment that can be reentrained during channel sediment routing")
                lSB.AppendLine(MakeString(.SPEXP, 3, 4, 8) & "| SPEXP : Exponent parameter for calculating sediment reentrained in channel sediment routing")
                lSB.AppendLine("Nutrient Cycling:")
                lSB.AppendLine(MakeString(.RCN, 3, 4, 8) & "| RCN : Concentration of nitrogen in rainfall [mg N/l]")
                lSB.AppendLine(MakeString(.CMN, 3, 4, 8) & "| CMN : Rate factor for humus mineralization of active organic nitrogen")
                lSB.AppendLine(MakeString(.N_UPDIS, 3, 4, 8) & "| N_UPDIS : Nitrogen uptake distribution parameter")
                lSB.AppendLine(MakeString(.P_UPDIS, 3, 4, 8) & "| P_UPDIS : Phosphorus uptake distribution parameter")
                lSB.AppendLine(MakeString(.NPERCO, 3, 4, 8) & "| NPERCO : Nitrogen percolation coefficient")
                lSB.AppendLine(MakeString(.PPERCO, 3, 4, 8) & "| PPERCO : Phosphorus percolation coefficient")
                lSB.AppendLine(MakeString(.PHOSKD, 3, 4, 8) & "| PHOSKD : Phosphorus soil partitioning coefficient")
                lSB.AppendLine(MakeString(.PSP, 3, 4, 8) & "| PSP : Phosphorus sorption coefficient")
                lSB.AppendLine(MakeString(.RSDCO, 3, 4, 8) & "| RSDCO : Residue decomposition coefficient")
                lSB.AppendLine("Pesticide Cycling:")
                lSB.AppendLine(MakeString(.PERCOP, 3, 4, 8) & "| PERCOP : Pesticide percolation coefficient")
                lSB.AppendLine("Algae/CBOD/Dissolved Oxygen:")
                lSB.AppendLine(MakeString(.ISUBWQ, 0, 4, 4) & "| ISUBWQ: subbasin water quality parameter")
                lSB.AppendLine("Bacteria:")
                lSB.AppendLine(MakeString(.WDPQ, 3, 4, 8) & "| WDPQ : Die-off factor for persistent bacteria in soil solution. [1/day]")
                lSB.AppendLine(MakeString(.WGPQ, 3, 4, 8) & "| WGPQ : Growth factor for persistent bacteria in soil solution [1/day]")
                lSB.AppendLine(MakeString(.WDLPQ, 3, 4, 8) & "| WDLPQ : Die-off factor for less persistent bacteria in soil solution [1/day]")
                lSB.AppendLine(MakeString(.WGLPQ, 3, 4, 8) & "| WGLPQ : Growth factor for less persistent bacteria in soil solution. [1/day] ")
                lSB.AppendLine(MakeString(.WDPS, 3, 4, 8) & "| WDPS : Die-off factor for persistent bacteria adsorbed to soil particles. [1/day]")
                lSB.AppendLine(MakeString(.WGPS, 3, 4, 8) & "| WGPS : Growth factor for persistent bacteria adsorbed to soil particles. [1/day]")
                lSB.AppendLine(MakeString(.WDLPS, 3, 4, 8) & "| WDLPS : Die-off factor for less persistent bacteria adsorbed to soil particles. [1/day] ")
                lSB.AppendLine(MakeString(.WGLPS, 3, 4, 8) & "| WGLPS : Growth factor for less persistent bacteria adsorbed to soil particles. [1/day]")
                lSB.AppendLine(MakeString(.BACTKDQ, 3, 4, 8) & "| BACTKDQ : Bacteria partition coefficient")
                lSB.AppendLine(MakeString(.THBACT, 3, 4, 8) & "| THBACT : Temperature adjustment factor for bacteria die-off/growth")
                lSB.AppendLine(MakeString(.WOF_P, 3, 4, 8) & "| WOF_P: wash-off fraction for persistent bacteria on foliage")
                lSB.AppendLine(MakeString(.WOF_LP, 3, 4, 8) & "| WOF_LP: wash-off fraction for less persistent bacteria on foliage")
                lSB.AppendLine(MakeString(.WDPF, 3, 4, 8) & "| WDPF: persistent bacteria die-off factor on foliage")
                lSB.AppendLine(MakeString(.WGPF, 3, 4, 8) & "| WGPF: persistent bacteria growth factor on foliage ")
                lSB.AppendLine(MakeString(.WDLPF, 3, 4, 8) & "| WDLPF: less persistent bacteria die-off factor on foliage")
                lSB.AppendLine(MakeString(.WGLPF, 3, 4, 8) & "| WGLPF: less persistent bacteria growth factor on foliage")
                lSB.AppendLine(MakeString(.ISED_DET, 0, 4, 4) & "| ISED_DET: ")
                lSB.AppendLine("Modeling Options: Reaches")
                lSB.AppendLine(MakeString(.IRTE, 0, 4, 4) & "| IRTE: water routing method 0=variable travel-time 1=Muskingum")
                lSB.AppendLine(MakeString(.MSK_COL1, 3, 4, 8) & "| MSK_CO1 : Calibration coefficient used to control impact of the storage time constant (Km) for normal flow ")
                lSB.AppendLine(MakeString(.MSK_COL2, 3, 4, 8) & "| MSK_CO2 : Calibration coefficient used to control impact of the storage time constant (Km) for low flow ")
                lSB.AppendLine(MakeString(.MSK_X, 3, 4, 8) & "| MSK_X : Weighting factor controlling relative importance of inflow rate and outflow rate in determining water storage in reach segment")
                lSB.AppendLine(MakeString(.IDEG, 0, 4, 4) & "| IDEG: channel degradation code ")
                lSB.AppendLine(MakeString(.IWQ, 0, 4, 4) & "| IWQ: in-stream water quality: 1=model in-stream water quality ")
                lSB.AppendLine("   basins.wwq       | WWQFILE: name of watershed water quality file")
                lSB.AppendLine(MakeString(.TRNSRCH, 3, 4, 8) & "| TRNSRCH: reach transmission loss partitioning to deep aquifer")
                lSB.AppendLine(MakeString(.EVRCH, 3, 4, 8) & "| EVRCH : Reach evaporation adjustment factor")
                lSB.AppendLine(MakeString(.IRTPEST, 0, 4, 4) & "| IRTPEST : Number of pesticide to be routed through the watershed channel network")
                lSB.AppendLine(MakeString(.ICN, 0, 4, 4) & "| ICN")
                lSB.AppendLine(MakeString(.CNCOEF, 3, 4, 8) & "| CNCOEF")
                lSB.AppendLine(MakeString(.CDN, 3, 4, 8) & "| CDN")
                lSB.AppendLine(MakeString(.SDNCO, 3, 4, 8) & "| SDNCO")
                lSB.AppendLine(MakeString(.BACT_SWF, 3, 4, 8) & "| BACT_SWF")
                lSB.AppendLine(MakeString(.BACTMX, 3, 4, 8) & "| BACTMX")
                lSB.AppendLine(MakeString(.BACTMINLP, 3, 4, 8) & "| BACTMINLP")
                lSB.AppendLine(MakeString(.BACTMINP, 3, 4, 8) & "| BACTMINP")
                lSB.AppendLine(MakeString(.WDLPRCH, 3, 4, 8) & "| WDLPRCH")
                lSB.AppendLine(MakeString(.WDPRCH, 3, 4, 8) & "| WDPRCH")
                lSB.AppendLine(MakeString(.WDLPRES, 3, 4, 8) & "| WDLPRES")
                lSB.AppendLine(MakeString(.WDPRES, 3, 4, 8) & "| WDPRES")
                lSB.AppendLine(MakeString(.TB_ADJ, 3, 4, 8) & "| TB_ADJ")
                lSB.AppendLine(MakeString(.DEPIMP_BSN, 0, 4, 8) & "| DEPIMP_BSN")
                lSB.AppendLine(MakeString(.DDRAIN_BSN, 3, 4, 8) & "| DDRAIN_BSN")
                lSB.AppendLine(MakeString(.TDRAIN_BSN, 3, 4, 8) & "| TDRAIN_BSN")
                lSB.AppendLine(MakeString(.GDRAIN_BSN, 3, 4, 8) & "| GDRAIN_BSN")
                lSB.AppendLine(MakeString(.CN_FROZ, 3, 4, 8) & "| CN_FROZ")
            End With
            IO.File.WriteAllText(pSwatInput.TxtInOutFolder & "\basins.bsn", lSB.ToString)
        End Sub

        ''' <summary>
        ''' First draft of code to read SWAT input text file
        ''' </summary>
        Public Sub ReadText()
            pSwatInput.Status("Reading " & pTableName & " text ...")
            Dim lItem As New clsBsnItem()
            With lItem
                For Each lLine As String In atcUtility.LinesInFile(pSwatInput.TxtInOutFolder & "\basins.bsn")
                    Try
                        Dim lPipePos As Integer = lLine.IndexOf("|")
                        If lPipePos > 0 Then
                            Dim lFieldName As String = lLine.Substring(lPipePos + 1).TrimStart
                            Dim lSpacePos As Integer = lLine.IndexOf(" ")
                            Dim lColonPos As Integer = lLine.IndexOf(":")
                            If lSpacePos < 0 Then lSpacePos = lFieldName.Length
                            If lColonPos < 0 Then lColonPos = lFieldName.Length
                            lFieldName = lFieldName.Substring(0, Math.Min(lSpacePos, lColonPos))
                            Dim lValue As String = lLine.Substring(0, lPipePos - 1).Trim
                            If Not atcUtility.SetSomething(lItem, lFieldName, lValue) Then
                                Select Case lFieldName
                                    Case "PETFILE" : .ETFILE = lValue
                                    Case "MSK_CO1" : .MSK_COL1 = lValue
                                    Case "MSK_CO2" : .MSK_COL2 = lValue
                                        '"   basins.wwq       | WWQFILE: name of watershed water quality file")
                                    Case Else
                                        MsgBox("Could not set field '" & lFieldName & "'", MsgBoxStyle.Critical, "Error reading basins.bsn")
                                End Select
                            End If
                        End If
                    Catch e As Exception
                        MsgBox(e.Message, MsgBoxStyle.Critical, "Error reading basins.bsn")
                    End Try
                Next
                'TODO: write lItem into database? change Sub into Function and Return lItem?
            End With
        End Sub

    End Class
End Class
