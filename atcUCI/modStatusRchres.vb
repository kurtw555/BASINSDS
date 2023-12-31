Option Strict Off
Option Explicit On

Imports atcuci.HspfStatus.HspfStatusReqOptUnnEnum

Module modStatusRchres
    'Copyright 2006 AQUA TERRA Consultants - Royalty-free use permitted under open source license
	
    Public Sub UpdateRchres(ByRef aOperation As HspfOperation, _
                            ByRef aTableStatus As HspfStatus)
        Dim ltable As HspfTable
        Dim i, j As Integer
        Dim t, f As String
        'added j   THJ
        Dim Bedflg, nCons, Tgflg As Integer
        Dim mPhoto, mCloud, mPhval, nGqual, mWatemp, mRoxygen, mSedconc, mBio As Integer
        'added mBio flag   THJ
        Dim Oxidation, Hydrolysis, Photolysis As Boolean
        Dim Biodegradation, Volatilization As Boolean
        'added Biodegradation flag  THJ
        Dim nGenDec, nBiod, nPhot, nHydro, nOxid, nVolat, nBioM, nSedAs As Integer
        'added gqual process counters - these are SUB1-SUB7 in PGQUAL   THJ
        Dim Gdaufg(6) As Integer
        Dim Daufg(6) As Integer
        Dim nDaught As Integer
        'added gqual daughter product flags
        Dim Benrfg, Reamfg As Integer
        Dim Po4fg, Phflag, Tamfg, Amvfg, Adnhfg, Adpofg As Integer
        Dim Zoofg, Phyfg, Balfg As Integer

        'always can be present
        aTableStatus.Change("ACTIVITY", 1, HspfStatusRequired)
        aTableStatus.Change("PRINT-INFO", 1, HspfStatusOptional)
        aTableStatus.Change("GEN-INFO", 1, HspfStatusRequired)

        Dim lLakeFlag As Integer
        If aOperation.TableExists("GEN-INFO") Then
            lLakeFlag = aOperation.Tables.Item("GEN-INFO").ParmValue("LKFG")
        Else
            lLakeFlag = 0
        End If

        Dim lAcidph As Boolean
        If aOperation.TableExists("ACTIVITY") Then
            'moved Benrfg to OXRX  THJ
            ltable = aOperation.Tables.Item("ACTIVITY")
            If ltable.Parms("HYDRFG").Value = 1 Or ltable.Parms("HYDRFG").Value = 2 Then
                aTableStatus.Change("HYDR-PARM1", 1, HspfStatusOptional)
                If aOperation.TableExists("HYDR-PARM1") Then
                    If aOperation.Tables.Item("HYDR-PARM1").ParmValue("VCONFG") = 1 Then
                        aTableStatus.Change("MON-CONVF", 1, HspfStatusOptional)
                    End If
                End If
                aTableStatus.Change("HYDR-PARM2", 1, HspfStatusRequired)
                aTableStatus.Change("HYDR-IRRIG", 1, HspfStatusOptional)
                'added HYDR-IRRIG   THJ
                aTableStatus.Change("HYDR-INIT", 1, HspfStatusOptional)
                'must pass NCAT from CATEGORY to NCATS in HYDR to be able to
                'process categories.
            End If
            Dim nNodes As Integer = 0
            Dim nConds As Integer = 0
            If ltable.Parms("HYDRFG").Value = 2 Then
                If aOperation.TableExists("DYNAMIC-WAVE") Then
                    nNodes = aOperation.Tables.Item("DYNAMIC-WAVE").ParmValue("NNODE")
                    nConds = aOperation.Tables.Item("DYNAMIC-WAVE").ParmValue("NCOND")
                End If
                aTableStatus.Change("DYNAMIC-WAVE", 1, HspfStatusRequired)
                aTableStatus.Change("CONDUIT-PARM", 1, HspfStatusRequired)
                aTableStatus.Change("CONDUIT-XS", 1, HspfStatusRequired)
                aTableStatus.Change("NODE-PARM", 1, HspfStatusRequired)
            End If
            If ltable.Parms("HYDRFG").Value = 1 Then
                If aOperation.TableExists("DYNAMIC-WAVE") Then
                    aOperation.Tables.Remove("DYNAMIC-WAVE")
                End If
                If aOperation.TableExists("CONDUIT-PARM") Then
                    aOperation.Tables.Remove("CONDUIT-PARM")
                End If
                If aOperation.TableExists("CONDUIT-XS") Then
                    aOperation.Tables.Remove("CONDUIT-XS")
                End If
                If aOperation.TableExists("NODE-PARM") Then
                    aOperation.Tables.Remove("NODE-PARM")
                End If
            End If
            If ltable.Parms("ADFG").Value = 1 Then
                aTableStatus.Change("ADCALC-DATA", 1, HspfStatusOptional)
            End If
            If ltable.Parms("CONSFG").Value = 1 Then
                aTableStatus.Change("NCONS", 1, HspfStatusOptional)
                aTableStatus.Change("CONS-AD-FLAG", 1, HspfStatusOptional)
                If aOperation.TableExists("NCONS") Then
                    nCons = aOperation.Tables.Item("NCONS").ParmValue("NCONS")
                Else
                    nCons = 1
                End If
                For i = 1 To nCons
                    aTableStatus.Change("CONS-DATA", i, HspfStatusRequired)
                Next i
            End If
            If ltable.Parms("HTFG").Value = 1 Then
                aTableStatus.Change("HT-BED-FLAGS", 1, HspfStatusOptional)
                aTableStatus.Change("HEAT-PARM", 1, HspfStatusOptional)
                If aOperation.TableExists("HT-BED-FLAGS") Then
                    Bedflg = aOperation.Tables.Item("HT-BED-FLAGS").ParmValue("BEDFLG")
                    Tgflg = aOperation.Tables.Item("HT-BED-FLAGS").ParmValue("TGFLG")
                    If Bedflg = 1 Or Bedflg = 2 Then
                        aTableStatus.Change("HT-BED-PARM", 1, HspfStatusOptional)
                        If Tgflg = 3 Then
                            aTableStatus.Change("MON-HT-TGRND", 1, HspfStatusOptional)
                        End If
                    ElseIf Bedflg = 3 Then
                        aTableStatus.Change("HT-BED-DELH", 1, HspfStatusOptional)
                        aTableStatus.Change("HT-BED-DELTT", 1, HspfStatusOptional)
                    End If
                End If
                aTableStatus.Change("HEAT-INIT", 1, HspfStatusOptional)
            End If
            If ltable.Parms("SEDFG").Value = 1 Then
                aTableStatus.Change("SANDFG", 1, HspfStatusOptional)
                aTableStatus.Change("SED-GENPARM", 1, HspfStatusRequired)
                If ltable.Parms("HYDRFG").Value = 0 Then
                    aTableStatus.Change("SED-HYDPARM", 1, HspfStatusRequired)
                End If
                aTableStatus.Change("SAND-PM", 1, HspfStatusRequired)
                aTableStatus.Change("SILT-CLAY-PM", 1, HspfStatusRequired)
                aTableStatus.Change("SILT-CLAY-PM", 2, HspfStatusRequired)
                'added second occurrence  Technically, these tables are defaultable,
                'but not realistically.  I'd just as soon leave them Required  THJ
                aTableStatus.Change("SSED-INIT", 1, HspfStatusOptional)
                aTableStatus.Change("BED-INIT", 1, HspfStatusOptional)
            End If
            If ltable.Parms("GQALFG").Value = 1 Then
                aTableStatus.Change("GQ-GENDATA", 1, HspfStatusOptional)
                aTableStatus.Change("GQ-AD-FLAGS", 1, HspfStatusOptional)
                If aOperation.TableExists("GQ-GENDATA") Then
                    With aOperation.Tables.Item("GQ-GENDATA")
                        nGqual = .ParmValue("NGQUAL")
                        mWatemp = .ParmValue("TEMPFG")
                        mPhval = .ParmValue("PHFLAG")
                        mRoxygen = .ParmValue("ROXFG")
                        mCloud = .ParmValue("CLDFG")
                        mSedconc = .ParmValue("SDFG")
                        mPhoto = .ParmValue("PHYTFG")
                    End With
                Else 'defaults
                    nGqual = 1
                    mWatemp = 2
                    mPhval = 2
                    mRoxygen = 2
                    mCloud = 2
                    mSedconc = 2
                    mPhoto = 2
                End If
                Hydrolysis = False
                Oxidation = False
                Photolysis = False
                Volatilization = False
                Biodegradation = False
                nHydro = 0
                nOxid = 0
                nPhot = 0
                nVolat = 0
                nBiod = 0
                nBioM = 0
                nGenDec = 0
                nSedAs = 0
                For j = 1 To 6
                    Gdaufg(j) = 0
                Next j
                nDaught = 0
                For i = 1 To nGqual
                    aTableStatus.Change("GQ-QALDATA", i, HspfStatusRequired)
                    aTableStatus.Change("GQ-QALFG", i, HspfStatusOptional)
                    aTableStatus.Change("GQ-FLG2", i, HspfStatusOptional)
                    f = "GQ-FLG2"
                    If i > 1 Then f = f & ":" & i
                    'first parse flg2 to get dauther relationships and bio
                    If aOperation.TableExists(f) Then
                        With aOperation.Tables.Item(f)
                            Daufg(1) = .ParmValue("GQPM21")
                            Daufg(2) = .ParmValue("GQPM22")
                            Daufg(3) = .ParmValue("GQPM23")
                            Daufg(4) = .ParmValue("GQPM24")
                            Daufg(5) = .ParmValue("GQPM25")
                            Daufg(6) = .ParmValue("GQPM26")
                            For j = 1 To 5
                                If Daufg(j) = 1 Then
                                    Gdaufg(j) = 1
                                End If
                            Next j
                            mBio = .ParmValue("GQPM27")
                        End With
                    Else
                        mBio = 2
                    End If
                    'now can parse qalfg  THJ
                    t = "GQ-QALFG"
                    If i > 1 Then t = t & ":" & i
                    If aOperation.TableExists(t) Then
                        With aOperation.Tables.Item(t)
                            If .ParmValue("QALFG1") = 1 Then
                                nHydro = nHydro + 1
                                aTableStatus.Change("GQ-HYDPM", nHydro, HspfStatusRequired)
                                Hydrolysis = True
                            End If
                            If .ParmValue("QALFG2") = 1 Then
                                nOxid = nOxid + 1
                                aTableStatus.Change("GQ-ROXPM", nOxid, HspfStatusRequired)
                                Oxidation = True
                            End If
                            If .ParmValue("QALFG3") = 1 Then
                                nPhot = nPhot + 1
                                aTableStatus.Change("GQ-PHOTPM", nPhot, HspfStatusOptional)
                                Photolysis = True
                            End If
                            If .ParmValue("QALFG4") = 1 Then
                                nVolat = nVolat + 1
                                aTableStatus.Change("GQ-CFGAS", nVolat, HspfStatusRequired)
                                Volatilization = True
                            End If
                            If .ParmValue("QALFG5") = 1 Then
                                nBiod = nBiod + 1
                                aTableStatus.Change("GQ-BIOPM", nBiod, HspfStatusRequired)
                                Biodegradation = True
                                If mBio = 3 Then
                                    nBioM = nBioM + 1
                                    aTableStatus.Change("MON-BIO", nBioM, HspfStatusRequired)
                                End If
                            End If
                            If .ParmValue("QALFG6") = 1 Then
                                nGenDec = nGenDec + 1
                                aTableStatus.Change("GQ-GENDECAY", nGenDec, HspfStatusRequired)
                            End If
                            If .ParmValue("QALFG7") = 1 Then
                                nSedAs = nSedAs + 1
                                aTableStatus.Change("GQ-SEDDECAY", nSedAs, HspfStatusOptional)
                                aTableStatus.Change("GQ-KD", nSedAs, HspfStatusRequired)
                                aTableStatus.Change("GQ-ADRATE", nSedAs, HspfStatusRequired)
                                aTableStatus.Change("GQ-ADTHETA", nSedAs, HspfStatusOptional)
                                aTableStatus.Change("GQ-SEDCONC", nSedAs, HspfStatusOptional)
                            End If
                        End With
                    End If
                Next i

                aTableStatus.Change("GQ-VALUES", 1, HspfStatusOptional)
                'only one occurrence  THJ
                If mWatemp = 3 Then
                    aTableStatus.Change("MON-WATEMP", 1, HspfStatusOptional)
                End If
                If mPhval = 3 And Hydrolysis Then
                    aTableStatus.Change("MON-PHVAL", 1, HspfStatusOptional)
                End If
                If mRoxygen And Oxidation Then
                    aTableStatus.Change("MON-ROXYGEN", 1, HspfStatusOptional)
                    'corrected spelling of table name  THJ
                End If
                If Photolysis Then
                    aTableStatus.Change("GQ-ALPHA", 1, HspfStatusRequired)
                    aTableStatus.Change("GQ-GAMMA", 1, HspfStatusOptional)
                    aTableStatus.Change("GQ-DELTA", 1, HspfStatusOptional)
                    aTableStatus.Change("GQ-CLDFACT", 1, HspfStatusOptional)
                    If mCloud Then
                        aTableStatus.Change("MON-CLOUD", 1, HspfStatusOptional)
                    End If
                    If mSedconc Then
                        aTableStatus.Change("MON-SEDCONC", 1, HspfStatusOptional)
                    End If
                    If mPhoto Then
                        aTableStatus.Change("MON-PHYTO", 1, HspfStatusOptional)
                    End If
                    If ltable.Parms("HTFG").Value = 0 Then
                        aTableStatus.Change("SURF-EXPOSED", 1, HspfStatusOptional)
                    End If
                End If
                If Volatilization Then
                    aTableStatus.Change("OX-FLAGS", 1, HspfStatusOptional)
                    If aOperation.TableExists("OX-FLAGS") Then
                        Reamfg = aOperation.Tables.Item("OX-FLAGS").ParmValue("REAMFG")
                    Else
                        Reamfg = 2
                    End If
                    If ltable.Parms("HTFG").Value = 0 Then
                        aTableStatus.Change("ELEV", 1, HspfStatusOptional)
                        'added check on HTFG   THJ
                    End If
                    If lLakeFlag = 1 Then
                        aTableStatus.Change("OX-CFOREA", 1, HspfStatusOptional)
                        'added check on Lkfg   THJ
                    Else
                        If Reamfg = 1 Then
                            aTableStatus.Change("OX-TSIVOGLOU", 1, HspfStatusOptional)
                            If ltable.Parms("HYDRFG").Value = 0 Then
                                aTableStatus.Change("OX-LEN-DELTH", 1, HspfStatusRequired)
                            End If
                        ElseIf Reamfg = 2 Then
                            aTableStatus.Change("OX-TCGINV", 1, HspfStatusOptional)
                        ElseIf Reamfg = 3 Then
                            aTableStatus.Change("OX-REAPARM", 1, HspfStatusRequired)
                        End If
                    End If
                End If
                'reworked GQ-DAUGHTER   THJ
                For i = 1 To 6
                    If Gdaufg(i) = 1 Then
                        nDaught = nDaught + 1
                        aTableStatus.Change("GQ-DAUGHTER", nDaught, HspfStatusOptional)
                    End If
                Next i
            End If
            If ltable.Parms("OXFG").Value = 1 Then
                aTableStatus.Change("BENTH-FLAG", 1, HspfStatusOptional)
                If aOperation.TableExists("BENTH-FLAG") Then
                    Benrfg = aOperation.Tables.Item("BENTH-FLAG").ParmValue("BENRFG")
                Else
                    Benrfg = 0
                End If
                aTableStatus.Change("SCOUR-PARMS", 1, HspfStatusOptional)
                aTableStatus.Change("OX-FLAGS", 1, HspfStatusOptional)
                If aOperation.TableExists("OX-FLAGS") Then
                    Reamfg = aOperation.Tables.Item("OX-FLAGS").ParmValue("REAMFG")
                Else
                    Reamfg = 2
                End If
                If ltable.Parms("HTFG").Value = 0 Then
                    aTableStatus.Change("ELEV", 1, HspfStatusOptional)
                End If
                If lLakeFlag = 1 Then
                    aTableStatus.Change("OX-CFOREA", 1, HspfStatusOptional)
                Else
                    If Reamfg = 1 Then
                        aTableStatus.Change("OX-TSIVOGLOU", 1, HspfStatusOptional)
                        If ltable.Parms("HYDRFG").Value = 0 Then
                            aTableStatus.Change("OX-LEN-DELTH", 1, HspfStatusRequired)
                        End If
                    ElseIf Reamfg = 2 Then
                        aTableStatus.Change("OX-TCGINV", 1, HspfStatusOptional)
                    ElseIf Reamfg = 3 Then
                        aTableStatus.Change("OX-REAPARM", 1, HspfStatusRequired)
                    End If
                End If
                If Benrfg = 1 Then
                    aTableStatus.Change("OX-BENPARM", 1, HspfStatusOptional)
                End If
                aTableStatus.Change("OX-GENPARM", 1, HspfStatusRequired)
                aTableStatus.Change("OX-INIT", 1, HspfStatusOptional)
            End If
            If ltable.Parms("NUTFG").Value = 1 Then
                If ltable.Parms("PLKFG").Value = 1 Then
                    aTableStatus.Change("NUT-FLAGS", 1, HspfStatusRequired)
                Else
                    aTableStatus.Change("NUT-FLAGS", 1, HspfStatusOptional)
                End If
                If aOperation.TableExists("NUT-FLAGS") Then
                    With aOperation.Tables.Item("NUT-FLAGS")
                        Tamfg = .ParmValue("NH3FG")
                        Amvfg = .ParmValue("AMVFG")
                        Phflag = .ParmValue("PHFLAG")
                        Adnhfg = .ParmValue("ADNHFG")
                        Po4fg = .ParmValue("PO4FG")
                        Adpofg = .ParmValue("ADPOFG")
                    End With
                Else 'defaults
                    Tamfg = 0
                    Amvfg = 0
                    Phflag = 2
                    Adnhfg = 0
                    Po4fg = 0
                    Adpofg = 0
                End If
                aTableStatus.Change("NUT-AD-FLAGS", 1, HspfStatusOptional)
                aTableStatus.Change("CONV-VAL1", 1, HspfStatusOptional)
                If Benrfg = 1 Then
                    aTableStatus.Change("NUT-BENPARM", 1, HspfStatusOptional)
                End If
                aTableStatus.Change("NUT-NITDENIT", 1, HspfStatusRequired)
                If Tamfg = 1 And Amvfg = 1 Then
                    aTableStatus.Change("NUT-NH3VOLAT", 1, HspfStatusOptional)
                End If
                If Tamfg = 1 And Phflag = 3 Then
                    aTableStatus.Change("NUT-PHVAL", 1, HspfStatusOptional)
                End If
                If (Tamfg = 1 And Adnhfg = 1) Or (Po4fg = 1 And Adpofg = 1) Then
                    aTableStatus.Change("NUT-BEDCONC", 1, HspfStatusOptional)
                    aTableStatus.Change("NUT-ADSPARM", 1, HspfStatusOptional)
                    aTableStatus.Change("NUT-ADSINIT", 1, HspfStatusOptional)
                End If
                aTableStatus.Change("NUT-DINIT", 1, HspfStatusOptional)
            End If
            If ltable.Parms("PLKFG").Value = 1 Then
                aTableStatus.Change("PLNK-FLAGS", 1, HspfStatusRequired)
                If aOperation.TableExists("PLNK-FLAGS") Then
                    With aOperation.Tables.Item("PLNK-FLAGS")
                        Phyfg = .ParmValue("PHYFG")
                        Zoofg = .ParmValue("ZOOFG")
                        Balfg = .ParmValue("BALFG")
                    End With
                Else
                    Phyfg = 0
                    Zoofg = 0
                    Balfg = 0
                End If
                aTableStatus.Change("PLNK-AD-FLAGS", 1, HspfStatusOptional)
                If ltable.Parms("HTFG").Value = 0 Then
                    aTableStatus.Change("SURF-EXPOSED", 1, HspfStatusOptional)
                    'changed from required to optional - default is 1  THJ
                End If
                aTableStatus.Change("NUT-BENPARM", 1, HspfStatusOptional)
                'No, Really!!!!  I couldn't believe it!   THJ
                aTableStatus.Change("PLNK-PARM1", 1, HspfStatusRequired)
                aTableStatus.Change("PLNK-PARM2", 1, HspfStatusOptional)
                aTableStatus.Change("PLNK-PARM3", 1, HspfStatusOptional)
                If Phyfg = 1 Then
                    aTableStatus.Change("PHYTO-PARM", 1, HspfStatusRequired)
                    If Zoofg = 1 Then
                        aTableStatus.Change("ZOO-PARM1", 1, HspfStatusRequired)
                        aTableStatus.Change("ZOO-PARM2", 1, HspfStatusOptional)
                    End If
                End If
                If Balfg = 1 Then
                    aTableStatus.Change("BENAL-PARM", 1, HspfStatusOptional)
                End If
                aTableStatus.Change("PLNK-INIT", 1, HspfStatusOptional)
            End If
            If ltable.Parms("PHFG").Value = 1 Or _
               ltable.Parms("PHFG").Value = 3 Then
                aTableStatus.Change("PH-PARM1", 1, HspfStatusOptional)
                aTableStatus.Change("PH-PARM2", 1, HspfStatusOptional)
                aTableStatus.Change("PH-INIT", 1, HspfStatusOptional)
            End If

            lAcidph = False
            'check to see if acidph is available

            '.net conversion issue: Changedindex from {1,.Count} to {0, .Count - 1}
            For i = 0 To aOperation.Uci.Msg.BlockDefs("RCHRES").SectionDefs.Count - 1
                If aOperation.Uci.Msg.BlockDefs("RCHRES").SectionDefs(i).Name = "ACIDPH" Then
                    lAcidph = True
                End If
            Next i
            If lAcidph Then
                If ltable.Parms("PHFG").Value = 2 Or _
                   ltable.Parms("PHFG").Value = 3 Then
                    'acidph is on
                    aTableStatus.Change("ACID-FLAGS", 1, HspfStatusOptional)
                    aTableStatus.Change("ACID-PARMS", 1, HspfStatusOptional)
                    aTableStatus.Change("ACID-INIT", 1, HspfStatusOptional)
                End If
            End If
        End If
    End Sub
End Module