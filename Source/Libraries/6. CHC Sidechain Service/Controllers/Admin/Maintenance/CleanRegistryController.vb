﻿Option Compare Text
Option Explicit On

Imports System.Web.Http
Imports System.Threading
Imports CHCModelsLibrary.AreaModel.Network.Response
Imports CHCModelsLibrary.AreaModel.Information




Namespace Controllers


    ' GET: api/{GUID service}/administration/maintenance/cleanRegistry
    <Route("MaintenanceApi")>
    Public Class CleanRegistryController

        Inherits ApiController



        ''' <summary>
        ''' This method provide to get a registry stream
        ''' </summary>
        ''' <param name="signature"></param>
        ''' <returns></returns>
        Public Function PutValue(ByVal securityToken As String) As BaseRemoteResponse
            Dim ownerId As String = "CleanRegistry-" & Guid.NewGuid.ToString
            Dim result As New BaseRemoteResponse
            Dim response As String = ""
            Dim asynchThread As Thread
            Dim enter As Boolean = False
            Try
                If (AreaCommon.Main.serviceInformation.currentStatus = InternalServiceInformation.EnumInternalServiceState.started) Then
                    AreaCommon.Main.environment.log.trackEnter("CleanRegistryController.PutValue", ownerId,, CHCModelsLibrary.AreaModel.Log.AccessTypeEnumeration.api)

                    enter = True
                    response = AreaCommon.Main.environment.adminToken.check(securityToken)

                    If (response.Length = 0) Then
                        asynchThread = New Thread(AddressOf AreaAsynchronous.executeRegistryClean)

                        asynchThread.Start()
                    Else
                        result.responseStatus = RemoteResponse.EnumResponseStatus.missingAuthorization
                        result.errorDescription = response
                    End If
                Else
                    result.responseStatus = RemoteResponse.EnumResponseStatus.systemOffline
                End If
            Catch ex As Exception
                result.responseStatus = RemoteResponse.EnumResponseStatus.inError
                result.errorDescription = "503 - Generic Error"

                AreaCommon.Main.environment.log.trackException("CleanRegistryController.PutValue", ownerId, ex.Message)
            Finally
                If enter Then
                    AreaCommon.Main.environment.log.trackExit("CleanRegistryController.PutValue", ownerId)
                End If
            End Try

            result.responseTime = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()

            Return result
        End Function

    End Class

End Namespace
