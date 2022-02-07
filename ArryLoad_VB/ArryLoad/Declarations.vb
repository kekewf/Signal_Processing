Imports System

Namespace ArryLoad
	Public Class Declarations
		Private Function IsCorrectDouble(str As String, <Out()> ByRef value As Double) As Boolean
			Dim flag As Boolean = Double.TryParse(str, value)
			Dim result As Boolean
			If flag Then
				Dim flag2 As Boolean = value >= 0.0
				result = flag2
			Else
				result = False
			End If
			Return result
		End Function
	End Class
End Namespace
