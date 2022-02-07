Imports ArryLoad.ArryLoad.SignalTypes
Imports System
Imports System.Diagnostics
Imports System.IO

Namespace ArryLoad
    Friend Class Program
        Private Shared Sub Main(args As String())
            Dim expr_07 As Double() = New Double() {1.0, 2.0, 3.0}
            Dim num As Integer = Program.requestMethod("D:\BeepChirpExp.txt")
            Dim array As Double() = New Double(num - 1) {}
            Dim streamReader As StreamReader = New StreamReader("D:\BeepChirpExp.txt")
            Dim num2 As Integer = 0
            While Not streamReader.EndOfStream
                Dim text As String = streamReader.ReadLine()
                Dim flag As Boolean = text <> ""
                If flag Then
                    array(num2) = Convert.ToDouble(text)
                End If
                num2 += 1
            End While
            Dim flag2 As Boolean = num2 < num
            If flag2 Then
                For i As Integer = num2 To num - 1
                    array(i) = 0.0
                Next
            End If
            streamReader.Close()
            Dim num3 As Integer = num
            Console.WriteLine("===R0=====")
            num = Program.requestMethod("D:\BeepChirpExp - Ref.txt")
            Dim array2 As Double() = New Double(num - 1) {}
            streamReader = New StreamReader("D:\BeepChirpExp - Ref.txt")
            num2 = 0
            While Not streamReader.EndOfStream
                Dim text2 As String = streamReader.ReadLine()
                Dim flag3 As Boolean = text2 <> ""
                If flag3 Then
                    array2(num2) = Convert.ToDouble(text2)
                End If
                num2 += 1
            End While
            Dim flag4 As Boolean = num2 < num
            If flag4 Then
                For j As Integer = num2 To num - 1
                    array2(j) = 0.0
                Next
            End If
            streamReader.Close()
            Console.WriteLine("===RS=====")
            Dim array3 As Double()
            Dim array4 As Double()
            Dim num4 As Integer = AnalyseAudio.Xcorrr1d(array, array2, array3, array4)
            Console.WriteLine("===RS=====" + num4.ToString())
            array = New Double(num3 - num4 - 1) {}
            For k As Integer = 0 To num3 - num4 - 1
                array(k) = array4(k)
            Next
            Dim sampleRate As Integer = 48000
            Dim freq As Double = 1000.0
            Dim signalData As SFR.SignalData = AnalyseAudio.SFR_DFT(New SFR.SignalData() With {.Freq = freq, .Data = array, .SampleRate = sampleRate})
            Console.WriteLine("===FFT data =====Amp: " + signalData.Amplitude.ToString("0.0000000000000000000000000"))
            Console.WriteLine("===FFT data =====Phase: " + signalData.Phase.ToString("###0.00000000000000"))
            Dim num5 As Single = 20.0F
            Dim maxFrequency As Double = 10000.0
            Dim amplitude As Double = 500.0
            Dim duration As Double = 500.0
            Beep.BeepChirpExp(sampleRate, 50, amplitude, CDec(num5), maxFrequency, duration, 0.00001, True, 16)
            Console.WriteLine(" ===== END ===== ")
            Console.Read()
        End Sub

        Public Shared Function requestMethod(_fileName As String) As Integer
            Dim stopwatch As Stopwatch = New Stopwatch()
            Dim num As Integer = 0
            stopwatch.Restart() 'Dim
            Using streamReader As StreamReader = New StreamReader(_fileName)
                While streamReader.ReadLine() <> Nothing
                    num += 1
                End While
            End Using
            stopwatch.[Stop]()
            Return num
        End Function

        Public Shared Sub writefile(data As Double(), n As Integer, df As Double)
            For i As Integer = 0 To data.Length - 1
                Dim data2 As String = (CDec(i) * df).ToString() + "," + data(i).ToString()
                Dim text As String = Program.SavaProcess("corr" + n.ToString() + ".txt", data2)
            Next
            Console.WriteLine("===Wirte data " + n.ToString() + "OK=====")
        End Sub

        Public Shared Function SavaProcess(FileName As String, data As String) As String
            Dim baseDirectory As String = AppDomain.CurrentDomain.BaseDirectory
            Dim text As String = baseDirectory + FileName
            Dim streamWriter As StreamWriter = New StreamWriter(text, True)
            streamWriter.WriteLine(data)
            streamWriter.Close()
            streamWriter.Dispose()
            Return text
        End Function
    End Class
End Namespace
