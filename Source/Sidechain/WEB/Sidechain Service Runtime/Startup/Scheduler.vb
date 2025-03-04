﻿Option Compare Text
Option Explicit On

Imports CHCSidechainServiceLibrary.AreaCommon.Main




Namespace AreaCommon.Startup

    Module Scheduler

        Private _Engine As New CHCSidechainServiceLibrary.AreaScheduler.MainEngine

        ''' <summary>
        ''' This method provide to start service processor
        ''' </summary>
        ''' <returns></returns>
        Private Sub startServiceProcessor()
            Try
                Environment.log.trackEnter("Scheduler.startServiceProcessor")

                _Engine.loadScheduleList()

                Do While (serviceInformation.currentStatus = CHCModels.AreaModel.Information.InternalServiceInformation.EnumInternalServiceState.started)
                    _Engine.manageJobProcessList(AreaCommon.Customized.CUSTOM_ChainServiceName)

                    Threading.Thread.Sleep(100)
                Loop

                Environment.log.trackExit("Scheduler.startServiceProcessor")
            Catch ex As Exception
                Environment.log.trackException("Scheduler.startServiceProcessor", ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' This method provide to add work local machine job
        ''' </summary>
        ''' <returns></returns>
        Public Function addWorkLocalMachineJob() As Boolean
            Return _Engine.addLocalWorkMachineJob()
        End Function

        ''' <summary>
        ''' This method provide to start all maintenance service
        ''' </summary>
        ''' <returns></returns>
        Public Function run() As Boolean
            Try
                Dim objWS As Threading.Thread

                Environment.log.trackEnter("Scheduler.run")

                Do While (serviceInformation.currentStatus <> CHCModels.AreaModel.Information.InternalServiceInformation.EnumInternalServiceState.started)
                    Threading.Thread.Sleep(100)
                Loop

                objWS = New Threading.Thread(AddressOf startServiceProcessor)

                objWS.Start()

                Environment.log.trackExit("Scheduler.run")

                Return True
            Catch ex As Exception
                Environment.log.trackException("Scheduler.run", ex.Message)

                Return False
            End Try
        End Function

    End Module

End Namespace
