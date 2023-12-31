Imports System.Windows.Forms
Imports System.Drawing
Imports atcUtility
Imports MapWinUtility

Friend Class frmManager
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        AddHandler atcDataManager.OpenedData, AddressOf ChangedData
        AddHandler atcDataManager.ClosedData, AddressOf ChangedData
        'treeFiles.DrawMode = TreeViewDrawMode.Normal
        treeFiles.DrawMode = TreeViewDrawMode.OwnerDrawText

        Dim lDisplayPlugins As ICollection = atcDataManager.GetPlugins(GetType(atcDataDisplay))
        For Each lDataDisplay As atcDataDisplay In lDisplayPlugins
            Dim lMenuText As String = lDataDisplay.Name
            If lMenuText.StartsWith("Analysis::") Then lMenuText = lMenuText.Substring(10)
            AnalysisToolStripMenuItem.DropDownItems.Add(lMenuText, Nothing, New EventHandler(AddressOf AnalysisToolStripMenuItem_Click))
        Next
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                RemoveHandler atcDataManager.OpenedData, AddressOf ChangedData
                RemoveHandler atcDataManager.ClosedData, AddressOf ChangedData
                If components IsNot Nothing Then
                    components.Dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents txtDetails As System.Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveInToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AnalysisToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewFileAsTextToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents RemoveDataFromFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshSelectedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents treeFiles As System.Windows.Forms.TreeView
    'Friend WithEvents panelOpening As System.Windows.Forms.Panel
    'Friend WithEvents lstDataSourceType As System.Windows.Forms.ListBox
    'Friend WithEvents lblDataSourceType As System.Windows.Forms.Label
    'Friend WithEvents btnOpen As System.Windows.Forms.Button
    'Friend WithEvents btnCancel As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmManager))
        Me.txtDetails = New System.Windows.Forms.TextBox()
        Me.treeFiles = New System.Windows.Forms.TreeView()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.ViewFileAsTextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveDataFromFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveInToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.RefreshSelectedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnalysisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtDetails
        '
        Me.txtDetails.AllowDrop = True
        Me.txtDetails.BackColor = System.Drawing.SystemColors.ControlLight
        Me.txtDetails.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.txtDetails.Location = New System.Drawing.Point(0, 236)
        Me.txtDetails.Multiline = True
        Me.txtDetails.Name = "txtDetails"
        Me.txtDetails.Size = New System.Drawing.Size(658, 73)
        Me.txtDetails.TabIndex = 1
        '
        'treeFiles
        '
        Me.treeFiles.AllowDrop = True
        Me.treeFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.treeFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.treeFiles.HideSelection = False
        Me.treeFiles.Location = New System.Drawing.Point(0, 25)
        Me.treeFiles.Name = "treeFiles"
        Me.treeFiles.Size = New System.Drawing.Size(658, 206)
        Me.treeFiles.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.AnalysisToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(658, 27)
        Me.MenuStrip1.TabIndex = 2
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.toolStripSeparator, Me.ViewFileAsTextToolStripMenuItem, Me.ShowFolderToolStripMenuItem, Me.RemoveDataFromFileToolStripMenuItem, Me.ToolStripSeparator3, Me.SaveInToolStripMenuItem, Me.toolStripSeparator1, Me.CloseToolStripMenuItem, Me.CloseAllToolStripMenuItem, Me.toolStripSeparator2, Me.RefreshSelectedToolStripMenuItem, Me.RefreshAllToolStripMenuItem, Me.toolStripSeparator4, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(45, 23)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Image = CType(resources.GetObject("NewToolStripMenuItem.Image"), System.Drawing.Image)
        Me.NewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.NewToolStripMenuItem.Text = "New"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(256, 6)
        '
        'ViewFileAsTextToolStripMenuItem
        '
        Me.ViewFileAsTextToolStripMenuItem.Name = "ViewFileAsTextToolStripMenuItem"
        Me.ViewFileAsTextToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.ViewFileAsTextToolStripMenuItem.Text = "View File As Text"
        '
        'ShowFolderToolStripMenuItem
        '
        Me.ShowFolderToolStripMenuItem.Name = "ShowFolderToolStripMenuItem"
        Me.ShowFolderToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.ShowFolderToolStripMenuItem.Text = "Show File Folder"
        '
        'RemoveDataFromFileToolStripMenuItem
        '
        Me.RemoveDataFromFileToolStripMenuItem.Name = "RemoveDataFromFileToolStripMenuItem"
        Me.RemoveDataFromFileToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.RemoveDataFromFileToolStripMenuItem.Text = "Remove Data From File"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(256, 6)
        '
        'SaveInToolStripMenuItem
        '
        Me.SaveInToolStripMenuItem.Image = CType(resources.GetObject("SaveInToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SaveInToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SaveInToolStripMenuItem.Name = "SaveInToolStripMenuItem"
        Me.SaveInToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.SaveInToolStripMenuItem.Text = "Save In..."
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(256, 6)
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.CloseToolStripMenuItem.Text = "Close Selected"
        '
        'CloseAllToolStripMenuItem
        '
        Me.CloseAllToolStripMenuItem.Name = "CloseAllToolStripMenuItem"
        Me.CloseAllToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.CloseAllToolStripMenuItem.Text = "Close All"
        '
        'toolStripSeparator2
        '
        Me.toolStripSeparator2.Name = "toolStripSeparator2"
        Me.toolStripSeparator2.Size = New System.Drawing.Size(256, 6)
        '
        'RefreshSelectedToolStripMenuItem
        '
        Me.RefreshSelectedToolStripMenuItem.Name = "RefreshSelectedToolStripMenuItem"
        Me.RefreshSelectedToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.RefreshSelectedToolStripMenuItem.Text = "Refresh Selected"
        '
        'RefreshAllToolStripMenuItem
        '
        Me.RefreshAllToolStripMenuItem.Name = "RefreshAllToolStripMenuItem"
        Me.RefreshAllToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.RefreshAllToolStripMenuItem.Text = "Refresh All"
        '
        'toolStripSeparator4
        '
        Me.toolStripSeparator4.Name = "toolStripSeparator4"
        Me.toolStripSeparator4.Size = New System.Drawing.Size(256, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(259, 24)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'AnalysisToolStripMenuItem
        '
        Me.AnalysisToolStripMenuItem.Name = "AnalysisToolStripMenuItem"
        Me.AnalysisToolStripMenuItem.Size = New System.Drawing.Size(79, 23)
        Me.AnalysisToolStripMenuItem.Text = "Analysis"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(53, 23)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'frmManager
        '
        Me.AllowDrop = True
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(658, 309)
        Me.Controls.Add(Me.treeFiles)
        Me.Controls.Add(Me.txtDetails)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmManager"
        Me.Text = "Data Sources"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private pDelayPopulate As Boolean = False

    Public Sub Edit(Optional ByVal aNodeKey As Integer = -1)
        Populate(aNodeKey)
        Me.Show()
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub Populate(ByVal aNodeKey As Integer)

        If Application.ProductName = "USGSHydroToolbox" Then
            RemoveDataFromFileToolStripMenuItem.Visible = False
        End If

        If Not pDelayPopulate Then
            treeFiles.Nodes.Clear()
            SaveInToolStripMenuItem.DropDownItems.Clear()
            txtDetails.Text = ""
            Dim lName As String = ""
            Dim lCount As String = 0
            Dim lNode As TreeNode
            For Each lDataSource As atcTimeseriesSource In atcDataManager.DataSources
                Dim lTypeAndSource() As String = lDataSource.Name.Split(":")
                Dim lSourceName As String = "Unknown"
                If lTypeAndSource.GetUpperBound(0) > 1 Then
                    lSourceName = lTypeAndSource(2)
                End If

                'TODO: leave code for lTypeAndSource(0) - now only Timeseries, might want to add Model or Table later
                'If Not treeFiles.Nodes.ContainsKey(lTypeAndSource(0)) Then
                '    treeFiles.Nodes.Add(lTypeAndSource(0), lTypeAndSource(0))
                'End If
                'Dim lSourceNodes As TreeNodeCollection = treeFiles.Nodes(lTypeAndSource(0)).Nodes
                'If Not lSourceNodes.ContainsKey(lSourceName) Then
                '    lSourceNodes.Add(lSourceName, lSourceName)
                'End If
                If Not treeFiles.Nodes.ContainsKey(lSourceName) Then
                    lNode = treeFiles.Nodes.Add(lSourceName, lSourceName)
                    lNode.BackColor = treeFiles.BackColor
                    lNode.ForeColor = treeFiles.ForeColor
                End If

                lName = lDataSource.Specification & " (" & lDataSource.DataSets.Count & ")"
                'treeFiles.Nodes(lTypeAndSource(0)).Nodes(lSourceName).Nodes.Add(lCount, lName)
                lNode = treeFiles.Nodes(lSourceName).Nodes.Add(lCount, lName)
                lNode.BackColor = treeFiles.BackColor
                lNode.ForeColor = treeFiles.ForeColor

                If lCount = aNodeKey Then
                    'treeFiles.SelectedNode = treeFiles.Nodes(lTypeAndSource(0)).Nodes(lSourceName).Nodes(lCount)
                    treeFiles.SelectedNode = treeFiles.Nodes(lSourceName).Nodes(lCount)
                    treeFiles.SelectedNode.EnsureVisible()
                    RefreshDetails(aNodeKey)
                End If
                If lDataSource.CanSave Then
                    SaveInToolStripMenuItem.DropDownItems.Add(lDataSource.Specification, Nothing, AddressOf SaveInHandler)
                End If
                lCount += 1
            Next
            If atcDataManager.DataSources.Count = 0 Then
                SelectedNodes.Clear()
            End If
            treeFiles.ExpandAll()
            treeFiles.Refresh()
        End If
    End Sub

    Private Sub SaveInHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim lSaveInSpecification As String = sender.Text
        Dim lSaveIn As atcTimeseriesSource = Nothing
        Dim lSaveGroup As atcTimeseriesGroup = atcDataManager.UserSelectData("Select Data to Save", SelectedTimeseries, Nothing, True, True, Me.Icon)
        If lSaveGroup IsNot Nothing AndAlso lSaveGroup.Count > 0 Then

            'If we already have specified data source open, skip asking user
            If Not String.IsNullOrEmpty(lSaveInSpecification) Then
                lSaveIn = atcDataManager.DataSourceBySpecification(lSaveInSpecification)
            End If

            If lSaveIn Is Nothing Then
                lSaveIn = atcDataManager.UserOpenDataFile(False, True)
            End If

            If lSaveIn IsNot Nothing AndAlso lSaveIn.Specification.Length > 0 Then
                If lSaveIn.AddDataSets(lSaveGroup) Then
                    Populate(-1)
                End If
            End If
        End If
    End Sub

    Private Sub SelectionAction(ByVal aAction As String)
        Dim lSources As Generic.List(Of atcTimeseriesSource) = SelectedSources()
        Select Case lSources.Count
            Case 0
                If treeFiles.Nodes.Count = 0 Then
                    Logger.Msg("No data sources to " & aAction, aAction & " Problem")
                Else
                    Logger.Msg("Choose at least one data source to " & aAction & " before choosing the action", aAction & " Problem")
                End If
            Case 1
                DoAction(aAction, lSources(0), False)
            Case Is > 1
                'If Logger.Msg(aAction & " " & lSources.Count & " sources?", vbYesNo, "More than one data source affected") = MsgBoxResult.Yes Then
                If aAction.StartsWith("Analysis:") Then
                    Dim lAllSelectedSources As New atcTimeseriesSource
                    For Each lDataSource As atcDataSource In lSources
                        lAllSelectedSources.DataSets.AddRange(lDataSource.DataSets)
                    Next
                    DoAction(aAction, lAllSelectedSources, True)
                ElseIf aAction = "ShowFolder" Then
                    Dim lDirectoryNames As New Generic.List(Of String)
                    For Each lDataSource As atcDataSource In lSources
                        If Not String.IsNullOrEmpty(lDataSource.Specification) Then
                            Dim lDirectoryName As String = IO.Path.GetDirectoryName(lDataSource.Specification).ToLower
                            If Not lDirectoryNames.Contains(lDirectoryName) AndAlso IO.Directory.Exists(lDirectoryName) Then
                                lDirectoryNames.Add(lDirectoryName)
                            End If
                        End If
                    Next
                    'Open a file or URL using the default method the system would have used if it was double-clicked
                    For Each lDirectoryName As String In lDirectoryNames
                        OpenFile(lDirectoryName)
                    Next
                Else
                    pDelayPopulate = True
                    For Each lDataSource As atcDataSource In lSources
                        DoAction(aAction, lDataSource, True)
                    Next
                    pDelayPopulate = False
                    Populate(-1)
                End If
                'End If
        End Select
    End Sub

    Private Function SelectedTimeseries() As atcTimeseriesGroup
        Dim lTSgroup As New atcTimeseriesGroup
        Dim lSources As Generic.List(Of atcTimeseriesSource) = SelectedSources()
        For Each lSource As atcTimeseriesSource In lSources
            lTSgroup.AddRange(lSource.DataSets)
        Next
        Return lTSgroup
    End Function

    Private Function SelectedSources() As Generic.List(Of atcTimeseriesSource)
        Dim lSources As New Generic.List(Of atcTimeseriesSource)
        For Each lSelectedNode As TreeNode In SelectedNodes
            Dim lDataSourceIndex As Integer
            If Integer.TryParse(lSelectedNode.Name, lDataSourceIndex) Then
                lSources.Add(atcDataManager.DataSources.Item(lDataSourceIndex))
            Else
                If lSelectedNode.Nodes IsNot Nothing Then
                    Select Case lSelectedNode.Nodes.Count
                        Case 1
                            If Integer.TryParse(lSelectedNode.Nodes(0).Name, lDataSourceIndex) Then
                                lSources.Add(atcDataManager.DataSources.Item(lDataSourceIndex))
                            End If
                        Case Is > 1
                            Dim lNodes As New Generic.List(Of Integer)
                            For Each lNode As TreeNode In lSelectedNode.Nodes
                                If Integer.TryParse(lNode.Name, lDataSourceIndex) Then
                                    lSources.Insert(0, atcDataManager.DataSources.Item(lDataSourceIndex))
                                End If
                            Next
                    End Select
                End If
            End If
        Next
        Return lSources
    End Function

    Private Sub DoAction(ByVal aAction As String, ByVal aDataSource As atcDataSource, ByVal aMultipleSelected As Boolean)
        With aDataSource
            Logger.Dbg(aAction & ":" & .Specification)
            Dim lActionArgs() As String = aAction.Split(":")
            Select Case lActionArgs(0)
                Case "Close"
                    atcDataManager.RemoveDataSource(aDataSource)
                    pSelectedNodes.Clear()
                    Populate(-1)
                Case "Refresh"
                    Dim lSpecification As String = aDataSource.Specification
                    atcDataManager.RemoveDataSource(aDataSource)
                    atcDataManager.OpenDataSource(lSpecification)
                Case "View"
                    .View()
                Case "ShowFolder"
                    If Not String.IsNullOrEmpty(.Specification) Then
                        Dim lDirectoryName As String = IO.Path.GetDirectoryName(.Specification)
                        If IO.Directory.Exists(lDirectoryName) Then
                            OpenFile(lDirectoryName)
                        End If
                    End If
                Case "Analysis"
                    Dim lSelected As atcTimeseriesGroup = .DataSets
                    If Not aMultipleSelected Then
                        lSelected = lSelected.Clone
                    End If
                    lSelected = atcDataManager.UserSelectData("Select data to " & lActionArgs(1), lSelected, Nothing, True, False, Me.Icon)
                    If lSelected IsNot Nothing AndAlso lSelected.Count > 0 Then
                        Dim lDisplayName As String = lActionArgs(1)
                        'Re-assemble display name if nested category was disassembled by Split above
                        For lNextPart As Integer = 2 To lActionArgs.GetUpperBound(0)
                            lDisplayName &= ":" & lActionArgs(lNextPart)
                        Next
                        atcDataManager.ShowDisplay(lDisplayName, lSelected, Me.Icon)
                    End If
                Case "RemoveDatasets"
                    If .CanRemoveDataset Then
                        Dim lDataGroup As atcDataGroup = atcDataManager.UserSelectData( _
                            "Select Datasets to remove from " & .Specification, Nothing, .DataSets.Clone, True, True, Me.Icon)
                        If lDataGroup.Count > 0 AndAlso _
                            Logger.Msg("Remove " & lDataGroup.Count & " datasets from " & vbCrLf & .Specification & "?", _
                                       MsgBoxStyle.OkCancel, "Confirm Remove") = MsgBoxResult.Ok Then
                            For Each lDataSet As atcDataSet In lDataGroup
                                .RemoveDataset(lDataSet)
                            Next
                            Populate(atcDataManager.DataSources.IndexOf(aDataSource))
                        End If
                    End If
                Case "SaveIn"
                    Dim lDataGroup As atcDataGroup = atcDataManager.UserSelectData( _
                        "Select Datasets to save " & .Specification, Nothing, .DataSets.Clone, True, True, Me.Icon)
                    If lDataGroup.Count > 0 Then
                        Dim lFormSave As New frmSaveData
                        Dim lSaveSource As atcDataSource = lFormSave.AskUser(lDataGroup)
                        If lSaveSource IsNot Nothing AndAlso Not String.IsNullOrEmpty(lSaveSource.Specification) Then
                            lSaveSource.AddDataSets(lDataGroup)
                        End If
                    End If
                Case Else
                    Logger.Dbg("Unknown action: " & aAction)
            End Select
        End With
    End Sub

    'Private Sub DoDisplay()
    '    Dim lSelected As atcTimeseriesGroup = SelectedTimeseries()
    '    If lSelected IsNot Nothing AndAlso lSelected.Count > 0 Then
    '        lSelected = atcDataManager.UserSelectData("Select data to Display", Nothing, lSelected, True, False)
    '        If lSelected IsNot Nothing AndAlso lSelected.Count > 0 Then
    '            atcDataManager.UserSelectDisplay("Select display", lSelected)
    '        End If
    '    End If
    'End Sub

    Private Sub ChangedData(ByVal aDataSource As atcTimeseriesSource)
        Edit(atcDataManager.DataSources.Count - 1)
    End Sub

    Private Sub Form_DragEnter(
        ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) _
        Handles Me.DragEnter, treeFiles.DragEnter, txtDetails.DragEnter

        If e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop) Then
            e.Effect = System.Windows.Forms.DragDropEffects.All
        End If
    End Sub

    Private Sub Form_DragDrop( _
        ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) _
        Handles Me.DragDrop, treeFiles.DragDrop, txtDetails.DragDrop

        If e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop) Then
            Dim lFileNames() As String = e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop)
            Dim lIndex As Integer = 0
            For Each lFileName As String In lFileNames
                lIndex += 1
                Logger.Progress("Opening " & lFileName, lIndex, lFileNames.Length)
                atcDataManager.OpenDataSource(lFileName)
            Next
            Logger.Status("")
        End If
    End Sub

    Private Sub treeFiles_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles treeFiles.AfterSelect
        Dim lSelectedDataSourceIndex As String = e.Node.Name
        If IsNumeric(lSelectedDataSourceIndex) Then
            RefreshDetails(lSelectedDataSourceIndex)
        Else
            txtDetails.Text = ""
        End If
    End Sub

    Private Sub RefreshDetails(ByVal aDataSourceIndex As Integer)
        If aDataSourceIndex > -1 AndAlso _
           aDataSourceIndex < atcDataManager.DataSources.Count Then
            Dim lDataSource As atcTimeseriesSource = atcDataManager.DataSources.Item(aDataSourceIndex)
            txtDetails.Text = lDataSource.Name
            If Not lDataSource.Specification Is Nothing AndAlso lDataSource.Specification.Length > 0 Then
                txtDetails.Text &= vbCrLf & lDataSource.Specification
            End If
            If lDataSource.DataSets.Count > 0 Then
                txtDetails.Text &= vbCrLf & Format(lDataSource.DataSets.Count, "#,###") & " Timeseries"
            End If
            If FileExists(lDataSource.Specification) Then
                txtDetails.Text &= vbCrLf & Format(FileLen(lDataSource.Specification), "#,###") & " bytes"
                txtDetails.Text &= vbCrLf & "Modified " & System.IO.File.GetLastWriteTime(lDataSource.Specification)
            End If
        Else
            txtDetails.Text = ""
        End If
    End Sub

    Private Sub treeFiles_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles treeFiles.DoubleClick
        'DoDisplay()
    End Sub

    ' Draws a node.
    Private Sub treeFiles_DrawNode(ByVal sender As Object, ByVal e As DrawTreeNodeEventArgs) Handles treeFiles.DrawNode
        Dim lNode As TreeNode = e.Node

        Dim lBackgroundBrush As Drawing.Brush = Nothing
        If lNode.BackColor <> treeFiles.BackColor Then
            lBackgroundBrush = New Drawing.SolidBrush(lNode.BackColor)
            'ElseIf lNode.IsSelected Then
            '    lBackgroundBrush = SystemBrushes.Highlight
        End If

        If lBackgroundBrush IsNot Nothing Then
            ' Draw the background of the selected node. The NodeBounds
            ' method makes the highlight rectangle large enough to
            ' include the text of a node tag, if one is present.
            e.Graphics.FillRectangle(lBackgroundBrush, NodeBounds(e.Node))
        End If

        ' Retrieve the node font. If the node font has not been set,
        ' use the TreeView font.
        Dim lNodeFont As Font = e.Node.NodeFont
        If lNodeFont Is Nothing Then
            lNodeFont = CType(sender, TreeView).Font
        End If

        Dim lTextBrush As Drawing.Brush = Nothing
        If lNode.IsSelected Then
            lTextBrush = SystemBrushes.HighlightText
        Else
            lTextBrush = New SolidBrush(lNode.ForeColor)
        End If

        ' Draw the node text.
        e.Graphics.DrawString(e.Node.Text, lNodeFont, lTextBrush, e.Bounds.Left - 2, e.Bounds.Top)

        ' If a node tag is present, draw its string representation 
        ' to the right of the label text.
        If e.Node.Tag IsNot Nothing Then
            e.Graphics.DrawString(e.Node.Tag.ToString(), e.Node.NodeFont, _
                Brushes.Yellow, e.Bounds.Right + 2, e.Bounds.Top)
        End If

        ' If the node has focus, draw the focus rectangle large, making
        ' it large enough to include the text of the node tag, if present.
        If (e.State And TreeNodeStates.Focused) <> 0 Then
            Dim focusPen As New Pen(Color.Black)
            Try
                focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
                Dim focusBounds As Rectangle = NodeBounds(e.Node)
                focusBounds.Size = New Size(focusBounds.Width - 1, _
                    focusBounds.Height - 1)
                e.Graphics.DrawRectangle(focusPen, focusBounds)
            Finally
                focusPen.Dispose()
            End Try
        End If
    End Sub

    Private Function NodeBounds(ByVal e As System.Windows.Forms.TreeNode) As Rectangle
        Return New Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height)
    End Function

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        Dim lFilesOnly As New ArrayList(1)
        lFilesOnly.Add("File")
        Dim lNewSource As atcTimeseriesSource = atcDataManager.UserSelectDataSource(lFilesOnly, "Select a File Type", False, True)
        If lNewSource IsNot Nothing Then 'user did not cancel
            If Not atcDataManager.OpenDataSource(lNewSource, lNewSource.Specification, Nothing) Then
                If Logger.LastDbgText.Length > 0 Then
                    Logger.Msg(Logger.LastDbgText, "Data New Problem")
                End If
            End If
        End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Dim lCollection As New ArrayList
        lCollection.Add("File")
        Dim lNewSource As atcTimeseriesSource = atcDataManager.UserSelectDataSource(lCollection)
        If Not lNewSource Is Nothing Then
            If Not (atcDataManager.OpenDataSource(lNewSource, lNewSource.Specification, Nothing)) Then
                If Logger.LastDbgText.Length > 0 Then
                    Logger.Msg(Logger.LastDbgText, "Data Open Problem")
                End If
            End If
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        SelectionAction("Close")
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseAllToolStripMenuItem.Click
        atcDataManager.Clear()
        Populate(-1)
    End Sub

    Private Sub RefreshSelectedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshSelectedToolStripMenuItem.Click
        SelectionAction("Refresh")
    End Sub

    Private Sub RefreshAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshAllToolStripMenuItem.Click
        For Each lDataSource As atcTimeseriesSource In atcDataManager.DataSources
            DoAction("Refresh", lDataSource, True)
        Next
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ViewFileAsTextToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewFileAsTextToolStripMenuItem.Click
        SelectionAction("View")
    End Sub

    Private Sub ShowFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowFolderToolStripMenuItem.Click
        SelectionAction("ShowFolder")
    End Sub

    Private Sub AnalysisToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AnalysisToolStripMenuItem.Click
        If sender.text <> "Analysis" Then
            SelectionAction("Analysis:" & sender.Text)
        End If
    End Sub

    Private Sub RemoveDataFromFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveDataFromFileToolStripMenuItem.Click
        SelectionAction("RemoveDatasets")
    End Sub

    Private Sub SaveInToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveInToolStripMenuItem.Click
        SelectionAction("SaveIn")
    End Sub

    Private Sub frmManager_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyValue = System.Windows.Forms.Keys.F1 Then
            If Application.ProductName = "USGSHydroToolbox" Then
                ShowHelp("Getting Started (File, Project, and Data Menus)/GIS and Time-Series Data/Download and Manage Data/Time-Series Management.html")
            Else
                ShowHelp("BASINS Details\Project Creation and Management\GIS and Time-Series Data\Time-Series Management.html")
            End If
        End If
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        If Application.ProductName = "USGSHydroToolbox" Then
            ShowHelp("Getting Started (File, Project, and Data Menus)/GIS and Time-Series Data/Download and Manage Data/Time-Series Management.html")
        Else
            ShowHelp("BASINS Details\Project Creation and Management\GIS and Time-Series Data\Time-Series Management.html")
        End If
    End Sub

#Region "TreeViewMultiSelect"
    'http://www.arstdesign.com/articles/treeviewms.html
    Protected pSelectedNodes As New ArrayList
    Protected pFirstNode, pLastNode As TreeNode

    Public Property SelectedNodes() As ArrayList
        Get
            Return pSelectedNodes
        End Get
        Set(ByVal value As ArrayList)
            removePaintFromNodes()
            pSelectedNodes.Clear()
            pSelectedNodes = value
            paintSelectedNodes()
        End Set
    End Property

    Protected Sub OnBeforeSelect(ByVal sender As Object, ByVal e As TreeViewCancelEventArgs) Handles treeFiles.BeforeSelect
        ' e.Node is the current node exposed by the base TreeView control

        Dim bControl As Boolean = ModifierKeys = Keys.Control
        Dim bShift As Boolean = ModifierKeys = Keys.Shift

        ' selecting twice the node while pressing CTRL ?
        If bControl And pSelectedNodes.Contains(e.Node) Then
            ' unselect it (let framework know we don't want selection this time)
            e.Cancel = True

            ' update nodes
            removePaintFromNodes()
            pSelectedNodes.Remove(e.Node)
            paintSelectedNodes()
            Return
        End If

        pLastNode = e.Node
        If Not bShift Then
            pFirstNode = e.Node ' store begin of shift sequence
        End If
    End Sub 'OnBeforeSelect

    Private Function isParent(ByVal aCheckParent As TreeNode, ByVal aDescendant As TreeNode) As Boolean
        If aDescendant Is Nothing Then
            Return False
        Else
            Return isParent(aCheckParent, aDescendant.Parent)
        End If
    End Function

    Protected Sub OnAfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles treeFiles.AfterSelect
        ' e.Node is the current node exposed by the base TreeView control

        Dim bControl As Boolean = ModifierKeys = Keys.Control
        Dim bShift As Boolean = ModifierKeys = Keys.Shift

        If bControl Then
            If Not pSelectedNodes.Contains(e.Node) Then ' new node ?
                pSelectedNodes.Add(e.Node)
                ' not new, remove it from the collection
            Else
                removePaintFromNodes()
                pSelectedNodes.Remove(e.Node)
            End If
            paintSelectedNodes()
        Else
            If bShift Then
                Dim myQueue As New Queue()

                Dim uppernode As TreeNode = pFirstNode
                Dim bottomnode As TreeNode = e.Node

                ' case 1 : begin and end nodes are parent
                Dim bParent As Boolean = isParent(pFirstNode, e.Node) ' is pFirstNode parent (direct or not) of e.Node
                If Not bParent Then
                    bParent = isParent(bottomnode, uppernode)
                    If bParent Then ' swap nodes
                        Dim t As TreeNode = uppernode
                        uppernode = bottomnode
                        bottomnode = t
                    End If
                End If
                If bParent Then
                    Dim n As TreeNode = bottomnode
                    While Not n.Equals(uppernode.Parent)
                        If Not pSelectedNodes.Contains(n) Then ' new node ?
                            myQueue.Enqueue(n)
                        End If
                        n = n.Parent
                    End While
                    ' case 2 : nor the begin nor the end node are descendant one another
                Else
                    If uppernode.Parent Is Nothing And bottomnode.Parent Is Nothing Or (Not (uppernode.Parent Is Nothing) And uppernode.Parent.Nodes.Contains(bottomnode)) Then ' are they siblings ?
                        Dim nIndexUpper As Integer = uppernode.Index
                        Dim nIndexBottom As Integer = bottomnode.Index
                        If nIndexBottom < nIndexUpper Then ' reversed?
                            Dim t As TreeNode = uppernode
                            uppernode = bottomnode
                            bottomnode = t
                            nIndexUpper = uppernode.Index
                            nIndexBottom = bottomnode.Index
                        End If

                        Dim n As TreeNode = uppernode
                        While nIndexUpper <= nIndexBottom
                            If Not pSelectedNodes.Contains(n) Then ' new node ?
                                myQueue.Enqueue(n)
                            End If
                            n = n.NextNode

                            nIndexUpper += 1
                        End While ' end while
                    Else
                        If Not pSelectedNodes.Contains(uppernode) Then
                            myQueue.Enqueue(uppernode)
                        End If
                        If Not pSelectedNodes.Contains(bottomnode) Then
                            myQueue.Enqueue(bottomnode)
                        End If
                    End If
                End If

                pSelectedNodes.AddRange(myQueue)

                paintSelectedNodes()
                pFirstNode = e.Node ' let us chain several SHIFTs if we like it
                ' end if m_bShift
            Else
                ' in the case of a simple click, just add this item
                If pSelectedNodes IsNot Nothing AndAlso pSelectedNodes.Count > 0 Then
                    removePaintFromNodes()
                    pSelectedNodes.Clear()
                End If
                pSelectedNodes.Add(e.Node)
            End If
        End If
    End Sub 'OnAfterSelect

    Protected Sub paintSelectedNodes()
        Dim n As TreeNode
        For Each n In pSelectedNodes
            n.BackColor = Drawing.SystemColors.Highlight
            n.ForeColor = Drawing.SystemColors.HighlightText
        Next n
    End Sub 'paintSelectedNodes

    Protected Sub removePaintFromNodes()
        If pSelectedNodes.Count = 0 Then
            Return
        End If
        Dim n0 As TreeNode = CType(pSelectedNodes(0), TreeNode)
        Dim back As Drawing.Color = treeFiles.BackColor
        Dim fore As Drawing.Color = treeFiles.ForeColor

        For Each n As TreeNode In pSelectedNodes
            n.BackColor = back
            n.ForeColor = fore
        Next n
    End Sub 'removePaintFromNodes

#End Region

End Class


