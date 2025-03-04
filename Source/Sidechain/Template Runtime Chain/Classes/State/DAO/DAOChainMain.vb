﻿Option Compare Text
Option Explicit On

Imports System.Data.SQLite
Imports CHCLedgerLibrary.AreaDataAccess.Generic






Namespace AreaCommon.DAO

    ''' <summary>
    ''' This class contain all method manage a DB main properties
    ''' </summary>
    Public Class DBNetwork

        ''' <summary>
        ''' This enumeration contain all element of a network state
        ''' </summary>
        Public Enum MainPropertyID

            undefined
            networkCreationDate
            genesisPublicAddress
            networkName
            specialEnvironment
            whitePaper
            yellowPaper
            assetData
            transactionChainConfiguration
            privacyPolicy
            generalCondition
            refundPlan

        End Enum


        Private _DBStateFileName As String = "State.Db"
        Private _DBStateConnectionString As String = "Data source = {0};Version=3;"


        ''' <summary>
        ''' This method provide to create a main db table
        ''' </summary>
        ''' <returns></returns>
        Private Function createMainDBTable() As Boolean
            Dim sql As String = ""

            sql += "CREATE TABLE mainProperties "
            sql += " (property_id INTEGER PRIMARY KEY, "
            sql += "  value NVARCHAR(1024) NOT NULL, "
            sql += "  recordRegistrationTimeStamp REAL, "
            sql += "  recordCoordinate NVARCHAR(128) NOT NULL, "
            sql += "  recordHash NVARCHAR(65) NOT NULL, "
            sql += "  hashContent NVARCHAR(65) NOT NULL "
            sql += ");"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to delete old data
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Private Function deleteOldDataNetwork(ByVal id As MainPropertyID) As Boolean
            Try
                Dim sql As String = ""

                log.track("DBMainProperties.deleteOldData", "Begin")

                sql = "DELETE FROM mainProperties WHERE property_id = " & id

                If DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName)) Then
                    log.track("DBMainProperties.deleteOldData", "Delete data db")
                End If

                Return True
            Catch ex As Exception
                log.track("DBMainProperties.deleteOldData", "Failed = " & ex.Message, "fatal", True)

                Return False
            Finally
                log.track("DBMainProperties.deleteOldData", "Completed")
            End Try
        End Function


        ''' <summary>
        ''' This method provide to insert a sql property identity on db
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function insertSQLPropertyIdentityDB(ByVal id As MainPropertyID, ByVal value As String) As Boolean
            Dim sql As String = ""

            sql += "INSERT INTO dbIdentity "
            sql += " (property_id, value) "
            sql += "VALUES "
            sql += " (" & id & ", '"
            sql += value
            sql += "')"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to insert a new record into mainProperties table
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <param name="transactionChainRecord"></param>
        ''' <param name="hashContent"></param>
        ''' <param name="writeValueOnDB"></param>
        ''' <returns></returns>
        Public Function insertSQLPropertyNetwork(ByVal id As MainPropertyID, ByVal value As String, ByVal transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction, Optional ByVal hashContent As String = "", Optional ByVal writeValueOnDB As Boolean = False) As Boolean
            Dim sql As String = ""

            sql += "INSERT INTO mainProperties "
            sql += " (property_id, value, recordRegistrationTimeStamp, recordCoordinate, recordHash, hashContent) "
            sql += "VALUES "
            sql += " (" & id & ", '"
            If writeValueOnDB Then
                sql += value
            Else
                sql += "(ext. content)"
            End If

            sql += "', '" & transactionChainRecord.registrationTimeStamp & "','" & transactionChainRecord.coordinate & "', '" & transactionChainRecord.progressiveHash & "'"

            If (hashContent.Length = 0) Then
                sql += ", '---'"
            Else
                sql += ", '" & hashContent & "'"
            End If

            sql += ")"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to insert a property into db
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <param name="transactionChainRecord"></param>
        ''' <param name="hashContent"></param>
        ''' <param name="writeValueOnDB"></param>
        ''' <returns></returns>
        Public Function updatePropertNetworky(ByVal id As MainPropertyID, ByVal value As String, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction, Optional ByVal hashContent As String = "", Optional ByVal writeValueOnDB As Boolean = False) As Boolean
            If deleteOldDataNetwork(id) Then
                Return insertSQLPropertyNetwork(id, value, transactionChainRecord, hashContent, writeValueOnDB)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' This method provide to get a content hash
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function getContentHash(ByVal id As MainPropertyID) As String
            Try
                Dim sql As String = ""
                Dim connectionDB As SQLiteConnection
                Dim result As Object

                log.track("DBMainProperties.getContentHash", "Begin")

                sql += "SELECT hashContent FROM mainProperties WHERE property_id = " & id

                connectionDB = New SQLiteConnection(String.Format(_DBStateConnectionString, _DBStateFileName))

                connectionDB.Open()

                log.track("DBMainProperties.getContentHash", "DB Open")

                result = DBGeneric.selectResultDataTable(sql,, False, connectionDB)

                If Not IsNothing(result) Then
                    Return result
                Else
                    Return ""
                End If
            Catch ex As Exception
                log.track("DBMainProperties.getContentHash", "Failed = " & ex.Message, "fatal", True)

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to initialize a class
        ''' </summary>
        ''' <param name="workPath"></param>
        ''' <returns></returns>
        Public Function init(ByVal workPath As String) As Boolean
            Try
                Dim proceed As Boolean = True

                log.track("DBMainProperties.init", "Begin")

                _DBStateFileName = IO.Path.Combine(workPath, _DBStateFileName)

                DBGeneric.logIstance = log

                log.track("DBMainProperties.init", "Set path = " & _DBStateFileName)

                If Not IO.File.Exists(_DBStateFileName) Then
                    log.track("DBMainProperties.init", "File DB not exist")

                    SQLiteConnection.CreateFile(_DBStateFileName)
                End If

                If proceed Then
                    proceed = DBGeneric.createIdentityDBTable(String.Format(_DBStateConnectionString, _DBStateFileName))
                End If
                If proceed Then
                    proceed = createMainDBTable()
                End If
                If proceed Then
                    proceed = DBGeneric.writeIdentityDB(String.Format(_DBStateConnectionString, _DBStateFileName), "State", "State")
                End If

                log.track("DBMainProperties.init", "Completed")

                Return True
            Catch ex As Exception
                log.track("DBMainProperties.init", "Failed = " & ex.Message, "fatal", True)

                Return False
            End Try
        End Function

    End Class

    ''' <summary>
    ''' This class contain all method manage a DB chain
    ''' </summary>
    Public Class DBChain

        ''' <summary>
        ''' This enumeration contain all element of a chain detail
        ''' </summary>
        Public Enum DetailPropertyID

            undefined
            chainParameters
            priceList
            policyPrivacy
            termAndConditions
            lastTransactionBlock
            lastNodeList

        End Enum

        Private _DBStateFileName As String = "State.Db"
        Private _DBStateConnectionString As String = "Data source = {0};Version=3;"



        ''' <summary>
        ''' This method provide to create a chain db table
        ''' </summary>
        ''' <returns></returns>
        Private Function createChainMainDBTable() As Boolean
            Dim sql As String = ""

            sql += "CREATE TABLE chains "
            sql += " (chain_id INTEGER PRIMARY KEY AUTOINCREMENT, "
            sql += "  name NVARCHAR(1024) Not NULL, "
            sql += "  privateChain INTEGER Not NULL, "
            sql += "  description NVARCHAR(65535) Not NULL, "
            sql += "  recordRegistrationTimeStamp REAL, "
            sql += "  recordCoordinate NVARCHAR(128) Not NULL, "
            sql += "  recordHash NVARCHAR(65) Not NULL); "

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to create a chain set protocols db table
        ''' </summary>
        ''' <returns></returns>
        Private Function createChainSetProtocols() As Boolean
            Dim sql As String = ""

            sql += "CREATE TABLE chainProtocolSets "
            sql += " (chain_id INTEGER Not NULL, "
            sql += "  setCode NVARCHAR(25), "
            sql += "  protocol NVARCHAR(255), "
            sql += "  recordRegistrationTimeStamp REAL, "
            sql += "  recordCoordinate NVARCHAR(128) Not NULL, "
            sql += "  recordHash NVARCHAR(65) Not NULL, "
            sql += "  hashContent NVARCHAR(65) Not NULL, "
            sql += " PRIMARY KEY (chain_id, setCode) "
            sql += "); "

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to create a chain detail DB table
        ''' </summary>
        ''' <returns></returns>
        Private Function createChainDetailDBTable() As Boolean
            Dim sql As String = ""

            sql += "CREATE TABLE chainDetails "
            sql += " (chain_id INTEGER Not NULL, "
            sql += "  propertyChain_id INTEGER Not NULL, "
            sql += "  recordRegistrationTimeStamp REAL, "
            sql += "  recordCoordinate NVARCHAR(128) Not NULL, "
            sql += "  recordHash NVARCHAR(65) Not NULL, "
            sql += "  hashContent NVARCHAR(65) Not NULL, "
            sql += "  PRIMARY KEY (chain_id, propertyChain_id) "
            sql += ");"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to create a token chain DB table
        ''' </summary>
        ''' <returns></returns>
        Private Function createChainTokenDBTable() As Boolean
            Dim sql As String = ""

            sql += "CREATE TABLE chainTokenConfigurations "
            sql += " (chain_id INTEGER Not NULL, "
            sql += "  shortName NVARCHAR(20) Not Null, "
            sql += "  recordRegistrationTimeStamp REAL, "
            sql += "  recordCoordinate NVARCHAR(128) Not NULL, "
            sql += "  recordHash NVARCHAR(65) Not NULL, "
            sql += "  hashContent NVARCHAR(65) Not NULL, "
            sql += "  PRIMARY KEY (chain_id, shortName) "
            sql += ");"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to delete a token configuration
        ''' </summary>
        ''' <param name="chainID"></param>
        ''' <param name="shortName"></param>
        ''' <returns></returns>
        Private Function deleteTokenConfiguration(ByVal chainID As Integer, ByVal shortName As String) As Boolean
            Dim sql As String = ""

            sql += "DELETE "
            sql += "FROM chainTokenConfigurations "
            sql += "WHERE chain_id = '" & chainID & "'"
            sql += "  AND shortName = '" & shortName & "'"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to delete a single protocol
        ''' </summary>
        ''' <param name="chainID"></param>
        ''' <param name="setCode"></param>
        ''' <returns></returns>
        Private Function deleteProtocol(ByVal chainID As Integer, ByVal setCode As String) As Boolean
            Dim sql As String = ""

            sql += "DELETE "
            sql += "FROM chainProtocolSets "
            sql += "WHERE chain_id = " & chainID
            sql += "  AND setCode = '" & setCode & "'"

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to delete a single detail
        ''' </summary>
        ''' <param name="chainReferement"></param>
        ''' <param name="propertyChain_id"></param>
        ''' <returns></returns>
        Private Function deleteDetail(ByRef chainReferement As String, ByRef propertyChain_id As Integer) As Boolean
            Dim sql As String = ""

            sql += "DELETE "
            sql += "FROM chainDetails "
            sql += "WHERE chain_id = '" & chainReferement & "'"
            sql += "  AND propertyChain_id = " & propertyChain_id

            Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
        End Function

        ''' <summary>
        ''' This method provide to get a last record ID
        ''' </summary>
        ''' <returns></returns>
        Private Function getLastRecordID(Optional ByRef connectionDB As SQLiteConnection = Nothing, Optional ByVal closeDB As Boolean = False) As Integer
            Try
                Dim sql As String = ""
                Dim result As Object

                log.track("DBChain.getLastRecordID", "Begin")

                sql += "SELECT last_insert_rowid()"

                log.track("DBChain.getLastRecordID", "DB Open")

                If IsNothing(connectionDB) Then
                    connectionDB = New SQLiteConnection(String.Format(_DBStateConnectionString, _DBStateFileName))

                    connectionDB.Open()
                End If

                result = DBGeneric.selectResultDataTable(sql,, False, connectionDB)

                If closeDB Then
                    connectionDB.Close()
                End If

                If Not IsNothing(result) Then
                    Return result
                Else
                    Return ""
                End If
            Catch ex As Exception
                log.track("DBChain.getLastRecordID", "Failed = " & ex.Message, "fatal", True)

                Return False
            End Try
        End Function


        ''' <summary>
        ''' This method provide to add a new chain into db
        ''' </summary>
        ''' <param name="chainName"></param>
        ''' <param name="privateChain"></param>
        ''' <param name="description"></param>
        ''' <param name="transactionChainRecord"></param>
        ''' <returns></returns>
        Public Function addNewChain(ByVal chainName As String, ByVal privateChain As Boolean, ByVal description As String, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction) As Integer
            Dim sql As String = ""
            Dim connectionDB As SQLiteConnection

            sql += "INSERT INTO chains "
            sql += " (name, privateChain, description, "
            sql += " recordRegistrationTimeStamp, "
            sql += " recordCoordinate, "
            sql += " recordHash"
            sql += "  ) "
            sql += "VALUES "
            sql += " ('" & chainName & "', "

            If privateChain Then
                sql += "1"
            Else
                sql += "0"
            End If

            sql += ",'" & description & "'"
            sql += ",'" & transactionChainRecord.registrationTimeStamp & "'"
            sql += ",'" & transactionChainRecord.coordinate & "'"
            sql += ",'" & transactionChainRecord.progressiveHash
            sql += "')"

            connectionDB = New SQLiteConnection(String.Format(_DBStateConnectionString, _DBStateFileName))

            connectionDB.Open()

            If DBGeneric.executeDataTable(sql,, False, connectionDB) Then
                Return getLastRecordID(connectionDB, True)
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' This method provide to update the protocol
        ''' </summary>
        ''' <param name="chainID"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function updateProtocol(ByVal chainID As Integer, ByRef value As CHCProtocolLibrary.AreaCommon.Models.Chain.ProtocolMinimalData, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction) As Boolean
            If deleteProtocol(chainID, value.setCode) Then
                Dim sql As String = ""

                sql += "INSERT INTO chainProtocolSets "
                sql += " (chain_id, setCode, protocol , "
                sql += "  recordRegistrationTimeStamp, "
                sql += "  recordCoordinate, recordHash, "
                sql += "  hashContent)"
                sql += " VALUES "
                sql += " (" & chainID & ", "
                sql += "  '" & value.setCode & "'"
                sql += ",'" & value.protocol & "'"
                sql += ",'" & transactionChainRecord.registrationTimeStamp & "'"
                sql += ",'" & transactionChainRecord.coordinate & "'"
                sql += ",'" & transactionChainRecord.progressiveHash & "'"
                sql += ",'" & value.getHash() & "')"

                Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' This method provide to update the protocol
        ''' </summary>
        ''' <param name="chainReferement"></param>
        ''' <param name="propertyChain_id"></param>
        ''' <param name="hashContent"></param>
        ''' <param name="transactionChainRecord"></param>
        ''' <returns></returns>
        Public Function updateDetail(ByVal chainID As Integer, ByVal propertyChain_id As DetailPropertyID, ByRef hashContent As String, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction) As Boolean
            If deleteDetail(chainID, propertyChain_id) Then
                Dim sql As String = ""

                sql += "INSERT INTO chainDetails "
                sql += " (chain_id, propertyChain_id, "
                sql += "  recordRegistrationTimeStamp, "
                sql += "  recordCoordinate, recordHash, "
                sql += "  hashContent)"
                sql += " VALUES "
                sql += " (" & chainID
                sql += "," & propertyChain_id
                sql += ",'" & transactionChainRecord.registrationTimeStamp & "'"
                sql += ",'" & transactionChainRecord.coordinate & "'"
                sql += ",'" & transactionChainRecord.progressiveHash & "'"
                sql += ",'" & hashContent & "')"

                Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' This method provide to insert a new token into DB
        ''' </summary>
        ''' <param name="chainID"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function addNewToken(ByVal chainID As Integer, ByVal value As CHCProtocolLibrary.AreaCommon.Models.PrimaryChain.AssetConfigurationModel, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction, ByRef hashContent As String) As Boolean
            If deleteTokenConfiguration(chainID, value.assetInformation.shortName) Then
                Dim sql As String = ""

                sql += "INSERT INTO chainTokenConfigurations "
                sql += " (chain_id, shortName, "
                sql += "  recordRegistrationTimeStamp, "
                sql += "  recordCoordinate, recordHash, "
                sql += "  hashContent)"
                sql += " VALUES "
                sql += " (" & chainID
                sql += ",'" & value.assetInformation.shortName & "'"
                sql += ",'" & transactionChainRecord.registrationTimeStamp & "'"
                sql += ",'" & transactionChainRecord.coordinate & "'"
                sql += ",'" & transactionChainRecord.progressiveHash & "'"
                sql += ",'" & hashContent & "')"

                Return DBGeneric.executeDataTable(sql, String.Format(_DBStateConnectionString, _DBStateFileName))
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' This method provide to initialize a class
        ''' </summary>
        ''' <param name="workPath"></param>
        ''' <returns></returns>
        Public Function init(ByVal workPath As String) As Boolean
            Try
                Dim proceed As Boolean = True

                log.track("ChainStateEngine.init", "Begin")

                _DBStateFileName = IO.Path.Combine(workPath, _DBStateFileName)

                log.track("ChainStateEngine.init", "Set path = " & _DBStateFileName)

                If proceed Then
                    proceed = createChainMainDBTable()
                End If
                If proceed Then
                    proceed = createChainDetailDBTable()
                End If
                If proceed Then
                    proceed = createChainSetProtocols()
                End If
                If proceed Then
                    proceed = createChainTokenDBTable()
                End If

                log.track("ChainStateEngine.init", "Completed")

                Return True
            Catch ex As Exception
                log.track("ChainStateEngine.init", "Failed = " & ex.Message, "fatal", True)

                Return False
            End Try
        End Function

    End Class

End Namespace
