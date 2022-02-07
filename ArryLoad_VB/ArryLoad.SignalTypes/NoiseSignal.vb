Imports System

Namespace ArryLoad.SignalTypes
	Public Class NoiseSignal
		Inherits Signal

		Private Shared Random As Random = New Random()

		Private Const MinValue As Double = -1.0

		Private Const MaxValue As Double = 1.0

		Public Sub New(A As Double, f As Double, fi As Double)
			MyBase.[New](A, f, fi)
		End Sub

		Public Overrides Function GetValue(x As Integer, N As Integer) As Double
			Return NoiseSignal.Random.NextDouble() * 2.0 + -1.0
		End Function

		Public Overrides Function GetValue(x As Integer) As Double
			Throw New NotImplementedException()
		End Function
	End Class
End Namespace
