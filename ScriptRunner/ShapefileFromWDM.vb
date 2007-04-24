Imports atcData
Imports atcData.atcDataGroup
Imports atcUtility
Imports MapWindow.Interfaces
Imports MapWinUtility
Imports atcGisUtilities

Imports Microsoft.VisualBasic
Imports System

Public Module ScriptShapefileFromWDM
    Private Const pFieldWidth As Integer = 12
    'Private Const pTestPath As String = "D:\Basins\data\03130003\met\"
    'Private Const pTestWDMFileName As String = "FtBenMet.wdm"
    Private Const pTestPath As String = "D:\Basins\data\03130003\flow\"
    Private Const pTestWDMFileName As String = "flow.wdm"

    Public Sub ScriptMain(ByRef aMapWin As IMapWin)

        'For the specified WDM file, build a shapefile containing 
        'a point for each timeseries

        'Output:
        'writes to the pTestPath folder, a shapefile named: 
        '  'base name of wdm file'.shp

        Logger.Dbg("Start")

        'change to the directory of the current project
        ChDriveDir(pTestPath)
        Logger.Dbg(" CurDir:" & CurDir())

        'declare a new data manager to manage the wdm file
        Dim lDataManager As New atcDataManager(aMapWin)

        'open the wdm file
        Dim lHspfWdmFile As atcDataSource = New atcWDM.atcDataSourceWDM
        lDataManager.OpenDataSource(lHspfWdmFile, pTestWDMFileName, Nothing)
        Logger.Dbg(" DataSetCount " & lHspfWdmFile.DataSets.Count)

        'populate the timeseries grid source
        Dim lXfieldName As String = "longitude"
        Dim lYfieldName As String = "latitude"
        Dim lDisplayAttributes As New ArrayList
        lDisplayAttributes = lDataManager.DisplayAttributes
        lDisplayAttributes.Add(lXfieldName)
        lDisplayAttributes.Add(lYfieldName)
        lDisplayAttributes.Add("scenario")
        lDisplayAttributes.Add("location")
        lDisplayAttributes.Add("constituent")
        lDisplayAttributes.Add("stanam")
        lDisplayAttributes.Add("sjday")
        lDisplayAttributes.Add("ejday")
        lDisplayAttributes.Add("time unit")
        lDisplayAttributes.Add("sumannual")
        Dim lDataGroup As New atcDataGroup
        lDataGroup = lDataManager.DataSets
        Dim lSource As atcTimeseriesGridSource
        lSource = New atcTimeseriesGridSource(lDataManager, lDataGroup, lDisplayAttributes, "False")

        'convert that grid source to a shapefile
        Dim lOutputProjection As String = aMapWin.Project.ProjectProjection
        Dim lShapefileName As String = pTestPath & FilenameOnly(pTestWDMFileName) & ".shp"
        GisUtilities.GridSourceToShapefile(aMapWin, lShapefileName, lSource, lXfieldName, lYfieldName, lOutputProjection)

        lDataManager.DataSources.Remove(lHspfWdmFile)
        lHspfWdmFile.DataSets.Clear()
        lHspfWdmFile = Nothing

    End Sub

End Module
