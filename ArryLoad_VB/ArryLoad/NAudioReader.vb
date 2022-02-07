Imports NAudio.Wave
Imports System
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace ArryLoad
	Public Class NAudioReader
		Implements ISampleProvider

		Private maxValue As Single

		Private minValue As Single

		Private count As Integer

		Private source As ISampleProvider

		Private channels As Integer

		<CompilerGenerated()>
		<DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated()>
		Public Event MaximumCalculated As EventHandler(Of MaxSampleEventArgs)

		Public Property NotificationCount() As Integer

		Public ReadOnly Property WaveFormat() As WaveFormat
			Get
				Return Me.source.WaveFormat
			End Get
		End Property

		Public Sub New(source As ISampleProvider, Optional fftLength As Integer=1024)
			Me.channels = source.WaveFormat.Channels
			Dim flag As Boolean = Not NAudioReader.IsPowerOfTwo(fftLength)
			If flag Then
				Throw New ArgumentException("FFT Length must be a power of two")
			End If
			Me.source = source
			AddressOf Me.NotificationCount = source.WaveFormat.SampleRate / 100
		End Sub

		Private Shared Function IsPowerOfTwo(x As Integer) As Boolean
			Return(x And x - 1) = 0
		End Function

		Public Function Read(buffer As Single(), offset As Integer, count As Integer) As Integer
			Dim num As Integer = Me.source.Read(buffer, offset, count)
			For i As Integer = 0 To num - 1
				Me.Add(buffer(i + offset))
			Next
			Return num
		End Function

		Private Sub Add(value As Single)
			Me.maxValue = Math.Max(Me.maxValue, value)
			Me.minValue = Math.Min(Me.minValue, value)
			Me.count += 1
			Dim flag As Boolean = Me.count >= AddressOf Me.NotificationCount AndAlso AddressOf Me.NotificationCount > 0
			If flag Then
				Dim expr_58 As EventHandler(Of MaxSampleEventArgs) = Me.MaximumCalculated
				If expr_58 IsNot Nothing Then
					expr_58(Me, New MaxSampleEventArgs(Me.minValue, Me.maxValue))
				End If
				Me.Reset()
			End If
		End Sub

		Public Sub Reset()
			Me.count = 0
			Dim expr_0F As Single = 0F
			Dim num As Single = expr_0F
			Me.minValue = expr_0F
			Me.maxValue = num
		End Sub

		Friend Sub Start()
		End Sub

		Friend Sub [Stop]()
		End Sub
	End Class
End Namespace
