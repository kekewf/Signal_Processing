Imports NAudio.Dsp
Imports System
Imports System.Diagnostics

Namespace ArryLoad
	Public Class FftEventArgs
		Inherits EventArgs

		Public Property Result() As Complex()

		<DebuggerStepThrough()>
		Public Sub New(result As Complex())
			AddressOf Me.Result = result
		End Sub
	End Class
End Namespace
