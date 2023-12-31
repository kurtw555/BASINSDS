Imports atcUtility
Imports MapWinUtility
Imports System.Collections.Generic
Imports System.Linq

Public Class frmSelectDisplay
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    Friend WithEvents grpDisplay As System.Windows.Forms.GroupBox
    Friend WithEvents lblDescribeDatasets As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnDiscard As System.Windows.Forms.Button
    Friend WithEvents btnSeasonal As System.Windows.Forms.Button
    Friend WithEvents btnTree As System.Windows.Forms.Button
    Friend WithEvents btnGraph As System.Windows.Forms.Button
    Friend WithEvents btnList As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.Button
    Friend WithEvents btnDuration As System.Windows.Forms.Button

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSelectDisplay))
        Me.grpDisplay = New System.Windows.Forms.GroupBox
        Me.btnSeasonal = New System.Windows.Forms.Button
        Me.btnTree = New System.Windows.Forms.Button
        Me.btnGraph = New System.Windows.Forms.Button
        Me.btnList = New System.Windows.Forms.Button
        Me.lblDescribeDatasets = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnSave = New System.Windows.Forms.Button
        Me.btnDiscard = New System.Windows.Forms.Button
        Me.btnSelect = New System.Windows.Forms.Button
        Me.btnDuration = New System.Windows.Forms.Button
        Me.grpDisplay.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpDisplay
        '
        Me.grpDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDisplay.Controls.Add(Me.btnSeasonal)
        Me.grpDisplay.Controls.Add(Me.btnTree)
        Me.grpDisplay.Controls.Add(Me.btnGraph)
        Me.grpDisplay.Controls.Add(Me.btnList)
        Me.grpDisplay.Location = New System.Drawing.Point(12, 144)
        Me.grpDisplay.Name = "grpDisplay"
        Me.grpDisplay.Size = New System.Drawing.Size(211, 138)
        Me.grpDisplay.TabIndex = 0
        Me.grpDisplay.TabStop = False
        Me.grpDisplay.Text = "Display"
        '
        'btnSeasonal
        '
        Me.btnSeasonal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSeasonal.Location = New System.Drawing.Point(6, 106)
        Me.btnSeasonal.Name = "btnSeasonal"
        Me.btnSeasonal.Size = New System.Drawing.Size(197, 23)
        Me.btnSeasonal.TabIndex = 7
        Me.btnSeasonal.Tag = "Analysis::Seasonal Attributes"
        Me.btnSeasonal.Text = "Seasonal Attributes"
        Me.btnSeasonal.UseVisualStyleBackColor = True
        '
        'btnTree
        '
        Me.btnTree.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnTree.Location = New System.Drawing.Point(6, 77)
        Me.btnTree.Name = "btnTree"
        Me.btnTree.Size = New System.Drawing.Size(197, 23)
        Me.btnTree.TabIndex = 6
        Me.btnTree.Tag = "Analysis::Data Tree"
        Me.btnTree.Text = "Data Tree"
        Me.btnTree.UseVisualStyleBackColor = True
        '
        'btnGraph
        '
        Me.btnGraph.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnGraph.Location = New System.Drawing.Point(6, 48)
        Me.btnGraph.Name = "btnGraph"
        Me.btnGraph.Size = New System.Drawing.Size(197, 23)
        Me.btnGraph.TabIndex = 5
        Me.btnGraph.Tag = "Analysis::Graph"
        Me.btnGraph.Text = "Graph"
        Me.btnGraph.UseVisualStyleBackColor = True
        '
        'btnList
        '
        Me.btnList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnList.Location = New System.Drawing.Point(6, 19)
        Me.btnList.Name = "btnList"
        Me.btnList.Size = New System.Drawing.Size(197, 23)
        Me.btnList.TabIndex = 5
        Me.btnList.Tag = "Analysis::List"
        Me.btnList.Text = "List"
        Me.btnList.UseVisualStyleBackColor = True
        '
        'lblDescribeDatasets
        '
        Me.lblDescribeDatasets.AutoSize = True
        Me.lblDescribeDatasets.Location = New System.Drawing.Point(12, 9)
        Me.lblDescribeDatasets.Name = "lblDescribeDatasets"
        Me.lblDescribeDatasets.Size = New System.Drawing.Size(152, 13)
        Me.lblDescribeDatasets.TabIndex = 1
        Me.lblDescribeDatasets.Text = "New Data has been computed"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(158, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Select what to do with this data:"
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(18, 57)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(197, 23)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save to file"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnDiscard
        '
        Me.btnDiscard.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDiscard.Location = New System.Drawing.Point(18, 86)
        Me.btnDiscard.Name = "btnDiscard"
        Me.btnDiscard.Size = New System.Drawing.Size(197, 23)
        Me.btnDiscard.TabIndex = 4
        Me.btnDiscard.Text = "Discard"
        Me.btnDiscard.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelect.Location = New System.Drawing.Point(18, 115)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(197, 23)
        Me.btnSelect.TabIndex = 5
        Me.btnSelect.Text = "Re-Select Datasets"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnDuration
        '
        Me.btnDuration.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDuration.Location = New System.Drawing.Point(18, 288)
        Me.btnDuration.Name = "btnDuration"
        Me.btnDuration.Size = New System.Drawing.Size(197, 23)
        Me.btnDuration.TabIndex = 8
        Me.btnDuration.Tag = "Analysis::Duration"
        Me.btnDuration.Text = "Duration/Compare"
        Me.btnDuration.UseVisualStyleBackColor = True
        '
        'frmSelectDisplay
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(235, 340)
        Me.Controls.Add(Me.btnDuration)
        Me.Controls.Add(Me.btnSelect)
        Me.Controls.Add(Me.btnDiscard)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblDescribeDatasets)
        Me.Controls.Add(Me.grpDisplay)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "frmSelectDisplay"
        Me.Text = "Display Data"
        Me.grpDisplay.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Const pPADDING As Integer = 5
    'Private pArgButton() As Windows.Forms.Button
    Private pTimeseriesGroup As atcTimeseriesGroup

    Public Sub AskUser(ByVal aTimeseriesGroup As atcTimeseriesGroup)
        pTimeseriesGroup = aTimeseriesGroup
        FormFromGroup()
        'iArg -= 1
        'If iArg >= 0 Then
        '    Me.Height = pArgButton(iArg).Top + pArgButton(iArg).Height + pPADDING + (Me.Height - Me.ClientRectangle.Height)
        Me.Show() 'Dialog()
        'Else
        '    Me.Close()
        'End If
    End Sub

    Private Sub FormFromGroup()
        lblDescribeDatasets.Text = pTimeseriesGroup.Count & " dataset"
        If pTimeseriesGroup.Count <> 1 Then lblDescribeDatasets.Text &= "s"
    End Sub

    Private Sub btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnList.Click, btnGraph.Click, btnTree.Click, btnSeasonal.Click
        atcDataManager.ShowDisplay(sender.tag, pTimeseriesGroup, Me.Icon)
    End Sub

    Private Sub btnDiscard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDiscard.Click
        Dim lDataSource As atcDataSource
        Dim lGroupEqual As Boolean
        Dim list2 As List(Of atcDataSet) = New List(Of atcDataSet)()
        For Each lds As atcDataSet In pTimeseriesGroup
            list2.Add(lds)
        Next
        Dim lDSRemove As New List(Of atcDataSource)
        For iDataSource As Integer = 0 To atcDataManager.DataSources.Count - 1
            lDataSource = atcDataManager.DataSources.Item(iDataSource)
#If GISProvider = "DotSpatial" Then
            Dim list1 As List(Of atcDataSet) = New List(Of atcDataSet)()
            For Each lds As atcDataSet In lDataSource.DataSets
                list1.Add(lds)
            Next
            Dim oneintwo As List(Of atcDataSet) = list1.Except(list2).ToList()
            Dim twoinone As List(Of atcDataSet) = list2.Except(list1).ToList()
            lGroupEqual = (Not oneintwo.Count > 0) And (Not twoinone.Count > 0)
#Else
            lGroupEqual = lDataSource.DataSets.Equals(pTimeseriesGroup) 
#End If
            If lGroupEqual Then
                lDSRemove.Add(lDataSource)
            End If
        Next
        For Each lDS As atcDataSource In lDSRemove
            If Logger.Msg("Discard " & lDS.ToString, MsgBoxStyle.YesNo, "Discard Data") = MsgBoxResult.Yes Then
                atcDataManager.RemoveDataSource(lDS)
                pTimeseriesGroup.Dispose()
            End If
        Next
        Me.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        SaveData()
    End Sub

    Private Function SaveData() As Boolean
        Dim lFormSave As New frmSaveData
        Dim lSaveSource As atcDataSource = lFormSave.AskUser(pTimeseriesGroup)
        If lSaveSource IsNot Nothing AndAlso Not String.IsNullOrEmpty(lSaveSource.Specification) Then
            Return lSaveSource.AddDataSets(pTimeseriesGroup)
        End If
        Return False
    End Function

    Private Function UserOpenDataFile(Optional ByVal aNeedToOpen As Boolean = True, _
                                      Optional ByVal aNeedToSave As Boolean = False) As atcTimeseriesSource
        Dim lFilesOnly As New ArrayList(1)
        lFilesOnly.Add("File")
        Dim lNewSource As atcDataSource = atcDataManager.UserSelectDataSource(lFilesOnly, "Select a File Type", aNeedToOpen, aNeedToSave)
        If Not lNewSource Is Nothing Then 'user did not cancel
            atcDataManager.OpenDataSource(lNewSource, lNewSource.Specification, Nothing)
        End If
        Return lNewSource
    End Function

    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        pTimeseriesGroup = atcDataManager.UserSelectData("Select Data", pTimeseriesGroup, Nothing, True, True, Me.Icon)
        FormFromGroup()
    End Sub

    Private Sub frmSelectDisplay_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyValue = System.Windows.Forms.Keys.F1 Then
            ShowHelp("BASINS Details\Project Creation and Management\GIS and Time-Series Data\Time-Series Management.html")
        End If
    End Sub

    Private Sub btnDuration_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDuration.Click
        atcDataManager.ShowDisplay("Analysis::USGS Surface Water Statistics (SWSTAT)::Duration/Compare", pTimeseriesGroup, Me.Icon)
    End Sub
End Class
