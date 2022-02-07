Imports System

Namespace ArryLoad.SignalTypes
	Public MustInherit Class Signal
		Public Property PI() As Double

		Public Property A() As Double

		Public Property F() As Double

		Public Property Fi() As Double

		Public Property Amplitude() As Double

		Public Property Frequency() As Double

		Public Property PhaseDeg() As Double

		Public Property SampleRate() As Integer

		Public Property SignalDC() As Double

		Public Sub New(A As Double, f As Double, fi As Double)
			AddressOf Me.A = A
			AddressOf Me.F = f
			AddressOf Me.Fi = fi
			AddressOf Me.Amplitude = A
			AddressOf Me.Frequency = f
			AddressOf Me.PhaseDeg = fi * 3.1415926535897931 / 180.0
			AddressOf Me.SignalDC = 0.0
		End Sub

		Public Sub New(amplitude As Double, frequency As Double, deg As Double, dc As Double, samplerate As Integer)
			AddressOf Me.Amplitude = amplitude
			AddressOf Me.Frequency = frequency
			AddressOf Me.PhaseDeg = deg
			AddressOf Me.SampleRate = samplerate
			AddressOf Me.SignalDC = dc
			AddressOf Me.A = amplitude
			AddressOf Me.F = frequency
			AddressOf Me.Fi = deg * 180.0 / 3.1415926535897931
		End Sub

		Public MustOverride Function GetValue(x As Integer, N As Integer) As Double

		Public MustOverride Function GetValue(x As Integer) As Double
	End Class
End Namespace
