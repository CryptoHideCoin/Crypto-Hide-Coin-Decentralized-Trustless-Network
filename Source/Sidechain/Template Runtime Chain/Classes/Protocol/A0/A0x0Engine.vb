﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.AreaEngine.DataFileManagement.Json
Imports CHCCommonLibrary.AreaEngine.Encryption
Imports CHCPrimaryRuntimeService.AreaCommon.Models.Network.Request
Imports CHCProtocolLibrary.AreaCommon.Models.Ledger





Namespace AreaProtocol

    ''' <summary>
    ''' This class contain all element to manage a A0x0 command
    ''' </summary>
    Public Class A0x0

        ''' <summary>
        ''' This method contain a request model
        ''' </summary>
        Public Class RequestModel : Implements IRequestModel

            Public Property common As New CommonRequest Implements IRequestModel.common
            Public Property content As New CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel

            ''' <summary>
            ''' This method provide to convert into a string the element of the object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function toString() As String Implements IRequestModel.toString
                Dim tmp As String = ""

                tmp += common.toString()
                tmp += content.toString()

                Return tmp
            End Function

            ''' <summary>
            ''' This methdo provide to get an hash of the object
            ''' </summary>
            ''' <returns></returns>
            Public Function getHash() As String Implements IRequestModel.getHash
                Return HashSHA.generateSHA256(Me.toString())
            End Function

        End Class

        ''' <summary>
        ''' This method contain all element of a request response
        ''' </summary>
        Public Class RequestResponseModel

            Inherits CHCCommonLibrary.AreaCommon.Models.General.RemoteResponse : Implements IRequestModel

            Private _Base As New RequestModel

            Public Property common As CommonRequest Implements IRequestModel.common
                Get
                    Return _Base.common
                End Get
                Set(value As CommonRequest)
                    _Base.common = value
                End Set
            End Property
            Public Property content As CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel
                Get
                    Return _Base.content
                End Get
                Set(value As CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel)
                    _Base.content = value
                End Set
            End Property
            Public Overrides Property signature As String
                Get
                    Return MyBase.signature
                End Get
                Set(value As String)
                    MyBase.signature = value
                End Set
            End Property

            ''' <summary>
            ''' This method provide to create a string of an element of this object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function toString() As String Implements IRequestModel.toString
                Return MyBase.toString() & _Base.toString()
            End Function

            ''' <summary>
            ''' This method provide to get the hash of this object
            ''' </summary>
            ''' <returns></returns>
            Public Function getHash() As String Implements IRequestModel.getHash
                Return _Base.getHash()
            End Function

        End Class

        ''' <summary>
        ''' This class contain all static member to recovery state 
        ''' </summary>
        Public Class RecoveryState

            ''' <summary>
            ''' This method provide to update the state from a request
            ''' </summary>
            ''' <param name="value"></param>
            ''' <param name="transactionChainRecord"></param>
            ''' <returns></returns>
            Public Shared Function fromRequest(ByRef value As RequestModel, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction) As Boolean
                Try
                    Dim proceed As Boolean = True

                    AreaCommon.log.track("RecoveryState.fromRequest", "Begin")

                    If proceed Then
                        proceed = AreaCommon.state.runtimeState.addNetworkProperty(AreaCommon.DAO.DBNetwork.MainPropertyID.networkCreationDate, value.common.requestDateTimeStamp, transactionChainRecord)
                    End If
                    If proceed Then
                        proceed = AreaCommon.state.runtimeState.addNetworkProperty(AreaCommon.DAO.DBNetwork.MainPropertyID.genesisPublicAddress, value.common.publicAddressRequester, transactionChainRecord)
                    End If
                    If proceed Then
                        AreaCommon.state.runtimeState.addNetworkProperty(AreaCommon.DAO.DBNetwork.MainPropertyID.networkName, value.content.netName, transactionChainRecord)
                    End If
                    If proceed Then
                        AreaCommon.state.runtimeState.addNetworkProperty(AreaCommon.DAO.DBNetwork.MainPropertyID.specialEnvironment, value.content.specialEnvironment, transactionChainRecord)
                    End If

                    AreaCommon.log.track("RecoveryState.fromRequest", "Completed")

                    Return proceed
                Catch ex As Exception
                    AreaCommon.log.track("RecoveryState.fromRequest", ex.Message, "fatal")

                    Return False
                End Try
            End Function

            Public Shared Function fromTransactionLedger(ByRef value As SingleTransactionLedger) As Boolean
                ''' TODO: A0x0 RecoveryState.fromTransactionLedger
            End Function

        End Class

        ''' <summary>
        ''' This class provides the static method to validate the request
        ''' </summary>
        Public Class FormalCheck

            ''' <summary>
            ''' This method provide to verify a formal request
            ''' </summary>
            ''' <param name="requestHash"></param>
            ''' <returns></returns>
            Shared Function verify(ByVal requestHash As String) As Nullable(Of Boolean)
                Try
                    Dim proceed As Boolean = True

                    AreaCommon.log.track("FormalCheck.verify", "Begin")

                    With AreaCommon.flow.getActiveRequest(requestHash).data
                        If proceed Then
                            proceed = (.common.netWorkReferement.Length > 0)
                        End If
                        If proceed Then
                            proceed = (.common.requestDateTimeStamp <= CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime())
                        End If
                        If proceed Then
                            proceed = (.content.netName.Trim.Length > 0)
                        End If
                        If proceed Then
                            proceed = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.SingleKeyPair.checkFormatPublicAddress(.common.publicAddressRequester)
                        End If
                        If proceed Then
                            proceed = AreaSecurity.checkSignature(.getHash, .common.signature, .common.publicAddressRequester)
                        End If
                    End With

                    AreaCommon.log.track("FormalCheck.verify", "Completed")

                    Return proceed
                Catch ex As Exception
                    AreaCommon.log.track("FormalCheck.verify", ex.Message, "fatal")

                    Return Nothing
                End Try
            End Function

            ''' <summary>
            ''' This method provide to evaluate a request
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Shared Function evaluate(ByRef value As AreaFlow.RequestExtended) As Boolean
                Try
                    Dim request As RequestModel = value.data

                    AreaCommon.log.track("FormalCheck.evaluate", "Begin")

                    If (request.common.requestDateTimeStamp <= CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime(Now.ToUniversalTime.AddDays(-1))) Then
                        value.evaluations.rejectedNote = "Request expired"
                        value.position.verify = AreaFlow.EnumOperationPosition.completeWithNegativeResult

                        Return True
                    End If
                    If (AreaCommon.state.network.position <> CHCRuntimeChainLibrary.AreaRuntime.AppState.EnumConnectionState.genesisOperation) Then
                        value.evaluations.rejectedNote = "Not permitted"
                        value.position.verify = AreaFlow.EnumOperationPosition.completeWithNegativeResult

                        Return True
                    End If
                    value.position.verify = AreaFlow.EnumOperationPosition.completeWithPositiveResult

                    AreaCommon.log.track("FormalCheck.evaluate", "Completed")

                    Return True
                Catch ex As Exception
                    AreaCommon.log.track("FormalCheck.evaluate", ex.Message, "fatal")

                    Return False
                End Try
            End Function

        End Class

        ''' <summary>
        ''' This static class provides to static method to manage a request
        ''' </summary>
        Public Class Manager

            ''' <summary>
            ''' This method provide to write request into ledger
            ''' </summary>
            ''' <returns></returns>
            Shared Function addIntoLedger(ByVal approverPublicAddress As String, ByVal consensusHash As String, ByVal registrationTimeStamp As String, ByVal value As CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel, ByVal requesterPublicAddress As String, ByVal requestHash As String) As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                Try
                    Dim contentPath As String = AreaCommon.state.ledger.proposeNewTransaction.pathData.contents
                    Dim hash As String = value.getHash()

                    AreaCommon.log.track("Manager.addIntoLedger", "Begin")

                    contentPath = IO.Path.Combine(contentPath, hash & ".Content")

                    If IOFast(Of CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel).save(contentPath, value) Then
                        With AreaCommon.state.ledger.proposeNewTransaction
                            .type = "a0x0"
                            .approverPublicAddress = approverPublicAddress
                            .consensusHash = consensusHash
                            .detailInformation = hash
                            .registrationTimeStamp = registrationTimeStamp
                            .requesterPublicAddress = requesterPublicAddress
                            .requestHash = requestHash
                            .currentHash = .getHash
                        End With

                        Return AreaCommon.state.ledger.saveAndClean()
                    Else
                        Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                    End If
                Catch ex As Exception
                    AreaCommon.state.currentService.currentAction.setError(Err.Number, ex.Message)

                    AreaCommon.log.track("A0x0.Manager.addIntoLedger", ex.Message, "fatal")

                    Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                Finally
                    AreaCommon.log.track("Manager.addIntoLedger", "Completed")
                End Try
            End Function

            ''' <summary>
            ''' This method provide to save a request into temporally position
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Public Shared Function saveTemporallyRequest(ByRef value As RequestModel) As Boolean
                Try
                    Return IOFast(Of RequestModel).save(IO.Path.Combine(AreaCommon.paths.workData.requestData.received, value.getHash & ".request"), value)
                Catch ex As Exception
                    Return False
                End Try
            End Function

            ''' <summary>
            ''' This method provide to save a request into temporally position from RequestResponseModel
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Public Shared Function saveTemporallyRequest(ByRef value As RequestResponseModel) As Boolean
                Return saveTemporallyRequest(value)
            End Function

            ''' <summary>
            ''' This method provide to load a request from a repository
            ''' </summary>
            ''' <param name="hash"></param>
            ''' <returns></returns>
            Public Shared Function loadRequest(ByVal completePath As String, ByVal hash As String) As RequestModel
                Try
                    Return IOFast(Of RequestModel).read(IO.Path.Combine(completePath, hash & ".request"))
                Catch ex As Exception
                    Return New RequestModel
                End Try
            End Function

            ''' <summary>
            ''' This method provide to create a initial procedure A0x0
            ''' </summary>
            ''' <param name="inputData"></param>
            ''' <returns></returns>
            Public Shared Function createInternalRequest(ByVal inputData As CHCProtocolLibrary.AreaCommon.Models.Network.BaseNetworkModel) As String
                Try
                    Dim request As New RequestModel

                    AreaCommon.log.track("A0x0Manager.createInternalRequest", "Begin")

                    AreaCommon.state.currentService.currentAction.setAction("1x0001", "BuildManager - A0x0 - A0x0Manager")

                    If AreaCommon.state.currentService.requestCancelCurrentRunCommand Then Return False

                    If (inputData.netName.CompareTo(AreaCommon.state.serviceInformation.netWorkName) <> 0) Then
                        AreaCommon.state.currentService.currentAction.setError("-1", "Network not compatible")
                        AreaCommon.state.currentService.currentAction.reset()

                        AreaCommon.log.track("A0x0Manager.createInternalRequest", "Error: Network not compatible", "fatal")

                        Return False
                    End If

                    With AreaCommon.state.keys.key(TransactionChainLibrary.AreaEngine.KeyPair.KeysEngine.KeyPair.enumWalletType.identity)
                        request.content.netName = inputData.netName
                        request.content.specialEnvironment = inputData.specialEnvironment

                        request.common.netWorkReferement = inputData.netName
                        request.common.chainReferement = AreaCommon.state.serviceInformation.chainName
                        request.common.type = "a0x0"
                        request.common.publicAddressRequester = .publicAddress
                        request.common.requestDateTimeStamp = AreaCommon.state.runtimeState.activeNetwork.networkCreationDate
                        request.common.hash = request.getHash()
                        request.common.signature = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.createSignature(.privateKey, request.common.hash)
                    End With

                    If saveTemporallyRequest(request) Then
                        AreaCommon.log.track("A0x0Manager.createInternalRequest", "request - Saved")

                        If AreaCommon.flow.addNewRequestDirect(request) Then
                            Return request.common.hash
                        Else
                            Return ""
                        End If
                    End If
                Catch ex As Exception
                    AreaCommon.state.currentService.currentAction.setError(Err.Number, ex.Message)

                    AreaCommon.log.track("A0x0Manager.createInternalRequest", ex.Message, "fatal")
                Finally
                    AreaCommon.log.track("A0x0Manager.createInternalRequest", "Completed")
                End Try

                Return ""
            End Function

        End Class

    End Class

End Namespace
