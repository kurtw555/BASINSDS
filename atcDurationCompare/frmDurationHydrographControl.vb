﻿Imports atcData
Imports atcUtility
Imports MapWinUtility

Public Class frmDurationHydrographControl
    Private pDataGroup As atcTimeseriesGroup
    Private pResultForm As System.Windows.Forms.Form

    Private pDefaultNumClasses As Integer = 35
    Private pDefaultClasses As Double() = {0.0, 0.1, 0.2, 0.3, 0.5, 0.7, 0.8, 0.9, 1.0}

    Public Property DataGroup() As atcTimeseriesGroup
        Get
            Return pDataGroup
        End Get
        Set(ByVal value As atcTimeseriesGroup)
            pDataGroup = value
        End Set
    End Property

    Public Property ResultForm() As System.Windows.Forms.Form
        Get
            Return pResultForm
        End Get
        Set(ByVal value As System.Windows.Forms.Form)
            pResultForm = value
        End Set
    End Property

    Public Property DefaultClasses() As Double()
        Get
            Return pDefaultClasses
        End Get
        Set(ByVal value As Double())
            pDefaultClasses = value
        End Set
    End Property

    Public Sub New(ByVal aDataGroup As atcTimeseriesGroup)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        '
        DataGroup = aDataGroup
    End Sub

    Private Sub frmDurationHydrographControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        pDataGroup.Clear()
        pDataGroup = Nothing
    End Sub

    Private Sub frmDurationHydrographControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'lblAnalysisInfo.Text = ""
        'lblAnalysisInfo.Text = "Step1. Select timeseries data" & vbNewLine & _
        '                       "Step2. Examine the class limits (use default or create anew)" & vbNewLine & _
        '                       "Step3. Choose an anlysis" & vbNewLine & _
        '                       "Step4. Click either Graph or Report to carry out analysis and see results"
        lstPctExceed.CurrentValues = DefaultClasses
        ResultForm = CreateForm("durationhydrograph")
    End Sub

    Private Sub DisplayDefault() Handles lstPctExceed.DefaultRequested
        lstPctExceed.CurrentValues = DefaultClasses
    End Sub

    Private Sub btnReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReport.Click
        If ResultForm Is Nothing Then
            Logger.Dbg("Duration Hydrograph analysis failed to initialize result form")
            Exit Sub
        End If

        Dim lListPct As List(Of Double) = ListOK()

        If DataGroup.Count = 0 Then
            mnuSelectData_Click(Nothing, Nothing)
        End If
        Dim ltxtBox As Windows.Forms.TextBox = Nothing
        Select Case ResultForm.Name
            Case "frmResult"
                If ResultForm.IsDisposed Then
                    ResultForm = Nothing
                    ResultForm = CreateForm("durationhydrograph")
                End If
                With CType(ResultForm, frmResult)
                    .Initialize("DurationHydrograph", DataGroup, lListPct.ToArray(), "report")
                    ltxtBox = .txtReport
                End With

        End Select
        ResultForm.Show()
        If ltxtBox IsNot Nothing Then
            ltxtBox.SelectionLength = 0
        End If
    End Sub

    Private Sub btnGraph_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGraph.Click
        If ResultForm Is Nothing Then
            Logger.Dbg("Duration/Compare analysis failed to initialize proper form")
            Exit Sub
        End If

        Select Case ResultForm.Name
            Case "frmDuration"
                CType(ResultForm, frmDuration).doDurPlot(DataGroup)
            Case "frmCompare"
                If DataGroup.Count < 2 Then
                    Logger.Msg("Need to select two timeseries to conduct compare analysis.")
                    'If txtDS1.Text = "" Then
                    '    txtDS1.Focus()
                    'ElseIf txtDS2.Text = "" Then
                    '    txtDS2.Focus()
                    'End If
                    Exit Sub
                End If
                CType(ResultForm, frmCompare).doComparePlot(DataGroup)
            Case "frmResult"
                Dim lListPct As List(Of Double) = ListOK()

                If DataGroup.Count = 0 Then
                    mnuSelectData_Click(Nothing, Nothing)
                End If

                If ResultForm.IsDisposed Then
                    ResultForm = Nothing
                    ResultForm = CreateForm("durationhydrograph")
                End If
                With CType(ResultForm, frmResult)
                    .Initialize("DurationHydrograph", DataGroup, lListPct.ToArray(), "graph")
                End With
        End Select
    End Sub

    Private Sub mnuSelectData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectData.Click
        pDataGroup = atcDataManager.UserSelectData("Select Data For Analysis", pDataGroup)
    End Sub

    Private Sub mnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuExit.Click
        If pDataGroup IsNot Nothing Then
            pDataGroup.Clear()
            pDataGroup = Nothing
        End If
        Me.Close()
    End Sub

#Region "Util"
    Private Function CreateForm(ByVal Analysis As String) As Object
        Dim DisplayPlugins As ICollection = atcDataManager.GetPlugins(GetType(atcDataDisplay))
        Dim lFrm As Object = Nothing
        Analysis = Analysis.ToLower()
        If Analysis = "duration" Then
            lFrm = New frmDuration
        ElseIf Analysis = "compare" Then
            lFrm = New frmCompare
        ElseIf Analysis = "durationhydrograph" Then
            lFrm = New frmResult
        End If
        For Each lDisp As atcDataDisplay In DisplayPlugins
            Dim lMenuText As String = lDisp.Name
            If lMenuText.StartsWith("Analysis::") Then lMenuText = lMenuText.Substring(10)
            If TypeOf (lFrm) Is frmResult Then
                CType(lFrm, frmResult).mnuAnalysis.DropDownItems().Add(lMenuText, Nothing, New EventHandler(AddressOf lFrm.mnuAnalysis_Click))
            Else
                lFrm.mnuAnalysis.MenuItems.Add(lMenuText, New EventHandler(AddressOf lFrm.mnuAnalysis_Click))
            End If
        Next
        Return lFrm
    End Function

    Private Function ListOK() As List(Of Double)
        Dim lDict As New Dictionary(Of Double, Integer)
        Dim lList As New List(Of Double)
        For Each lPct As Double In lstPctExceed.CurrentValues
            If lPct >= 0.0 AndAlso lPct <= 1.0 Then
                If Not lList.Contains(lPct) Then
                    lList.Add(lPct)
                End If
            End If
        Next
        If Not lList.Contains(0) Then lList.Add(0)
        If Not lList.Contains(1) Then lList.Add(1)
        lList.Sort()
        Return lList
    End Function

#End Region

End Class