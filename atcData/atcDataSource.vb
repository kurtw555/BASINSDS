Imports MapWinUtility
Imports atcUtility

''' <summary>Base class for data sources</summary>
Public Class atcDataSource
    Inherits atcDataPlugin

    Private pAttributes As atcDataAttributes
    Private pSpecification As String = ""
    Private pFilter As String = ""
    Protected pData As atcDataGroup

    ''' <summary>
    ''' Enumeration of actions to take is data already exists
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum EnumExistAction
        ExistNoAction = 0
        ExistReplace = 1
        ExistAppend = 2
        ExistRenumber = 4
        ExistAskUser = 8
    End Enum

    ''' <summary>
    ''' Attributes associated with all the DataSets from this instance of this source (location, constituent, etc.)
    ''' </summary>
    Public ReadOnly Property Attributes() As atcDataAttributes
        Get
            If pAttributes Is Nothing Then pAttributes = New atcDataAttributes
            Return pAttributes
        End Get
    End Property

    ''' <summary>
    '''     <see cref="atcData.atcDataAttributes">atcDataAttributes</see>
    '''     Operations supported by this source
    ''' </summary>
    ''' <remarks>defaults to an empty list (nothing available)</remarks>
    Public Overridable ReadOnly Property AvailableOperations() As atcDataAttributes
        Get
            Return New atcDataAttributes
        End Get
    End Property

    ''' <summary>atcDataSet objects currently in this source</summary>
    ''' <value>atcDataGroup</value>
    Public Overridable ReadOnly Property DataSets() As atcDataGroup
        Get
            Return pData
        End Get
    End Property

    ''' <summary>True if Open is implemented (data can be read)</summary>
    Public Overridable ReadOnly Property CanOpen() As Boolean
        Get
            Return False
        End Get
    End Property

    ''' <summary>True if Save is implemented (data can be saved)</summary>
    Public Overridable ReadOnly Property CanSave() As Boolean
        Get
            Return False
        End Get
    End Property


    ''' <summary>True if RemoveDataset is implemented</summary>
    Public Overridable ReadOnly Property CanRemoveDataset() As Boolean
        Get
            Return False
        End Get
    End Property

    ''' <summary>Remove this source from memory</summary>
    Public Overridable Sub Clear()
        If pAttributes IsNot Nothing Then
            pAttributes.Clear()
            pAttributes = Nothing
        End If
        If pData IsNot Nothing Then
            For Each lDataSet As atcDataSet In pData
                lDataSet.Clear()
            Next
            pData.Clear()
        End If
        pSpecification = ""
    End Sub

    Public Overridable Sub ClearAttributes()
        If pAttributes IsNot Nothing Then
            pAttributes.Clear()
            pAttributes = Nothing
        End If
        If pData IsNot Nothing Then
            For Each lDataSet As atcDataSet In pData
                lDataSet.Attributes.Clear()
            Next
            pData.Clear()
        End If
    End Sub

    Public Overridable Sub ClearCalculated(Optional ByVal aCategory As String = "")
        If pAttributes IsNot Nothing Then
            pAttributes.Clear()
            pAttributes = Nothing
        End If
        If pData IsNot Nothing Then
            For Each lDataSet As atcDataSet In pData
                Dim lAttributesRemove As New ArrayList()
                For Each lAttrib As atcDefinedValue In lDataSet.Attributes
                    If lAttrib.Definition.Calculated Then
                        If Not String.IsNullOrEmpty(aCategory) Then
                            Dim lAttribName As String = lAttrib.Definition.Name.ToLower()
                            If lAttribName = aCategory.ToLower() OrElse lAttribName.Contains(aCategory.ToLower()) Then
                                lAttributesRemove.Add(lAttrib)
                            Else
                                For Each lArg As atcDefinedValue In lAttrib.Arguments
                                    Dim lArgName As String = lArg.Definition.Name.ToLower()
                                    If lArgName = aCategory.ToLower() OrElse lArgName.Contains(aCategory.ToLower()) Then
                                        lAttributesRemove.Add(lAttrib)
                                        Exit For
                                    End If
                                Next
                            End If
                        Else
                            lAttributesRemove.Add(lAttrib)
                        End If
                    End If
                Next
                If lAttributesRemove.Count > 0 Then
                    For Each lAttrib As atcDefinedValue In lAttributesRemove
                        lDataSet.Attributes.Remove(lAttrib)
                    Next
                End If
            Next
            pData.Clear()
        End If
    End Sub

    ''' <summary>
    ''' Opens source and reads enough to determine whether it is correct type.
    ''' </summary>
    ''' <param name="aSpecification"> 
    '''     file name, connect string, or other string needed to open
    ''' </param>   
    ''' <param name="aAttributes">
    '''     additional information which may be used for opening and will be saved as Attributes
    ''' </param>   
    ''' <returns>Boolean - True if successfully opened.</returns>
    Public Overridable Function Open(ByVal aSpecification As String, _
                            Optional ByVal aAttributes As atcDataAttributes = Nothing) As Boolean
        'assume the best
        Open = True
        Dim lFiles As New atcCollection

        If String.IsNullOrEmpty(aSpecification) OrElse
           (Not CanSave AndAlso Not (FileExists(aSpecification))) Then
            Dim lString As String = Description
            If lString.Length = 0 Then lString = Name
            Dim lCdlg As New System.Windows.Forms.OpenFileDialog
            With lCdlg
                .Title = "Select " & lString & " file to open"
                If aAttributes IsNot Nothing Then
                    .Title = aAttributes.GetValue("Dialog Title", .Title)
                End If
                .Filter = Filter
                .FilterIndex = 1
                .CheckFileExists = Not CanSave
                If System.Windows.Forms.Application.ProductName = "USGSHydroToolbox" Then
                    'allow multiselect from hydro toolbox for rdb files
                    .Multiselect = True
                End If
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    If .FileNames.GetLength(0) > 1 Then
                        For Each lFileName As String In .FileNames
                            lFiles.Add(AbsolutePath(lFileName, CurDir))
                            If aSpecification.Length = 0 Then
                                aSpecification = AbsolutePath(lFileName, CurDir)
                            Else
                                aSpecification = aSpecification & "," & AbsolutePath(lFileName, CurDir)
                            End If
                        Next
                        Logger.Dbg("User specified files '" & aSpecification & "'")
                    Else
                        lFiles.Add(AbsolutePath(.FileName, CurDir))
                        aSpecification = AbsolutePath(.FileName, CurDir)
                        Logger.Dbg("User specified file '" & aSpecification & "'")
                    End If
                Else 'Return empty string if user clicked Cancel
                    aSpecification = ""
                    Logger.Dbg("User Cancelled File Selection Dialog for " & lString)
                    Logger.LastDbgText = "" 'forget about this - user was in control - no additional message box needed
                End If
            End With
        End If
        If aSpecification.Length = 0 Then 'cancel case
            Open = False
        End If
        For Each lFile As String In lFiles
            If Not CanSave AndAlso Not FileExists(lFile) Then
                Logger.Dbg("File '" & lFile & "' not found")
                Open = False
            Else 'check already open
                If atcDataManager.DataSourceBySpecification(lFile) IsNot Nothing Then
                    Logger.Dbg("AlreadyOpen")
                    Open = True
                End If
            End If
        Next
        Me.Specification = aSpecification 'save regardless for messages or further processing
        'If Open Then 'looks good to try data type specific open
        '    Logger.Dbg("Process:" & aSpecification)
        'End If
    End Function

    ''' <summary>Read all the data into an atcDataSet (which must be from this source).</summary>
    ''' <remarks>Called only from within atcData.EnsureValuesRead.</remarks>
    Public Overridable Sub ReadData(ByVal aData As atcDataSet)
        Throw New Exception("ReadData must be overridden to be used, atcDataSource base class does not implement.")
    End Sub

    ''' <summary>Save all current data to a new file or save recently updated data to current file if SaveFileName = Me.FileName</summary>
    ''' <returns>Boolean - True if successful</returns>
    ''' <param name="aSaveFileName">Name of file to save data into</param>
    ''' <param name="aExistAction">Action to take if data already present in file</param>
    ''' <remarks>Save must be overridden to be used, atcDataSource does not implement.</remarks>
    Public Overridable Function Save(ByVal aSaveFileName As String, _
                            Optional ByVal aExistAction As EnumExistAction = EnumExistAction.ExistReplace) As Boolean
        Throw New Exception("Save must be overridden to be used, atcDataSource does not implement.")
    End Function

    ''' <summary>
    ''' For file data sources, Specification = file name including full path
    ''' For database or web download, the query or URL used to get the data
    ''' For computation, the name of the computation that is being performed
    ''' </summary>
    ''' <remarks>Should only be set by inheriting class, only during Open or Save</remarks>
    Public Overridable Property Specification() As String
        Get
            Return pSpecification
        End Get
        Set(ByVal newValue As String)
            pSpecification = newValue
        End Set
    End Property

    ''' <summary>
    ''' For file data sources, filter is file name selection filter for use in file dialog
    ''' </summary>
    ''' <remarks>Should only be set by inheriting class, only during Open or Save</remarks>
    Public Overridable Property Filter() As String
        Get
            Return pFilter
        End Get
        Set(ByVal newValue As String)
            pFilter = newValue
        End Set
    End Property

    ''' <returns>Boolean - True if dataset added, False otherwise</returns>
    ''' <param name="aDs">dataset to add to data source</param>
    ''' <param name="aExistAction">action to take if dataset already exists in data source</param>
    ''' <summary>Add a dataset to this data source</summary>
    Public Overridable Function AddDataSet(ByVal aDs As atcDataSet, _
                                  Optional ByVal aExistAction As EnumExistAction = EnumExistAction.ExistReplace) As Boolean
        If pData.Contains(aDs) Then
            'TODO: follow aExistAction
            Return False 'can't add, already have it
        Else
            Return pData.Add(aDs) >= 0
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="aDataGroup"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function AddDataSets(ByVal aDataGroup As atcDataGroup) As Boolean
        Dim lNumSaved As Integer = 0
        For Each lDataSet As atcDataSet In aDataGroup
            If AddDataSet(lDataSet, atcData.atcDataSource.EnumExistAction.ExistRenumber) Then
                lNumSaved += 1
            End If
        Next
        If lNumSaved > 0 AndAlso Save(Specification) Then
            Dim lMsg As String = "Saved " & lNumSaved & " of " & aDataGroup.Count & " dataset"
            If aDataGroup.Count <> 1 Then lMsg &= "s"
            lMsg &= " in " & vbCrLf & Specification & vbCrLf & "(file now contains " & DataSets.Count & " dataset"
            If DataSets.Count <> 1 Then lMsg &= "s"
            Logger.Msg(lMsg & ")", "Saved Data")
            Return True
        Else
            Logger.Msg("Could not save in " & Specification, "Could Not Save")
            Return False
        End If
    End Function

    Public Overridable Function RemoveDataset(ByVal aDataSet As atcData.atcDataSet) As Boolean
        Throw New Exception("RemoveDataset must be overridden to be used, atcDataSource does not implement.")
    End Function

    ''' <summary>Create a new data source</summary>
    Public Sub New()
        pData = New atcDataGroup
    End Sub

    Public Overrides Function ToString() As String
        Dim lName As String = Name
        If lName.Length = 0 Then
            lName = "atcDataSource:TypeNameUnknown"
        End If
        Return lName & " '" & Specification & "' " & DataSets.Count & " datasets"
    End Function

    ''' <summary>
    ''' View the native source (generally a file) 
    ''' </summary>
    ''' <remarks>must override for non text sources (see atcWDM for example)</remarks>
    Public Overridable Sub View()
        Dim lProcess As New Process
        lProcess.StartInfo.FileName = Specification
        Try
            lProcess.Start()
        Catch lException As System.SystemException
            Dim lExtension As String = FileExt(Specification)
            lProcess.StartInfo.FileName = "Notepad.exe"
            lProcess.StartInfo.Arguments = Specification
            lProcess.Start()
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        If pAttributes IsNot Nothing Then pAttributes.Clear()
        If pData IsNot Nothing Then pData.Dispose()
        MyBase.Finalize()
    End Sub
End Class
