Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace ArryLoad.SignalTypes
	Public Class WaveSignal
		Public Structure WaveHeader
			Public RIFF As String

			Public FileSize As UInteger

			Public WAVE As String

			Public FORMAT As String

			Public FormatSize As UInteger

			Public FilePadding As UShort

			Public FormatChannels As UShort

			Public SamplesPerSecond As UInteger

			Public AverageBytesPerSecond As UInteger

			Public BytesPerSample As UShort

			Public BitsPerSample As UShort

			Public FormatExtra As UShort

			Public FACT As String

			Public FactSize As UInteger

			Public FactInf As UInteger

			Public DATA As String

			Public DataSize As UInteger
		End Structure

		Public Function GetWaveHeaderFromBytes(data As Byte()) As WaveSignal.WaveHeader
			Dim waveHeader As WaveSignal.WaveHeader = Nothing
			waveHeader.RIFF = Convert.ToString(Encoding.ASCII.GetChars(data, 0, 4))
			waveHeader.FileSize = BitConverter.ToUInt32(data, 4)
			waveHeader.WAVE = Convert.ToString(Encoding.ASCII.GetChars(data, 8, 4))
			waveHeader.FORMAT = Convert.ToString(Encoding.ASCII.GetChars(data, 12, 4))
			waveHeader.FormatSize = BitConverter.ToUInt32(data, 16)
			waveHeader.FilePadding = BitConverter.ToUInt16(data, 20)
			waveHeader.FormatChannels = BitConverter.ToUInt16(data, 22)
			waveHeader.SamplesPerSecond = BitConverter.ToUInt32(data, 24)
			waveHeader.AverageBytesPerSecond = BitConverter.ToUInt32(data, 28)
			waveHeader.BytesPerSample = BitConverter.ToUInt16(data, 32)
			waveHeader.BitsPerSample = BitConverter.ToUInt16(data, 34)
			Dim flag As Boolean = waveHeader.FormatSize = 18U
			If flag Then
				waveHeader.FormatExtra = BitConverter.ToUInt16(data, 36)
			Else
				waveHeader.FormatExtra = 0
			End If
			Dim num As UShort = CUShort((20U + waveHeader.FormatSize))
			waveHeader.FACT = Convert.ToString(Encoding.ASCII.GetChars(data, CInt(num), 4))
			Dim flag2 As Boolean = waveHeader.FACT = "fact"
			If flag2 Then
				waveHeader.FactSize = BitConverter.ToUInt32(data, CInt((num + 4)))
				waveHeader.FactInf = (If((waveHeader.FactSize = 2U), (CUInt(BitConverter.ToUInt16(data, CInt((num + 8))))), BitConverter.ToUInt32(data, CInt((num + 8)))))
				num = CUShort((CUInt(num) + waveHeader.FactSize + 8U))
			Else
				waveHeader.FACT = "NULL"
				waveHeader.FactSize = 0U
				waveHeader.FactInf = 0U
			End If
			waveHeader.DATA = Convert.ToString(Encoding.ASCII.GetChars(data, CInt(num), 4))
			waveHeader.DataSize = BitConverter.ToUInt32(data, CInt((num + 4)))
			Return waveHeader
		End Function

		Public Shared Function CreateWaveFileHeader(data_Len As Integer, data_SoundCH As Integer, data_Sample As Integer, data_SamplingBits As Integer) As Byte()
			Dim list As List(Of Byte) = New List(Of Byte)()
			list.AddRange(Encoding.ASCII.GetBytes("RIFF"))
			list.AddRange(BitConverter.GetBytes(data_Len + 44 - 8))
			list.AddRange(Encoding.ASCII.GetBytes("WAVE"))
			list.AddRange(Encoding.ASCII.GetBytes("fmt "))
			list.AddRange(BitConverter.GetBytes(16))
			Dim <>f__AnonymousType As var = New With{ Key .PCM_Code = 1, Key .SoundChannel = CShort(data_SoundCH), Key .SampleRate = data_Sample, Key .BytesPerSec = data_SamplingBits * data_Sample * data_SoundCH / 8, Key .BlockAlign = CShort((data_SamplingBits * data_SoundCH / 8)), Key .SamplingBits = CShort(data_SamplingBits) }
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.PCM_Code))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SoundChannel))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SampleRate))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.BytesPerSec))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.BlockAlign))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SamplingBits))
			list.AddRange(Encoding.ASCII.GetBytes("data"))
			list.AddRange(BitConverter.GetBytes(data_Len))
			Return list.ToArray()
		End Function

		Public Shared Function GenWaveFile(data_Len As Integer, data_SoundCH As Integer, data_Sample As Integer, data_SamplingBits As Integer, data As Byte()) As Byte()
			Dim list As List(Of Byte) = New List(Of Byte)()
			list.AddRange(Encoding.ASCII.GetBytes("RIFF"))
			list.AddRange(BitConverter.GetBytes(data_Len + 44 - 8))
			list.AddRange(Encoding.ASCII.GetBytes("WAVE"))
			list.AddRange(Encoding.ASCII.GetBytes("fmt "))
			list.AddRange(BitConverter.GetBytes(16))
			Dim <>f__AnonymousType As var = New With{ Key .PCM_Code = 1, Key .SoundChannel = CShort(data_SoundCH), Key .SampleRate = data_Sample, Key .BytesPerSec = data_SamplingBits * data_Sample * data_SoundCH / 8, Key .BlockAlign = CShort((data_SamplingBits * data_SoundCH / 8)), Key .SamplingBits = CShort(data_SamplingBits) }
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.PCM_Code))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SoundChannel))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SampleRate))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.BytesPerSec))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.BlockAlign))
			list.AddRange(BitConverter.GetBytes(<>f__AnonymousType.SamplingBits))
			list.AddRange(Encoding.ASCII.GetBytes("data"))
			list.AddRange(BitConverter.GetBytes(data_Len))
			Dim list2 As List(Of Byte) = New List(Of Byte)()
			list2.AddRange(list)
			list2.AddRange(data)
			Return list2.ToArray()
		End Function
	End Class
End Namespace
