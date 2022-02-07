Imports NAudio.Wave
Imports System
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace ArryLoad
	Friend Class NAudioRecorder
		Implements ISpeechRecorder

		Private maxValue As Single

		Private minValue As Single

		Private count As Integer

		Private fftLength As Integer

		Private channels As Integer

		Public waveSource As WaveIn = Nothing

		Public waveFile As WaveFileWriter = Nothing

		Private fileName As String = String.Empty

		<CompilerGenerated()>
		<DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated()>
		Public Event MaximumCalculated As EventHandler(Of MaxSampleEventArgs)

		<CompilerGenerated()>
		<DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated()>
		Public Event DataAvailable As Action(Of Integer)

		Public Property NotificationCount() As Integer

		Public Property PerformFFT() As Boolean

		Public Sub New()
			Me.fftLength = 1024
			Me.channels = 2
			Dim flag As Boolean = Not NAudioRecorder.IsPowerOfTwo(Me.fftLength)
			If flag Then
				Throw New ArgumentException("FFT Length must be a power of two")
			End If
		End Sub

		Private Shared Function IsPowerOfTwo(x As Integer) As Boolean
			Return(x And x - 1) = 0
		End Function

		Public Sub StartRec()
			Me.waveSource = New WaveIn()
			Me.waveSource.WaveFormat = New WaveFormat(16000, 16, 2)
			AddHandler Me.waveSource.DataAvailable, AddressOf Me.waveSource_DataAvailable
			AddHandler Me.waveSource.RecordingStopped, AddressOf Me.waveSource_RecordingStopped
			Me.waveFile = New WaveFileWriter(Me.fileName, Me.waveSource.WaveFormat)
			Me.waveSource.StartRecording()
		End Sub

		Public Sub StopRec()
			Dim flag As Boolean = Me.waveSource IsNot Nothing
			If flag Then
				Me.waveSource.StopRecording()
			End If
		End Sub

		Public Sub SetFileName(fileName As String)
			Me.fileName = fileName
		End Sub

		Private Sub waveSource_DataAvailable(sender As Object, e As WaveInEventArgs)
			Dim flag As Boolean = Me.waveFile IsNot Nothing
			If flag Then
				Me.waveFile.Write(e.Buffer, 0, e.BytesRecorded)
				Me.waveFile.Flush()
				Dim obj As Integer = CInt((Me.waveFile.Length / CLng(Me.waveFile.WaveFormat.AverageBytesPerSecond)))
				Dim expr_5C As Action(Of Integer) = Me.DataAvailable
				If expr_5C IsNot Nothing Then
					expr_5C(obj)
				End If
				Dim array As Single() = New Single(e.Buffer.Length / Me.channels - 1) {}
				Dim num As Integer = 0
				For i As Integer = 0 To e.Buffer.Length - 1
					Dim arg_A0_0 As Single() = array
					Dim expr_88 As Integer = num
					num = expr_88 + 1
					arg_A0_0(expr_88) = CSng(BitConverter.ToInt16(e.Buffer, i)) / 32768F
				Next
				For j As Integer = 0 To array.Length - 1
					Me.Add(array(j))
				Next
			End If
		End Sub

		Private Sub Add(value As Single)
			Me.maxValue = Math.Max(Me.maxValue, value)
			Me.minValue = Math.Min(Me.minValue, value)
			Me.count += 1
			Dim flag As Boolean = Me.count >= Me.waveSource.WaveFormat.SampleRate / 100
			If flag Then
				Dim expr_5C As EventHandler(Of MaxSampleEventArgs) = Me.MaximumCalculated
				If expr_5C IsNot Nothing Then
					expr_5C(Me, New MaxSampleEventArgs(Me.minValue, Me.maxValue))
				End If
				Me.count = 0
				Me.maxValue = 0F
				Me.minValue = 0F
			End If
		End Sub

		Private Sub waveSource_RecordingStopped(sender As Object, e As StoppedEventArgs)
			Dim flag As Boolean = Me.waveSource IsNot Nothing
			If flag Then
				Me.waveSource.Dispose()
				Me.waveSource = Nothing
			End If
			Dim flag2 As Boolean = Me.waveFile IsNot Nothing
			If flag2 Then
				Me.waveFile.Dispose()
				Me.waveFile = Nothing
			End If
		End Sub

		Public Sub Reset()
			Me.count = 0
			Dim expr_0F As Single = 0F
			Dim num As Single = expr_0F
			Me.minValue = expr_0F
			Me.maxValue = num
		End Sub
	End Class
End Namespace
