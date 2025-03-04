﻿Option Compare Text
Option Explicit On

Imports TradingBotSystemModelsLibrary.AreaModel.Currency






Namespace AreaBusiness

    ''' <summary>
    ''' This engine contain all method relative of all currency
    ''' </summary>
    Public Class PairsEngine

        Private Property _EngineDB As New CHCDataAccess.AreaCommon.Database.DatabaseGeneric
        Private Property _DBStateFileName As String = "EvaluationState.Db"
        Private Property _Initialize As Boolean = False
        Private Property _OwnerId As String = ""
        Private Property _CacheById As New Dictionary(Of Integer, CurrencyStructure)
        Private Property _CacheByName As New Dictionary(Of String, CurrencyStructure)

        Public Property useCache As Boolean = False



        ''' <summary>
        ''' This method provide to create a currency table
        ''' </summary>
        ''' <returns></returns>
        Private Function createPairTable() As Boolean
            Try
                Dim sql As String = ""

                sql += "CREATE TABLE pairs "
                sql += " ([id] INTEGER PRIMARY KEY AUTOINCREMENT, "
                sql += "  [name] NVARCHAR(32),"
                sql += "  baseCurrencyId as INTEGER, "
                sql += "  quoteCurrencyId as INTEGER);"

                Return _EngineDB.executeDataTable(sql, _OwnerId)
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("PairEngine.createPairTable", _OwnerId, ex.Message)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create an index name pair
        ''' </summary>
        ''' <returns></returns>
        Private Function createNamePairIndex() As Boolean
            Try
                Dim sql As String = ""

                sql += " CREATE UNIQUE INDEX namePairIndex "
                sql += " ON pairs([name])"

                Return _EngineDB.executeDataTable(sql, _OwnerId)
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("PairEngine.createNamePairIndex", _OwnerId, ex.Message)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create an base currency Id Index
        ''' </summary>
        ''' <returns></returns>
        Private Function createBaseCurrencyIdIndex() As Boolean
            Try
                Dim sql As String = ""

                sql += " CREATE UNIQUE INDEX baseCurrencyIndex "
                sql += " ON pairs(baseCurrencyId)"

                Return _EngineDB.executeDataTable(sql, _OwnerId)
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("PairEngine.createBaseCurrencyIdIndex", _OwnerId, ex.Message)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create a quote currency Id Index
        ''' </summary>
        ''' <returns></returns>
        Private Function createQuoteCurrencyIdIndex() As Boolean
            Try
                Dim sql As String = ""

                sql += " CREATE UNIQUE INDEX quoteCurrencyIndex "
                sql += " ON pairs(quoteCurrencyId)"

                Return _EngineDB.executeDataTable(sql, _OwnerId)
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("PairEngine.createQuoteCurrencyIdIndex", _OwnerId, ex.Message)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create a DB structures
        ''' </summary>
        ''' <returns></returns>
        Private Function createStructures() As Boolean
            Try
                Dim proceed As Boolean = True

                If proceed Then
                    proceed = createPairTable()
                End If
                If proceed Then
                    proceed = createNamePairIndex()
                End If
                If proceed Then
                    proceed = createBaseCurrencyIdIndex()
                End If
                If proceed Then
                    proceed = createQuoteCurrencyIdIndex()
                End If

                Return proceed
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.createStructures", _OwnerId, ex.Message)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to add a new pair into table
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="forceOwner"></param>
        ''' <returns></returns>
        Private Function addNew(ByVal data As CurrencyStructure, ByVal forceOwner As String) As Integer
            Try
                Dim sql As String = ""

                sql += $"INSERT INTO currencies "
                sql += $"(shortName, [name], displayName, tipology, minSize, maxPrecision, symbol, source, supplier, nature, networkReferementId, networkContract, unitName, acquireTimestamp, isUsed) "
                sql += $" VALUES "
                sql += $"('{data.shortName}', '{data.name}', '{data.displayName}', {Val(data.tipology)}, {data.minSize}, {data.maxPrecision}, "
                sql += $"'{data.symbol}', '{data.source}', '{data.supplier}', {Val(data.nature)}, {getNetworkReferement(data.networkReferement)}, '{data.contractNetwork}', "
                sql += $"'{data.unitName}', '{data.acquireTimeStamp.ToString.Replace(",", ".")}', 0) "

                Return _EngineDB.executeDataTableAndReturnID(sql, forceOwner)
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.addNew", forceOwner, ex.Message)

                Return 0
            End Try
        End Function

        ''' <summary>
        ''' This method Add or Get the Id of a name of the currency
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="forceOwner"></param>
        ''' <returns></returns>
        Public Function addOrGet(ByVal data As CurrencyStructure, Optional ByVal forceOwner As String = "", Optional ByVal manual As Boolean = False) As CurrencyStructure
            Dim response As New CurrencyStructure With {.shortName = data.shortName}

            If (forceOwner.Length = 0) Then
                forceOwner = _OwnerId
            End If

            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.addOrGet", forceOwner)

                If _Initialize Then

                    If useCache Then
                        If Not _CacheByName.ContainsKey(data.shortName) Then

                            If manual Then
                                data.source = "User"
                                data.supplier = "Edit"
                                data.acquireTimeStamp = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
                            End If

                            data.id = addNew(data, forceOwner)

                            _CacheByName.Add(data.shortName, data)
                            _CacheById.Add(data.id, data)
                        End If

                        response = _CacheByName(data.shortName)
                    Else
                        response = [select](data.shortName)

                        If IsNothing(response) Then
                            response.id = addNew(data, forceOwner)
                        End If
                    End If
                End If

                Return response
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.addOrGet", forceOwner, ex.Message)

                Return response
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.addOrGet", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to modify into db and into cache data the new data of an currency
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="newName"></param>
        ''' <returns></returns>
        Public Function updateData(ByVal id As Integer, ByVal newData As CurrencyStructure, Optional ByVal forceOwner As String = "") As Boolean
            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.updateData", _OwnerId)

                If (forceOwner.Length = 0) Then
                    forceOwner = _OwnerId
                End If

                If _Initialize Then
                    Dim sql As String = ""

                    If useCache Then
                        Dim originalItem As CurrencyStructure

                        If _CacheById.ContainsKey(id) Then
                            originalItem = _CacheById(id)

                            _CacheByName.Remove(originalItem.name)

                            originalItem = newData

                            _CacheByName.Add(newData.shortName, originalItem)
                        Else
                            Return False
                        End If
                    End If

                    sql += $"UPDATE currencies SET "
                    sql += $"shortName = '{newData.shortName}', [name] = '{newData.name}',"
                    sql += $"displayName = '{newData.displayName}', tipology = {Val(newData.tipology)},"
                    sql += $"minSize = {newData.minSize}, maxPrecision = {newData.maxPrecision},"
                    sql += $"symbol = '{newData.symbol}', nature = {Val(newData.nature)}, "
                    sql += $"networkReferement = {getNetworkReferement(newData.networkReferement)}, "
                    sql += $"networkContract = '{newData.contractNetwork}', "
                    sql += $"unitName = '{newData.unitName}'"

                    sql += " WHERE [id] = '{id}'"

                    Return _EngineDB.executeDataTable(sql, forceOwner)
                End If

                Return False
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.updateData", forceOwner, ex.Message)

                Return False
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.updateData", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get data from a recordset
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        Private Function getDataFromRecordset(ByRef id As Integer, ByRef shortName As String, ByRef item As Object, ByVal forceOwner As String) As CurrencyStructure
            Try
                Dim result As New CurrencyStructure

                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.getSelectFrom", forceOwner)

                id = item.getInt32(0)
                shortName = item.getString(1)

                result.id = id
                result.shortName = shortName
                result.name = item.getString(2)
                result.displayName = item.getString(3)

                Select Case item.getInt32(4)
                    Case 0 : result.tipology = CurrencyStructure.tipologyAsset.undefined
                    Case 1 : result.tipology = CurrencyStructure.tipologyAsset.fiat
                    Case 2 : result.tipology = CurrencyStructure.tipologyAsset.crypto
                End Select

                result.minSize = item.getInt32(5)
                result.maxPrecision = item.getInt32(6)
                result.symbol = item.getString(7)
                result.source = item.getString(8)
                result.supplier = item.getString(9)

                Select Case item.getInt32(10)
                    Case 0 : result.nature = CurrencyStructure.natureAsset.undefined
                    Case 1 : result.nature = CurrencyStructure.natureAsset.coin
                    Case 2 : result.nature = CurrencyStructure.natureAsset.token
                    Case 3 : result.nature = CurrencyStructure.natureAsset.stableCoin
                End Select

                result.networkReferement = getNetworkReferementFromId(item.getInt32(11))
                result.contractNetwork = item.getString(12)
                result.acquireTimeStamp = item.getDouble(13)
                result.isUsed = (item.getInt32(14) <> 0)

                Return result
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.updateData", forceOwner, ex.Message & " - id = {id}")

                Return New CurrencyStructure
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.updateData", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get data from a db
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="shortName"></param>
        ''' <returns></returns>
        Private Function getSelectFrom(Optional ByVal id As Integer = 0, Optional ByVal shortName As String = "", Optional ByVal forceOwner As String = "") As CurrencyStructure
            Dim dbOpened As Boolean = False
            Dim openDB As Object
            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.getSelectFrom", forceOwner)

                Dim item As New CurrencyStructure
                Dim sql As String = ""
                Dim result As Object

                openDB = _EngineDB.openDatabase(forceOwner)
                dbOpened = True

                sql += " SELECT id, shortName, [name], displayName, tipology, minSize, maxPrecision, symbol, source, supplier, nature, networkReferementId, networkContract, acquireTimestamp, isUsed "
                sql += "FROM assets WHERE "

                If (id <> 0) Then
                    sql += $"id = {id}"
                ElseIf (shortName.Length > 0) Then
                    sql += $"shortName = '{shortName}'"
                Else
                    Return item
                End If

                If Not IsNothing(openDB) Then
                    result = _EngineDB.selectDataReader(openDB, sql, forceOwner)

                    If Not IsNothing(result) Then
                        item = getDataFromRecordset(id, shortName, result.read(), forceOwner)

                        If Not _CacheById.ContainsKey(item.id) Then
                            _CacheById.Add(item.id, item)
                        End If
                        If Not _CacheByName.ContainsKey(item.shortName) Then
                            _CacheByName.Add(item.shortName, item)
                        End If
                    End If
                End If

                openDB.close()

                dbOpened = False

                Return item
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.getSelectFrom", forceOwner, ex.Message)

                Return New CurrencyStructure
            Finally
                If dbOpened Then
#Disable Warning BC42104
                    openDB.close()
#Enable Warning BC42104
                End If

                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.getSelectFrom", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get an name of an Currency from an Id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="forceOwner"></param>
        ''' <returns></returns>
        Public Function [select](ByVal id As Integer, Optional ByVal forceOwner As String = "") As CurrencyStructure
            If (forceOwner.Length = 0) Then
                forceOwner = _OwnerId
            End If

            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.[select]", forceOwner)

                If _Initialize Then
                    If useCache Then
                        Return _CacheById(id)
                    Else
                        Return getSelectFrom(id,, forceOwner)
                    End If
                Else
                    Return New CurrencyStructure
                End If
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.[select]", forceOwner, ex.Message)

                Return New CurrencyStructure
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.[select]", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get an shortName of a currency
        ''' </summary>
        ''' <param name="shortName"></param>
        ''' <param name="forceOwner"></param>
        ''' <returns></returns>
        Public Function [select](ByVal shortName As String, Optional ByVal forceOwner As String = "") As CurrencyStructure
            If (forceOwner.Length = 0) Then
                forceOwner = _OwnerId
            End If

            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.[select]", forceOwner)

                If _Initialize Then
                    If useCache Then
                        If _CacheByName.ContainsKey(shortName.ToLower) Then
                            Return _CacheByName(shortName.ToLower)
                        Else
                            Return New CurrencyStructure
                        End If
                    Else
                        Return getSelectFrom(, shortName, forceOwner)
                    End If
                Else
                    Return New CurrencyStructure
                End If
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.[select]", forceOwner, ex.Message)

                Return New CurrencyStructure
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.[select]", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to return a count of a currencies table
        ''' </summary>
        ''' <returns></returns>
        Public Function count(Optional ByVal forceOwner As String = "") As Integer
            Try
                If (forceOwner.Length = 0) Then
                    forceOwner = _OwnerId
                End If

                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.count", forceOwner)

                If _Initialize Then
                    If useCache Then
                        Return _CacheById.Count
                    Else
                        Dim sql As String
                        Dim result As Object

                        sql = $"SELECT Count(id) as numExchange FROM currencies"

                        result = _EngineDB.selectResultDataTable(sql, forceOwner)

                        If Not IsNothing(result) Then
                            Return result
                        End If
                    End If
                End If

                Return 0
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.count", forceOwner, ex.Message)

                Return 0
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.count", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get a list of a table's currency
        ''' </summary>
        ''' <returns></returns>
        Public Function list(Optional ByVal loadEntireCache As Boolean = False, Optional ByVal forceOwner As String = "") As List(Of CurrencyStructure)
            Dim sql As String
            Dim id As Integer = 0
            Dim shortName As String = ""
            Dim dataReader As Object
            Dim openDB As Object
            Dim dbOpened As Boolean = False

            If (forceOwner.Length = 0) Then
                forceOwner = _OwnerId
            End If

            Try
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.list", forceOwner)

                If _Initialize Then
                    If useCache And Not loadEntireCache Then
                        Return _CacheById.Values.ToList()
                    Else
                        sql = $"SELECT id, shortName, name, displayName, tipology, minSize, maxPrecision, symbol, source, supplier, nature, networkReferementId, networkContract, acquireTimeStamp, isUsed FROM currencies "

                        openDB = _EngineDB.openDatabase(forceOwner)

                        dbOpened = True

                        If Not IsNothing(openDB) Then
                            dataReader = _EngineDB.selectDataReader(openDB, sql, forceOwner)

                            If Not IsNothing(dataReader) Then
                                Dim item As CurrencyStructure

                                While dataReader.read()
                                    item = getDataFromRecordset(id, shortName, dataReader, forceOwner)

                                    If loadEntireCache Then
                                        _CacheById.Add(item.id, item)
                                        _CacheByName.Add(item.shortName.ToLower, item)
                                    End If
                                End While
                            End If

                            openDB.close()

                            dbOpened = False
                        End If
                    End If
                End If
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.list", forceOwner, ex.Message)

                Return New List(Of CurrencyStructure)
            Finally

                If dbOpened Then
#Disable Warning BC42104
                    openDB.close()
#Enable Warning BC42104
                End If
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.list", forceOwner)
            End Try

            Return New List(Of CurrencyStructure)
        End Function

        ''' <summary>
        ''' This method provide to delete an unique id item's
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function delete(ByVal id As Integer, Optional ByVal forceOwner As String = "") As Boolean
            Try
                If (forceOwner.Length = 0) Then
                    forceOwner = _OwnerId
                End If

                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.delete", forceOwner)

                If useCache Then
                    _CacheByName.Remove(_CacheById(id).name)
                    _CacheById.Remove(id)
                End If

                If True Then
                    Return _EngineDB.executeDataTable($"DELETE currencies WHERE [id] = {id}", forceOwner)
                Else
                    Return False
                End If
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.delete", forceOwner, ex.Message)

                Return False
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.delete", forceOwner)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to initialize the engine
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function init() As Boolean
            Try
                _OwnerId = "CurrenciesEngine"

                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackEnter("CurrencyEngine.init", _OwnerId)

                _EngineDB.path = CHCSidechainServiceLibrary.AreaCommon.Main.environment.paths.workData.state.path
                _EngineDB.fileName = _DBStateFileName
                _EngineDB.logInstance = CHCSidechainServiceLibrary.AreaCommon.Main.environment.log

                If Not IO.Directory.Exists(_EngineDB.path) Then
                    IO.Directory.CreateDirectory(_EngineDB.path)
                End If

                _Initialize = True

                If _EngineDB.init("Evaluation", "Custom") Then
                    If Not _EngineDB.existTable("currencies", _OwnerId) Then
                        CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.track("CurrencyEngine.init", _OwnerId, "Currency table not exist")

                        If Not createStructures() Then
                            CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.track("CurrencyEngine.init", _OwnerId, "Problem during create db structures")

                            Return False
                        End If
                    ElseIf useCache Then
                        list(True)
                    End If

                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackException("CurrencyEngine.init", _OwnerId, ex.Message)

                Return False
            Finally
                CHCSidechainServiceLibrary.AreaCommon.Main.environment.log.trackExit("CurrencyEngine.init", _OwnerId)
            End Try
        End Function

    End Class

End Namespace
