﻿Option Compare Text
Option Explicit On

' ****************************************
' Engine: Virtual Path Structure
' Release Engine: 1.0 
' 
' Date last successfully test: 03/10/2021
' ****************************************


Imports CHCCommonLibrary.AreaEngine.DataFileManagement.XML





Namespace AreaSystem

    ''' <summary>
    ''' This class manage a Virtual Path of a Crypto Hide Coin project
    ''' </summary>
    Public Class VirtualPathEngine

        ''' <summary>
        ''' This enumeration specify module of a system
        ''' </summary>
        Public Enum EnumSystemType
            admin
            loader
            maintenance
            runTime
        End Enum

        ''' <summary>
        ''' This class contain the information relative of a other path not code into engine
        ''' </summary>
        Public Class OtherPathEngine

            Inherits BaseFile(Of List(Of OtherPath))


            ''' <summary>
            ''' This class contain the property of a single property of Other Path
            ''' </summary>
            Public Class OtherPath

                Public Property context As String = ""
                Public Property alternativePath As String = ""

            End Class

        End Class

        ''' <summary>
        ''' This class contain the specific path of all element of the system path
        ''' </summary>
        Public Class SystemPath

            Public Property path As String = ""
            Public Property counters As String = ""
            Public Property events As String = ""
            Public Property logs As String = ""

        End Class

        ''' <summary>
        ''' This class contain the contain all information reguard the define of the transaction chain
        ''' </summary>
        Public Class DefinePath

            Public Property path As String = ""

            Public Property assets As String = ""
            Public Property pricesList As String = ""
            Public Property privacyPapers As String = ""

            Public Property referenceProtocols As String = ""
            Public Property refundPlans As String = ""
            Public Property sideChainContracts As String = ""

            Public Property termsAndConditionsPapers As String = ""

            Public Property whitePapers As String = ""
            Public Property yellowPapers As String = ""

        End Class

        ''' <summary>
        ''' This class contain the elements relative of a State data
        ''' </summary>
        Public Class StateWorkPath

            Public Property path As String = ""

            Public Property db As String = ""
            Public Property contents As String = ""

        End Class

        ''' <summary>
        ''' This class contain the element of a work volume
        ''' </summary>
        Public Class WorkVolumePath

            Public Property path As String = ""

            Public Property requests As String = ""
            Public Property ledger As String = ""
            Public Property bulletines As String = ""
            Public Property consensus As String = ""

        End Class

        ''' <summary>
        ''' This class contain the element of a work path
        ''' </summary>
        Public Class WorkPath

            Public Property path As String = ""

            Public Property currentVolume As New WorkVolumePath
            Public Property previousVolume As New WorkVolumePath
            Public Property messages As String = ""
            Public Property temporally As String = ""
            Public Property state As StateWorkPath = New StateWorkPath
            Public Property internal As String = ""

        End Class


        Private Const keyStoreFolderName As String = "KeyStore"
        Private Const settingsFolderName As String = "Settings"
        Private Const systemFolderName As String = "System"
        Private Const workFolderName As String = "Work"
        Private Const storageFolderName As String = "Storage"
        Private Const adminFolderName As String = "Admin"
        Private Const runTimeFolderName As String = "RunTime"
        Private Const maintenanceFolderName As String = "Maintenance"
        Private Const loaderFolderName As String = "Loader"
        Private Const countersFolderName As String = "Counters"
        Private Const eventsFolderName As String = "Events"
        Private Const logsFolderName As String = "Logs"
        Private Const chainFolderName As String = "Chain-"
        Private Const ledgerName As String = "Ledger"
        Private Const requestsName As String = "Requests"
        Private Const messagesName As String = "Messages"
        Private Const temporallyName As String = "Temporally"
        Private Const stateName As String = "State"
        Private Const dbName As String = "Db"
        Private Const contentsName As String = "Contents"
        Private Const internalName As String = "Internal"
        Private Const bulletinName As String = "Bulletin"
        Private Const consensusName As String = "Consensus"
        Private Const defineName As String = "Define"
        Private Const assetsName As String = "Assets"
        Private Const priceListName As String = "PriceList"
        Private Const privacyPapersName As String = "PrivacyPapers"
        Private Const referenceProtocolsName As String = "ReferenceProtocols"
        Private Const refundPlansName As String = "RefundPlans"
        Private Const sideChainContractsName As String = "SidechainContracts"
        Private Const termAndConditionPapersName As String = "TermAndConditionPapers"
        Private Const whitePapersName As String = "WhitePapers"
        Private Const yellowPapersName As String = "YellowPapers"

        Private Const otherPathFileName As String = "other.path"

        Private Const defaultAdminServiceSettings As String = "AdminService.Settings"
        Private Const defaultRunTimeServiceSettings As String = "RunTimeService.Settings"
        Private Const defaultLoaderServiceSettings As String = "LoaderService.Settings"

        Public Property settingFileName As String = ""
        Public Property activeChain As String = ""

        Public Property directoryData As String = ""
        Public Property keyStore As String = ""
        Public Property system As New SystemPath
        Public Property settings As String = ""
        Public Property workData As New WorkPath
        Public Property workDefine As New DefinePath
        Public Property storage As String = ""


        ''' <summary>
        ''' This method provide to manage a single path
        ''' </summary>
        ''' <param name="pathParent"></param>
        ''' <param name="pathDirectory"></param>
        ''' <param name="pathOptional"></param>
        ''' <returns></returns>
        Private Function manageSinglePath(ByVal pathParent As String, ByVal pathDirectory As String, Optional ByVal pathOptional As String = "") As String
            Try
                If (pathOptional.Length = 0) Then
                    pathParent = IO.Path.Combine(pathParent, pathDirectory)
                Else
                    pathParent = IO.Path.Combine(pathParent, pathDirectory, pathOptional)
                End If

                If Not IO.Directory.Exists(pathParent) Then
                    IO.Directory.CreateDirectory(pathParent)
                End If

                Return pathParent
            Catch ex As Exception
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' This method provide to initialize the engine
        ''' </summary>
        ''' <param name="[type]"></param>
        ''' <param name="chainName"></param>
        ''' <returns></returns>
        Public Function init(ByVal [type] As EnumSystemType, Optional ByVal chainName As String = "Primary") As Boolean
            Try
                If (directoryData.Trim.Length > 0) Then
                    Dim folderName As String

                    activeChain = chainName

                    Select Case type
                        Case EnumSystemType.admin
                            settingFileName = defaultAdminServiceSettings
                            folderName = adminFolderName
                        Case EnumSystemType.runTime
                            settingFileName = chainName & "-" & defaultRunTimeServiceSettings
                            folderName = runTimeFolderName
                        Case EnumSystemType.maintenance
                            settingFileName = defaultLoaderServiceSettings
                            folderName = maintenanceFolderName
                        Case Else
                            settingFileName = defaultLoaderServiceSettings
                            folderName = loaderFolderName
                    End Select

                    keyStore = manageSinglePath(directoryData, keyStoreFolderName)

                    Try
                        Dim fileName As String = IO.Path.Combine(keyStore, "define.path")

                        If (IO.File.Exists(fileName)) Then
                            keyStore = IO.File.ReadAllText(fileName)
                        End If
                    Catch ex As Exception
                    End Try

                    settings = manageSinglePath(directoryData, settingsFolderName)

                    With system
                        .path = manageSinglePath(directoryData, systemFolderName)
                        .counters = manageSinglePath(.path, countersFolderName, folderName)
                        .events = manageSinglePath(.path, eventsFolderName, folderName)
                        .logs = manageSinglePath(.path, logsFolderName, folderName)
                    End With

                    With workData
                        folderName = manageSinglePath(directoryData, workFolderName, chainFolderName & chainName)

                        .path = folderName

                        With .currentVolume
                            .path = manageSinglePath(folderName, "Ledger-" & Now.ToUniversalTime.Year.ToString())

                            .ledger = manageSinglePath(.path, ledgerName)
                            .requests = manageSinglePath(.path, requestsName)
                            .bulletines = manageSinglePath(.path, bulletinName)
                            .consensus = manageSinglePath(.path, consensusName)
                        End With

                        With .previousVolume
                            .path = manageSinglePath(folderName, "Ledger-" & (Now.ToUniversalTime.Year - 1).ToString())

                            .ledger = manageSinglePath(.path, ledgerName)
                            .requests = manageSinglePath(.path, requestsName)
                            .bulletines = manageSinglePath(.path, bulletinName)
                            .consensus = manageSinglePath(.path, consensusName)
                        End With

                        .messages = manageSinglePath(.path, messagesName)
                        .temporally = manageSinglePath(.path, temporallyName)

                        With .state
                            .path = manageSinglePath(folderName, stateName)

                            .db = manageSinglePath(.path, dbName)
                            .contents = manageSinglePath(.path, contentsName)
                        End With

                        .internal = manageSinglePath(.path, internalName)
                    End With

                    With workDefine
                        .path = manageSinglePath(directoryData, workFolderName, defineName)

                        .assets = manageSinglePath(.path, assetsName)
                        .pricesList = manageSinglePath(.path, priceListName)
                        .privacyPapers = manageSinglePath(.path, privacyPapersName)
                        .referenceProtocols = manageSinglePath(.path, referenceProtocolsName)
                        .refundPlans = manageSinglePath(.path, refundPlansName)
                        .sideChainContracts = manageSinglePath(.path, sideChainContractsName)
                        .termsAndConditionsPapers = manageSinglePath(.path, termAndConditionPapersName)
                        .whitePapers = manageSinglePath(.path, whitePapersName)
                        .yellowPapers = manageSinglePath(.path, yellowPapersName)
                    End With

                    Dim engine As New OtherPathEngine

                    engine.fileName = IO.Path.Combine(directoryData, otherPathFileName)

                    If engine.read() Then

                        For Each except In engine.data
                            If (except.context = storageFolderName) Then
                                storage = except.alternativePath
                            End If
                        Next

                    End If

                    If (storage = "") Then
                        storage = manageSinglePath(directoryData, storageFolderName, chainFolderName & chainName)
                    End If
                End If
            Catch ex As Exception
            End Try

            Return True
        End Function

        ''' <summary>
        ''' This method provide to test to read a settings path
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Private Function trySettingsPath(ByVal path As String) As Boolean
            Try
                path = IO.Path.Combine(path, "define.path")
                Return IO.File.Exists(path)
            Catch ex As Exception
            End Try

            Return False
        End Function

        ''' <summary>
        ''' This method provide to test write a path
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Private Function tryWritePath(ByVal path As String) As Boolean
            path = IO.Path.Combine(path, "define.path")

            Try
                IO.File.WriteAllText(path, "Test")

                If IO.File.Exists(path) Then
                    IO.File.Delete(path)
                    Return True
                End If
            Catch ex As Exception
            End Try

            Return False
        End Function

        ''' <summary>
        ''' This method test a single path
        ''' </summary>
        ''' <param name="found"></param>
        ''' <param name="path"></param>
        ''' <param name="newPath"></param>
        ''' <param name="trySettings"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Private Function testPath(ByVal found As Boolean, ByRef path As String, ByVal newPath As String, Optional ByVal trySettings As Boolean = False) As Boolean
            If Not found Then

                path = newPath

                If trySettings Then
                    Return trySettingsPath(path)
                Else
                    Return tryWritePath(path)
                End If

            End If

            Return found
        End Function

        ''' <summary>
        ''' This method search define path
        ''' </summary>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function searchDefinePath() As String
            Dim found As Boolean = False
            Dim path As String = ""

            Try
                found = testPath(found, path, Application.StartupPath, True)
                found = testPath(found, path, Application.LocalUserAppDataPath, True)
                found = testPath(found, path, Application.UserAppDataPath, True)

                If found Then
                    Return path
                End If
            Catch ex As Exception
            End Try

            Return ""
        End Function

        ''' <summary>
        ''' This method search and read a define path
        ''' </summary>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function readDefinePath() As String
            Try
                Dim path As String = searchDefinePath()

                If (path.Length > 0) Then
                    Return IO.File.ReadAllText(IO.Path.Combine(searchDefinePath, "define.path"))
                End If
            Catch ex As Exception
            End Try

            Return ""
        End Function

        ''' <summary>
        ''' This method provide to update root path
        ''' </summary>
        ''' <param name="dataPath"></param>
        <DebuggerHiddenAttribute()> Public Sub updateRootPath(ByVal dataPath As String)
            Dim found As Boolean = False
            Dim path As String = ""

            Try
                found = testPath(found, path, Application.StartupPath, True)
                found = testPath(found, path, Application.LocalUserAppDataPath, True)
                found = testPath(found, path, Application.UserAppDataPath, True)
                found = testPath(found, path, Application.StartupPath)
                found = testPath(found, path, Application.LocalUserAppDataPath)
                found = testPath(found, path, Application.UserAppDataPath)

                If found Then
                    IO.File.WriteAllText(IO.Path.Combine(path, "define.path"), dataPath)
                End If

            Catch ex As Exception
            End Try
        End Sub


    End Class


End Namespace