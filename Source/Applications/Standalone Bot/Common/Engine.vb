﻿Option Compare Text
Option Explicit On




Namespace AreaCommon

    Module Engine

        Public Property IO As New AreaEngines.IO.IOEngine

    End Module

    Module Base

        Public Function roundBase(ByVal value As Double, ByVal precision As String, ByVal buy As Boolean) As Double
            Dim support As String = value.ToString().Replace(",", ".")
            Dim integerValue As String
            Dim decimalValue As String

            If (value = 0) Then
                Return 0
            End If

            precision = precision.Replace(",", ".")

            If (support.Split(".").Length > 0) Then
                integerValue = support.Split(".")(0)
                If (support.Split(".").Length > 1) Then
                    decimalValue = support.Split(".")(1)
                End If
            Else
                integerValue = support
            End If

            If (Val(precision) > 0.1) Or (decimalValue = 0) Then
                Return integerValue
            Else
#Disable Warning BC42104
                If precision.Length - 2 <= decimalValue.ToString.Length Then
                    Return Val(integerValue & "." & decimalValue.ToString.Substring(0, precision.Length - 2))
                Else
                    Return Val(integerValue & "." & decimalValue.ToString())
                End If
#Enable Warning BC42104
            End If
        End Function

    End Module

End Namespace
