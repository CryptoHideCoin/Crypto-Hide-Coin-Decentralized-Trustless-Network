﻿Option Compare Text
Option Explicit On


Imports System.Web.Http
Imports CHCCommonLibrary.AreaCommon.Models




Namespace Controllers


    ' GET: API/{GUID service}/Administration/DoAction/VerifyData
    <Route("AdministrationDoActionApi")>
    Public Class VerifyDataController

        Inherits ApiController




        ''' <summary>
        ''' This method provides to get a verify data procedure 
        ''' </summary>
        ''' <param name="signature"></param>
        ''' <returns></returns>
        Public Function getValue(ByVal signature As String) As General.RemoteResponse
            Dim result As New General.RemoteResponse

            Try
                AreaCommon.log.track("VerifyDataController.getValue", "Begin")

                result.requestTime = CHCCommonLibrary.AreaEngine.Miscellaneous.atMomentGMT()

                If (AreaCommon.state.serviceInformation.currentStatus = CHCProtocolLibrary.AreaCommon.Models.Service.InternalServiceInformation.EnumInternalServiceState.started) Then
                    If AreaSecurity.checkSignature(signature) Then
                        For Each item In AreaCommon.state.currentService.listAvailableCommand
                            If (item = CHCProtocolLibrary.AreaCommon.Models.Administration.EnumActionAdministration.verifyData) Then
                                AreaCommon.state.currentService.currentRunCommand = CHCProtocolLibrary.AreaCommon.Models.Administration.EnumActionAdministration.verifyData

                                AreaCommon.state.currentService.listAvailableCommand.Clear()
                                AreaCommon.state.currentService.listAvailableCommand.Add(CHCProtocolLibrary.AreaCommon.Models.Administration.EnumActionAdministration.cancelCurrentAction)

                                Dim ais As New Threading.Thread(AddressOf AreaData.analyzeInternalState)

                                ais.Start()

                                result.responseStatus = General.RemoteResponse.EnumResponseStatus.responseComplete

                                Exit For
                            End If
                        Next

                        If (result.responseStatus <> General.RemoteResponse.EnumResponseStatus.responseComplete) Then
                            result.responseStatus = General.RemoteResponse.EnumResponseStatus.commandNotAllowed
                        End If
                    Else
                        result.responseStatus = General.RemoteResponse.EnumResponseStatus.missingAuthorization
                    End If
                Else
                    result.responseStatus = General.RemoteResponse.EnumResponseStatus.systemOffline
                End If

                AreaCommon.log.track("VerifyDataController.getValue", "Completed")
            Catch ex As Exception
                result.responseStatus = General.RemoteResponse.EnumResponseStatus.inError
                result.errorDescription = "503 - Generic Error"

                AreaCommon.log.track("VerifyDataController.getValue", "An error occurrent during execute: " & ex.Message, "fatal")
            End Try

            result.responseTime = CHCCommonLibrary.AreaEngine.Miscellaneous.atMomentGMT()

            Return result
        End Function

    End Class


End Namespace