﻿Option Compare Text
Option Explicit On

Imports System.Web.Http

Imports CHCModelsLibrary.AreaModel.Administration.Security
Imports CHCModelsLibrary.AreaModel.Network.Response
Imports CHCModelsLibrary.AreaModel.Information




Namespace Controllers


    ' GET: api/{GUID service}/administration/security/requestAdminSecurityToken/
    <Route("SecurityApi")>
    Public Class RequestAdminSecurityTokenController

        Inherits ApiController



        ''' <summary>
        ''' This method provides to return an Admin Security Token 
        ''' </summary>
        ''' <param name="signature">Is asignature of a certificate with a admin private key</param>
        ''' <returns></returns>
        Public Function GetValue(ByVal signature As String) As ResponseRequestAdminSecurityTokenModel
            Dim ownerId As String = "RequestAdminSecurityToken-" & Guid.NewGuid.ToString
            Dim result As New ResponseRequestAdminSecurityTokenModel
            Dim enter As Boolean = False
            Try
                If (AreaCommon.Main.serviceInformation.currentStatus = InternalServiceInformation.EnumInternalServiceState.started) Then
                    If AreaSecurity.checkSignature(signature, AreaCommon.Main.environment.adminToken.getCurrentAccessKey()) Then
                        AreaCommon.Main.environment.log.trackEnter("RequestAdminSecurityToken.GetValue", ownerId,, CHCModelsLibrary.AreaModel.Log.AccessTypeEnumeration.api)

                        enter = True
                        result.tokenValue = AreaCommon.Main.environment.adminToken.createNewToken()

                        AreaCommon.Main.environment.log.trackIntoConsole("Security token created")
                        AreaCommon.Main.environment.registry.addNew(CHCModelsLibrary.AreaModel.Registry.RegistryData.TypeEvent.adminTokenReleased)
                    Else
                        result.responseStatus = RemoteResponse.EnumResponseStatus.missingAuthorization
                    End If
                Else
                    result.responseStatus = RemoteResponse.EnumResponseStatus.systemOffline
                End If
            Catch ex As Exception
                result.responseStatus = RemoteResponse.EnumResponseStatus.inError
                result.errorDescription = "503 - Generic Error"

                AreaCommon.Main.environment.log.trackException("RequestAdminSecurityTokenController.GetValue", ownerId, ex.Message)
            Finally
                If enter Then
                    AreaCommon.Main.environment.log.trackExit("RequestAdminSecurityToken.GetValue", ownerId, result.tokenValue)
                End If
            End Try

            result.responseTime = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()

            Return result
        End Function


    End Class

End Namespace

