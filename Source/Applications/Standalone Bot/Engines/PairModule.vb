﻿Option Compare Text
Option Explicit On

Imports Coinbase.Pro





Namespace AreaCommon.Engines.Pairs

    Module PairModule

        Private Const c_Second As Double = 1000
        Private Const c_Minute As Double = c_Second * 60


        Private Property _InWorkJob As Boolean = False
        Private Property _Init As Boolean = False
        Private Property _ClientPro As CoinbaseProClient

        Private Property _LastUpdateTick As Double = 0


        ''' <summary>
        ''' This method provide to update tick pair
        ''' </summary>
        ''' <param name="pair"></param>
        Private Async Sub updateTick(ByVal pair As Models.Pair.PairInformation)
            Try
                Dim market = Await _ClientPro.MarketData.GetTickerAsync(pair.key)
                Dim tick As New Models.Pair.TickInformation

                pair.lastUpdateTick = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
                _LastUpdateTick = pair.lastUpdateTick

                pair.currentValue = market.Price

                AreaState.updateChange(pair.key, pair.currentValue)

                If (pair.currentRelativeAverageValue = 0) Then
                    pair.currentRelativeAverageValue = market.Price
                Else
                    pair.currentRelativeAverageValue = (pair.currentRelativeAverageValue + market.Price) / 2
                End If

                tick.time = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
                tick.value = market.Price

                If (market.Price > pair.currentRelativeAverageValue) Then
                    tick.position = Models.Pair.TickInformation.tickPositionEnumeration.increase
                ElseIf market.Price = pair.currentRelativeAverageValue Then
                    tick.position = Models.Pair.TickInformation.tickPositionEnumeration.same
                Else
                    tick.position = Models.Pair.TickInformation.tickPositionEnumeration.decrease
                End If

                pair.addNewItem(tick)
            Catch ex As Exception
                If Not ex.Message Like "*Call timed out*" Then
                    MessageBox.Show("Problem during updateTick - " & ex.Message)
                End If

            End Try
        End Sub


        ''' <summary>
        ''' This method provide to process a pair
        ''' </summary>
        ''' <param name="currentIndex"></param>
        Private Function process(ByVal currentIndex As Integer) As Boolean
            Try
                Dim pair As Models.Pair.PairInformation = AreaState.pairs.ElementAt(currentIndex).Value
                Dim currentTime As Double = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()

                If ((pair.lastUpdateTick + c_Minute) < currentTime) Then
                    If (currentTime > _LastUpdateTick + 100) Then
                        Task.Run(Sub() updateTick(pair)).Start()
                    End If
                Else
                    Return False
                End If
            Catch ex As Exception
            End Try

            Return True
        End Function

        ''' <summary>
        ''' This method provide to start service processor
        ''' </summary>
        Private Sub startServiceProcessor()
            Try
                Dim currentIndex As Integer = 0

                Do While _InWorkJob
                    If (AreaState.pairs.Count > 0) Then
                        If (currentIndex + 1 > AreaState.pairs.Count) Then
                            currentIndex = 0
                        End If

                        If Not process(currentIndex) Then
                            currentIndex += 1
                        End If
                    End If

                    Threading.Thread.Sleep(10)
                Loop
            Catch ex As Exception
                _InWorkJob = False
            End Try
        End Sub

        ''' <summary>
        ''' This method provide to check if the pair exist
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Async Function testPair(ByVal value As String) As Task(Of Boolean)
            Try
                If Not _Init Then
                    _Init = init()
                End If

                Threading.Thread.Sleep(100)

                Dim market = Await _ClientPro.MarketData.GetTickerAsync(value)

                Threading.Thread.Sleep(100)

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function init() As Boolean
            Try
                _ClientPro = New CoinbaseProClient(New Config With {.ApiKey = AreaState.defaultUserDataAccount.exchangeAccess.APIKey, .Passphrase = AreaState.defaultUserDataAccount.exchangeAccess.passphrase, .Secret = AreaState.defaultUserDataAccount.exchangeAccess.secret, .ApiUrl = AreaState.defaultUserDataAccount.exchangeAccess.apiURL})

                _Init = True

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to start a pair job
        ''' </summary>
        ''' <returns></returns>
        Public Function [start]() As Boolean
            If Not _Init Then
                _Init = init()

                _Init = True
            End If

            If Not _InWorkJob Then
                Dim objWS As Threading.Thread

                _InWorkJob = True

                objWS = New Threading.Thread(AddressOf startServiceProcessor)

                objWS.Start()
            End If

            Return True
        End Function

        Public Function [stop]() As Boolean
            _InWorkJob = False

            Return True
        End Function

    End Module

End Namespace