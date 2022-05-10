﻿Option Explicit On
Option Compare Text

' ****************************************
' Engine: Registry Engine
' Release Engine: 1.0 
' 
' Date last successfully test: 02/10/2021
' ****************************************


Imports CHCCommonLibrary.AreaEngine.DataFileManagement.XML
Imports CHCModelsLibrary.AreaModel.Registry




Namespace AreaEngine.Registry

    ''' <summary>
    ''' This class write a registry file
    ''' </summary>
    Public Class RegistryEngine

        Inherits BaseFile(Of List(Of RegistryData))


        Private _Path As String = ""
        Private _Day As String = ""

        Public noSave As Boolean = False



        ''' <summary>
        ''' This method provide to addNew event in the Registry
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="description"></param>
        ''' <param name="fileDetail"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function addNew(ByVal type As RegistryData.TypeEvent, Optional ByVal description As String = "", Optional ByVal fileDetail As String = "") As Boolean
            Try
                Dim item As New RegistryData
                Dim localDay As String = Now.ToUniversalTime().ToString("yyyy-MM-dd")
                Dim localFileName As String = IO.Path.Combine(_Path, localDay & ".registry")

                If (localFileName.CompareTo(_Path) <> 0) Then
                    MyBase.data = New List(Of RegistryData)

                    _Day = localDay
                    _Path = localFileName
                End If

                item.istant = Miscellaneous.timeStampFromDateTime()
                item.type = type
                item.description = description
                item.fileDetail = fileDetail

                MyBase.data.Add(item)

                If Not noSave Then
                    MyBase.save()
                End If

                Return init()
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get a data
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function getData(ByVal value As String) As List(Of RegistryData)
            Try
                Dim engine As New BaseFile(Of List(Of RegistryData))

                engine.fileName = IO.Path.Combine(_Path, value & ".registry")

                If engine.read() Then
                    Return engine.data
                End If

                Return New List(Of RegistryData)
            Catch ex As Exception
                Return New List(Of RegistryData)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to get a history registry list
        ''' </summary>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function getHistoryList() As List(Of String)
            Try
                Dim result As New List(Of String)

                For Each singleFile In IO.Directory.GetFiles(_Path)
                    result.Add(IO.Path.GetFileName(singleFile).Replace(".registry", ""))
                Next

                Return result
            Catch ex As Exception
                Return New List(Of String)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create path into base directory
        ''' and set file name
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function init(Optional ByVal path As String = "", Optional ByVal onlySetPath As Boolean = False) As Boolean
            Try
                If (_Day <> Now.ToUniversalTime().ToString("yyyy-MM-dd")) Then
                    _Path = path

                    If Not IO.Directory.Exists(path) Then
                        IO.Directory.CreateDirectory(path)
                    End If

                    If Not onlySetPath Then
                        _Day = Now.ToUniversalTime().ToString("yyyy-MM-dd")

                        MyBase.fileName = IO.Path.Combine(path, _Day & ".registry")

                        MyBase.read()
                    End If
                End If

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class


End Namespace
