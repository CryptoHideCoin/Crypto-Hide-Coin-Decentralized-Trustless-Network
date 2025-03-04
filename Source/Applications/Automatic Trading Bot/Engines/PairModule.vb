﻿Option Compare Text
Option Explicit On

'Imports Coinbase.Pro
'Imports Coinbase.Pro.WebSockets
'Imports Coinbase.Pro.Models
'Imports WebSocket4Net





Namespace AreaCommon.Engines.Pairs

    Module PairModule

        Private Enum SourceTickerEnum
            undefined
            readFromFile
            ticker
            subscription
        End Enum

        Private Const c_Second As Double = 1000
        Private Const c_Minute As Double = c_Second * 60

        'Private Property _ClientPro As New CoinbaseProClient

        Private Property _LastUpdateTick As Double = 0
        Private Property _SourceMode As SourceTickerEnum = SourceTickerEnum.undefined

        Public Property inWorkJob As Boolean = False


        Public Async Sub manageFilledProductInformation(ByVal pair As String)
            With AreaState.products.getCurrency(pair.Split("-")(0)).header
                If (.name.Length > 0) And (.baseIncrement.Length = 0) Then

                    'Dim currency = Await _ClientPro.MarketData.GetSingleProductAsync(pair)
                    Dim currency = Await AreaState.exchangeProxy.getSingleProduct(pair)

                    .baseIncrement = currency.BaseIncrement.ToString()
                    .limitOnly = currency.LimitOnly
                    .minMarketFunds = currency.MinMarketFunds.ToString()
                    .postOnly = currency.PostOnly
                    .quoteIncrement = currency.QuoteIncrement.ToString()
                    .status = currency.Status
                    .statusMessage = currency.StatusMessage
                    .tradingDisabled = currency.TradingDisabled

                End If
            End With
        End Sub

        ''' <summary>
        ''' This method provide to update tick pair
        ''' </summary>
        ''' <param name="pair"></param>
        Private Async Sub updateTick(ByVal pair As Models.Pair.PairInformation)
            Try
                pair.lastUpdateTick = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
                _LastUpdateTick = pair.lastUpdateTick

                'Dim market = Await _ClientPro.MarketData.GetTickerAsync(pair.key)
                Dim market = Await AreaState.exchangeProxy.getTicker(pair.key)
                Dim tick As New Models.Pair.TickInformation

                pair.currentValue = market.Price

                AreaState.updateChange(pair.key, pair.currentValue)
                manageFilledProductInformation(pair.key)

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

                If AreaState.defaultUserDataAccount.saveTickToFile Then
                    Engine.IO.updateTickValue(pair.key, tick)
                End If

                pair.addNewItem(tick)
            Catch ex As Exception
                If ex.Message Like "*Bad request*" Then
                    AreaState.products.getCurrency(pair.key.Split("-")(0)).userData.preference = Models.Products.ProductUserDataModel.PreferenceEnumeration.automaticDisabled

                    AreaState.pairs.Remove(pair.key)

                    MessageBox.Show($"Problem during updateTick ({pair.id}) - " & ex.Message)
                ElseIf Not ex.Message Like "*Call timed out*" Then
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

                Do While inWorkJob
                    If (AreaState.pairs.Count > 0) Then
                        If (currentIndex + 1 > AreaState.pairs.Count) Then
                            currentIndex = 0
                        End If

                        If Not process(currentIndex) Then
                            currentIndex += 1
                        End If
                    End If

                    Threading.Thread.Sleep(100)
                Loop
            Catch ex As Exception
                inWorkJob = False
            End Try
        End Sub

        ''' <summary>
        ''' This method provide to start a reader processor
        ''' </summary>
        Private Sub startReaderProcessor()
            Try
                Dim currentIndex As Integer = 0
                Dim pair As Models.Pair.PairInformation
                Dim tick As New Models.Pair.TickInformation

                Do While inWorkJob
                    If (AreaState.pairs.Count > 0) Then
                        If (currentIndex + 1 > AreaState.pairs.Count) Then
                            currentIndex = 0
                        End If

                        pair = AreaState.pairs.ElementAt(currentIndex).Value

                        tick = Engine.IO.readTickValue(pair.key)

                        If (tick.time > 0) Then
                            If (pair.lastUpdateTick < tick.time) Then
                                pair.lastUpdateTick = tick.time
                                pair.currentValue = tick.value

                                AreaState.updateChange(pair.key, pair.currentValue)

                                manageFilledProductInformation(pair.key)

                                If (pair.currentRelativeAverageValue = 0) Then
                                    pair.currentRelativeAverageValue = tick.value
                                Else
                                    pair.currentRelativeAverageValue = (pair.currentRelativeAverageValue + tick.value) / 2
                                End If

                                pair.addNewItem(tick)
                            End If
                        End If

                        currentIndex += 1
                    End If

                    Threading.Thread.Sleep(10)
                Loop
            Catch ex As Exception
                inWorkJob = False
            End Try
        End Sub

        ''' <summary>
        ''' This method provide to check if the pair exist
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Async Function testPair(ByVal value As String) As Task(Of Boolean)
            Try
                Threading.Thread.Sleep(100)

                'Dim market = Await _ClientPro.MarketData.GetTickerAsync(value)
                Dim market = Await AreaState.exchangeProxy.getTicker(value)

                Threading.Thread.Sleep(100)

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
            If Not inWorkJob Then
                Dim objWS As Threading.Thread

                inWorkJob = True

                If AreaState.defaultUserDataAccount.readTickFromFile Then
                    _SourceMode = SourceTickerEnum.readFromFile

                    objWS = New Threading.Thread(AddressOf startReaderProcessor)
                ElseIf AreaState.defaultUserDataAccount.useSubscription Then
                    _SourceMode = SourceTickerEnum.subscription

                    objWS = New Threading.Thread(AddressOf AreaState.exchangeProxy.startSubscriptionProcessor)
                Else
                    _SourceMode = SourceTickerEnum.ticker

                    objWS = New Threading.Thread(AddressOf startServiceProcessor)
                End If

                objWS.Start()
            End If

            Return True
        End Function

        Public Function [stop]() As Boolean
            _InWorkJob = False

            If (_SourceMode = SourceTickerEnum.subscription) Then
                AreaState.exchangeProxy.removeSubscription()
            End If

            Return True
        End Function

        Public Function tryReset() As Boolean
            If ((_SourceMode = SourceTickerEnum.readFromFile) And Not AreaState.defaultUserDataAccount.readTickFromFile) Or
               ((_SourceMode = SourceTickerEnum.subscription) And Not AreaState.defaultUserDataAccount.useSubscription) Or
               ((_SourceMode = SourceTickerEnum.ticker) And Not (AreaState.defaultUserDataAccount.useSubscription Or AreaState.defaultUserDataAccount.readTickFromFile)) Or
               (_SourceMode = SourceTickerEnum.undefined) Then

                [stop]()
                [start]()

            End If

            Return True
        End Function

    End Module

End Namespace
