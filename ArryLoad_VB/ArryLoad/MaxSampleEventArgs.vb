Imports System
Imports System.Diagnostics

Namespace ArryLoad
	Public Class MaxSampleEventArgs
		Inherits EventArgs

		Public Property MaxSample() As Single

		Public Property MinSample() As Single

		<DebuggerStepThrough()>
		Public Sub New(minValue As Single, maxValue As Single)
			AddressOf Me.MaxSample = maxValue
			AddressOf Me.MinSample = minValue
		End Sub
	End Class
End Namespace
