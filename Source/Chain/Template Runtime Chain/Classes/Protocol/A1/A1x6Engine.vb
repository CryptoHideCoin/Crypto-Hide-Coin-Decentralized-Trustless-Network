﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.Support
Imports CHCCommonLibrary.AreaEngine.DataFileManagement
Imports CHCCommonLibrary.AreaEngine.Encryption




Namespace AreaProtocol

    Public Class A1x6

        Public Class RequestModel

            Public Property requestDateTimeStamp As Double = 0
            Public Property publicWalletAddressRequester As String = ""
            Public Property requestHash As String = ""
            Public Property signature As String = ""

            Public Property chainName As String = ""
            Public Property generalConditionsDocument As String = ""

            Public Overrides Function toString() As String
                Dim tmp As String = ""

                tmp += MyBase.toString()
                tmp += chainName
                tmp += GeneralConditionsDocument

                Return tmp
            End Function

            Public Function getHash() As String
                Return HashSHA.generateSHA256(Me.toString())
            End Function

        End Class

        Public Class FileEngine

            Inherits BaseFileDB(Of RequestModel)

        End Class

        Public Class RecoveryState

            Public Shared Function fromRequest(ByRef value As RequestModel, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger) As Boolean
                With AreaCommon.state.runtimeState.getDataChain(value.chainName).generalCondition
                    .recordCoordinate = transactionChainRecord.recordCoordinate
                    .recordHash = transactionChainRecord.recordHash
                    .value = value.generalConditionsDocument
                End With

                Return True
            End Function

            Public Shared Function fromTransactionLedger(ByVal statePath As String, ByVal chainName As String, ByRef data As TransactionChainLibrary.AreaLedger.LedgerEngine.SingleRecordLedger) As Boolean
                Try
                    AreaCommon.state.runtimeState.activeChains(chainName).generalCondition.value = TransactionChainLibrary.AreaEngine.Ledger.State.StateEngine.readContentFromFile(statePath, data.detailInformation)

                    Return True
                Catch ex As Exception
                    Return False
                End Try

                Return True
            End Function

        End Class

        Public Class Manager

            Private data As New RequestModel

            Public Property log As LogEngine
            Public Property currentService As CHCProtocolLibrary.AreaCommon.Models.Administration.ServiceStateResponse


            Private Function writeDataIntoLedger(ByVal contentStatePath As String) As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
                Try
                    With AreaCommon.state.currentBlockLedger.currentRecord
                        .actionCode = "a1x6"
                        .approvedDate = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime
                        .detailInformation = HashSHA.generateSHA256(data.generalConditionsDocument)
                        .requester = data.publicWalletAddressRequester
                        .requestHash = data.requestHash
                    End With

                    TransactionChainLibrary.AreaEngine.Ledger.State.StateEngine.writeDataContent(contentStatePath, data.generalConditionsDocument, AreaCommon.state.currentBlockLedger.currentRecord.detailInformation)

                    If AreaCommon.state.currentBlockLedger.BlockComplete() Then
                        Return AreaCommon.state.currentBlockLedger.saveAndClean()
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A1x6Manager.init", ex.Message, "fatal")
                End Try

                Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
            End Function


            Public Function init(ByRef paths As CHCProtocolLibrary.AreaSystem.VirtualPathEngine, ByVal generalConditionParameter As String, ByVal publicWalletIdAddress As String, ByVal privateKeyRAW As String) As Boolean
                Try
                    Dim requestFileEngine As New FileEngine
                    Dim ledgerCoordinate As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger

                    log.track("A1x6Manager.init", "Begin")

                    currentService.currentAction.setAction("3x0005", "BuildManager - A1x6 - A1x6Manager")

                    If currentService.requestCancelCurrentRunCommand Then Return False

                    data.generalConditionsDocument = generalConditionParameter
                    data.publicWalletAddressRequester = publicWalletIdAddress
                    data.requestDateTimeStamp = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                    data.requestHash = data.getHash
                    data.signature = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.createSignature(privateKeyRAW, data.requestHash)

                    requestFileEngine.data = data

                    requestFileEngine.fileName = IO.Path.Combine(AreaCommon.paths.workData.currentVolume.requests, data.requestHash & ".request")

                    If requestFileEngine.save() Then
                        log.track("A1x6Manager.init", "request - Saved")

                        ledgerCoordinate = writeDataIntoLedger(paths.workData.state.contents)

                        If (ledgerCoordinate.recordCoordinate.Length = 0) Then
                            currentService.currentAction.setError("-1", "Error during update ledger")
                            currentService.currentAction.reset()

                            log.track("A1x6Manager.init", "Error: Error during update ledger", "fatal")

                            Return False
                        End If

                        log.track("A1x6Manager.init", "Ledger updated")

                        If Not RecoveryState.fromRequest(data, ledgerCoordinate) Then
                            currentService.currentAction.setError("-1", "Error create state")
                            currentService.currentAction.reset()

                            log.track("A1x6Manager.init", "Error: Error during update State", "fatal")

                            Return False
                        End If

                        log.track("A1x6Manager.init", "State updated")

                        Return True
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A1x6Manager.init", ex.Message, "fatal")
                End Try

                Return False
            End Function

        End Class

    End Class

End Namespace
