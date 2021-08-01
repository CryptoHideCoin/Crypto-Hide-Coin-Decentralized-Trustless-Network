﻿Option Explicit On
Option Compare Text


Imports System.Net
Imports Newtonsoft.Json
Imports System.Text








Namespace AreaEngine.Communication


    Public Class ProxyWS(Of ClassType As {New})

        Public data As New ClassType
        Public remoteResponse As AreaCommon.Models.General.RemoteResponse



        Public url As String



        ''' <summary>
        ''' This method provides to get a remote data
        ''' </summary>
        ''' <returns></returns>
        Public Function getData() As String
            Try
                Dim request As WebRequest = WebRequest.Create(url)
                Dim response As WebResponse = request.GetResponse()
                Dim dataStream As IO.Stream = response.GetResponseStream()
                Dim reader As New IO.StreamReader(dataStream)
                Dim responseFromServer As String = reader.ReadToEnd()

                data = JsonConvert.DeserializeObject(Of ClassType)(responseFromServer)

                reader.Close()
                response.Close()

                Return ""
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function


        ''' <summary>
        ''' This method provide to standardize a data to prepare to communicate
        ''' </summary>
        ''' <returns></returns>
        Public Function standardize() As Boolean
            Try
                data = JsonConvert.DeserializeObject(Of ClassType)(JsonConvert.SerializeObject(data, Formatting.Indented))

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function


        ''' <summary>
        ''' This method provides to send a remote data
        ''' </summary>
        ''' <returns></returns>
        Public Function sendData(Optional ByVal methodType As String = "PUT") As String
            Dim webClient As New WebClient()
            Dim reqString() As Byte

            Try
                reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented))

                Dim req As WebRequest = WebRequest.Create(url)

                req.ContentType = "application/json"
                req.Method = methodType
                req.ContentLength = reqString.Length

                Dim stream = req.GetRequestStream()
                stream.Write(reqString, 0, reqString.Length)
                stream.Close()

                Dim dataStream As IO.Stream = req.GetResponse().GetResponseStream()
                Dim reader As New IO.StreamReader(dataStream)
                Dim responseFromServer As String = reader.ReadToEnd()

                remoteResponse = JsonConvert.DeserializeObject(Of AreaCommon.Models.General.RemoteResponse)(responseFromServer)

                reader.Close()
                dataStream.Close()

                Return ""
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

    End Class



    Public Class ProxySimplyWS(Of ClassType)

        Public data As ClassType

        Public url As String





        ''' <summary>
        ''' This method's provides to get a remote data
        ''' </summary>
        ''' <returns></returns>
        Public Function getData() As String
            Try
                Dim request As WebRequest = WebRequest.Create(url)
                Dim response As WebResponse = request.GetResponse()
                Dim dataStream As IO.Stream = response.GetResponseStream()
                Dim reader As New IO.StreamReader(dataStream)
                Dim responseFromServer As String = reader.ReadToEnd()

                data = JsonConvert.DeserializeObject(Of ClassType)(responseFromServer)

                reader.Close()
                response.Close()

                Return ""
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function


        ''' <summary>
        ''' This method's provides to send a remote data
        ''' </summary>
        ''' <returns></returns>
        Public Function sendData(Optional ByVal methodType As String = "PUT") As String
            Dim webClient As New WebClient()
            Dim reqString() As Byte

            Try
                reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented))

                Dim req As WebRequest = WebRequest.Create(url)

                req.ContentType = "application/json"
                req.Method = methodType
                req.ContentLength = reqString.Length

                Dim stream = req.GetRequestStream()
                stream.Write(reqString, 0, reqString.Length)
                stream.Close()

                Dim response = req.GetResponse().GetResponseStream()

                Dim reader As New IO.StreamReader(response)
                Dim res = reader.ReadToEnd()
                reader.Close()
                response.Close()

                Return res
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

    End Class


End Namespace