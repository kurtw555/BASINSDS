Imports atcUtility
Imports MapWinUtility

Public Class PlugIn
    Implements MapWindow.Interfaces.IPlugin

    Public ReadOnly Property Name() As String Implements MapWindow.Interfaces.IPlugin.Name
        'This is one of the more important plug-in properties because if it is not set to something then
        'your plug-in will not load at all. This is the name that appears in the Plug-ins menu to identify
        'this plug-in.
        Get
            Return "Projector"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements MapWindow.Interfaces.IPlugin.Author
        'This is the author of the plug-in.  It can be a company name, individual, or organization name.
        'You can use the license generator to generate a plug-in license, or use the name:
        '"Open Source Author"
        Get
            Return "AQUA TERRA Consultants"
        End Get
    End Property

    Public ReadOnly Property SerialNumber() As String Implements MapWindow.Interfaces.IPlugin.SerialNumber
        'This is the plug-in serial number and it corresponds with a particular author name (see the Author property).  
        'You can generate a serial number for your author name using the license generator, or use the serial number:
        'P/3I39l+3m8dSpr   - this corrsponds to the Author "Open Source Author"
        Get
            Return "G14R/KCU1FOWVVI"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements MapWindow.Interfaces.IPlugin.Description
        'This is a description of the plug-in.  It appears in the plug-ins dialog box when a user selects
        'your plug-in.  
        Get
            Return "This plug-in can change the projection of layers on the map."
        End Get
    End Property

    Public ReadOnly Property BuildDate() As String Implements MapWindow.Interfaces.IPlugin.BuildDate
        'This is the Build Date for the plug-in.  You can either return a string of a hard-coded date
        'such as "January 1, 2003" or you can use the .NET function below to dynamically obtain the build
        'date of the assembly.
        Get
            Return System.IO.File.GetLastWriteTime(Me.GetType().Assembly.Location)
        End Get
    End Property

    Public ReadOnly Property Version() As String Implements MapWindow.Interfaces.IPlugin.Version
        'This is the version number of the plug-in.  You can either return a hard-coded string
        'such as "1.0.0.1" or you can use the .NET function shown below to dynamically return 
        'the version number from the assembly itself.
        Get
            Return System.Diagnostics.FileVersionInfo.GetVersionInfo(Me.GetType().Assembly.Location).FileVersion
        End Get
    End Property

    Public Sub Initialize(ByVal MapWin As MapWindow.Interfaces.IMapWin, ByVal ParentHandle As Integer) Implements MapWindow.Interfaces.IPlugin.Initialize
        'This event is fired when the user loads your plug-in either through the plug-in dialog 
        'box, or by checkmarking it in the plug-ins menu.  This is where you would add buttons to the
        'tool bar or menu items to the menu.  
        '
        'It is also standard to set a global reference to the IMapWin that is passed through here so that
        'you can access it elsewhere in your project to act on MapWindow.
        g_MapWin = MapWin
    'g_MapWin.Toolbar.AddButton("Re-Project")
    End Sub

    Public Sub Terminate() Implements MapWindow.Interfaces.IPlugin.Terminate
        'This event is fired when the user unloads your plug-in either through the plug-in dialog 
        'box, or by un-checkmarking it in the plug-ins menu.  This is where you would remove any
        'buttons from the tool bar tool bar or menu items from the menu that you may have added.
        'If you don't do this, then you will leave dangling menus and buttons that don't do anything.
    'g_MapWin.Toolbar.RemoveButton("Re-Project")
    End Sub

    Public Sub ItemClicked(ByVal ItemName As String, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.ItemClicked
        'This event fires when a menu item or toolbar button is clicked.  So if you added a button or menu
        'on the Initialize event, then this is where you would handle it.
        Select Case ItemName
            Case "Re-Project" : ChangeProjection()
        End Select
    End Sub

    Private Sub ChangeProjection()

        Dim NumShapes As Integer
        Dim NumNonShapes As Integer

        Dim hLayer As Integer
        Dim iLayer As Integer

        For iLayer = 0 To g_MapWin.Layers.NumLayers - 1
            hLayer = g_MapWin.Layers.GetHandle(iLayer)
            Select Case g_MapWin.Layers(hLayer).LayerType
                Case MapWindow.Interfaces.eLayerType.Grid, _
                     MapWindow.Interfaces.eLayerType.Image
                    NumNonShapes = NumNonShapes + 1
                Case MapWindow.Interfaces.eLayerType.LineShapefile, _
                     MapWindow.Interfaces.eLayerType.PointShapefile, _
                     MapWindow.Interfaces.eLayerType.PolygonShapefile
                    NumShapes = NumShapes + 1
            End Select
        Next
    If Logger.Msg("This action rewrites all the shape files in the current project." & vbCr _
            & "Are you sure you want to do this?", "Change Projection", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
      Dim fp As New frmProjection
      Dim projectionSource As String
      Dim projectionDest As String

      'prompt for projection
      projectionSource = System.IO.Path.GetDirectoryName(g_MapWin.Project.FileName) & "\prj.proj"
      If Not FileExists(projectionSource) Then
        fp.Text = "Specify Current Projection"
        projectionSource = fp.AskUser()
      End If
      If projectionSource.Length > 0 Then
        fp.Text = "Specify Desired Projection"
        projectionDest = fp.AskUser()
        If projectionDest.Length > 0 Then
          For iLayer = 0 To g_MapWin.Layers.NumLayers - 1
            hLayer = g_MapWin.Layers.GetHandle(iLayer)
            Select Case g_MapWin.Layers(hLayer).LayerType
              Case MapWindow.Interfaces.eLayerType.Grid, MapWindow.Interfaces.eLayerType.Image
                Logger.Msg("Projection of grid and image layers not yet available for " & g_MapWin.Layers(hLayer).Name, "Projection")
              Case MapWindow.Interfaces.eLayerType.LineShapefile, _
                   MapWindow.Interfaces.eLayerType.PointShapefile, _
                   MapWindow.Interfaces.eLayerType.PolygonShapefile
                ProjectShapefile(projectionSource, projectionDest, g_MapWin.Layers(hLayer).GetObject)
            End Select
          Next
          g_MapWin.View.ZoomToMaxExtents()
          g_MapWin.View.Redraw()
          SaveFileString(System.IO.Path.GetDirectoryName(g_MapWin.Project.FileName) & "\prj.proj", projectionDest)
        End If
      End If
    End If
    End Sub

    Public Sub LayerRemoved(ByVal Handle As Integer) Implements MapWindow.Interfaces.IPlugin.LayerRemoved
        'This event fires when the user removes a layer from MapWindow.  This is useful to know if your
        'plug-in depends on a particular layer being present. 
    End Sub

    Public Sub LayersAdded(ByVal Layers() As MapWindow.Interfaces.Layer) Implements MapWindow.Interfaces.IPlugin.LayersAdded
        'This event fires when the user adds a layer to MapWindow.  This is useful to know if your
        'plug-in depends on a particular layer being present. Also, if you keep an internal list of 
        'available layers, for example you may be keeping a list of all "point" shapefiles, then you
        'would use this event to know when layers have been added or removed.
    End Sub

    Public Sub LayersCleared() Implements MapWindow.Interfaces.IPlugin.LayersCleared
        'This event fires when the user clears all of the layers from MapWindow.  As with LayersAdded 
        'and LayersRemoved, this is useful to know if your plug-in depends on a particular layer being 
        'present or if you are maintaining your own list of layers.
    End Sub

    Public Sub LayerSelected(ByVal Handle As Integer) Implements MapWindow.Interfaces.IPlugin.LayerSelected
        'This event fires when a user selects a layer in the legend. 
    End Sub

    Public Sub LegendDoubleClick(ByVal Handle As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendDoubleClick
        'This event fires when a user double-clicks a layer in the legend.
    End Sub

    Public Sub LegendMouseDown(ByVal Handle As Integer, ByVal Button As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendMouseDown
        'This event fires when a user holds a mouse button down in the legend.
    End Sub

    Public Sub LegendMouseUp(ByVal Handle As Integer, ByVal Button As Integer, ByVal Location As MapWindow.Interfaces.ClickLocation, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.LegendMouseUp
        'This event fires when a user releases a mouse button in the legend.
    End Sub

    Public Sub MapDragFinished(ByVal Bounds As System.Drawing.Rectangle, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapDragFinished
        'If a user drags (ie draws a box) with the mouse on the map, this event fires at completion of the drag
        'and returns a system.drawing.rectangle that has the bounds of the box that was "drawn"
    End Sub

    Public Sub MapExtentsChanged() Implements MapWindow.Interfaces.IPlugin.MapExtentsChanged
        'This event fires any time there is a zoom or pan that changes the extents of the map.
    End Sub

    Public Sub MapMouseDown(ByVal Button As Integer, ByVal Shift As Integer, ByVal x As Integer, ByVal y As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseDown
        'This event fires when the user holds a mouse button down on the map. Note that x and y are returned
        'as screen coordinates (in pixels), not map coordinates.  So if you really need the map coordinates
        'then you need to use g_MapWin.View.PixelToProj()
    End Sub

    Public Sub MapMouseMove(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseMove
        'This event fires when the user moves the mouse over the map. Note that x and y are returned
        'as screen coordinates (in pixels), not map coordinates.  So if you really need the map coordinates
        'then you need to use g_MapWin.View.PixelToProj()
    End Sub

    Public Sub MapMouseUp(ByVal Button As Integer, ByVal Shift As Integer, ByVal x As Integer, ByVal y As Integer, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.MapMouseUp
        'This event fires when the user releases a mouse button down on the map. Note that x and y are returned
        'as screen coordinates (in pixels), not map coordinates.  So if you really need the map coordinates
        'then you need to use g_MapWin.View.PixelToProj()
    End Sub

    Public Sub Message(ByVal msg As String, ByRef Handled As Boolean) Implements MapWindow.Interfaces.IPlugin.Message
        'Plug-ins can communicate with eachother using Messages.  If a message is sent then this event fires.
        'If you know the message is "for you" then you can set Handled=True and then it will not be sent to any
        'other plug-ins.
    End Sub

    Public Sub ProjectLoading(ByVal ProjectFile As String, ByVal SettingsString As String) Implements MapWindow.Interfaces.IPlugin.ProjectLoading
        'When the user opens a project in MapWindow, this event fires.  The ProjectFile is the file name of the
        'project that the user opened (including its path in case that is important for this this plug-in to know).
        'The SettingsString variable contains any string of data that is connected to this plug-in but is stored 
        'on a project level. For example, a plug-in that shows streamflow data might allow the user to set a 
        'separate database for each project (i.e. one database for the upper Missouri River Basin, a different 
        'one for the Lower Colorado Basin.) In this case, the plug-in would store the database name in the 
        'SettingsString of the project. 
    End Sub

    Public Sub ProjectSaving(ByVal ProjectFile As String, ByRef SettingsString As String) Implements MapWindow.Interfaces.IPlugin.ProjectSaving
        'When the user saves a project in MapWindow, this event fires.  The ProjectFile is the file name of the
        'project that the user is saving (including its path in case that is important for this this plug-in to know).
        'The SettingsString variable contains any string of data that is connected to this plug-in but is stored 
        'on a project level. For example, a plug-in that shows streamflow data might allow the user to set a 
        'separate database for each project (i.e. one database for the upper Missouri River Basin, a different 
        'one for the Lower Colorado Basin.) In this case, the plug-in would store the database name in the 
        'SettingsString of the project. 
    End Sub

    Public Sub ShapesSelected(ByVal Handle As Integer, ByVal SelectInfo As MapWindow.Interfaces.SelectInfo) Implements MapWindow.Interfaces.IPlugin.ShapesSelected
        'This event fires when the user selects one or more shapes using the select tool in MapWindow. Handle is the 
        'Layer handle for the shapefile on which shapes were selected. SelectInfo holds information abou the 
        'shapes that were selected. 
    End Sub

End Class
