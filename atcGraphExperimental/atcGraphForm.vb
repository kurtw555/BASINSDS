Imports atcData
Imports atcUtility
Imports MapWinUtility

Imports ZedGraph

Imports System.Drawing
Imports System.Windows.Forms
Imports System.Text.Json


Public Class atcGraphForm

    'Form object that contains graph(s)
    Private pMaster As ZedGraph.MasterPane
    Private pAuxEnabled As Boolean = False
    Public AuxFraction As Single = 0.2

    'Graph editing form
    Private WithEvents pEditor As frmGraphEditor ' ZedGraph.frmEdit 'frmGraphEdit

    Private WithEvents pZgc As ZedGraphControl

    Private pGrapher As clsGraphBase
    Private Shared SaveImageExtension As String = ".png"

    Public Property Grapher() As clsGraphBase
        Get
            Return pGrapher
        End Get
        Set(ByVal newValue As clsGraphBase)
            pGrapher = newValue
            RefreshGraph()
        End Set
    End Property

    Private Sub InitMasterPane()
        'InitMatchingColors(FindFile("", "GraphColors.txt")) 'Becky moved this here from atcGraph10/CreateZgc so that
        'the file is found ONCE instead of a gazillion times and the colors are initialized ONCE rather than a gazillion
        'times
        pZgc = CreateZgc()
        Me.Controls.Add(pZgc)
        With pZgc
            .Dock = DockStyle.Fill
            .IsEnableHZoom = mnuViewHorizontalZoom.Checked
            .IsEnableHPan = mnuViewHorizontalZoom.Checked
            .IsEnableVZoom = mnuViewVerticalZoom.Checked
            .IsEnableVPan = mnuViewVerticalZoom.Checked
            '.IsZoomOnMouseCenter = mnuViewZoomMouse.Checked
            pMaster = .MasterPane
        End With

        RefreshGraph()
    End Sub

    Public ReadOnly Property ZedGraphCtrl() As ZedGraphControl
        Get
            Return pZgc
        End Get
    End Property

    Public ReadOnly Property Pane() As GraphPane
        Get
            If pMaster.PaneList.Count > 1 Then
                Return pMaster.PaneList(1)
            Else
                Return pMaster.PaneList(0)
            End If
        End Get
    End Property

    Public Function SaveGraph(ByVal aFilename As String) As Boolean
        RefreshGraph()
        Dim lSaved As Boolean = False
        Try
            '### need to fix
            'pZgc.SaveIn(aFilename)
            lSaved = True
        Catch ex As Exception
            lSaved = False
        End Try
        Return lSaved
    End Function

    Private Sub PageSettings(ByVal sender As System.Object, ByVal e As Printing.QueryPageSettingsEventArgs)
        If pMaster.Rect.Width > pMaster.Rect.Height Then
            e.PageSettings.Landscape = True
        Else
            e.PageSettings.Landscape = False
        End If
    End Sub

    '' <summary> Prints the displayed graph. </summary> 
    '' <param name="sender"> Object raising this event. </param> 
    '' <param name="e"> Event arguments passing graphics context to print to. </param> 
    Private Sub PrintPage(ByVal sender As System.Object, ByVal e As Printing.PrintPageEventArgs)
        ' Validate. 
        If (e Is Nothing) Then Return
        If (e.Graphics Is Nothing) Then Return

        ' Resize the graph to fit the printout. 
        With e.MarginBounds
            pMaster.ReSize(e.Graphics, New RectangleF(.X, .Y, .Width, .Height))
        End With

        ' Print the graph. 
        pMaster.Draw(e.Graphics)

        e.HasMorePages = False 'ends the print job
    End Sub

    Private Sub pEditor_Apply() Handles pEditor.Apply
        RefreshGraph()
    End Sub

    Public Sub RefreshGraph()
        pZgc.AxisChange()
        Invalidate()
        Refresh()
    End Sub

    Private Sub ShowHelpForGraph()
        If System.Reflection.Assembly.GetEntryAssembly.Location.EndsWith("TimeseriesUtility.exe") Then
            ShowHelp("View\Graph.html")
        ElseIf Application.ProductName = "USGSHydroToolbox" Then
            ShowHelp("Time-Series Tools\Graph.html")
        Else
            ShowHelp("BASINS Details\Analysis\Time Series Functions\Graph.html")
        End If
    End Sub

    Private Function pZgc_MouseMoveEvent(ByVal sender As ZedGraph.ZedGraphControl, ByVal e As System.Windows.Forms.MouseEventArgs) As System.Boolean Handles pZgc.MouseMoveEvent
        If mnuCoordinatesOnMenuBar.Checked Then
            ' Save the mouse location
            Dim mousePt As New PointF(e.X, e.Y)
            Dim lPositionText As String = "Coordinates"
            ' Find the pane that contains the current mouse location
            Dim lPane As GraphPane = sender.MasterPane.FindChartRect(mousePt)
            ' If pane is non-null, we have a valid location.  Otherwise, the mouse is not within any chart rect.
            If Not lPane Is Nothing Then
                Try
                    Dim x, y As Double
                    ' Convert the mouse location to X, Y scale values
                    lPane.ReverseTransform(mousePt, x, y)
                    If Double.IsNaN(x) OrElse Double.IsNaN(y) Then
                        lPositionText = ""
                    Else
                        ' Format the status label text
                        Select Case lPane.XAxis.Type
                            'Case AxisType.DateDual
                            '    Dim lDate As Date = Date.FromOADate(x)
                            '    If lPane.XAxis.Scale.Max - lPane.XAxis.Scale.Min > 10 Then
                            '        lPositionText = lDate.ToString("yyyy MMM d")
                            '    Else
                            '        lPositionText = lDate.ToString("yyyy MMM d HH:mm")
                            '    End If
                            'Case AxisType.Probability
                            '    Dim lProbScale As ZedGraph.ProbabilityScale = lPane.XAxis.Scale
                            '    Select Case lProbScale.LabelStyle
                            '        Case ProbabilityScale.ProbabilityLabelStyle.Percent
                            '            lPositionText = DoubleToString(x * 100, 3, , , , 3) & "%"
                            '        Case ProbabilityScale.ProbabilityLabelStyle.Fraction
                            '            lPositionText = DoubleToString(x, 7, , , , 5)
                            '        Case ProbabilityScale.ProbabilityLabelStyle.ReturnInterval
                            '            If x > 0 Then
                            '                lPositionText = DoubleToString(1 / x, , , , , 3) & "yr"
                            '            Else
                            '                lPositionText = ""
                            '            End If
                            '    End Select
                            Case Else
                                lPositionText = DoubleToString(x)
                        End Select
                        lPositionText = "(" & lPositionText & ", " & DoubleToString(y) & ")"
                    End If
                Catch
                    'Ignore any error setting coordinate text, default label is fine
                End Try
            End If
            mnuCoordinates.Text = lPositionText
        End If
        ' Return false to indicate we have not processed the MouseMoveEvent
        ' ZedGraphControl should still go ahead and handle it
        Return False
    End Function

    Public Sub FreeResources()
        If Not pEditor Is Nothing Then
            pEditor.Dispose()
            pEditor = Nothing
        End If
        pMaster = Nothing
        pZgc = Nothing
        If Not pGrapher Is Nothing Then
            pGrapher.Dispose()
            pGrapher = Nothing
        End If
    End Sub

    Private Sub mnuFileSelectData_Click(sender As Object, e As EventArgs) Handles mnuFileSelectData.Click
        atcDataManager.UserSelectData("", pGrapher.Datasets, Nothing, False, True, Me.Icon)
    End Sub

    Private Sub mnuFileSaveJson_Click(sender As Object, e As EventArgs) Handles mnuFileSaveJson.Click
        Dim lSaveDialog As New System.Windows.Forms.SaveFileDialog
        With lSaveDialog
            .Title = "Save graph specs to file"
            .DefaultExt = ".json"

            .Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
            .FileName = ReplaceString(Me.Text, " ", "_") & ".json"
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If
            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                Dim lStr As String = ""
                Dim lSerial As String = ""
                'Dim ser As JavaScriptSerializer = New JavaScriptSerializer()

                'lStr += "CURVES" & vbCrLf
                'For Each lPane As GraphPane In pZgc.MasterPane.PaneList
                '    For Each lCurve As CurveItem In lPane.CurveList
                '        lStr += ser.Serialize(lCurve) & vbCrLf
                '    Next
                'Next

                'lStr += "LEGENDS" & vbCrLf
                'For Each lPane As GraphPane In pZgc.MasterPane.PaneList
                '    lStr += ser.Serialize(lPane.Legend) & vbCrLf
                'Next

                'lStr += "OBJECTS" & vbCrLf
                'For Each lPane As GraphPane In pZgc.MasterPane.PaneList
                '    For Each lObj As GraphObj In lPane.GraphObjList
                '        lStr += ser.Serialize(lObj) & vbCrLf
                '    Next
                'Next

                'lStr += "AXES" & vbCrLf
                'For Each lPane As GraphPane In pZgc.MasterPane.PaneList
                '    For Each lAxis As YAxis In lPane.YAxisList
                '        lAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                '        lStr += ser.Serialize(lAxis) & vbCrLf
                '    Next
                '    For Each lAxis As Y2Axis In lPane.Y2AxisList
                '        lAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                '        lStr += ser.Serialize(lAxis) & vbCrLf
                '    Next
                '    lPane.XAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                '    lStr += ser.Serialize(lPane.XAxis) & vbCrLf
                'Next

                'fix a problem that will cause serialize to not work
                For Each lPane As GraphPane In pZgc.MasterPane.PaneList
                    For Each lAxis As YAxis In lPane.YAxisList
                        lAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                    Next
                    For Each lAxis As Y2Axis In lPane.Y2AxisList
                        lAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                    Next
                    lPane.XAxis.Scale.FontSpec.Border.GradientFill.Brush = Nothing
                    'also add axis location to Tag
                    For i As Integer = 0 To lPane.CurveList.Count - 1
                        If lPane.CurveList(i).IsY2Axis Then
                            lPane.CurveList(i).Tag &= "|RIGHT"
                        Else
                            lPane.CurveList(i).Tag &= "|LEFT"
                        End If
                    Next
                Next

                Try
                    'temporarily remove Points to avoid blowing out 'Serial' points buffer,
                    'values will come from referenced TS
                    Dim lPoints As ZedGraph.IPointList = pZgc.MasterPane.PaneList(0).CurveList(0).Points
                    pZgc.MasterPane.PaneList(0).CurveList(0).Points = Nothing
                    '### fix
                    'lSerial += ser.Serialize(pZgc.MasterPane)
                    lSerial += JsonSerializer.Serialize(pZgc.MasterPane)
                    pZgc.MasterPane.PaneList(0).CurveList(0).Points = lPoints
                Catch ex As Exception
                    lSerial += ex.ToString
                End Try

                SaveFileString(.FileName, lSerial)
            End If
        End With
    End Sub

    Private Sub mnuFileApplyJson_Click(sender As Object, e As EventArgs) Handles mnuFileApplyJson.Click
        Dim lOpenDialog As New System.Windows.Forms.OpenFileDialog
        With lOpenDialog
            .Title = "Open file containing graph specs"
            .DefaultExt = ".json"
            .FileName = ReplaceString(Me.Text, " ", "_") & ".json"
            If FileExists(IO.Path.GetDirectoryName(.FileName), True, False) Then
                .InitialDirectory = IO.Path.GetDirectoryName(.FileName)
            End If

            If .ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                ApplySpecsFromJSON(.FileName, pZgc)
                RefreshGraph()
            End If
        End With
    End Sub

    Private Sub mnuFileSave_Click(sender As Object, e As EventArgs) Handles mnuFileSave.Click
        RefreshGraph()
        Dim lSavedAs As String
        lSavedAs = pZgc.SaveAs(SaveImageExtension)
        If lSavedAs.Length > 0 Then
            SaveImageExtension = System.IO.Path.GetExtension(lSavedAs)
        End If
    End Sub

    Private Sub mnuFilePrint_Click(sender As Object, e As EventArgs) Handles mnuFilePrint.Click
        Dim lPrintDialog As New PrintDialog
        Dim lPrintDocument As New Printing.PrintDocument
        AddHandler lPrintDocument.PrintPage, AddressOf Me.PrintPage
        AddHandler lPrintDocument.QueryPageSettings, AddressOf Me.PageSettings

        With lPrintDialog
            .Document = lPrintDocument
            .AllowSelection = False
            .ShowHelp = True
            .UseEXDialog = True
            Dim lDialogResult As System.Windows.Forms.DialogResult = .ShowDialog(Me)
            If (lDialogResult = System.Windows.Forms.DialogResult.OK) Then
                Dim lSaveRectangle As RectangleF = pMaster.Rect
                lPrintDocument.Print()
                ' Restore graph size to fit form's bounds. 
                pMaster.ReSize(Me.CreateGraphics, lSaveRectangle)
            End If
        End With
    End Sub

    Private Sub mnuEditGraph_Click(sender As Object, e As EventArgs) Handles mnuEditGraph.Click
        If pEditor IsNot Nothing Then 'Try to re-use existing editor
            Try
                pEditor.Show()
                pEditor.BringToFront()
                Exit Sub
            Catch ex As Exception
                'Probably existing one was already disposed, fall through to creating a new one below
            End Try
        End If
        pEditor = New frmGraphEditor
        pEditor.Text = "Edit " & Me.Text
        pEditor.Icon = Me.Icon
        pEditor.Edit(pZgc)
    End Sub

    Private Sub mnuEditCopy_Click(sender As Object, e As EventArgs) Handles mnuEditCopy.Click
        Clipboard.SetDataObject(pZgc.MasterPane.GetImage)
    End Sub

    Private Sub mnuEditCopyMetafile_Click(sender As Object, e As EventArgs) Handles mnuEditCopyMetafile.Click

    End Sub

    Private Sub mnuViewVerticalZoom_Click(sender As Object, e As EventArgs) Handles mnuViewVerticalZoom.Click
        mnuViewVerticalZoom.Checked = Not mnuViewVerticalZoom.Checked
        pZgc.IsEnableVZoom = mnuViewVerticalZoom.Checked
        pZgc.IsEnableVPan = mnuViewVerticalZoom.Checked
    End Sub

    Private Sub mnuViewHorizontalZoom_Click(sender As Object, e As EventArgs) Handles mnuViewHorizontalZoom.Click
        mnuViewHorizontalZoom.Checked = Not mnuViewHorizontalZoom.Checked
        pZgc.IsEnableHZoom = mnuViewHorizontalZoom.Checked
        pZgc.IsEnableHPan = mnuViewHorizontalZoom.Checked
    End Sub

    Private Sub mnuViewZoomAll_Click(sender As Object, e As EventArgs) Handles mnuViewZoomAll.Click

    End Sub

    Private Sub mnuAnalysis_Click(sender As Object, e As EventArgs) Handles mnuAnalysis.Click
        atcDataManager.ShowDisplay(sender.Text, pGrapher.Datasets, Me.Icon)
    End Sub

    Private Sub mnuCoordinatesOnMenuBar_Click(sender As Object, e As EventArgs) Handles mnuCoordinatesOnMenuBar.Click
        mnuCoordinatesOnMenuBar.Checked = Not mnuCoordinatesOnMenuBar.Checked
        If Not mnuCoordinatesOnMenuBar.Checked Then mnuCoordinates.Text = "Coordinates"
    End Sub

    Private Sub mnuHelp_Click(sender As Object, e As EventArgs) Handles mnuHelp.Click
        ShowHelpForGraph()
    End Sub
End Class

