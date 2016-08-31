Imports GTA
Imports GTA.Math
Imports GTA.Native

Public Class Helper

    Public Shared Function IsVehicleAttachedToTrailer(vh As Vehicle) As Boolean
        Return Native.Function.Call(Of Boolean)(Hash.IS_VEHICLE_ATTACHED_TO_TRAILER, vh)
    End Function

    Public Shared Function GetVehicleTrailerVehicle(vh As Vehicle) As Vehicle
        Dim out As New OutputArgument()
        Native.Function.Call(Hash.GET_VEHICLE_TRAILER_VEHICLE, vh, out)
        Return out.GetResult(Of Vehicle)()
    End Function

    Public Shared Sub UpdateSlot1ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot1 = New Vector3(0.0, 4.85, 2.85 + Value)
        Else
            Trailer.slot1 = New Vector3(0.0, 4.85, 2.85 - Value)
        End If
    End Sub

    Public Shared Sub UpdateSlot2ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot2 = New Vector3(0.0, -2.5, 0.7 + Value)
        Else
            Trailer.slot2 = New Vector3(0.0, -2.5, 0.7 - Value)
        End If
    End Sub

    Public Shared Sub UpdateSlot3ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot3 = New Vector3(0.0, -7.5, 0.91 + Value)
        Else
            Trailer.slot3 = New Vector3(0.0, -7.5, 0.91 - Value)
        End If
    End Sub

    Public Shared Sub UpdateSlot4ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot4 = New Vector3(0.0, 4.8, 0.92 + Value)
        Else
            Trailer.slot4 = New Vector3(0.0, 4.8, 0.92 - Value)
        End If
    End Sub

    Public Shared Sub UpdateSlot5ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot5 = New Vector3(0.0, 0.0, 1.03 + Value)
        Else
            Trailer.slot5 = New Vector3(0.0, 0.0, 1.03 - Value)
        End If
    End Sub

    Public Shared Sub UpdateSlot6ZCoords(PlusMinus As String, Value As Single)
        If PlusMinus = "+" Then
            Trailer.slot6 = New Vector3(0.0, -4.8, 1.03 + Value)
        Else
            Trailer.slot6 = New Vector3(0.0, -4.8, 1.03 - Value)
        End If
    End Sub

    Public Shared Sub AttachTo(entity1 As Entity, entity2 As Entity, boneindex As Integer, position As Vector3, rotation As Vector3)
        Native.Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entity1.Handle, entity2.Handle, boneindex, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, False, False, True, False, 2, True)
    End Sub

End Class
