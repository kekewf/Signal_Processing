Imports ArryLoad.SignalTypes
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace ArryLoad
	Public Class Beep
		Public Shared Function SavaProcess(FileName As String, data As String) As String
			Dim baseDirectory As String = AppDomain.CurrentDomain.BaseDirectory
			Dim text As String = baseDirectory + FileName
			Dim streamWriter As StreamWriter = New StreamWriter(text, True)
			streamWriter.WriteLine(data)
			streamWriter.Close()
			streamWriter.Dispose()
			Return text
		End Function

		Public Shared Sub BeepChirpLinear(SampleRate As Integer, Amplitude As Double, startFrequency As Double, maxFrequency As Double, Duration As Double)
			Dim num As Double = Amplitude * Math.Pow(2.0, 15.0) / 1000.0 - 1.0
			Dim num2 As Integer = CInt((CDec(SampleRate) * Duration)) / 1000
			Dim num3 As Double = (maxFrequency - startFrequency) / CDec(num2) / 2.0
			Dim num4 As Integer = num2 * 4
			Dim array As Short() = New Short(num2 * 2 - 1) {}
			Dim array2 As Integer() = New Integer()() { 1179011410, 0, 1163280727, 544501094, 16, 131073, 0, 0, 1048580, 1635017060, 0 }
			For i As Integer = 0 To array2.Length - 1
				array(i) = CShort((CByte(array2(i))))
			Next
			Dim num5 As Integer = array2.Length
			Using Dim memoryStream As MemoryStream = New MemoryStream(36 + num4)
				Using Dim binaryWriter As BinaryWriter = New BinaryWriter(memoryStream)
					For j As Integer = 0 To array2.Length - 1
						binaryWriter.Write(array2(j))
					Next
					For k As Integer = 0 To num2 - 1
						Dim num6 As Double = CDec(k) / CDec(SampleRate)
						Dim num7 As Short = Convert.ToInt16(Math.Sin(6.2831853071795862 * (startFrequency + num3 * CDec(k)) * num6) * num)
						binaryWriter.Write(num7)
						binaryWriter.Write(num7)
						Beep.SavaProcess("BeepChirpLinear.txt", num7.ToString())
					Next
					For l As Integer = 0 To num2 - 1 Step 2
						Dim num6 As Double = CDec(l) / CDec(SampleRate)
						Dim num7 As Short = Convert.ToInt16(Math.Sin(6.2831853071795862 * (startFrequency + num3 * CDec(l)) * num6) * num)
						array(l) = num7
						num6 = CDec((l + 1)) / CDec(SampleRate)
						num7 = Convert.ToInt16(Math.Sin(6.2831853071795862 * (startFrequency + num3 * CDec(l)) * num6) * num)
						array(l + 1) = num7
					Next
					binaryWriter.Flush()
					memoryStream.Seek(0L, SeekOrigin.Begin)
				End Using
			End Using
		End Sub

		Public Shared Sub BeepChirpExp(SampleRate As Integer, TotalTime As Integer, Amplitude As Double, startFrequency As Double, maxFrequency As Double, Duration As Double, expGrowth As Double, autoCorrect As Boolean, SamplingBits As Integer)
			Dim num As Integer = SamplingBits / 8
			Dim num2 As Double = Amplitude * Math.Pow(2.0, CDec((SamplingBits - 1))) / 1000.0 - 1.0
			Dim d As Decimal = SampleRate * TotalTime / 1000
			Dim num3 As Integer = CInt(Math.Ceiling(d))
			Dim list As List(Of Byte) = New List(Of Byte)()
			list.AddRange(WaveSignal.CreateWaveFileHeader(num3 * num * 2, 2, SampleRate, SamplingBits))
			If autoCorrect Then
				expGrowth = Math.Pow(maxFrequency / startFrequency, 1.0 / CDec(num3)) - 1.0
			End If
			Dim array As Byte() = New Byte(num3 * 2 - 1) {}
			For i As Integer = 0 To num3 - 1
				Dim num4 As Double = CDec(i) / CDec(SampleRate)
				Dim num5 As Double = (Math.Pow(1.0 + expGrowth, CDec(i)) - 1.0) / Math.Log(Math.Pow(1.0 + expGrowth, CDec(i)))
				Beep.SavaProcess("BeepChirpExp.txt", (CShort((Math.Sin(6.2831853071795862 * startFrequency * num5 * num4) * num2))).ToString())
			Next
			For j As Integer = 0 To num3 - 1
				Dim num4 As Double = CDec(j) / CDec(SampleRate)
				Dim num6 As Double = (Math.Pow(1.0 + expGrowth, CDec(j)) - 1.0) / Math.Log(Math.Pow(1.0 + expGrowth, CDec(j)))
				Dim value As Short = CShort((Math.Sin(6.2831853071795862 * startFrequency * num6 * num4) * num2))
				Dim bytes As Byte() = BitConverter.GetBytes(value)
				list.AddRange(bytes)
				list.AddRange(bytes)
			Next
			Dim path As String = "D:\BeepChirpExp.wav"
			Dim fileStream As FileStream = New FileStream(path, FileMode.OpenOrCreate)
			fileStream.Write(list.ToArray(), 0, list.ToArray().Length)
			fileStream.Close()
		End Sub
	End Class
End Namespace
