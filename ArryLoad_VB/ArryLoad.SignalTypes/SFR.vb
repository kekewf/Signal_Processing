Imports System
Imports System.Collections.Generic

Namespace ArryLoad.SignalTypes
	Public Class SFR
		Public Structure SFRcomplex
			Public x As Double

			Public y As Double

			Public Re As Double

			Public Im As Double
		End Structure

		Public Structure SignalData
			Public Freq As Double

			Public SampleRate As Integer

			Public Amplitude As Double

			Public Phase As Double

			Public DC As Double

			Public Samples As Integer

			Public Harmonic As Integer

			Public Noise As Double

			Public SinData As Double()

			Public CosData As Double()

			Public Data As Double()

			Public HarmDist As Double()

			Public Re As Double

			Public Im As Double

			Public r As Double

			Public theta As Double
		End Structure

		Public Structure SweepSignalData
			Public SampleRate As Integer

			Public DC As Double

			Public Samples As Integer

			Public MinCycleNum As Integer

			Public MinDuration As Integer

			Public MaxDelayTime As Double

			Public StartFreqency As Double

			Public StopFreqency As Double

			Public SweepType As Integer

			Public SweepISO As List(Of String)

			Public FreqList As Double()

			Public Amplitude As Double()

			Public Phase As Double()

			Public DataIndexPos As Double()

			Public DelayPoint As Double

			Public Data As Double()

			Public Harmonic As Integer()

			Public Noise As Double()

			Public Re As Double()

			Public Im As Double()

			Public r As Double()

			Public theta As Double()
		End Structure
	End Class
End Namespace
