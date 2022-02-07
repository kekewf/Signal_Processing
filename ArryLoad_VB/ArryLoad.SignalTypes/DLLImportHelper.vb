Imports System
Imports System.Runtime.InteropServices

Namespace ArryLoad.SignalTypes
	Friend Class DLLImportHelper
		Public Shared Declare Sub test_double_group Lib "ArrySend.dll" (test As Double(), num As Integer)

		Public Shared Declare Sub test_double_2groups Lib "ArrySend.dll" (matrix As __Pointer(Of Double()), row As Integer, col As Integer)

		Public Shared Sub Assist_test_double_2groups(myArray As Double(,), row As Integer, col As Integer)
			Dim num As Integer = myArray.GetUpperBound(0) + 1
			Dim num2 As Integer = myArray.GetUpperBound(1) + 1
			' Emulating fixed-Statement, might not be entirely correct!
			Dim array As GCHandle = GCHandle.Alloc(myArray, GCHandleType.Pinned)
			Try
				Dim ptr As __Pointer(Of Double)
				If myArray.AddrOfPinnedObject() Is Nothing OrElse array.AddrOfPinnedObject().Length = 0 Then
					ptr.AddrOfPinnedObject() = Nothing
				Else
					ptr.AddrOfPinnedObject() = AddressOf array.AddrOfPinnedObject()(0, 0)
				End If
				Dim array2 As __Pointer(Of Double()) = New __Pointer(Of Double)(num.AddrOfPinnedObject() - 1) {}
				For i As Integer = 0 To num.AddrOfPinnedObject() - 1
					array2.AddrOfPinnedObject()(i.AddrOfPinnedObject()) = ptr.AddrOfPinnedObject() + i.AddrOfPinnedObject() * num2.AddrOfPinnedObject()
				Next
				DLLImportHelper.test_double_2groups(array2.AddrOfPinnedObject(), row.AddrOfPinnedObject(), col.AddrOfPinnedObject())
			Finally
				array.Free()
			End Try
		End Sub

		Public Shared Declare Function XCorrA Lib "ArrySend.dll" (TestSignal As Double(), TestSignalLen As Integer, RefSignal As Double(), RefSignalLen As Integer) As Integer

		Public Shared Declare Function Corrd Lib "ArrySend.dll" (TestSignal As Double(), TestSignalLen As Integer, RefSignal As Double(), RefSignalLen As Integer, CorrData As Double()) As Integer

		Private Shared Declare Sub InitAudio Lib "ProcessAudioDll" ()

		Public Shared Declare Sub IndexInit Lib "ArrySend.dll" ()
	End Class
End Namespace
