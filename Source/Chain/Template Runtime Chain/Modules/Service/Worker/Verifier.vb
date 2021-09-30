﻿Option Explicit On
Option Compare Text




Namespace AreaWorker

    Module Verifier

        Public Property workerOn As Boolean = False



        Private Function evaluateTheRequest(ByRef value As AreaFlow.RequestExtended) As Nullable(Of Boolean)
            Try
                AreaCommon.log.track("Verifier.evaluateTheRequest", "Begin")

                Select Case value.requestCode
                    Case "a0x0" : Return AreaProtocol.A0x0.FormalCheck.evaluate(value)
                End Select

                Return False
            Catch ex As Exception
                AreaCommon.log.track("Verifier.evaluateTheRequest", ex.Message, "fatal")

                Return Nothing
            End Try
        End Function


        Public Function work() As Boolean
            Try
                Dim item As AreaFlow.RequestExtended

                AreaCommon.log.track("Verifier.work", "Begin")

                workerOn = True

                Do While (AreaCommon.flow.workerOn And workerOn)
                    item = AreaCommon.flow.getFirstRequestToVerify()

                    If (item.requestHash.Length > 0) Then
                        item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.inWork

                        Select Case evaluateTheRequest(item)
                            Case True : item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.completeWithPositiveResult
                            Case False : item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.completeWithNegativeResult
                            Case Else : item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.inError
                        End Select

                        If item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.inError Then
                            AreaCommon.flow.removeRequest(item)
                        Else
                            item.dateAssessment = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                            item.evaluations.notExpressed = AreaCommon.flow.createConsensusList()
                            item.evaluations.currentChainNodetotalVotes = item.evaluations.notExpressed.totalValuePoints

                            If item.verifyPosition = AreaFlow.RequestExtended.EnumOperationPosition.completeWithPositiveResult Then
                                item.evaluations.setApproved(AreaCommon.state.network.publicAddressIdentity)
                            Else
                                item.evaluations.setRejected(AreaCommon.state.network.publicAddressIdentity)
                            End If

                            AreaCommon.flow.setRequestToProcess(item)
                        End If
                    End If

                    Threading.Thread.Sleep(5)
                Loop

                workerOn = False

                AreaCommon.log.track("Verifier.work", "Complete")

                Return True
            Catch ex As Exception
                AreaCommon.log.track("Verifier.work", ex.Message, "fatal")

                Return False
            End Try
        End Function


    End Module

End Namespace