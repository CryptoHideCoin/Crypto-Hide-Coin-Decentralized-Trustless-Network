﻿Option Compare Text
Option Explicit On

' ****************************************
' Engine: Base Encryption
' Release Engine: 1.0 
' 
' Date last successfully test: 02/10/2021
' ****************************************


Imports System.IO
Imports System.Xml.Serialization
Imports CHCCommonLibrary.AreaEngine.Encryption





Namespace AreaEngine.DataFileManagement.Encrypted

    ''' <summary>
    ''' This module contain a secureBaseKey general
    ''' </summary>
    Public Module ModuleMain

        Public secureBaseKey As String

    End Module

    ''' <summary>
    ''' This class manage (crypt encrypt) a file with a generic object
    ''' </summary>
    ''' <typeparam name="ClassType"></typeparam>
    Public Class BaseFile(Of ClassType As {New})

        Private _originalCryptoKEY As String
        Private _combineCryptoKEY As String

        Public Property data As New ClassType
        Public Property fileName As String = ""
        Public Property noCrypt As Boolean = False
        Public Property cryptoKEY() As String
            Get
                Return _originalCryptoKEY
            End Get
            Set(value As String)
                _originalCryptoKEY = value
                _combineCryptoKEY = secureBaseKey & "-" & value
            End Set
        End Property

        ''' <summary>
        ''' This method provide to initialize the object
        ''' </summary>
        <DebuggerHiddenAttribute()> Public Sub New()
            _combineCryptoKEY = secureBaseKey & "-"
        End Sub

        ''' <summary>
        ''' This method provide to read a file and decode the content
        ''' </summary>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function read() As Boolean
            Dim serializer As New XmlSerializer(data.GetType)
            Dim stream As StreamReader
            Dim memory As MemoryStream

            Try
                If File.Exists(fileName) Then
                    stream = New StreamReader(fileName)

                    If (_combineCryptoKEY.CompareTo("-") = 0) Then
                        memory = New MemoryStream(Text.Encoding.ASCII.GetBytes(stream.ReadToEnd()))
                    Else
                        memory = New MemoryStream(Text.Encoding.ASCII.GetBytes(Encryption.AES.decrypt(stream.ReadToEnd(), _combineCryptoKEY)))
                    End If

                    stream.Close()
                    stream = New StreamReader(memory)
                    data = DirectCast(serializer.Deserialize(stream), ClassType)
                    stream.Close()
                    stream = Nothing

                    Return True
                Else
                    data = New ClassType

                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to save a file and encode the content
        ''' </summary>
        ''' <returns></returns>
        <DebuggerHiddenAttribute()> Public Function save() As Boolean
            Dim temp As String
            Dim streamWriter As StreamWriter
            Dim engine As New XmlSerializer(data.GetType)

            Try
                If noCrypt Then
                    temp = fileName
                Else
                    temp = IO.Path.GetTempFileName()
                End If

                streamWriter = New StreamWriter(temp, False)
                engine.Serialize(streamWriter, data)
                streamWriter.Close()

                If Not noCrypt Then
                    If (_combineCryptoKEY = "-") Then
                        File.Delete(fileName)
                        IO.File.Move(temp, fileName)
                    Else
                        IO.File.WriteAllText(fileName, AES.encrypt(File.ReadAllText(temp), _combineCryptoKEY))
                        File.Delete(temp)
                    End If
                End If
                Return True
            Catch ex As Exception
                Try
#Disable Warning BC42104
                    If temp.Length > 0 Then
#Enable Warning BC42104
                        File.Delete(temp)
                    End If
                Catch
                End Try

                Return False
            End Try
        End Function

    End Class

End Namespace
