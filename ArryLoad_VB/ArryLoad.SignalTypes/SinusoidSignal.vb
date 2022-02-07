Imports System

Namespace ArryLoad.SignalTypes
	Public Class SinusoidSignal
		Inherits Signal

		Public Sub New(Amplitude As Double, Frequency As Double, PhaseDeg As Double, SignalDC As Double, SampleRate As Integer)
			MyBase.[New](Amplitude, Frequency, PhaseDeg, SignalDC, SampleRate)
		End Sub

		Public Overrides Function GetValue(x As Integer) As Double
			Dim num As Double = 3.1415926535897931
			Return AddressOf MyBase.Amplitude * (Math.Sin(2.0 * num * AddressOf MyBase.Frequency * CDec(x) / CDec(AddressOf MyBase.SampleRate) + AddressOf MyBase.PhaseDeg * num / 180.0) + AddressOf MyBase.SignalDC)
		End Function

		Public Overrides Function GetValue(x As Integer, N As Integer) As Double
			Dim num As Double = 3.1415926535897931
			Return AddressOf MyBase.Amplitude * (Math.Sin(2.0 * num * AddressOf MyBase.Frequency * CDec(x) / CDec(N) + AddressOf MyBase.PhaseDeg * num / 180.0) + AddressOf MyBase.SignalDC)
		End Function
	End Class
End Namespace
