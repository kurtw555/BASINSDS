Imports atcUtility
Imports atcMwGisUtility
Imports MapWinUtility
Imports System.Drawing
Imports System

Public Class frmSWMMSetup
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdExisting As System.Windows.Forms.Button
    Friend WithEvents cmdHelp As System.Windows.Forms.Button
    Friend WithEvents cmdAbout As System.Windows.Forms.Button
    Friend WithEvents ofdExisting As System.Windows.Forms.OpenFileDialog
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSWMMSetup))
        Me.cmdOK = New System.Windows.Forms.Button
        Me.cmdExisting = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdHelp = New System.Windows.Forms.Button
        Me.cmdAbout = New System.Windows.Forms.Button
        Me.ofdExisting = New System.Windows.Forms.OpenFileDialog
        Me.SuspendLayout()
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOK.Location = New System.Drawing.Point(16, 256)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(72, 32)
        Me.cmdOK.TabIndex = 2
        Me.cmdOK.Text = "OK"
        '
        'cmdExisting
        '
        Me.cmdExisting.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdExisting.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdExisting.Location = New System.Drawing.Point(96, 256)
        Me.cmdExisting.Name = "cmdExisting"
        Me.cmdExisting.Size = New System.Drawing.Size(120, 32)
        Me.cmdExisting.TabIndex = 4
        Me.cmdExisting.Text = "Open Existing"
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.Location = New System.Drawing.Point(224, 256)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(88, 32)
        Me.cmdCancel.TabIndex = 5
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdHelp
        '
        Me.cmdHelp.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdHelp.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdHelp.Location = New System.Drawing.Point(349, 256)
        Me.cmdHelp.Name = "cmdHelp"
        Me.cmdHelp.Size = New System.Drawing.Size(79, 32)
        Me.cmdHelp.TabIndex = 6
        Me.cmdHelp.Text = "Help"
        '
        'cmdAbout
        '
        Me.cmdAbout.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAbout.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAbout.Location = New System.Drawing.Point(437, 256)
        Me.cmdAbout.Name = "cmdAbout"
        Me.cmdAbout.Size = New System.Drawing.Size(87, 32)
        Me.cmdAbout.TabIndex = 7
        Me.cmdAbout.Text = "About"
        '
        'ofdExisting
        '
        Me.ofdExisting.DefaultExt = "inp"
        Me.ofdExisting.Filter = "SWMM INP files (*.inp)|*.inp"
        Me.ofdExisting.InitialDirectory = "/BASINS/modelout/"
        Me.ofdExisting.Title = "Select SWMM inp file"
        '
        'frmSWMMSetup
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(541, 301)
        Me.Controls.Add(Me.cmdAbout)
        Me.Controls.Add(Me.cmdHelp)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdExisting)
        Me.Controls.Add(Me.cmdOK)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "frmSWMMSetup"
        Me.Text = "BASINS SWMM"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pPlugIn As PlugIn
    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub cmdAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAbout.Click
        Logger.Msg("BASINS SWMM for MapWindow" & vbCrLf & vbCrLf & "Version 1.0", MsgBoxStyle.OkOnly, "BASINS SWMM")
    End Sub

    Private Sub cmdExisting_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExisting.Click
        If ofdExisting.ShowDialog() = Windows.Forms.DialogResult.OK Then
            pPlugIn.SWMMProject.Run(ofdExisting.FileName)
        End If
    End Sub

    Private Sub cmdHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdHelp.Click
        ShowHelp("BASINS Details\Watershed and Instream Model Setup\SWMM.html")
    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        'hard code some things to test SWMM classes
        With pPlugIn.SWMMProject
            .Name = "TestProject"
            .Title = "SWMM Project Written from BASINS"
            Dim lBasinsFolder As String = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\AQUA TERRA Consultants\BASINS", "Base Directory", "C:\Basins")
            'TODO: still use modelout?
            Dim lSWMMProjectFileName As String = lBasinsFolder & "\modelout\" & .Name & "\" & .Name & ".inp"

            'set start and end dates
            .SJDate = MJD(2006, 10, 24)
            .EJDate = MJD(2006, 10, 31)

            'create rain gages from shapefile and selected station
            CreateRaingageFromShapefile(lBasinsFolder & "\data\02060006-1\met\met.shp", "MD189070", .RainGages)

            'create met constituents from wdm file and selected station
            Dim lMetWDMFileName As String = lBasinsFolder & "\data\02060006-1\met\met.wdm"
            CreateMetConstituent(lMetWDMFileName, "MD189070", "ATEM", .MetConstituents)
            CreateMetConstituent(lMetWDMFileName, "MD189070", "PEVT", .MetConstituents)

            'prepare the shapefile specs to test
            Dim lNodeSpecs As New NodeShapefileSpecs
            Dim lConduitSpecs As New ConduitShapefileSpecs
            Dim lCatchmentSpecs As New CatchmentShapefileSpecs
            'FillSpecsForBASINSTest(lNodeSpecs, lConduitSpecs, lCatchmentSpecs)
            FillSpecsForGenericTest(lNodeSpecs, lConduitSpecs, lCatchmentSpecs)

            'populate the SWMM classes from the shapefiles
            If lNodeSpecs.ShapefileName.Length > 0 Then
                CreateNodesFromShapefile(lNodeSpecs, .Nodes)
            End If
            CreateConduitsFromShapefile(lConduitSpecs, pPlugIn.SWMMProject, .Conduits)
            CreateCatchmentsFromShapefile(lCatchmentSpecs, pPlugIn.SWMMProject, .Catchments)

            'create landuses from nlcd landcover
            CreateLandusesFromGrid("C:\BASINS\data\02060006-1\NLCD\NLCD_LandCover_2001.tif", lCatchmentSpecs.ShapefileName, .Catchments, .Landuses)

            'add backdrop file
            .BackdropFile = lBasinsFolder & "\Predefined Delineations\West Branch\wbranch.bmp"
            .BackdropX1 = 328689.908 'GisUtil.MapExtentXmin
            .BackdropY1 = 4294938.703 'GisUtil.MapExtentYmin
            .BackdropX2 = 357258.259 'GisUtil.MapExtentXmax
            .BackdropY2 = 4319284.185 'GisUtil.MapExtentYmax

            'save project file and start SWMM
            .Save(lSWMMProjectFileName)
            .Run(lSWMMProjectFileName)
        End With
        Me.Close()
    End Sub

    Private Sub EnableControls(ByVal aEnabled As Boolean)
        cmdOK.Enabled = aEnabled
        cmdExisting.Enabled = aEnabled
        cmdHelp.Enabled = aEnabled
        cmdCancel.Enabled = aEnabled
        cmdAbout.Enabled = aEnabled
    End Sub

    Public Sub InitializeUI(ByVal aPlugIn As PlugIn)
        pPlugIn = aPlugIn
    End Sub

    Private Sub frmSWMMSetup_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyValue = Windows.Forms.Keys.F1 Then
            ShowHelp("BASINS Details\Watershed and Instream Model Setup\SWMM.html")
        End If
    End Sub

    Private Sub FillSpecsForBASINSTest(ByRef aNodeSpecs As NodeShapefileSpecs, ByRef aConduitSpecs As ConduitShapefileSpecs, ByRef aCatchmentSpecs As CatchmentShapefileSpecs)

        'prepare a test assuming the user has the typical files from a BASINS delineation
        Dim lBasinsFolder As String = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\AQUA TERRA Consultants\BASINS", "Base Directory", "C:\Basins")

        aConduitSpecs.ShapefileName = lBasinsFolder & "\Predefined Delineations\West Branch\wb_strms.shp"
        aConduitSpecs.SubbasinFieldName = "SUBBASIN"
        aConduitSpecs.DownSubbasinFieldName = "SUBBASINR"
        aConduitSpecs.ElevHighFieldName = "MAXEL"
        aConduitSpecs.ElevLowFieldName = "MINEL"
        aConduitSpecs.MeanWidthFieldName = "WID2"
        aConduitSpecs.MeanDepthFieldName = "DEP2"
        aConduitSpecs.CreateNodes = True
        aConduitSpecs.ManningsNFieldName = ""
        aConduitSpecs.InletOffsetFieldName = ""
        aConduitSpecs.OutletOffsetFieldName = ""
        aConduitSpecs.InitialFlowFieldName = ""
        aConduitSpecs.MaxFlowFieldName = ""
        aConduitSpecs.ShapeFieldName = ""
        aConduitSpecs.Geometry1FieldName = ""
        aConduitSpecs.Geometry2FieldName = ""
        aConduitSpecs.Geometry3FieldName = ""
        aConduitSpecs.Geometry4FieldName = ""
        aConduitSpecs.NumBarrelsFieldName = ""

        aCatchmentSpecs.ShapefileName = lBasinsFolder & "\Predefined Delineations\West Branch\wb_subs.shp"
        aCatchmentSpecs.SubbasinFieldName = "SUBBASIN"
        aCatchmentSpecs.SlopeFieldName = "SLO1"
        aCatchmentSpecs.WidthFieldName = ""
        aCatchmentSpecs.CurbLengthFieldName = ""
        aCatchmentSpecs.SnowPackNameFieldName = ""
        aCatchmentSpecs.ManningsNImpervFieldName = ""
        aCatchmentSpecs.ManningsNPervFieldName = ""
        aCatchmentSpecs.DepressionStorageImpervFieldName = ""
        aCatchmentSpecs.DepressionStoragePervFieldName = ""
        aCatchmentSpecs.PercentZeroStorageFieldName = ""
        aCatchmentSpecs.RouteToFieldName = ""
        aCatchmentSpecs.PercentRoutedFieldName = ""
        aCatchmentSpecs.MaxInfiltRateFieldName = ""
        aCatchmentSpecs.MinInfiltRateFieldName = ""
        aCatchmentSpecs.DecayRateConstantFieldName = ""
        aCatchmentSpecs.DryTimeFieldName = ""
        aCatchmentSpecs.MaxInfiltVolumeFieldName = ""
        aCatchmentSpecs.SuctionFieldName = ""
        aCatchmentSpecs.ConductivityFieldName = ""
        aCatchmentSpecs.InitialDeficitFieldName = ""
        aCatchmentSpecs.CurveNumberFieldName = ""
    End Sub

    Private Sub FillSpecsForGenericTest(ByRef aNodeSpecs As NodeShapefileSpecs, ByRef aConduitSpecs As ConduitShapefileSpecs, ByRef aCatchmentSpecs As CatchmentShapefileSpecs)

        'prepare a test assuming the user has the generic storm sewer shapefiles
        Dim lBasinsFolder As String = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\AQUA TERRA Consultants\BASINS", "Base Directory", "C:\Basins")

        'create nodes from streams shapefile 
        aNodeSpecs.ShapefileName = lBasinsFolder & "\Predefined Delineations\West Branch\nodes.shp"
        aNodeSpecs.NameFieldName = "ID"
        aNodeSpecs.TypeFieldName = "Type"
        aNodeSpecs.InvertElevationFieldName = ""
        aNodeSpecs.MaxDepthFieldName = ""
        aNodeSpecs.InitDepthFieldName = ""
        aNodeSpecs.SurchargeDepthFieldName = ""
        aNodeSpecs.PondedAreaFieldName = ""
        aNodeSpecs.OutfallTypeFieldName = ""
        aNodeSpecs.StageTableFieldName = ""
        aNodeSpecs.TideGateFieldName = ""

        aConduitSpecs.ShapefileName = lBasinsFolder & "\Predefined Delineations\West Branch\conduits.shp"
        aConduitSpecs.InletNodeFieldName = "InNodeID"
        aConduitSpecs.OutletNodeFieldName = "OutNodeID"
        aConduitSpecs.CreateNodes = False
        aConduitSpecs.ManningsNFieldName = ""
        aConduitSpecs.InletOffsetFieldName = ""
        aConduitSpecs.OutletOffsetFieldName = ""
        aConduitSpecs.InitialFlowFieldName = ""
        aConduitSpecs.MaxFlowFieldName = ""
        aConduitSpecs.ShapeFieldName = ""
        aConduitSpecs.Geometry1FieldName = ""
        aConduitSpecs.Geometry2FieldName = ""
        aConduitSpecs.Geometry3FieldName = ""
        aConduitSpecs.Geometry4FieldName = ""
        aConduitSpecs.NumBarrelsFieldName = ""

        aCatchmentSpecs.ShapefileName = lBasinsFolder & "\Predefined Delineations\West Branch\catchments.shp"
        aCatchmentSpecs.SubbasinFieldName = "ID"
        aCatchmentSpecs.SlopeFieldName = "SLO1"
        aCatchmentSpecs.OutletNodeFieldName = "OutNode"
        aCatchmentSpecs.WidthFieldName = ""
        aCatchmentSpecs.CurbLengthFieldName = ""
        aCatchmentSpecs.SnowPackNameFieldName = ""
        aCatchmentSpecs.ManningsNImpervFieldName = ""
        aCatchmentSpecs.ManningsNPervFieldName = ""
        aCatchmentSpecs.DepressionStorageImpervFieldName = ""
        aCatchmentSpecs.DepressionStoragePervFieldName = ""
        aCatchmentSpecs.PercentZeroStorageFieldName = ""
        aCatchmentSpecs.RouteToFieldName = ""
        aCatchmentSpecs.PercentRoutedFieldName = ""
        aCatchmentSpecs.MaxInfiltRateFieldName = ""
        aCatchmentSpecs.MinInfiltRateFieldName = ""
        aCatchmentSpecs.DecayRateConstantFieldName = ""
        aCatchmentSpecs.DryTimeFieldName = ""
        aCatchmentSpecs.MaxInfiltVolumeFieldName = ""
        aCatchmentSpecs.SuctionFieldName = ""
        aCatchmentSpecs.ConductivityFieldName = ""
        aCatchmentSpecs.InitialDeficitFieldName = ""
        aCatchmentSpecs.CurveNumberFieldName = ""
    End Sub
End Class