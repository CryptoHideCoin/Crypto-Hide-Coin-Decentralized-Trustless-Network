﻿Option Compare Text
Option Explicit On





Public Class Manager

    Sub printTotBots(ByVal totActiveBot As Integer, ByVal dateFirstBot As Double, ByVal totOpenOrder As Integer, ByVal totCloseOrder As Integer)
        numBotValue.Text = AreaState.bots.Count
        numActiveBotValue.Text = totActiveBot

        If (dateFirstBot > 0) Then
            firstDateBotValue.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(dateFirstBot), True)
        Else
            firstDateBotValue.Text = "---"
        End If

        numOpenBotsValue.Text = totOpenOrder
        numCloseBotsValue.Text = totCloseOrder
    End Sub

    Sub refreshDataBot()
        Dim rowItem As New ArrayList
        Dim execute As Boolean = True
        Dim totActiveBot As Integer = 0
        Dim dateFirstBot As Double = 0
        Dim totOpenOrder As Integer = 0, totCloseOrder As Integer = 0

        If (botDataView.Rows.Count = AreaState.bots.Count) Then
            Do While execute
                execute = False

                Try
                    With AreaState.bots.ToList
                        For index As Integer = 0 To AreaState.bots.ToList.Count - 1
                            If .Item(index).Value.parameters.header.isActive Then
                                totActiveBot += 1
                            End If

                            If (dateFirstBot >= .Item(index).Value.data.timeStart) Or (dateFirstBot = 0) Then
                                dateFirstBot = .Item(index).Value.data.timeStart
                            End If

                            totOpenOrder += .Item(index).Value.data.tradeOpen.Count
                            totCloseOrder += .Item(index).Value.data.tradeClose.Count

                            botDataView.Rows(index).Cells(3).Value = .Item(index).Value.data.tradeOpen.Count
                            botDataView.Rows(index).Cells(4).Value = .Item(index).Value.data.tradeClose.Count
                            If .Item(index).Value.parameters.header.isActive Then
                                botDataView.Rows(index).Cells(5).Value = "OFF"
                            Else
                                botDataView.Rows(index).Cells(5).Value = "ON"
                            End If
                        Next
                    End With
                Catch ex As Exception
                    execute = True
                End Try
            Loop

            printTotBots(totActiveBot, dateFirstBot, totOpenOrder, totCloseOrder)

            Return
        End If

        Do While execute
            execute = False

            botDataView.Rows.Clear()

            Try
                For Each item In AreaState.bots.ToList
                    rowItem.Clear()

                    If item.Value.parameters.header.isActive Then
                        totActiveBot += 1
                    End If

                    If (dateFirstBot >= item.Value.data.timeStart) Or (dateFirstBot = 0) Then
                        dateFirstBot = item.Value.data.timeStart
                    End If

                    totOpenOrder += item.Value.data.tradeOpen.Count
                    totCloseOrder += item.Value.data.tradeClose.Count

                    rowItem.Add(item.Value.parameters.header.id)
                    rowItem.Add(CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(item.Value.parameters.header.created), True))
                    rowItem.Add(item.Value.data.pair)
                    rowItem.Add(item.Value.data.tradeOpen.Count)
                    rowItem.Add(item.Value.data.tradeClose.Count)

                    If item.Value.parameters.header.isActive Then
                        rowItem.Add("OFF")
                    Else
                        rowItem.Add("ON")
                    End If

                    botDataView.Rows.Add(rowItem.ToArray)
                Next
            Catch ex As Exception
                execute = True
            End Try
        Loop

        printTotBots(totActiveBot, dateFirstBot, totOpenOrder, totCloseOrder)
    End Sub

    Sub refreshDataAccount()
        Dim rowItem As New ArrayList
        Dim execute As Boolean = True
        Dim USDTValue As Double = 0

        If Not AreaState.defaultUserDataAccount.useVirtualAccount Then
            AreaCommon.Engines.Accounts.start()
        End If

        If (accountsGridView.Rows.Count = AreaState.accounts.Count) Then
            Do While execute
                execute = False

                Try
                    With AreaState.accounts.ToList
                        For index As Integer = 0 To AreaState.accounts.ToList.Count - 1
                            accountsGridView.Rows(index).Cells(1).Value = .Item(index).Value.amount.ToString("#,##0.00000")
                            accountsGridView.Rows(index).Cells(2).Value = .Item(index).Value.change.ToString("#,##0.00000")
                            accountsGridView.Rows(index).Cells(3).Value = .Item(index).Value.valueUSDT.ToString("#,##0.00000")

                            USDTValue += .Item(index).Value.valueUSDT
                        Next
                    End With
                Catch ex As Exception
                    execute = True
                End Try
            Loop

            totAccountValue.Text = AreaState.accounts.Count
            totUSDTValue.Text = USDTValue.ToString("#,##0.00000")

            totalFeesValue.Text = AreaState.summary.totalFeesValue.ToString("#,##0.00000") & " USDT"
            totalIncreaseValue.Text = AreaState.summary.increaseValue.ToString("#,##0.00000") & " USDT"
            totalVolumesValue.Text = AreaState.summary.totalVolumeValue.ToString("#,##0.00000") & " USDT"

            If (AreaState.summary.initialValue = 0) And (USDTValue <> 0) Then
                AreaState.summary.initialValue = USDTValue

                initialUSDTValue.Text = USDTValue.ToString("#,##0.00") & " USDT"

                AreaState.summary.initialValue = USDTValue
            ElseIf (AreaState.summary.initialValue <> 0) And (initialUSDTValue.Text.trim().Length = 0) Then
                initialUSDTValue.Text = USDTValue.ToString("#,##0.00") & " USDT"

                AreaState.summary.initialValue = USDTValue
            End If

            If initialUSDTValue.Text.Length > 0 Then
                earnValue.Text = (Val(totUSDTValue.Text.Replace(".", "").Replace(",", ".")) - Val(initialUSDTValue.Text.Replace(".", "").Replace(",", "."))).ToString("#,##0.00000")
            End If

            If (USDTValue - Val(initialUSDTValue.Text.Replace(".", "").Replace(",", "."))) >= 0 Then
                earnValue.ForeColor = Color.Black
            Else
                earnValue.ForeColor = Color.Red
            End If

            totUSDTValue.ForeColor = earnValue.ForeColor

            Return
        End If

        Do While execute
            execute = False

            accountsGridView.Rows.Clear()

            Try
                For Each item In AreaState.accounts.ToList
                    rowItem.Clear()

                    rowItem.Add(item.Value.id)
                    rowItem.Add(item.Value.amount.ToString("#,##0.00000"))
                    rowItem.Add(item.Value.change.ToString("#,##0.00000"))
                    rowItem.Add(item.Value.valueUSDT.ToString("#,##0.00000"))

                    accountsGridView.Rows.Add(rowItem.ToArray)
                Next
            Catch ex As Exception
                execute = True
            End Try
        Loop

        totAccountValue.Text = AreaState.accounts.Count
        totUSDTValue.Text = USDTValue.ToString("#,##0.00000")

        If (initialUSDTValue.Text.Length = 0) And (USDTValue <> 0) Then
            initialUSDTValue.Text = totUSDTValue.Text
        End If

        If initialUSDTValue.Text.Length > 0 Then
            earnValue.Text = (Val(totUSDTValue.Text.Replace(".", "").Replace(",", ".")) - Val(initialUSDTValue.Text.Replace(".", "").Replace(",", "."))).ToString("#,##0.00000")
        End If

        If (USDTValue - Val(initialUSDTValue.Text.Replace(".", "").Replace(",", "."))) >= 0 Then
            earnValue.ForeColor = Color.Black
        Else
            earnValue.ForeColor = Color.Red
        End If

        totUSDTValue.ForeColor = earnValue.ForeColor
    End Sub

    Sub refreshDataMarket()
        Dim rowItem As New ArrayList

        marketDataView.Rows.Clear()

        For Each item In AreaState.pairs.Values
            rowItem.Clear()

            rowItem.Add(item.key)
            rowItem.Add(item.currentValue.ToString("#,##0.00000"))

            marketDataView.Rows.Add(rowItem.ToArray)

            marketDataView.Rows(marketDataView.Rows.Count - 1).DefaultCellStyle.BackColor = Color.LightGray
        Next
    End Sub

    Sub refreshDataCurrencies()
        Dim rowItem As New ArrayList

        If (AreaState.products.items.Count <> CurrenciesDataView.Rows.Count) Then
            CurrenciesDataView.Rows.Clear()

            For Each item In AreaState.products.items
                rowItem.Clear()

                rowItem.Add(item.header.key)
                rowItem.Add(item.header.name)

                Select Case item.userData.preference
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.ignore : rowItem.Add("Ignore")
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.prefered : rowItem.Add("Prefered")
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.toWork : rowItem.Add("To work")
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.undefined : rowItem.Add("Undefined")
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.userOnly : rowItem.Add("User Only")
                End Select

                If item.userData.isCustomized Then
                    rowItem.Add(1)
                Else
                    rowItem.Add(0)
                End If

                If (item.currentValue = 0) Then
                    rowItem.Add("---")
                Else
                    rowItem.Add(item.currentValue.ToString("#,##0.00000"))
                End If

                If (item.currentSpread = 0) Then
                    rowItem.Add("---")
                    rowItem.Add("---")
                Else
                    rowItem.Add(item.currentSpread.ToString("#,##0.00"))
                    rowItem.Add(item.currentSpreadPerc.ToString("#,##0.00") & " %")
                End If

                If (item.maxTarget = 0) Then
                    rowItem.Add("---")
                Else
                    rowItem.Add(item.maxTarget.ToString("#,##0.00000"))
                End If

                CurrenciesDataView.Rows.Add(rowItem.ToArray)

                If (item.currentSpread > 0) Then
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(6).Style.ForeColor = Color.DarkGreen
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(7).Style.ForeColor = Color.DarkGreen
                ElseIf (item.currentSpread = 0) Then
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(6).Style.ForeColor = Color.Black
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(7).Style.ForeColor = Color.Black
                Else
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(6).Style.ForeColor = Color.Red
                    CurrenciesDataView.Rows(CurrenciesDataView.Rows.Count - 1).Cells(7).Style.ForeColor = Color.Red
                End If
            Next
        Else
            Dim item As AreaCommon.Models.Products.ProductModel

            For index As Integer = 0 To AreaState.products.items.Count - 1
                item = AreaState.products.items(index)

                If Not AreaState.pairs.ContainsKey(item.pairID) Then
                    If item.userData.isCustomized And (item.userData.preference <> AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.ignore) Then
                        AreaState.getPairID(item.pairID)
                    End If
                Else
                    item.value.current = AreaState.pairs(item.pairID).currentValue
                End If

                Select Case item.userData.preference
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.ignore : CurrenciesDataView.Rows(index).Cells(2).Value = "Ignore"
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.prefered : CurrenciesDataView.Rows(index).Cells(2).Value = "Prefered"
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.toWork : CurrenciesDataView.Rows(index).Cells(2).Value = "To work"
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.undefined : CurrenciesDataView.Rows(index).Cells(2).Value = "Undefined"
                    Case AreaCommon.Models.Products.ProductUserDataModel.PreferenceEnumeration.userOnly : CurrenciesDataView.Rows(index).Cells(2).Value = "User Only"
                End Select

                If item.userData.isCustomized Then
                    CurrenciesDataView.Rows(index).Cells(3).Value = 1
                Else
                    CurrenciesDataView.Rows(index).Cells(3).Value = 0
                End If

                If (item.value.current = 0) Or Not item.userData.isCustomized Then
                    CurrenciesDataView.Rows(index).Cells(4).Value = "---"
                Else
                    CurrenciesDataView.Rows(index).Cells(4).Value = item.value.current.ToString("#,##0.00000")
                End If

                If (item.currentValue = 0) Then
                    CurrenciesDataView.Rows(index).Cells(5).Value = "---"
                Else
                    CurrenciesDataView.Rows(index).Cells(5).Value = item.currentValue.ToString("#,##0.00000")
                End If

                If (item.currentSpread = 0) Then
                    CurrenciesDataView.Rows(index).Cells(6).Value = "---"
                    CurrenciesDataView.Rows(index).Cells(7).Value = "---"
                Else
                    CurrenciesDataView.Rows(index).Cells(6).Value = item.currentSpread.ToString("#,##0.00")
                    CurrenciesDataView.Rows(index).Cells(7).Value = item.currentSpreadPerc.ToString("#,##0.00") & " %"
                End If

                If (item.maxTarget = 0) Then
                    CurrenciesDataView.Rows(index).Cells(8).Value = "---"
                Else
                    CurrenciesDataView.Rows(index).Cells(8).Value = item.maxTarget.ToString("#,##0.00000")
                End If

                If (item.currentSpread > 0) Then
                    CurrenciesDataView.Rows(index).Cells(6).Style.ForeColor = Color.DarkGreen
                    CurrenciesDataView.Rows(index).Cells(7).Style.ForeColor = Color.DarkGreen
                ElseIf (item.currentSpread = 0) Then
                    CurrenciesDataView.Rows(index).Cells(6).Style.ForeColor = Color.Black
                    CurrenciesDataView.Rows(index).Cells(7).Style.ForeColor = Color.Black
                Else
                    CurrenciesDataView.Rows(index).Cells(6).Style.ForeColor = Color.Red
                    CurrenciesDataView.Rows(index).Cells(7).Style.ForeColor = Color.Red
                End If
            Next
        End If
    End Sub

    Sub refreshDailyOrderGrid()
        Dim rowItem As New ArrayList

        If (AreaState.journal.currentDayCounters.transactions.Count <> dayTransactionDataView.Rows.Count) Then
            dayTransactionDataView.Rows.Clear()

            Dim item As AreaCommon.Models.Journal.DayCounterModel.TransactionModel

            For count As Integer = AreaState.journal.currentDayCounters.transactions.Count - 1 To 0 Step -1

                item = AreaState.journal.currentDayCounters.transactions(count)

                rowItem.Clear()

                If item.buy Then
                    rowItem.Add(1)
                Else
                    rowItem.Add(0)
                End If

                rowItem.Add(CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(item.dateCompletate), True))
                rowItem.Add(item.pairID)
                rowItem.Add(item.amount.ToString("#,##0.00000"))
                rowItem.Add(item.value.ToString("#,##0.00000"))

                If item.daily Then
                    rowItem.Add(1)
                Else
                    rowItem.Add(0)
                End If

                dayTransactionDataView.Rows.Add(rowItem.ToArray)
            Next
        End If
    End Sub

    Sub refreshTickValue()
        Dim rowItem As New ArrayList
        Dim pairData As AreaCommon.Models.Pair.PairInformation
        Dim singleTick As AreaCommon.Models.Pair.TickInformation
        Dim market As AreaCommon.Models.Pair.TrendData
        Dim key As AreaCommon.Models.Pair.TrendData.periodTypeEnumeration

        tickValues.Rows.Clear()

        If IsNothing(marketDataView.CurrentRow) Then Return

        pairData = AreaState.pairs.Values(marketDataView.CurrentRow.Index)

        Select Case filterDetails.SelectedIndex
            Case 0 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastHour
            Case 1 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastDay
            Case 2 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastWeek
            Case 3 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastMonth
            Case Else : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastYear
        End Select

        If pairData.marketData.ContainsKey(key) Then
            market = pairData.marketData(key)
        Else
            Return
        End If

        For pos = market.ticks.Count - 1 To 0 Step -1
            singleTick = market.ticks(pos)

            rowItem.Clear()

            rowItem.Add(CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(singleTick.time), True))
            rowItem.Add(singleTick.value.ToString("#,##0.00000"))

            tickValues.Rows.Add(rowItem.ToArray)

            Select Case singleTick.position
                Case AreaCommon.Models.Pair.TickInformation.tickPositionEnumeration.decrease
                    tickValues.Rows(tickValues.Rows.Count - 1).DefaultCellStyle.ForeColor = Color.DarkRed
                Case AreaCommon.Models.Pair.TickInformation.tickPositionEnumeration.increase
                    tickValues.Rows(tickValues.Rows.Count - 1).DefaultCellStyle.ForeColor = Color.DarkGreen
                Case AreaCommon.Models.Pair.TickInformation.tickPositionEnumeration.same
                    tickValues.Rows(tickValues.Rows.Count - 1).DefaultCellStyle.ForeColor = Color.Black
            End Select

            tickValues.Rows(tickValues.Rows.Count - 1).DefaultCellStyle.BackColor = Color.LightGray
        Next
    End Sub

    Sub refreshGraphValue()
        If IsNothing(marketDataView.CurrentRow) Then
            Return
        End If

        Dim serie As New DataVisualization.Charting.Series
        Dim market As AreaCommon.Models.Pair.TrendData
        Dim key As AreaCommon.Models.Pair.TrendData.periodTypeEnumeration

        mainChart.Series.Clear()

        Select Case filterDetails.SelectedIndex
            Case 0 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastHour
            Case 1 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastDay
            Case 2 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastWeek
            Case 3 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastMonth
            Case Else : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastYear
        End Select

        If AreaState.pairs.Values(marketDataView.CurrentRow.Index).marketData.ContainsKey(key) Then
            market = AreaState.pairs.Values(marketDataView.CurrentRow.Index).marketData(key)
        Else
            Return
        End If

        If (mainChart.Titles.Count = 0) Then
            mainChart.Titles.Add(AreaState.pairs.Values(marketDataView.CurrentRow.Index).key)
        End If

        serie.Name = "."
        serie.ChartType = DataVisualization.Charting.SeriesChartType.Spline
        serie.BorderWidth = 3
        serie.IsVisibleInLegend = False

        For Each tick In market.ticks
            serie.Points.AddXY("", tick.value)
        Next

        mainChart.Series.Add(serie)

        mainChart.ChartAreas(0).AxisX.MajorGrid.Enabled = False
        mainChart.ChartAreas(0).AxisX.MinorGrid.Enabled = False
        'mainChart.ChartAreas(0).AxisX.Minimum = market.min
        'mainChart.ChartAreas(0).AxisX.Maximum = market.max
    End Sub

    Sub updateDataMarket()
        Dim index As Integer = 0

        For Each item In AreaState.pairs
            marketDataView.Rows.Item(index).SetValues(item.Key, item.Value.currentValue.ToString("#,##0.00000"))

            index += 1
        Next
    End Sub

    Private Sub AddNewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewToolStripMenuItem.Click
        Dim addNewForm As New SettingsBot

        addNewForm.Show()
    End Sub

    Private Sub loadBotParameters(ByRef jobParameters As AreaCommon.Models.Bot.BotParametersModel, ByVal pair As String)

    End Sub

    Private Sub UpdateToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If Not IsNothing(botDataView.CurrentRow) Then
            Dim id As String = botDataView.CurrentRow.Cells.Item(0).Value

            For Each job In AreaState.bots.ToList
                If (job.Value.parameters.header.id.CompareTo(id) = 0) Then
                    loadBotParameters(job.Value.parameters, job.Value.data.pair)

                    Return
                End If
            Next
        End If
    End Sub

    Private Sub refreshDetailValue()
        Dim rowItem As New ArrayList
        Dim pairData As AreaCommon.Models.Pair.PairInformation
        Dim market As AreaCommon.Models.Pair.TrendData
        Dim key As AreaCommon.Models.Pair.TrendData.periodTypeEnumeration

        If IsNothing(marketDataView.CurrentRow) Then Return

        pairData = AreaState.pairs.ElementAt(marketDataView.CurrentRow.Index).Value

        pairValue.Text = pairData.key

        Select Case filterDetails.SelectedIndex
            Case 0 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastHour
            Case 1 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastDay
            Case 2 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastWeek
            Case 3 : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastMonth
            Case Else : key = AreaCommon.Models.Pair.TrendData.periodTypeEnumeration.lastYear
        End Select

        If pairData.marketData.ContainsKey(key) Then
            market = pairData.marketData(key)
        Else
            minValue.Text = ""
            maxValue.Text = ""
            averageValue.Text = ""
            averageRelativeValue.Text = ""
            trendValue.Text = ""
            lastUpdateValue.Text = ""
            firstValue.Text = ""
            lastValue.Text = ""
            spreadValue.Text = ""

            Return
        End If

        minValue.Text = market.min.ToString("#,##0.00000")
        maxValue.Text = market.max.ToString("#,##0.00000")
        averageValue.Text = market.average.ToString("#,##0.00000")
        averageRelativeValue.Text = market.relativeAverage.ToString("#,##0.00000")
        firstValue.Text = market.firstValue.ToString("#,##0.00000")
        lastValue.Text = market.lastValue.ToString("#,##0.00000")
        spreadValue.Text = market.spread.ToString("#,##0.00") & " % (" & market.spreadValue.ToString("#,##0.00") & " " & pairData.key.Split("-")(1) & ")"

        If (market.spread > 0) Then
            spreadValue.ForeColor = Color.Green
            spreadLabel.ForeColor = Color.Green
        ElseIf (market.spread < 0) Then
            spreadValue.ForeColor = Color.DarkRed
            spreadLabel.ForeColor = Color.DarkRed
        ElseIf (market.spread = 0) Then
            spreadValue.ForeColor = Color.Black
            spreadLabel.ForeColor = Color.Black
        End If

        Select Case market.trend
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.decrease
                trendValue.Text = "Decrease"
                trendValue.ForeColor = Color.DarkRed
                trendLabel.ForeColor = Color.DarkRed
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.increase
                trendValue.ForeColor = Color.Green
                trendLabel.ForeColor = Color.Green
                trendValue.Text = "Increase"
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.deep
                trendValue.Text = "Deep"
                trendValue.ForeColor = Color.DarkRed
                trendLabel.ForeColor = Color.DarkRed
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.inBearMarket
                trendValue.Text = "In bear market"
                trendValue.ForeColor = Color.DarkRed
                trendLabel.ForeColor = Color.DarkRed
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.inBullRun
                trendValue.ForeColor = Color.Green
                trendLabel.ForeColor = Color.Green
                trendValue.Text = "In bullrun"
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.inHalving
                trendValue.Text = "In halving"
                trendValue.ForeColor = Color.Green
                trendLabel.ForeColor = Color.Green
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.inTop
                trendValue.Text = "Top"
                trendValue.ForeColor = Color.Green
                trendLabel.ForeColor = Color.Green
            Case AreaCommon.Models.Pair.TrendData.StatusValueEnumeration.undefined
                trendValue.Text = "Undefined"
                trendValue.ForeColor = Color.Black
                trendLabel.ForeColor = Color.Black
        End Select

        lastUpdateValue.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(pairData.lastUpdateTick), True)
    End Sub

    Private Sub formatValue(ByRef control As Object, ByVal value As Double, Optional ByVal colorValue As Boolean = False)
        If (value = 0) Then
            control.Text = "---"

            control.ForeColor = Color.Black
        Else
            control.Text = value.ToString("#,##0.00")

            If colorValue Then
                If (value < 0) Then
                    control.ForeColor = Color.Red
                Else
                    control.ForeColor = Color.DarkGreen
                End If
            End If
        End If
    End Sub

    Private Sub refreshJournalValue()
        AreaCommon.Engines.Bots.updateJournalCounter()

        If (AreaState.journal.initialDay = 0) Then
            startDateJournalDate.Text = "---"
        Else
            startDateJournalDate.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(AreaState.journal.initialDay), True)
        End If

        If (AreaState.journal.currentDayCounters.day = 0) Then
            currentDateValue.Text = "---"
        Else
            currentDateValue.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(AreaState.journal.currentDayCounters.day).ToShortDateString
        End If

        If (AreaState.journal.lastUpdate = 0) Then
            updateDate.Text = "---"
        Else
            updateDate.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(AreaState.journal.lastUpdate), True)
        End If

        formatValue(initialFundValue, AreaState.journal.initialFund)
        formatValue(currentFundValue, AreaState.journal.currentFund)
        formatValue(futureGainValue, AreaState.journal.futureGain)

        formatValue(freeFundSummaryValue, AreaState.journal.freeFund)
        formatValue(lockedFundSummaryValue, AreaState.journal.lockedFund)

        formatValue(feeValue, AreaState.journal.totalFee)
        formatValue(volumeValue, AreaState.journal.totalVolume)
        If (AreaState.journal.numPages = 0) Then
            numDaysValue.Text = "---"
        Else
            numDaysValue.Text = AreaState.journal.numPages
        End If

        formatValue(totalEarnValue, AreaState.journal.totalEarn + AreaState.journal.currentDayCounters.earn, True)

        If (AreaState.journal.apy = 0) Then
            apyValue.Text = "---"

            apyValue.ForeColor = Color.Black
        Else
            apyValue.Text = AreaState.journal.apy

            apyValue.ForeColor = totalEarnValue.ForeColor
        End If

        If (AreaState.journal.averageApy = 0) Then
            averageAPYValue.Text = "---"

            averageAPYValue.ForeColor = Color.Black
        Else
            averageAPYValue.Text = AreaState.journal.averageApy.ToString("#,##0.00")

            If (AreaState.journal.averageApy < 0) Then
                averageAPYValue.ForeColor = Color.Red
            Else
                averageAPYValue.ForeColor = Color.DarkGreen
            End If
        End If

        formatValue(increaseValue, AreaState.journal.totalIncrease)
        formatValue(increasePercentualTotal, AreaState.journal.increasePercentual)
        formatValue(averageIncreaseTotalPercentage, AreaState.journal.averageIncrease)

        formatValue(powerTotalValue, AreaState.journal.totalPower)

        If (AreaState.journal.currentDayCounters.day = 0) Then
            currentDayValue.Text = "---"
        Else
            currentDayValue.Text = CHCCommonLibrary.AreaEngine.Miscellaneous.formatDateTimeGMT(CHCCommonLibrary.AreaEngine.Miscellaneous.dateTimeFromTimeStamp(AreaState.journal.currentDayCounters.day), True)
        End If

        formatValue(initialDayFundStableValue, AreaState.journal.currentDayCounters.initialFundFree)
        formatValue(initialOtherFundDayValue, AreaState.journal.currentDayCounters.initialFundManage)

        numSellValue.Text = AreaState.journal.currentDayCounters.sellNumber

        formatValue(freeFundCurrentValue, AreaState.journal.currentDayCounters.freeFund)
        formatValue(lockedFundCurrentValue, AreaState.journal.currentDayCounters.lockedFund)

        numBuyValue.Text = AreaState.journal.currentDayCounters.buyNumber

        formatValue(currentPageValue, AreaState.journal.currentDayCounters.currentFund)
        formatValue(increaseCurrentValue, AreaState.journal.currentDayCounters.increase)

        formatValue(extraBuyDayValue, AreaState.journal.currentDayCounters.extraBuy)
        formatValue(dailyBuyDayValue, AreaState.journal.currentDayCounters.dailyBuy)

        formatValue(extraSellDayValue, AreaState.journal.currentDayCounters.extraSell)
        formatValue(dailySellDayValue, AreaState.journal.currentDayCounters.dailySell)

        formatValue(feeDayValue, AreaState.journal.currentDayCounters.feePayed)
        formatValue(volumesDayValue, AreaState.journal.currentDayCounters.volumes)

        formatValue(earnDayValue, AreaState.journal.currentDayCounters.earn, True)

        If (AreaState.journal.currentDayCounters.apy = 0) Then
            apyDayValue.Text = "---"

            apyDayValue.ForeColor = Color.Black
        Else
            apyDayValue.Text = (AreaState.journal.currentDayCounters.apy).ToString("#,##0.00")

            apyDayValue.ForeColor = earnDayValue.ForeColor
        End If

        alertValue.Text = AreaState.journal.alert
        workActionValue.Text = AreaState.automaticBot.workAction

        refreshDailyOrderGrid()
    End Sub

    Private Sub updateAllDataMarkets()
        Dim rowIndexPair As Integer

        If IsNothing(marketDataView.CurrentRow) Then
            rowIndexPair = -1
        Else
            rowIndexPair = marketDataView.CurrentRow.Index
        End If

        If (marketDataView.Rows.Count <> AreaState.pairs.Count) And (AreaState.pairs.Count <> 0) Then
            refreshDataMarket()
            refreshTickValue()
            refreshDetailValue()
            refreshGraphValue()

            If (rowIndexPair <> -1) Then
                marketDataView.Rows.Item(rowIndexPair).Selected = True
            End If
        ElseIf (AreaState.pairs.Count <> 0) Then
            updateDataMarket()
            refreshTickValue()
            refreshDetailValue()
            refreshGraphValue()
        End If

    End Sub

    Private Sub timerMain_Tick(sender As Object, e As EventArgs) Handles timerMain.Tick
        updateAllDataMarkets()
    End Sub

    Private Sub manageVirtualAccount()
        If Not AreaState.defaultUserDataAccount.useVirtualAccount Then
            AreaCommon.Engines.Accounts.start()
        ElseIf (AreaState.accounts.Count = 1) Then
            AreaCommon.Engines.Accounts.stop()

            If (AreaState.defaultUserDataAccount.initialBaseFund <> AreaState.accounts("USDT".ToLower()).valueUSDT) Then
                AreaState.accounts.Remove("USDT".ToLower())

                AreaState.addIntoAccount("USDT", AreaState.defaultUserDataAccount.initialBaseFund.ToString(), False)
            End If
        End If
    End Sub

    Private Sub Manager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim pathBase As String = ""

        filterDetails.SelectedIndex = 0

        AreaState.addIntoAccount("USDT", 1522, False)

        Me.Text = $"Simulator - Standard Bot - rel.{Application.ProductVersion}"

        If (Environment.GetCommandLineArgs.Count = 1) Then
            Dim selectorPath As New SelectPath

            If (selectorPath.ShowDialog = DialogResult.OK) Then
                pathBase = selectorPath.completePath
            Else
                MessageBox.Show("No path defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                End
            End If

            selectorPath.Close()
        Else
            pathBase = Environment.CommandLine.Replace(Environment.GetCommandLineArgs(0), "").Trim()
        End If

        If Not AreaCommon.Engine.IO.init(pathBase) Then
            MessageBox.Show("Problem during IO.init", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            If (AreaState.summary.initialValue <> 0) Then
                initialUSDTValue.Text = AreaState.summary.initialValue.ToString("#,##0.00000") & " USDT"
            End If

            Me.Text = AreaState.Common.defaultUserDataAccount.tenantName
        End If

        If AreaCommon.Engine.IO.newTenant Then
            If Not openPersonalData() Then
                End
            End If
        End If

        manageVirtualAccount()
    End Sub

    Private Sub marketDataView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles marketDataView.CellContentClick

    End Sub

    Private Sub marketDataView_SelectionChanged(sender As Object, e As EventArgs) Handles marketDataView.SelectionChanged
        refreshTickValue()
        refreshDetailValue()
    End Sub

    Private Sub Manager_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        For Each item In AreaState.bots
            AreaCommon.Engine.IO.updateBotData(item.Value.parameters.header.id, item.Value.data)
        Next

        AreaCommon.Engine.IO.updateSummary()
        AreaCommon.Engine.IO.updateWallet()
        AreaCommon.Engine.IO.updateCryptocurrency()
        AreaCommon.Engine.IO.updateAutomaticBot()

        If AreaCommon.Engines.Bots.AutomaticBotModule.updateJournalCounter() Then
            AreaCommon.Engine.IO.updateJournal()
        End If

        AreaState.closeApplication = True

        AreaCommon.Engines.Bots.BotModule.stop()
        AreaCommon.Engines.Bots.AutomaticBotModule.stop()
        AreaCommon.Engines.Pairs.stop()
        AreaCommon.Engines.Orders.stop()
    End Sub

    Private Sub noDataMarket_Paint(sender As Object, e As PaintEventArgs)

    End Sub

    Private Sub filterDetails_SelectedIndexChanged(sender As Object, e As EventArgs) Handles filterDetails.SelectedIndexChanged
        updateAllDataMarkets()

        Select Case filterDetails.SelectedIndex
            Case 0 : hourGridLabel.Text = "Last hour ticks"
            Case 1 : hourGridLabel.Text = "Last day ticks"
            Case 2 : hourGridLabel.Text = "Last week ticks"
            Case 3 : hourGridLabel.Text = "Last month ticks"
            Case 4 : hourGridLabel.Text = "Last year ticks"
        End Select
    End Sub

    Private Sub activeBot()
        If Not IsNothing(botDataView.CurrentRow) Then
            Dim id As String = botDataView.CurrentRow.Cells.Item(0).Value
            Dim continueActivity As Boolean = False

            If AreaState.bots(id).parameters.header.isActive Then
                continueActivity = (MessageBox.Show("Do you want to stop the bot activity?", "Confirm", MessageBoxButtons.OKCancel) = DialogResult.OK)
            Else
                continueActivity = (MessageBox.Show("Do you want to start the bot activity?", "Confirm", MessageBoxButtons.OKCancel) = DialogResult.OK)
            End If

            If continueActivity Then
                AreaState.bots(id).parameters.header.isActive = Not AreaState.bots(id).parameters.header.isActive
            End If
        End If
    End Sub

    Private Sub updateBotsTimer_Tick(sender As Object, e As EventArgs) Handles updateBotsTimer.Tick
        refreshDataBot()
        refreshDataAccount()
        refreshDataCurrencies()
        refreshJournalValue()

        If ((AreaState.bots.Count > 0) Or Not AreaState.defaultUserDataAccount.useVirtualAccount) And Not timerMain.Enabled Then
            timerMain.Enabled = True

            updateAllDataMarkets()
        End If
    End Sub



    Private Sub botDataView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles botDataView.CellContentClick
        Select Case e.ColumnIndex
            Case 5
                activeBot()
            Case 6
                Dim addNewForm As New SettingsBot

                addNewForm.currentID = botDataView.Rows.Item(e.RowIndex).Cells(0).Value
                addNewForm.pair = botDataView.Rows.Item(e.RowIndex).Cells(2).Value

                addNewForm.Show()
            Case 7
                Dim informationForm As New DataBot

                If Not IsNothing(botDataView.CurrentRow) Then
                    Dim id As String = botDataView.CurrentRow.Cells.Item(0).Value

                    informationForm.idReferement = id

                    informationForm.Show()
                End If
        End Select
    End Sub

    Private Sub SetNameToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        About.Show()
    End Sub

    Private Sub StartMultipleBotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartMultipleBotToolStripMenuItem.Click
        MultipleBot.Show()
    End Sub

    Private Sub Manager_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim execute As Boolean = True

        Do While execute
            execute = False

            Try
                For Each item In AreaState.bots
                    If item.Value.parameters.header.isActive Then
                        If (MessageBox.Show("Do you want to exit?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.Cancel) Then
                            e.Cancel = True

                            Return
                        End If

                        Exit For
                    End If
                Next

                If AreaState.automaticBot.isActive Then
                    If (MessageBox.Show("Do you want to exit?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.Cancel) Then
                        e.Cancel = True

                        Return
                    End If
                End If
            Catch ex As Exception
                execute = True
            End Try
        Loop
    End Sub

    Private Sub Manager_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Me.MouseDoubleClick

    End Sub

    ''' <summary>
    ''' This method show the personal data
    ''' </summary>
    Private Function openPersonalData() As Boolean
        Dim tempForm As New PersonalData

        If (tempForm.ShowDialog() = DialogResult.OK) Then
            Me.Text = AreaState.Common.defaultUserDataAccount.tenantName

            If tempForm.changeMode Then
                initialUSDTValue.Text = ""

                manageVirtualAccount()
            Else
                If (AreaState.accounts.Count = 1) Then
                    initialUSDTValue.Text = ""
                End If
            End If

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub PersonalToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles PersonalToolStripMenuItem1.Click
        openPersonalData()
    End Sub

    Private Sub ArchiveSelectedBotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ArchiveSelectedBotToolStripMenuItem.Click
        If Not IsNothing(botDataView.CurrentRow) Then
            Dim id As String = botDataView.CurrentRow.Cells.Item(0).Value

            If (MessageBox.Show("Do you want to archive this bot?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.Yes) Then
                AreaCommon.IO.archiveBot(id)
            End If
        End If
    End Sub

    Private Sub BotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BotToolStripMenuItem.Click
        ArchiveSelectedBotToolStripMenuItem.Enabled = Not IsNothing(botDataView.CurrentRow)
    End Sub

    Private Sub CryptocurrenciesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CryptocurrenciesToolStripMenuItem.Click
        AreaCommon.Engines.Currencies.quoteCurrency = InputBox("Insert a quote currency", "Request", "USDT")

        If (AreaCommon.Engines.Currencies.quoteCurrency.Trim.Length > 0) Then
            AreaCommon.Engines.Currencies.start()
        End If
    End Sub

    Private Sub CurrenciesDataView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles CurrenciesDataView.CellContentClick
        Select Case e.ColumnIndex
            Case 9
                Dim editService As EditProduct

                If Not IsNothing(CurrenciesDataView.CurrentRow) Then
                    editService = New EditProduct

                    editService.currencyID = CurrenciesDataView.CurrentRow.Cells.Item(0).Value

                    editService.Show()
                End If
        End Select
    End Sub

    Private Sub AutomaticBotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutomaticBotToolStripMenuItem.Click
    End Sub

    Private Sub ConfigurationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigurationToolStripMenuItem.Click
        Dim automaticBot As New AutomaticBotSetting

        automaticBot.Show()
    End Sub

    Private Sub RestartVirtualToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartVirtualToolStripMenuItem.Click
        Dim proceed As Boolean = True

        If proceed Then
            If AreaState.automaticBot.isActive Then
                proceed = (MessageBox.Show("Do you want to stop the automatic bot?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes)
            End If
        End If
        If proceed Then
            proceed = (MessageBox.Show("Do you want to reset automatic bot data?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes)
        End If
        If proceed Then
            If AreaState.automaticBot.resetData() Then
                For Each product In AreaState.products.items
                    product.resetData()
                Next

                AreaState.automaticBot.lastWorkAction = 0
                AreaCommon.Engines.Bots.AutomaticBotModule.stop()

                AreaState.accounts.Clear()

                AreaState.addIntoAccount("USDT", AreaState.defaultUserDataAccount.initialBaseFund, False)

                AreaState.summary.totalFeesValue = 0
                AreaState.summary.totalVolumeValue = 0
                AreaState.summary.increaseValue = 0

                AreaState.orders.Clear()

                AreaState.journal = New AreaCommon.Models.Journal.CumulativeModel

                initialUSDTValue.Text = ""

                refreshJournalValue()

                AreaState.automaticBot.lastWorkAction = 0
            End If
        End If
        If proceed Then
            MessageBox.Show("Automatic bot data is reset.")
        End If

    End Sub

    Private Sub ActionAutomaticBotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActionAutomaticBotToolStripMenuItem.Click
        If (ActionAutomaticBotToolStripMenuItem.Text.ToLower.CompareTo("start") = 0) Then
            If (MessageBox.Show("Do you want to start the automatic bot?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No) Then
                Return
            End If

            For Each item In AreaState.products.items
                If item.userData.isCustomized Then
                    AreaState.automaticBot.isActive = True
                    AreaState.automaticBot.lastWorkAction = 0
                    AreaState.automaticBot.workAction = AreaCommon.Models.Bot.BotAutomatic.WorkStateEnumeration.undefined

                    AreaCommon.Engines.Bots.AutomaticBotModule.start()

                    Return
                End If
            Next
        Else
            If (MessageBox.Show("Do you want to stop the automatic bot?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No) Then
                Return
            End If

            AreaState.automaticBot.isActive = False

            AreaCommon.Engines.Bots.AutomaticBotModule.stop()
        End If
    End Sub

    Private Sub AutomaticBotToolStripMenuItem_DropDownOpening(sender As Object, e As EventArgs) Handles AutomaticBotToolStripMenuItem.DropDownOpening
        RestartVirtualToolStripMenuItem.Enabled = AreaState.defaultUserDataAccount.useVirtualAccount

        If AreaState.automaticBot.isActive Then
            ActionAutomaticBotToolStripMenuItem.Text = "Stop"
        Else
            ActionAutomaticBotToolStripMenuItem.Text = "Start"
        End If
    End Sub

    Private Sub summaryButton_CheckedChanged(sender As Object, e As EventArgs) Handles summaryButton.CheckedChanged
        If summaryButton.Checked Then
            SummaryPanel.BringToFront()
            daySummaryButton.Checked = False
            warningButton.Checked = False
        Else
            SummaryPanel.SendToBack()
        End If
    End Sub

    Private Sub daySummaryButton_CheckedChanged(sender As Object, e As EventArgs) Handles daySummaryButton.CheckedChanged
        If daySummaryButton.Checked Then
            dayPanel.BringToFront()
            summaryButton.Checked = False
            warningButton.Checked = False
        Else
            dayPanel.SendToBack()
        End If
    End Sub


    Private Sub dayTransactionDataView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dayTransactionDataView.CellContentClick

    End Sub

    Private Sub clearButton_Click(sender As Object, e As EventArgs)
        AreaState.journal.alert = ""
    End Sub

    Private Sub DailyReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DailyReportToolStripMenuItem.Click
        Dim dailyReport As New DataPageList

        dailyReport.Show()
    End Sub

    Private Sub warningButton_CheckedChanged(sender As Object, e As EventArgs) Handles warningButton.CheckedChanged
        If warningButton.Checked Then
            warningPanel.BringToFront()
            summaryButton.Checked = False
            daySummaryButton.Checked = False
        Else
            warningPanel.SendToBack()
        End If
    End Sub
End Class
