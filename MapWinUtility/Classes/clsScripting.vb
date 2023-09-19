'********************************************************************************************************
'File Name: clsScripting.vb
'Description: A class providing scripting capability to MapWindow and plugins.
'********************************************************************************************************
'The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
'you may not use this file except in compliance with the License. You may obtain a copy of the License at 
'http://www.mozilla.org/MPL/ 
'Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
'ANY KIND, either express or implied. See the License for the specificlanguage governing rights and 
'limitations under the License. 
'
'The Original Code is MapWindow Open Source Utility Library. 
'
'The Initial Developer of this version of the Original Code is Christopher Michaelis, done
'by reshifting and moving about the various utility functions from MapWindow's modPublic.vb
'(which no longer exists) and some useful utility functions from Aqua Terra Consulting.
'
'Contributor(s): (Open source contributors should list themselves and their modifications here). 
'09/02/2010 - KW - Modified class to work with MW4 and MW6/DotSpatial
'********************************************************************************************************
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.IO

Public Class Scripting

    ''' <summary>
    ''' Unicode prefix sometimes gets included in a string, we have to remove it before interpreting as a filename or script
    ''' </summary>
    Private Shared ByteOrderMarker As String = Chr(239) & Chr(187) & Chr(191)
    Private Shared Sub RemoveByteOrderMarker(ByRef aString As String)
        If aString IsNot Nothing AndAlso aString.StartsWith(ByteOrderMarker) Then
            aString = aString.Substring(ByteOrderMarker.Length)
        End If
    End Sub

    ''' <summary>
    ''' Create a new assembly containing the compiled script
    ''' </summary>
    ''' <param name="aLanguage">Language code is written in. Valid values are "vb", "cs", "js".
    ''' Alternatively, "dll" will load an already-compiled assembly from file named aCode instead of compiling.</param>
    ''' <param name="aDLLfilename">Save compiled assembly in this file</param>
    ''' <param name="aCode">Source code or the name of a file containing source code</param>
    ''' <param name="aErrors">Error messages are returned in this string</param>
    ''' <param name="aPluginFolder">Folder to compile into if creating a plugin</param>
    ''' <returns>an assembly built by compiling aCode or the contents of the file named aCode.</returns>
    ''' <remarks>Output assembly can be loaded by the appropriate Plugin manager from the calling code.</remarks>
    Public Shared Function PrepareScript(ByVal aLanguage As String, _
                        ByVal aDLLfilename As String, _
                        ByVal aCode As String, _
                        ByRef aErrors As String, _
                        ByVal aPluginFolder As String) As Assembly

        Logger.Dbg("In PrepareScript")

        aErrors = "" 'No errors yet
        aLanguage = GetLanguageFromFilename(aLanguage)
        Dim lAssembly As System.Reflection.Assembly

        RemoveByteOrderMarker(aCode)

        If aLanguage = "dll" Then
            lAssembly = System.Reflection.Assembly.LoadFrom(aCode)
            If aDLLfilename Is Nothing OrElse aDLLfilename.Length = 0 Then
                aDLLfilename = MakeScriptName(aPluginFolder)
            End If
        Else 'compile the code into an assembly
            Try
                If System.IO.File.Exists(aCode) Then
                    aCode = Strings.WholeFileString(aCode)
                Else
                    Dim lCodeFile As String = PathNameOnly(aPluginFolder) & "\scripts\" & aCode
                    If System.IO.File.Exists(lCodeFile) Then
                        aCode = Strings.WholeFileString(lCodeFile)
                    End If
                End If
            Catch ex As Exception
                'Treat as code, not as file name
            End Try

            RemoveByteOrderMarker(aCode)
            If String.IsNullOrEmpty(aDLLfilename) Then
                aDLLfilename = MakeScriptName(aPluginFolder)
            End If
            lAssembly = Compile(aLanguage, aCode, aErrors, aDLLfilename)
        End If

        Return lAssembly
    End Function

    ''' <summary>
    ''' Run the ScriptMain method in aAssembly 
    ''' </summary>
    ''' <returns>return value of ScriptMain</returns>
    Public Shared Function Run(ByVal aAssembly As Assembly, _
                               ByRef aErrors As String, _
                               ByVal ParamArray aArgs() As Object) As Object

        Dim lAssembly As Assembly = aAssembly
        Dim lMethodName As String = "ScriptMain" 'Can't be MAIN or the C# compiler will have a heart attack.
        Dim lAssemblyTypes As Type() 'list of items within the assembly

        Logger.Dbg("In Scripting.Run")

        If aErrors Is Nothing OrElse aErrors.Length = 0 Then
            lAssemblyTypes = lAssembly.GetTypes()
            For Each lAssemblyType As Type In lAssemblyTypes
                Dim lScriptMethodInfo As MethodInfo = lAssemblyType.GetMethod(lMethodName)
                If Not lScriptMethodInfo Is Nothing Then
                    Logger.Dbg("Invoke:" & lScriptMethodInfo.Name)
                    If aArgs Is Nothing Then
                        Logger.Dbg("No Args")
                    Else
                        For Each lArg As Object In aArgs
                            If Not (lArg Is Nothing) Then
                                Logger.Dbg("Arg:" & lArg.GetType.ToString & ":<" & lArg.ToString & ">")
                            End If
                        Next
                    End If
                    Try
                        Return lScriptMethodInfo.Invoke(Nothing, aArgs) 'assy.CreateInstance(typ.Name)
                    Catch ex As Exception
                        Logger.Dbg("Exception:" & ex.ToString)
                        Return False
                    End Try
                    If Not lScriptMethodInfo Is Nothing Then
                        Return lScriptMethodInfo.Invoke(Nothing, aArgs) 'assy.CreateInstance(typ.Name)
                    End If
                End If
            Next
            aErrors = "Scripting.Run: '" & lMethodName & "' not found"
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Backward-compatible version of Run with older API
    ''' </summary>
    ''' <returns>result of running script</returns>
    Public Shared Function Run(ByVal aLanguage As String, _
                               ByVal aDLLfilename As String, _
                               ByVal aCode As String, _
                               ByRef aErrors As String, _
                               ByVal aPlugIn As Boolean, _
                               ByRef m_MapWin As Object, _
                               ByVal ParamArray aArgs() As Object) As Object
        Dim lFolder As String = Nothing
        Try
            If m_MapWin IsNot Nothing Then
                lFolder = m_MapWin.Plugins.PluginFolder
            End If
        Catch
        End Try
        If lFolder Is Nothing OrElse Not IO.Directory.Exists(lFolder) Then
            lFolder = CurDir()
        End If
        If IO.Directory.Exists(IO.Path.Combine(lFolder, "BASINS")) Then
            lFolder = IO.Path.Combine(lFolder, "BASINS")
        End If

        Dim lAssembly As System.Reflection.Assembly = PrepareScript(aLanguage, aDLLfilename, aCode, aErrors, lFolder)
        Return Run(lAssembly, aErrors, aArgs)
    End Function

    Public Shared Function MakeScriptName(ByVal aPluginFolder As String) As String
        Dim lTryName As String
        Dim lTryCount As Integer = 1
        Dim lScriptName As String = "RemoveMe-Script-"
        Do
            lTryName = IO.Path.Combine(aPluginFolder, lScriptName & lTryCount & ".dll")
            lTryCount += 1
        Loop While System.IO.File.Exists(lTryName)
        Return lTryName
    End Function

    ''' <summary>
    ''' Compile code into an assembly
    ''' </summary>
    ''' <param name="aLanguage">Language code is written in. Valid values are "vb", "cs", "js"</param>
    ''' <param name="aCode"></param>
    ''' <param name="aErrors"></param>
    ''' <param name="aOutputFilename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Compile(ByVal aLanguage As String, _
                                   ByVal aCode As String, _
                                   ByRef aErrors As String, _
                          Optional ByVal aOutputFilename As String = "") As System.Reflection.Assembly
        Dim lCompilerParameters As CompilerParameters
        Dim lCompilerResults As CompilerResults
        Dim lCodeDomProvider As CodeDomProvider
        Dim lNeedSupportCode As Boolean = False
        Dim lSupportCode As String = ""

        aLanguage = GetLanguageFromFilename(aLanguage)

        Logger.Dbg("Compiling: " + aLanguage)

        Select Case aLanguage
            Case "cs"
                lCodeDomProvider = New Microsoft.CSharp.CSharpCodeProvider
                'Paul Meems - 2010/09/13: Use FrameWork v3.5 so we can use the C#3.0 options:
                Dim lProviderOptions As New System.Collections.Generic.Dictionary(Of String, String)()
                lProviderOptions.Add("CompilerVersion", "v3.5")
                lCodeDomProvider = New Microsoft.CSharp.CSharpCodeProvider(lProviderOptions)
                If aCode.IndexOf("using ") < 0 Then lNeedSupportCode = True
                'Case "js" : provider = Activator.CreateInstance("Microsoft.JScript", "Microsoft.JScript.JScriptCodeProvider").Unwrap()
            Case "vb"
                lCodeDomProvider = New Microsoft.VisualBasic.VBCodeProvider
                If aCode.IndexOf("Public ") < 0 Then lNeedSupportCode = True
            Case Else : lCodeDomProvider = New Microsoft.VisualBasic.VBCodeProvider
        End Select

        lCompilerParameters = New System.CodeDom.Compiler.CompilerParameters

        'jlk&mg - 2010/09 - explicitly load assemblies that scripts are likely to need
        Dim lAssemblyNames() As String = {"MapWinGeoProc", "MapWinUtility", "MapWinInterfaces", "Zedgraph", _
                                            "DotSpatial.Common", "DotSpatial.Data", "DotSpatial.Desktop", _
                                            "DotSpatial.Projections", "DotSpatial.Topology"}

        For Each lAssemblyName As String In lAssemblyNames
            Dim lAssemblyFileName As String = IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), lAssemblyName & ".dll")
            If IO.File.Exists(lAssemblyFileName) Then
                lCompilerParameters.ReferencedAssemblies.Add(lAssemblyFileName)
            End If
        Next

        If aOutputFilename Is Nothing OrElse aOutputFilename.Length = 0 Then
            lCompilerParameters.GenerateInMemory = True      'Assembly is created in memory
        Else
            lCompilerParameters.OutputAssembly = aOutputFilename
        End If
        lCompilerParameters.TreatWarningsAsErrors = False
        lCompilerParameters.WarningLevel = 4
        'params.ReferencedAssemblies.AddRange(refs)

        Dim lAssemblyAdded As New ArrayList
        For Each lReferenceAssembly As System.Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies()
            Dim lAssemblyName As String = Strings.StrSplit(lReferenceAssembly.FullName, ",", "")
            Select Case lAssemblyName
                Case "mscorlib", _
                     "mwIdentifier", _
                     "TableEditor.mw", _
                     "MapWindow.resources", _
                     "MapWinGeoProc", _
                     "MapWinUtility", _
                     "ZedGraph", _
                     "MapWinInterfaces", _
                     "log4net", _
                     "System.Windows.Forms.Ribbon35"
                Case Else
                    'Chris M July 2006 -- Don't add 'RemoveMe' or resources with "" as the location.
                    If Not lAssemblyName.Contains("MapWindow") And Not lAssemblyName.Contains("RemoveMe") Then
                        'Chris M Jan 1 2006 -- don't add localized satellite assemblies
                        If Not lAssemblyName.EndsWith(".resources") Then
                            If lAssemblyName.StartsWith("RemoveMe") Then
                                'Don't add temporary assemblies
                            ElseIf Not lReferenceAssembly.Location.Trim() = "" Then
                                If lNeedSupportCode Then
                                    Select Case aLanguage
                                        Case "vb" : lSupportCode &= "Imports " & lAssemblyName & vbCrLf
                                        Case "cs" : lSupportCode &= "using " & lAssemblyName & ";" & vbCrLf
                                    End Select
                                End If
                                If Not lAssemblyAdded.Contains(lAssemblyName) Then
                                    lCompilerParameters.ReferencedAssemblies.Add(lReferenceAssembly.Location)
                                    lAssemblyAdded.Add(lAssemblyName)
                                End If
                            End If
                        End If
                    End If
            End Select
        Next

        If aLanguage.ToLower = "vb" And lNeedSupportCode Then
            aCode = lSupportCode & vbCrLf _
                                 & "Public Module ScriptModule" & vbCrLf _
                                 & "  Public Sub ScriptMain(ByVal aDataManager As atcDataManager, ByVal aBasinsPlugIn As Object)" & vbCrLf _
                                 & aCode & vbCrLf _
                                 & "  End Sub" & vbCrLf _
                                 & "End Module"
        End If

        'MsgBox(aCode)

        Try
            lCompilerResults = lCodeDomProvider.CompileAssemblyFromSource(lCompilerParameters, aCode)
            If lCompilerResults.Errors.Count = 0 Then        'No compile errors or warnings
                Return lCompilerResults.CompiledAssembly
            Else
                For Each lCompilerError As CompilerError In lCompilerResults.Errors
                    aErrors &= (String.Format( _
                        "Line {0}, Col {1}: Error {2} - {3}", _
                        lCompilerError.Line, lCompilerError.Column, _
                        lCompilerError.ErrorNumber, lCompilerError.ErrorText)) & vbCrLf
                Next
            End If
        Catch ex As Exception
            'Compile errors don't throw exceptions. This is a deeper problem
            aErrors &= ex.Message
        End Try

        If Not aErrors Is Nothing AndAlso aErrors.Length > 0 Then
            Logger.Dbg(aErrors)
        End If

        Return Nothing

    End Function

    Public Shared Function GetLanguageFromFilename(ByVal aFilename As String) As String
        'Paul Meems 3 aug 2009: Added, because somehow .cs wasn't changed in cs:
        aFilename = aFilename.Trim(".")

        If aFilename.StartsWith(".") Then
            aFilename.Remove(0, 1)
        End If
        Return aFilename.ToLower()
    End Function
End Class
