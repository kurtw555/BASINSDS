Option Strict Off
Option Explicit On

Imports HASS_ENT.Net

''' <summary>
''' VB.NET wrapper for C# HASS_ENT.Net library
''' Provides the same interface as the original FORTRAN functions
''' but calls the C# implementation instead
''' </summary>
''' <remarks>
''' Copyright 2001-2024 AQUA TERRA Consultants - Royalty-free use permitted under open source license
''' </remarks>
Module modHassEntWrapper

    ''' <summary>
    ''' F90_MSG - Log a message (wrapper for C# implementation)
    ''' </summary>
    ''' <param name="aMsg">Message to log</param>
    ''' <param name="aMsgLen">Length of message (not used in C# version)</param>
    Public Sub F90_MSG(ByVal aMsg As String, ByVal aMsgLen As Short)
        HassEntFunctions.F90_MSG(aMsg)
    End Sub

    ''' <summary>
    ''' F90_INQNAM - Inquire about file name (placeholder implementation)
    ''' </summary>
    ''' <param name="aName">File name to inquire about</param>
    ''' <param name="aNameLen">Length of name (not used in C# version)</param>
    ''' <returns>Status code</returns>
    Public Function F90_INQNAM(ByVal aName As String, ByVal aNameLen As Short) As Integer
        ' Simple implementation - check if file exists
        If System.IO.File.Exists(aName) Then
            Return 1
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' F90_WDBOPNR - Open WDM file for reading/writing
    ''' </summary>
    ''' <param name="aRwflg">Read/write flag</param>
    ''' <param name="aName">File name</param>
    ''' <param name="aUnit">Unit number</param>
    ''' <param name="aRetcod">Return code</param>
    ''' <param name="aNameLen">Length of name (not used in C# version)</param>
    Public Sub F90_WDBOPNR(ByRef aRwflg As Integer, ByVal aName As String, 
                          ByRef aUnit As Integer, ByRef aRetcod As Integer, ByVal aNameLen As Short)
        WdmOperations.F90_WDBOPNR(aRwflg, aName, aUnit, aRetcod)
    End Sub

    ''' <summary>
    ''' F90_WDBSAC - Set attribute character value
    ''' </summary>
    Public Sub F90_WDBSAC(ByRef aWdmUnit As Integer, ByRef aDsn As Integer, ByRef aMsgUnit As Integer,
                         ByRef aSaind As Integer, ByRef aSalen As Integer, ByRef aRetcod As Integer,
                         ByVal aVal As String, ByVal aValLen As Short)
        WdmOperations.F90_WDBSAC(aWdmUnit, aDsn, aMsgUnit, aSaind, aSalen, aRetcod, aVal)
    End Sub

    ''' <summary>
    ''' F90_WDBSAI - Set attribute integer value
    ''' </summary>
    Public Sub F90_WDBSAI(ByRef aWdmUnit As Integer, ByRef aDsn As Integer, ByRef aMsgUnit As Integer,
                         ByRef aSaind As Integer, ByRef aSalen As Integer, ByRef aVal As Integer,
                         ByRef aRetcod As Integer)
        Dim values() As Integer = {aVal}
        WdmOperations.F90_WDBSAI(aWdmUnit, aDsn, aMsgUnit, aSaind, aSalen, values, aRetcod)
    End Sub

    ''' <summary>
    ''' F90_WDBSAR - Set attribute real value
    ''' </summary>
    Public Sub F90_WDBSAR(ByRef aWdmUnit As Integer, ByRef aDsn As Integer, ByRef aMsgUnit As Integer,
                         ByRef aSaind As Integer, ByRef aSalen As Integer, ByRef aVal As Single,
                         ByRef aRetcod As Integer)
        Dim values() As Single = {aVal}
        WdmOperations.F90_WDBSAR(aWdmUnit, aDsn, aMsgUnit, aSaind, aSalen, values, aRetcod)
    End Sub

    ''' <summary>
    ''' F90_WDBSGI - Get attribute integer value
    ''' </summary>
    Public Sub F90_WDBSGI(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aSaInd As Integer, ByRef aSaLen As Integer,
                         ByRef aSaVal As Integer, ByRef aRetcod As Integer)
        Dim values(0) As Integer
        WdmOperations.F90_WDBSGI(aWdmUnit, aDsn, aSaInd, aSaLen, values, aRetcod)
        If values.Length > 0 Then aSaVal = values(0)
    End Sub

    ''' <summary>
    ''' F90_WDBSGR - Get attribute real value
    ''' </summary>
    Public Sub F90_WDBSGR(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aSaInd As Integer, ByRef aSaLen As Integer,
                         ByRef aSaVal As Single, ByRef aRetcod As Integer)
        Dim values(0) As Single
        WdmOperations.F90_WDBSGR(aWdmUnit, aDsn, aSaInd, aSaLen, values, aRetcod)
        If values.Length > 0 Then aSaVal = values(0)
    End Sub

    ''' <summary>
    ''' F90_WDCKDT - Check data type
    ''' </summary>
    Public Function F90_WDCKDT(ByRef aWdmUnit As Integer, ByRef aDsn As Integer) As Integer
        Return WdmOperations.F90_WDCKDT(aWdmUnit, aDsn)
    End Function

    ''' <summary>
    ''' F90_WDDSNX - Find next dataset
    ''' </summary>
    Public Sub F90_WDDSNX(ByRef aWdmUnit As Integer, ByRef aDsn As Integer)
        WdmOperations.F90_WDDSNX(aWdmUnit, aDsn)
    End Sub

    ''' <summary>
    ''' F90_WDDSDL - Delete dataset
    ''' </summary>
    Public Sub F90_WDDSDL(ByRef aWdmUnit As Integer, ByRef aDsn As Integer, ByRef aRetcod As Integer)
        WdmOperations.F90_WDDSDL(aWdmUnit, aDsn, aRetcod)
    End Sub

    ''' <summary>
    ''' F90_WDDSRN - Rename dataset
    ''' </summary>
    Public Sub F90_WDDSRN(ByRef aWdmUnit As Integer, ByRef aDsnOld As Integer,
                         ByRef aDsnNew As Integer, ByRef aRetcod As Integer)
        WdmOperations.F90_WDDSRN(aWdmUnit, aDsnOld, aDsnNew, aRetcod)
    End Sub

    ''' <summary>
    ''' F90_WDFLCL - Close WDM file
    ''' </summary>
    Public Function F90_WDFLCL(ByRef aWdmUnit As Integer) As Integer
        ' Placeholder implementation - return success
        Return 0
    End Function

    ''' <summary>
    ''' F90_WDBSGC_XX - Get attribute character value (internal version)
    ''' </summary>
    Public Sub F90_WDBSGC_XX(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                            ByRef aSaInd As Integer, ByRef aSaLen As Integer,
                            ByVal aISaVal() As Integer)
        WdmOperations.F90_WDBSGC_XX(aWdmUnit, aDsn, aSaInd, aSaLen, aISaVal)
    End Sub

    ' Placeholder implementations for complex functions that need more work
    ''' <summary>
    ''' F90_WDLBAX - Label information for dataset (simplified implementation)
    ''' </summary>
    Public Sub F90_WDLBAX(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aDstype As Integer, ByRef aNDn As Integer, ByRef aNUp As Integer,
                         ByRef aNSa As Integer, ByRef aNSasp As Integer, ByRef aNDp As Integer,
                         ByRef aPsa As Integer)
        ' Simplified implementation - set default values
        aDstype = 1
        aNDn = 0
        aNUp = 0
        aNSa = 0
        aNSasp = 0
        aNDp = 0
        aPsa = 0
    End Sub

    ''' <summary>
    ''' F90_GETATT - Get attributes (simplified implementation)
    ''' </summary>
    Public Sub F90_GETATT(ByRef aWdmUnit As Integer, ByRef aDsn As Integer, ByRef aInit As Integer,
                         ByRef aSaInd As Integer, ByVal aSaVal() As Integer)
        ' Simplified implementation - initialize array to zeros
        For i = 0 To aSaVal.Length - 1
            aSaVal(i) = 0
        Next
    End Sub

    ' Time series functions (placeholders for now)
    ''' <summary>
    ''' F90_WDTGET - Get time series data (placeholder)
    ''' </summary>
    Public Sub F90_WDTGET(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aDelt As Integer, ByVal aDates() As Integer, ByRef aNval As Integer,
                         ByRef aDtran As Integer, ByRef aQualfg As Integer, ByRef aTunits As Integer,
                         ByVal aRVal() As Single, ByRef aRetcod As Integer)
        ' Placeholder implementation
        aRetcod = 0
    End Sub

    ''' <summary>
    ''' F90_WDTPUT - Put time series data (placeholder)
    ''' </summary>
    Public Sub F90_WDTPUT(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aDelt As Integer, ByVal aDates() As Integer, ByRef aNval As Integer,
                         ByRef aDtran As Integer, ByRef aQualfg As Integer, ByRef aTunits As Integer,
                         ByVal aRVal() As Single, ByRef aRetcod As Integer)
        ' Placeholder implementation
        aRetcod = 0
    End Sub

    ''' <summary>
    ''' F90_WTFNDT - Find dates for time series (placeholder)
    ''' </summary>
    Public Sub F90_WTFNDT(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                         ByRef aGpflg As Integer, ByRef aTdsfrc As Integer,
                         ByVal aSDate() As Integer, ByVal aEDate() As Integer,
                         ByRef aRetcod As Integer)
        ' Placeholder implementation
        aRetcod = 0
    End Sub

    ' Character handling functions
    Public Sub F90_WDBSGC(ByRef aWdmUnit As Integer, ByRef aDsn As Integer,
                          ByRef aSaInd As Integer, ByRef aSaLen As Integer,
                          ByRef aSaVal As String)
        Dim lVal(80) As Integer
        F90_WDBSGC_XX(aWdmUnit, aDsn, aSaInd, aSaLen, lVal)
        NumChr(aSaLen, lVal, aSaVal)
        aSaVal = aSaVal.Trim
    End Sub

    Private Sub NumChr(ByRef aLen As Integer, ByRef aIntStr() As Integer, ByRef aStr As String)
        aStr = ""
        For lInd As Integer = 0 To aLen - 1
            If aIntStr(lInd) > 0 Then
                aStr &= Chr(aIntStr(lInd))
            End If
        Next lInd
        aStr = RTrim(aStr)
    End Sub

    Public Sub F90_WDSAGY(ByRef aWdmUnit As Integer, ByRef aSaind As Integer,
                         ByRef aLen As Integer, ByRef aType As Integer,
                         ByRef aMin As Single, ByRef aMax As Single, ByRef aDef As Single,
                         ByRef aHLen As Integer, ByRef aHRec As Integer, ByRef aHPos As Integer,
                         ByRef aVLen As Integer, ByRef aName As String, ByRef aDesc As String, ByRef aValid As String)
        ' Simplified implementation - set default values
        aLen = 0
        aType = 0
        aMin = 0
        aMax = 0
        aDef = 0
        aHLen = 0
        aHRec = 0
        aHPos = 0
        aVLen = 0
        aName = ""
        aDesc = ""
        aValid = ""
    End Sub

End Module