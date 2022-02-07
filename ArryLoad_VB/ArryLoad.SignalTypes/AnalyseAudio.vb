Imports System
Imports System.Runtime.InteropServices

Namespace ArryLoad.SignalTypes
	Friend Class AnalyseAudio
		Public Shared Sub unwrap(phase As Double(), <Out()> ByRef unwrapped_phase As Double())
			Dim num As Double = 3.1415926535897931
			Dim num2 As Double = 3.1415926535897931
			unwrapped_phase = New Double(phase.Length - 1) {}
			Dim num3 As Integer = phase.Length
			Dim array As Double() = New Double(num3 - 1 - 1) {}
			Dim array2 As Double() = New Double(num3 - 1 - 1) {}
			Dim array3 As Double() = New Double(num3 - 1 - 1) {}
			unwrapped_phase(0) = phase(0)
			Dim num4 As Double = 0.0
			For i As Integer = 0 To num3 - 1 - 1
				array(i) = phase(i + 1) - phase(i)
				array2(i) = AnalyseAudio.[mod](array(i) + num2, 2.0 * num2) - num2
				Dim flag As Boolean = array2(i) = -num2 AndAlso array(i) > 0.0
				If flag Then
					array2(i) = num2
				End If
				array3(i) = array2(i) - array(i)
				Dim flag2 As Boolean = Math.Abs(array(i)) < num
				If flag2 Then
					array3(i) = 0.0
				End If
				num4 += array3(i)
				unwrapped_phase(i + 1) = phase(i + 1) + num4
			Next
		End Sub

		Public Shared Function [mod](a As Double, b As Double) As Double
			Return a Mod b
		End Function

		Public Shared Function fft_phase(s As alglib.complex(), n As Integer) As Double()
			Dim array As Double() = New Double(n - 1) {}
			For i As Integer = 0 To n - 1
				Dim num As Double = s(i).x
				Dim num2 As Double = s(i).y
				Dim flag As Boolean = num < 0.0
				If flag Then
					num *= -1.0
				End If
				num /= num2
				num2 = Math.Atan(num2) * 180.0 / 3.1415926
				Dim flag2 As Boolean = i > 0
				If flag2 Then
					Dim flag3 As Boolean = Math.Abs(array(i - 1) - num2) > 160.0
					If flag3 Then
						Dim flag4 As Boolean = array(i - 1) - num2 > 0.0
						If flag4 Then
							num2 += 180.0
						End If
						Dim flag5 As Boolean = array(i - 1) - num2 < 0.0
						If flag5 Then
							num2 -= 180.0
						End If
					End If
				End If
				Dim num3 As Double = num2
				array(i) = num3
			Next
			For j As Integer = 0 To 10 - 1
				For k As Integer = 0 To n - 1
					Dim flag6 As Boolean = k > 0
					If flag6 Then
						Dim flag7 As Boolean = Math.Abs(array(k - 1) - array(k)) > 160.0
						If flag7 Then
							Dim flag8 As Boolean = array(k - 1) - array(k) > 0.0
							If flag8 Then
								array(k) += 180.0
							End If
							Dim flag9 As Boolean = array(k - 1) - array(k) < 0.0
							If flag9 Then
								array(k) -= 180.0
							End If
						End If
					End If
				Next
			Next
			Return array
		End Function

		Public Shared Function Correlation(IntputData As Double(), ReferenceData As Double(), n As Integer) As Integer
			Dim num As Integer = IntputData.Length
			Dim num2 As Integer = ReferenceData.Length
			Dim num3 As Integer = num + num2 - 1
			Dim array As Double() = New Double(num3 - 1) {}
			Dim array2 As Double() = New Double(num2 - 1) {}
			array2 = AnalyseAudio.flip_impulse_array(IntputData)
			For i As Integer = 0 To num - 1
				For j As Integer = 0 To num2 - 1
					array(i + j) = array(i + j) + array2(i) * ReferenceData(j)
				Next
			Next
			Return AnalyseAudio.IndexPos(array, num, num2)
		End Function

		Public Shared Function flip_impulse_array(data As Double()) As Double()
			Dim num As Integer = data.Length
			Dim array As Double() = New Double(num - 1) {}
			Dim i As Integer = num - 1
			Dim num2 As Integer = 0
			While i >= 0
				array(num2) = data(i)
				i -= 1
				num2 += 1
			End While
			Return array
		End Function

		Public Shared Function IndexPos(OutputData As Double(), inlen As Integer, reflen As Integer) As Integer
			Dim arg_09_0 As Integer = If((inlen > reflen), inlen, reflen)
			Dim num As Integer = 0
			Dim num2 As Double = Math.Abs(OutputData(0))
			For i As Integer = 0 To inlen - 1
				Dim num3 As Double = Math.Abs(OutputData(i))
				Dim flag As Boolean = num2 < num3
				If flag Then
					num2 = num3
					num = i
				End If
			Next
			Return inlen - num
		End Function

		Public Shared Function CshapCorr(datain As Double(), dataso As Double()) As Double()
			Dim num As Integer = datain.Length
			Dim num2 As Integer = dataso.Length
			Dim array As Double() = New Double(num + num2 - 1 - 1) {}
			Dim num3 As Integer = If((num > num2), num, num2)
			For i As Integer = -num3 + 1 To num3 - 1
				Dim num4 As Double = 0.0
				For j As Integer = 0 To num2 - 1
					Dim num5 As Integer = j + i
					Dim flag As Boolean = num5 < 0 OrElse num5 >= num3
					If Not flag Then
						num4 += datain(j) * dataso(num5)
					End If
				Next
				array(i + num3 - 1) = num4
			Next
			Return array
		End Function

		Public Shared Function alglibFFTr1d(datain As Double()) As alglib.complex()
			Dim num As Double = Math.Log(CDec(datain.Length), 2.0)
			Dim num2 As Integer = CInt(num)
			Dim flag As Boolean = CDec(num2) < num
			If flag Then
				num2 += 1
			End If
			Dim num3 As Integer = CInt(Math.Pow(2.0, CDec(num2)))
			Dim array As Double() = New Double(num3 - 1) {}
			For i As Integer = 0 To num3 - 1
				Dim flag2 As Boolean = datain.Length > i
				If flag2 Then
					array(i) = datain(i)
				Else
					array(i) = 0.0
				End If
			Next
			Dim result As alglib.complex()
			alglib.fftr1d(array, result)
			Console.WriteLine("===alglib FFT=====" + DateTime.UtcNow.ToString())
			Return result
		End Function

		Public Shared Function SFR_DFT(data As Double(), Freq As Double, SampleRate As Integer) As SFR.SFRcomplex
			Dim signal As Signal = New SinusoidSignal(1.0, Freq, 0.0, 0.0, SampleRate)
			Dim signal2 As Signal = New SinusoidSignal(1.0, Freq, 90.0, 0.0, SampleRate)
			Dim num As Integer = data.Length
			Dim array As Single() = New Single(num - 1) {}
			Dim array2 As Single() = New Single(num - 1) {}
			Dim array3 As Single() = New Single(num - 1) {}
			Dim num2 As Single = 0F
			Dim num3 As Single = 0F
			Dim num4 As Single = 0F
			For i As Integer = 0 To num - 1
				array2(i) = CSng(signal.GetValue(i))
				array3(i) = CSng(signal2.GetValue(i))
				num2 = CSng((CDec(num2) + data(i)))
			Next
			num2 /= CSng(num)
			For j As Integer = 0 To num - 1
				Dim num5 As Double = CDec((CSng(data(j)) - num2))
				array(j) = CSng(num5)
				num3 += array(j) * array2(j)
				num4 += array(j) * array3(j)
			Next
			num3 /= CSng(num)
			num3 *= 2F
			num4 /= CSng(num)
			num4 *= 2F
			Dim sFRcomplex As SFR.SFRcomplex
			sFRcomplex.Re = CDec(num3)
			sFRcomplex.Im = CDec(num4)
			sFRcomplex.x = 20.0 * Math.Log10(Math.Sqrt(sFRcomplex.Re * sFRcomplex.Re + sFRcomplex.Im * sFRcomplex.Im))
			sFRcomplex.y = Math.Atan2(sFRcomplex.Im, sFRcomplex.Re) * 180.0 / 3.1415926535897931
			Return sFRcomplex
		End Function

		Public Shared Function SFR_DFT(datain As SFR.SignalData) As SFR.SignalData
			Dim signalData As SFR.SignalData = datain
			Dim signal As Signal = New SinusoidSignal(1.0, datain.Freq, 0.0, 0.0, datain.SampleRate)
			Dim signal2 As Signal = New SinusoidSignal(1.0, datain.Freq, 90.0, 0.0, datain.SampleRate)
			Dim num As Integer = datain.Data.Length
			Dim array As Single() = New Single(num - 1) {}
			Dim array2 As Double() = New Double(num - 1) {}
			Dim array3 As Double() = New Double(num - 1) {}
			Dim num2 As Double = 0.0
			Dim num3 As Double = 0.0
			Dim num4 As Double = 0.0
			For i As Integer = 0 To num - 1
				array2(i) = signal.GetValue(i)
				array3(i) = signal2.GetValue(i)
				num2 += datain.Data(i)
			Next
			num2 /= CDec(num)
			For j As Integer = 0 To num - 1
				Dim num5 As Double = CDec((CSng(datain.Data(j)))) - num2
				array(j) = CSng(num5)
				num3 += CDec(array(j)) * array2(j)
				num4 += CDec(array(j)) * array3(j)
			Next
			num3 /= CDec((CSng(num)))
			num3 *= 2.0
			num4 /= CDec((CSng(num)))
			num4 *= 2.0
			signalData.CosData = array3
			signalData.SinData = array2
			signalData.Samples = num
			signalData.Re = num3
			signalData.Im = num4
			signalData.Amplitude = 20.0 * Math.Log10(Math.Sqrt(signalData.Re * signalData.Re + signalData.Im * signalData.Im))
			Dim num6 As Double = 3.1415926535897931
			Dim num7 As Double = Math.Atan2(signalData.Im, signalData.Re) * 180.0 / num6
			Dim flag As Boolean = num7 < 0.0
			If flag Then
				num7 = 360.0 + num7
			End If
			signalData.Phase = num7
			Return signalData
		End Function

		Public Shared Function AlglibFFT(data As Double()) As alglib.complex()
			Dim array As alglib.complex() = AnalyseAudio.alglibFFTr1d(data)
			Dim num As Integer = 48000
			Dim num2 As Double = CDec((num / data.Length))
			Dim num3 As Integer = array.Length
			Dim array2 As Double() = New Double(num3 - 1) {}
			Dim array3 As Double() = New Double(num3 - 1) {}
			For i As Integer = 0 To num3 - 1
				Dim num4 As Double = array(i).x / CDec(num3)
				Dim num5 As Double = array(i).y / CDec(num3)
				array2(i) = num4
				array3(i) = num5
			Next
			Return array
		End Function

		Public Shared Function AlglibFFT(datain As SFR.SignalData) As alglib.complex()
			Dim array As alglib.complex() = AnalyseAudio.alglibFFTr1d(datain.Data)
			Dim sampleRate As Integer = datain.SampleRate
			Dim num As Double = CDec((sampleRate / datain.Data.Length))
			Dim num2 As Integer = array.Length
			Dim array2 As Double() = New Double(num2 - 1) {}
			Dim array3 As Double() = New Double(num2 - 1) {}
			For i As Integer = 0 To num2 - 1
				Dim num3 As Double = array(i).x / CDec(num2)
				Dim num4 As Double = array(i).y / CDec(num2)
				array2(i) = num3
				array3(i) = num4
			Next
			Return array
		End Function

		Public Shared Function Xcorrr1d(datain As Double(), dataso As Double(), <Out()> ByRef corrdata As Double(), <Out()> ByRef Xdata As Double()) As Integer
			Dim num As Integer = datain.Length
			Dim num2 As Integer = dataso.Length
			Dim num3 As Integer = num + num2 - 1
			Dim array As Double() = New Double(num3 - 1) {}
			corrdata = New Double(num3 - 1) {}
			alglib.corrr1d(datain, num, dataso, num2, array)
			Dim num4 As Integer = AnalyseAudio.IndexPos(array, num, num2)
			num4 = num - num4
			corrdata = array
			Dim array2 As Double() = New Double(num - num4 - 1) {}
			For i As Integer = 0 To num - num4 - 1
				array2(i) = datain(i + num4)
			Next
			Xdata = array2
			Return num4
		End Function

		Public Shared Function HanningWin(data As Double(), <Out()> ByRef dataout As Double()) As Double()
			Dim num As Integer = data.Length
			Dim num2 As Double = 3.1415926535897931
			Dim array As Double() = New Double(num - 1) {}
			dataout = New Double(num - 1) {}
			For i As Integer = 0 To num - 1
				Dim num3 As Double = CDec(i) / CDec((num - 1))
				array(i) = data(i) * 0.5 * (1.0 - Math.Cos(2.0 * num2 * num3))
			Next
			For i As Integer = 0 To num - 1
				dataout(i) = data(i) * array(i)
			Next
			Return dataout
		End Function
	End Class
End Namespace
