Option Strict Off
Option Explicit On

Imports atcData
Imports atcUtility
Imports atcUCI
Imports MapWinUtility
Imports System.Xml

''' <summary>
''' 
''' </summary>
''' <remarks>
'''Copyright 2005-2008 AQUA TERRA Consultants - Royalty-free use permitted under open source license
''' </remarks>
Public Class atcTimeseriesFileHspfBinOut
    Inherits atcData.atcTimeseriesSource

    Private pFilter As String = "HSPF Binary Output Files (*.hbn)|*.hbn"
    Private pName As String = "Timeseries::HSPF Binary Output"
    Private Shared pNaN As Double = GetNaN()
    Private Const pUnknownUnits As String = "<unknown>"

    Private pBinFile As HspfBinary

    Private pUnitsEnglish As New Generic.Dictionary(Of String, String)
    Private pUnitsTable As atcTable
    Private pUnitsTableModified As Boolean = False
    Private Shared pUnitsTableTemplate As atcTable
    Private pCountUnitsFound As Integer
    Private pCountUnitsMissing As Integer
    Private pCountUnitsHardCode As Integer
    Private pUnitsMissing As atcCollection
    Private Shared pHspfMsgUnits As Generic.Dictionary(Of String, String)
    Public DebugLevel As Integer = 1

    Public ReadOnly Property AvailableAttributes() As Collection
        Get
            'needed to edit attributes? that can't be done for this type!
            Return New Collection 'empty!
        End Get
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "File"
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "HSPF Binary Output"
        End Get
    End Property

    Public WriteOnly Property HelpFilename() As String
        Set(ByVal newValue As String)
            'TODO:how do we handle helpfiles?
            'App.HelpFile = newvalue
        End Set
    End Property

    Public ReadOnly Property Label() As String
        Get
            Return "HSPFBinary"
        End Get
    End Property

    Private Sub BuildTSers()
        pCountUnitsFound = 0
        pCountUnitsMissing = 0
        pCountUnitsHardCode = 0
        pUnitsMissing = New atcCollection

        pBinFile = New HspfBinary
        pBinFile.Filename = Specification

        Try
            Dim lNumBinHeaders As Integer = pBinFile.Headers.Count
            If DebugLevel > 0 Then
                Logger.Dbg(MemUsage)
                Logger.Dbg("Parse " & lNumBinHeaders & " Headers")
            End If
            Dim lFileAttributes As New atcDataAttributes
            With lFileAttributes
                .SetValue("CIntvl", True)
                .SetValue("History 1", "Read from " & pBinFile.Filename)
                Dim lFileDetails As System.IO.FileInfo = New System.IO.FileInfo(pBinFile.Filename)
                .SetValue("Date Created", lFileDetails.CreationTime)
                .SetValue("Date Modified", lFileDetails.LastWriteTime)
                .SetValue("IDSCEN", IO.Path.GetFileNameWithoutExtension(Specification))
                .SetValue("Data Source", Specification)
            End With
            Dim lHeaderIndex As Integer = 0

            For Each lBinHeader As HspfBinaryHeader In pBinFile.Headers
                With lBinHeader
                    'attributes related to dates common to all in ts in a header
                    'Dim lAllTimeseriesInThisHeader As New Generic.List(Of atcTimeseries)
                    Dim lBaseAttributes As New atcDataAttributes
                    With lBaseAttributes
                        'lSJDate = TimAddJ(lBinHeader.Data.Item(0).JDate, lTu, lTs, -lIntvl)
                        .SharedAttributes = lFileAttributes
                        .SetValue("SJDay", lBinHeader.Dates.Value(0)) 'Is end of first interval correct for SJDay?
                        .SetValue("EJDay", lBinHeader.Dates.Value(lBinHeader.Dates.numValues))
                        .SetValue("Time Step", lBinHeader.Dates.Attributes.GetValue("Time Step"))
                        .SetValue("Time Unit", lBinHeader.Dates.Attributes.GetValue("Time Unit"))
                        If lBinHeader.Dates.Attributes.ContainsAttribute("Interval") Then .SetValue("Interval", lBinHeader.Dates.Attributes.GetValue("Interval"))
                        .SetValue("Operation", lBinHeader.Id.OperationName)
                        .SetValue("Section", lBinHeader.Id.SectionName)
                        .SetValue("IDLOCN", Left(lBinHeader.Id.OperationName, 1) & ":" & (lBinHeader.Id.OperationNumber))
                        '.SetValue("InSameHeader", lAllTimeseriesInThisHeader)
                    End With
                    For Each lConstituent As String In lBinHeader.VarNames
                        Dim lTSer As atcTimeseries = New atcTimeseries(Me)
                        lTSer.Attributes.RemoveByKey("Data Source") 'By default this is set separately for each ts, but we are setting it in shared attributes
                        lTSer.Attributes.SharedAttributes = lBaseAttributes
                        With lTSer
                            .Attributes.SetValue("IDCONS", lConstituent)
                            .Attributes.SetValue("UNITS", GetUnits(lConstituent, pBinFile.UnitSystem))
                            .Attributes.SetValue("ID", Me.DataSets.Count + 1)
                            If lConstituent = "LZS" Then 'TODO: need better check here
                                .Attributes.SetValue("Point", True)
                            End If
                            .ValuesNeedToBeRead = True
                            '.SetInterval(lTu, lTs)
                            .Dates = lBinHeader.Dates
                            AddDataSet(lTSer)
                            'lAllTimeseriesInThisHeader.Add(lTSer)
                        End With
                    Next
                End With
                lHeaderIndex += 1
                'Logger.Dbg("Loop " & i)
                Logger.Progress(lNumBinHeaders / 2 + lHeaderIndex / 2, lNumBinHeaders)
            Next
        Catch ex As ApplicationException
            Logger.Dbg(ex.Message)
        End Try
        If DebugLevel > 0 Then
            Logger.Dbg("Created " & DataSets.Count & " Datasets")
        End If

        If pUnitsTableModified Then
            With pUnitsTable
                Try
                    Logger.Dbg("SaveLocalUnitsTable:" & .FileName)
                    .WriteFile(.FileName)
                Catch exWriteUnits As Exception
                    Logger.Dbg("Could not save units table: " & exWriteUnits.Message)
                End Try
            End With
        End If
        Logger.Dbg("Units Assigned: " & pCountUnitsFound & ", Found in local dbf: " & pCountUnitsHardCode)
        Logger.Dbg("Units Missing: Unique: " & pUnitsMissing.Count & ", Total: " & pCountUnitsMissing)
        If DebugLevel > 1 Then
            For lIndex As Integer = 0 To pUnitsMissing.Count - 1
                Logger.Dbg("Missing " & pUnitsMissing.Keys(lIndex) & " " & pUnitsMissing.Item(lIndex))
            Next
        End If
    End Sub

    Private Function GetUnits(ByVal aConstituent As String, Optional ByVal aUnitSystem As atcUnitSystem = atcUnitSystem.atcEnglish) As String
        Dim lUnits As String = ""
        Dim lUnknownFlag As Boolean = False

        Dim lUnitsKey As String = aConstituent.ToLower
        If pUnitsEnglish.ContainsKey(lUnitsKey) Then
            Return pUnitsEnglish.Item(lUnitsKey)
        End If

        If pUnitsTable.FindFirst(1, aConstituent) Then 'cons in field 1
            lUnits = pUnitsTable.Value(2) 'units in field 2
            If lUnits = pUnknownUnits Then
                lUnknownFlag = True
                lUnits = "" 'forces another look at generic tables
            Else 'found a good one
                pCountUnitsHardCode += 1
            End If
        End If

        If lUnits.Length = 0 Then 'try generic unit table stored with dll - file may be customized for a specific installation
            If pUnitsTableTemplate Is Nothing Then 'open the generic table
                pUnitsTableTemplate = New atcTableDBF
                Dim lUnitsTemplateFileName As String = IO.Path.ChangeExtension(Reflection.Assembly.GetExecutingAssembly.Location, "units.dbf")
                If FileExists(lUnitsTemplateFileName) Then
                    pUnitsTableTemplate.OpenFile(lUnitsTemplateFileName)
                    Logger.Dbg("Using Units Template File:" & lUnitsTemplateFileName)
                Else
                    Logger.Dbg("Units Template File Not Found:" & lUnitsTemplateFileName)
                End If
            End If
            If pUnitsTableTemplate.FindFirst(1, aConstituent) Then 'cons in field 1
                lUnits = pUnitsTableTemplate.Value(2) 'units in field 2
                pCountUnitsFound += 1
            Else  'try HspfMsg
                If pHspfMsgUnits Is Nothing Then
                    Try
                        pHspfMsgUnits = New Generic.Dictionary(Of String, String)
                        Dim pHspfMsg As New atcUCI.HspfMsg
                        pHspfMsg.Open("hspfmsg.wdm")
                        For lTsGroupIndex As Integer = 0 To pHspfMsg.TSGroupDefs.Count - 1
                            Dim lTsGroup As atcUCI.HspfTSGroupDef = pHspfMsg.TSGroupDefs(lTsGroupIndex)
                            For lTsMemberIndex As Integer = 0 To lTsGroup.MemberDefs.Count - 1
                                Dim lTsMember As atcUCI.HspfTSMemberDef = lTsGroup.MemberDefs(lTsMemberIndex)
                                'todo: check english/metric flag, english assumed for now
                                If pHspfMsgUnits.ContainsKey(lTsMember.Name.ToLower) Then
                                    If pHspfMsgUnits.Item(lTsMember.Name.ToLower) <> lTsMember.EUnits Then
                                        If DebugLevel > 1 Then
                                            Logger.Dbg("UnitsConflict " & pHspfMsgUnits.Item(lTsMember.Name.ToLower).ToString & ":" & lTsMember.EUnits)
                                        End If
                                    End If
                                Else
                                    pHspfMsgUnits.Add(lTsMember.Name.ToLower, lTsMember.EUnits)
                                End If
                            Next
                        Next
                    Catch exHspfMsg As Exception
                        Logger.Dbg("Exception getting units from hspfmsg: " & exHspfMsg.ToString)
                    End Try
                    If DebugLevel > 1 Then
                        Logger.Dbg("MessageFileUnitCount " & pHspfMsgUnits.Count)
                    End If
                End If

                If pHspfMsgUnits.ContainsKey(lUnitsKey) Then
                    lUnits = pHspfMsgUnits.Item(lUnitsKey).ToString
                    If lUnits.Length > 0 Then pCountUnitsFound += 1
                End If

                If lUnits.Length = 0 Then
                    pCountUnitsMissing += 1
                    'Logger.Dbg("Missing " & aConstituent)
                    pUnitsMissing.Increment(aConstituent, 1)
                    lUnits = pUnknownUnits
                End If
            End If
            If Not lUnknownFlag Then 'save a local copy
                With pUnitsTable
                    .NumRecords += 1
                    .MoveLast()
                    .Value(1) = aConstituent
                    .Value(2) = lUnits
                    pUnitsTableModified = True
                End With
            End If
        End If
        pUnitsEnglish.Add(lUnitsKey, lUnits)
        Return lUnits
    End Function

    Public Overrides Sub ReadData(ByVal aDataSet As atcDataSet)

        Dim lTimeseries As atcTimeseries = aDataSet
        'Dim lShared As atcDataAttributes = lTimeseries.Attributes.SharedAttributes
        'If lShared Is Nothing Then 'Should not happen, all should have SharedAttributes
        '    lShared = lTimeseries.Attributes
        'End If
        lTimeseries.ValuesNeedToBeRead = False
        Dim lKey As String = KeyFromAttributes(lTimeseries.Attributes)
        Dim lNeedToClose As Boolean = pBinFile.Open(False)
        'Dim lNeedDates As Boolean = lTimeseries.Dates.numValues < 1 'Do not already have Dates assigned to this timeseries
        Dim lValues As New Generic.List(Of Double)
        'Dim lJDates As Generic.List(Of Double) = Nothing
        'If lNeedDates Then lJDates = New Generic.List(Of Double)
        'Dim lTimeUnit As atcTimeUnit = atcTimeUnit.TUUnknown
        'Dim lTimeStep As Integer = 1
        'Dim lInterval As Double = pNaN
        Try
            Dim lBinHeader As HspfBinaryHeader = pBinFile.Headers(lKey)
            With lBinHeader
                Dim lVariableIndex As Integer = .VarNames.IndexOf(lTimeseries.Attributes.GetValue("IDCONS"))
                If lVariableIndex >= 0 Then
                    Dim lSJday As Double = lTimeseries.Attributes.GetValue("SJDay")
                    Dim lEJday As Double = lTimeseries.Attributes.GetValue("EJDay")
                    'If lNeedDates Then lJDates.Add(lSJday)
                    lValues.Add(pNaN)
                    Dim lDateIndex As Integer = 0
                    For Each lValuesStartPosition As Long In .ValuesStartPosition
                        lDateIndex += 1
                        Dim lCurJday As Double = .Dates.Value(lDateIndex)
                        If lCurJday >= lSJday Then
                            If lCurJday > lEJday Then
                                Exit For
                            End If
                            lValues.Add(pBinFile.ReadValue(lValuesStartPosition, lVariableIndex))
                            'If lNeedDates Then
                            '    lJDates.Add(lCurJday)
                            '    If lJDates.Count = 3 Then 'Compute Interval and beginning of first interval
                            '        CalcTimeUnitStep(lJDates(1), lJDates(2), lTimeUnit, lTimeStep)
                            '        If lTimeUnit <> atcTimeUnit.TUUnknown Then
                            '            lJDates(0) = TimAddJ(lJDates(1), lTimeUnit, lTimeStep, -1)
                            '            lShared.SetValue("Time Step", lTimeStep)
                            '            lShared.SetValue("Time Unit", lTimeUnit)
                            '            lInterval = CalcInterval(lTimeUnit, lTimeStep)
                            '            If Double.IsNaN(lInterval) Then
                            '                lShared.RemoveByKey("Interval")
                            '            Else
                            '                lShared.SetValue("Interval", lInterval)
                            '            End If
                            '        End If
                            '    End If
                            'End If
                        End If
                    Next
                Else
                    Logger.Dbg("Could not retrieve HSPF Binary data values for variable: " & lTimeseries.Attributes.GetValue("IDCONS"))
                End If
            End With
        Catch ex As Exception
            Logger.Dbg("Could not retrieve data values for HSPF Binary TSER" & "Key = " & lKey & vbCrLf & _
                       "Message:" & ex.ToString)
        Finally
            If lNeedToClose Then pBinFile.Close(False)
        End Try
        With lTimeseries
            'If lNeedDates Then
            '    Dim lNumValues As Integer = lJDates.Count - 1
            '    'Search for existing Dates that exactly match these so we can share that one and save memory
            '    Dim lMatchingDates As atcTimeseries = Nothing
            '    For Each lOtherTimeseries As atcTimeseries In DataSets
            '        If Not lOtherTimeseries.ValuesNeedToBeRead Then
            '            Dim lOtherDates As atcTimeseries = lOtherTimeseries.Dates
            '            If lOtherDates.Serial <> .Dates.Serial AndAlso lOtherDates.numValues = lNumValues Then
            '                lMatchingDates = lOtherDates
            '                For lDateIndex As Integer = lNumValues To 1 Step -1
            '                    If lOtherDates.Value(lDateIndex) <> lJDates(lDateIndex) Then
            '                        lMatchingDates = Nothing
            '                        Exit For
            '                    End If
            '                Next
            '                If lMatchingDates IsNot Nothing Then Exit For
            '            End If
            '        End If
            '    Next
            '    If lMatchingDates Is Nothing Then
            '        'Found a new set of dates, use these in .Dates (which is already shared by all timeseries in same Header)
            '        .Dates.Values = lJDates.ToArray
            '    Else 'Found matching Dates from another header, share them with all timeseries sharing this header
            '        Dim lDisposingDates As atcTimeseries = .Dates
            '        Try
            '            Dim lInSameHeader As Generic.List(Of atcTimeseries) = lTimeseries.Attributes.SharedAttributes.GetValue("InSameHeader")
            '            If lInSameHeader IsNot Nothing Then
            '                For Each lOtherTimeseries As atcTimeseries In lInSameHeader
            '                    lOtherTimeseries.Dates = lMatchingDates
            '                    'All others in lAll should already have these shared attributes set above
            '                    'If lTimeUnit <> atcTimeUnit.TUUnknown Then
            '                    '    Dim lShared As atcDataAttributes = lOtherTimeseries.Attributes.SharedAttributes
            '                    '    If lShared Is Nothing Then 'Should not happen, all should have SharedAttributes
            '                    '        lOtherTimeseries.SetInterval(lTimeUnit, lTimeStep)
            '                    '    Else
            '                    '        lShared.SetValue("Time Step", lTimeStep)
            '                    '        lShared.SetValue("Time Unit", lTimeUnit)
            '                    '        If Double.IsNaN(lInterval) Then
            '                    '            lShared.RemoveByKey("Interval")
            '                    '        Else
            '                    '            lShared.SetValue("Interval", lInterval)
            '                    '        End If
            '                    '    End If
            '                    'End If
            '                Next
            '            End If
            '        Catch ex As Exception
            '            Logger.Dbg("Exception while trying to share Dates with all in same header: " & ex.ToString)
            '        End Try
            '        lDisposingDates.Clear()
            '    End If
            'End If

            'Setting .Values below will clear calculated attributes, but we want to preserve the ones we have already set such as start/end date.
            Dim lPreserveCalculated As New Generic.List(Of atcDefinedValue)
            For Each lAttribute As atcDefinedValue In .Attributes
                If lAttribute.Definition.Calculated Then
                    lPreserveCalculated.Add(lAttribute)
                End If
            Next

            .Values = lValues.ToArray

            For Each lAttribute As atcDefinedValue In lPreserveCalculated
                .Attributes.Add(lAttribute)
            Next

            'If (.Attributes.GetValue("Point", False)) Then
            '    .Values(0) = pNaN
            'Else
            '    .Values(0) = lValues(1)
            'End If
            .ValuesNeedToBeRead = False
        End With
        'atcDataManager.AddDiscardableTimeseries(lTimeseries)
    End Sub

    Private Function KeyFromAttributes(ByVal aAttributes As atcDataAttributes) As String
        Return aAttributes.GetValue("Operation", "unk") & ":" _
             & aAttributes.GetValue("IDLOCN", "u:unk").ToString.Substring(2) & ":" _
             & aAttributes.GetValue("Section", "unk")
    End Function

    Public Sub Refresh()
        pBinFile.ReadNewRecords()
    End Sub

    Public Function RemoveTimSer(ByVal aTimeseries As atcTimeseries) As Boolean
        Throw New ApplicationException("Unable to Remove Time Series " & aTimeseries.ToString & vbCrLf & "From:" & Specification)
    End Function

    Public Function RewriteTimSer(ByVal aTimeseries As atcTimeseries) As Boolean
        Throw New ApplicationException("Unable to Rewrite Time Series " & aTimeseries.ToString & vbCrLf & "From:" & Specification)
    End Function

    Public Function SaveAs(ByVal aFilename As String) As Boolean
        Throw New ApplicationException("Unable to SaveAs " & aFilename & vbCrLf & "From:" & Specification)
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return pName
        End Get
    End Property

    Public Overrides ReadOnly Property CanOpen() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanSave() As Boolean
        Get
            Return False 'TODO: change this when we can
        End Get
    End Property

    Public Overrides Function Open(ByVal aFileName As String, Optional ByVal aAttributes As atcDataAttributes = Nothing) As Boolean
        If DebugLevel > 0 Then Logger.Dbg("Opening " & aFileName)
        If MyBase.Open(aFileName, aAttributes) Then
            Logger.Status("Opening " & aFileName)
            pUnitsTable = New atcTableDBF
            Dim lFileName As String = IO.Path.ChangeExtension(Me.Specification, "units.dbf")
            If FileExists(lFileName) Then
                pUnitsTable.OpenFile(lFileName)
                If DebugLevel > 0 Then Logger.Dbg("UsingUnitsFile " & lFileName & " WithRecordCount " & pUnitsTable.NumRecords)
            Else 'create from template
                With pUnitsTable
                    .NumFields = 2
                    .FieldName(1) = "CONS"
                    .FieldType(1) = "C"
                    .FieldLength(1) = 32
                    .FieldName(2) = "UNITS"
                    .FieldType(2) = "C"
                    .FieldLength(2) = 16
                    .FileName = lFileName
                End With
                If DebugLevel > 0 Then Logger.Dbg("Start With Empty Units (" & lFileName & " not found)")
            End If
            BuildTSers()
            Return True
        End If
        Return False
    End Function

    Public Sub New()
        Filter = pFilter
    End Sub

    Private Shared pShowViewMessage As Boolean = True
    Public Overrides Sub View()
        If pShowViewMessage Then
            Select Case Logger.MsgCustom(Specification & vbCrLf & "No text viewer available for this file", "View", _
                                         "Ok", "Show File Folder", "Stop showing this message")
                Case "Show File Folder"
                    OpenFile(IO.Path.GetDirectoryName(Specification))
                Case "Stop showing this message"
                    pShowViewMessage = False
            End Select
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class