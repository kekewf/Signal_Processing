Imports System

Namespace ArryLoad.SignalTypes
	Friend Class DutyCycleSignal
		Inherits Signal

		Public Property DutyFactor() As Double

		Public Sub New(A As Double, f As Double, fi As Double, dutyFactor As Double)
			MyBase.[New](A, f, fi)
			AddressOf Me.DutyFactor = dutyFactor
		End Sub

		Public Overrides Function GetValue(x As Integer, N As Integer) As Double
			Dim num As Double = AddressOf MyBase.Fi * 3.1415926535897931 / 180.0
			Dim num2 As Integer = If((Math.Sin(6.2831853071795862 * AddressOf MyBase.F * CDec(x) / CDec(N) + num) + 1.0 >= 2.0 - 2.0 * AddressOf Me.DutyFactor), 1, 0)
			Return AddressOf MyBase.A * CDec(num2)
		End Function

		Public Overrides Function GetValue(x As Integer) As Double
			Dim sampleRate As Integer = AddressOf MyBase.SampleRate
			Dim num As Double = AddressOf MyBase.Fi * 3.1415926535897931 / 180.0
			Dim num2 As Integer = If((Math.Sin(6.2831853071795862 * AddressOf MyBase.F * CDec(x) / CDec(sampleRate) + num) + 1.0 >= 2.0 - 2.0 * AddressOf Me.DutyFactor), 1, 0)
			Return AddressOf MyBase.A * CDec(num2)
		End Function
	End Class
End Namespace
