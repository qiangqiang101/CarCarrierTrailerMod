Imports GTA
Imports GTA.Math
Imports GTA.Native
Imports System.Windows.Forms
Imports INMNativeUI
Imports TrailerMod.Helper
Imports System.Drawing

Public Class Trailer
    Inherits Script

    Public Shared PlayerChar As Ped
    Public Shared TRL As Vehicle
    Public Shared TRK As Vehicle
    Public Shared TRLModels As List(Of Model) = New List(Of Model) From {"tr2"}
    Public Shared AllowClass As List(Of VehicleClass) = New List(Of VehicleClass) From {VehicleClass.Compacts, VehicleClass.Coupes, VehicleClass.Muscle, VehicleClass.OffRoad, VehicleClass.Sedans, VehicleClass.Sports, VehicleClass.SportsClassics, VehicleClass.Super, VehicleClass.SUVs, VehicleClass.Vans}
    Public Shared T As Timer = New Timer(1500)
    Public Shared T2 As Timer = New Timer(1500)

    Public Shared slot1 As Vector3 = New Vector3(0.0, 0.8, 3.33), slot1r As Vector3 = New Vector3(2.0, 0.0, 0.0)
    Public Shared slot2 As Vector3 = New Vector3(0.0, -2.5, 0.7), slot2r As Vector3 = New Vector3(-6.0, 0.0, 0.0)
    Public Shared slot3 As Vector3 = New Vector3(0.0, -7.5, 0.91), slot3r As Vector3 = New Vector3(-1.0, 0.0, 0.0)
    Public Shared slot4 As Vector3 = New Vector3(0.0, 0.5, 1.4), slot4r As Vector3 = Vector3.Zero
    Public Shared slot5 As Vector3 = New Vector3(0.0, -4.0, 1.45), slot5r As Vector3 = New Vector3(-4.0, 0.0, 0.0)
    Public Shared slot6 As Vector3 = New Vector3(0.0, -9.1, 1.45), slot6r As Vector3 = New Vector3(4.0, 0.0, 0.0)
    Public Shared slot1v, slot2v, slot3v, slot4v, slot5v, slot6v As Vehicle
    Public Shared MainMenu, AttachMenu As UIMenu
    Public Shared _MenuPool As MenuPool
    Public Shared islot1 As New UIMenuItem("Top Deck Front"), islot2 As New UIMenuItem("Top Deck Middle"), islot3 As New UIMenuItem("Top Deck Rear")
    Public Shared islot4 As New UIMenuItem("Lower Deck Front"), islot5 As New UIMenuItem("Lower Deck Middle"), islot6 As New UIMenuItem("Lower Deck Rear")
    Public Shared iTrunk As New UIMenuItem("Release/Raise Ramp"), iHood As New UIMenuItem("Lower/Raise Top Deck"), iRepair As New UIMenuItem("Repair Trailer")
    Public Shared iSpawn As New UIMenuListItem("Spawn a Car Carrier Trailer & a", New List(Of Object)() From {"Hauler", "Packer", "Phantom"}, 0)
    Public Shared iAttach As New UIMenuItem("Detach Trailer"), iAttachTo As New UIMenuItem("Attach/Detach Vehicle")
    Public Shared SelectedTruck As Model = "Hauler"
    Public Shared Config As ScriptSettings = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")

    Public Sub New()
        PlayerChar = Game.Player.Character

        _MenuPool = New MenuPool()

        CreateMainMenu()
        CreateCTrailerMenu()
    End Sub

    Public Sub CreateMainMenu()
        MainMenu = New UIMenu("", "CAR CARRIER TRAILER MOD", New Point(0, -107))
        Dim Rectangle = New UIResRectangle()
        Rectangle.Color = Color.FromArgb(0, 0, 0, 0)
        MainMenu.SetBannerType(Rectangle)
        _MenuPool.Add(MainMenu)
        MainMenu.AddItem(iSpawn)
        MainMenu.AddItem(iAttach)
        MainMenu.AddItem(iTrunk)
        MainMenu.AddItem(iHood)
        MainMenu.AddItem(iRepair)
        MainMenu.AddItem(iAttachTo)
        MainMenu.RefreshIndex()
        AddHandler MainMenu.OnItemSelect, AddressOf ItemSelectHandler
        AddHandler MainMenu.OnListChange, AddressOf ItemListChangeHandler
    End Sub

    Public Sub CreateCTrailerMenu()
        AttachMenu = New UIMenu("", "ATTACH VEHICLE", New Point(0, -107))
        Dim Rectangle = New UIResRectangle()
        Rectangle.Color = Color.FromArgb(0, 0, 0, 0)
        AttachMenu.SetBannerType(Rectangle)
        _MenuPool.Add(AttachMenu)
        With islot1
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot1)
        With islot2
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot2)
        With islot3
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot3)
        With islot4
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot4)
        With islot5
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot5)
        With islot6
            .SetRightLabel("Empty")
        End With
        AttachMenu.AddItem(islot6)
        MainMenu.BindMenuToItem(AttachMenu, iAttachTo)
        AttachMenu.RefreshIndex()
        AddHandler AttachMenu.OnItemSelect, AddressOf ItemSelectHandler
    End Sub

    Public Sub OnTick(o As Object, e As EventArgs) Handles Me.Tick
        PlayerChar = Game.Player.Character

        _MenuPool.ProcessMenus()

        If IsVehicleAttachedToTrailer(PlayerChar.LastVehicle) Then
            If TRLModels.Contains(GetVehicleTrailerVehicle(Game.Player.LastVehicle).Model) Then
                TRL = GetVehicleTrailerVehicle(Game.Player.LastVehicle)
            End If
            TRK = Game.Player.LastVehicle
        End If

        If Not TRL = Nothing AndAlso TRL.IsDead Then
            If Not slot1v = Nothing Then
                slot1v.Detach()
                slot1v.IsPersistent = False
                islot1.SetRightLabel("Empty")
            End If
            If Not slot2v = Nothing Then
                slot2v.Detach()
                slot2v.IsPersistent = False
                islot2.SetRightLabel("Empty")
            End If
            If Not slot3v = Nothing Then
                slot3v.Detach()
                slot3v.IsPersistent = False
                islot3.SetRightLabel("Empty")
            End If
            If Not slot4v = Nothing Then
                slot4v.Detach()
                slot4v.IsPersistent = False
                islot4.SetRightLabel("Empty")
            End If
            If Not slot5v = Nothing Then
                slot5v.Detach()
                slot5v.IsPersistent = False
                islot5.SetRightLabel("Empty")
            End If
            If Not slot6v = Nothing Then
                slot6v.Detach()
                slot6v.IsPersistent = False
                islot6.SetRightLabel("Empty")
            End If
        End If

        If T.Enabled Then
            If Game.GameTime > T.Waiter Then
                T.Enabled = False
                TRL.CloseDoor(VehicleDoor.Hood, True)
            End If
        End If
        If T2.Enabled Then
            If Game.GameTime > T2.Waiter Then
                TRL.OpenDoor(VehicleDoor.Hood, False, True)
            End If
        End If

        If (Native.Function.Call(Of Boolean)(Native.Hash._GET_LAST_INPUT_METHOD, 2) = False AndAlso Game.IsControlPressed(2, Config.GetValue(Of GTA.Control)("CONTROL", "PADPRIMARY", GTA.Control.ScriptPadRight)) AndAlso Game.IsControlPressed(2, Config.GetValue(Of GTA.Control)("CONTROL", "PADSECONDARY", GTA.Control.ScriptRUp)) AndAlso (Not _MenuPool.IsAnyMenuOpen())) Then
            MainMenu.Visible = Not MainMenu.Visible
        End If
    End Sub

    Public Shared Sub ItemListChangeHandler(sender As UIMenu, selectedItem As UIMenuListItem, index As Integer)
        If selectedItem Is iSpawn Then
            SelectedTruck = selectedItem.IndexToItem(index).ToString
        End If
    End Sub

    Public Sub ItemSelectHandler(sender As UIMenu, selectedItem As UIMenuItem, index As Integer)
        If selectedItem Is iTrunk Then
            If Not TRL = Nothing Then
                If TRL.IsDoorOpen(VehicleDoor.Trunk) Then TRL.CloseDoor(VehicleDoor.Trunk, False)
                If Not TRL.IsDoorOpen(VehicleDoor.Trunk) Then TRL.OpenDoor(VehicleDoor.Trunk, False, False)
            Else
                UI.ShowSubtitle("Trailer Not Found!")
            End If
        End If

        If selectedItem Is iHood Then
            If Not TRL = Nothing Then
                If TRL.IsDoorOpen(VehicleDoor.Hood) Then
                    T2.Enabled = False
                    TRL.CloseDoor(VehicleDoor.Hood, False)
                    T.Start()
                End If

                If Not TRL.IsDoorOpen(VehicleDoor.Hood) Then
                    TRL.OpenDoor(VehicleDoor.Hood, False, False)
                    T2.Start()
                End If
            Else
                UI.ShowSubtitle("Trailer Not Found!")
            End If
        End If

        If selectedItem Is iRepair Then
            If Not TRL = Nothing Then
                TRL.Repair()
            Else
                UI.ShowSubtitle("Trailer Not Found!")
            End If
        End If

        If selectedItem Is iAttach Then
            If IsVehicleAttachedToTrailer(PlayerChar.LastVehicle) Then
                Native.Function.Call(Hash.DETACH_VEHICLE_FROM_TRAILER, PlayerChar.LastVehicle)
            Else
                UI.ShowSubtitle("Trailer is not attach to " & PlayerChar.LastVehicle.FriendlyName & ".")
            End If
        End If

        If selectedItem Is iSpawn Then
            If TRK = Nothing AndAlso TRL = Nothing Then
                TRK = World.CreateVehicle(SelectedTruck, World.GetNextPositionOnStreet(PlayerChar.Position), PlayerChar.Heading)
                TRL = World.CreateVehicle(VehicleHash.TR2, TRK.Position, TRK.Heading)
                Native.Function.Call(Hash.ATTACH_VEHICLE_TO_TRAILER, TRK, TRL, 1.0)
                TRK.Repair()
                TRL.Repair()
                PlayerChar.Task.WarpIntoVehicle(TRK, VehicleSeat.Driver)
                Wait(2000)
            Else
                TRK.Delete()
                TRL.Delete()
                TRK = World.CreateVehicle(SelectedTruck, World.GetNextPositionOnStreet(PlayerChar.Position), PlayerChar.Heading)
                TRL = World.CreateVehicle(VehicleHash.TR2, TRK.Position, TRK.Heading)
                Native.Function.Call(Hash.ATTACH_VEHICLE_TO_TRAILER, TRK, TRL, 1.0)
                TRK.Repair()
                TRL.Repair()
                PlayerChar.Task.WarpIntoVehicle(TRK, VehicleSeat.Driver)
                Wait(2000)
            End If
            islot1.SetRightLabel("Empty")
            islot2.SetRightLabel("Empty")
            islot3.SetRightLabel("Empty")
            islot4.SetRightLabel("Empty")
            islot5.SetRightLabel("Empty")
            islot6.SetRightLabel("Empty")
        End If

        If Not TRL = Nothing AndAlso (PlayerChar.Position.DistanceTo(TRL.Position) <= 10.0) Then
            If selectedItem Is islot1 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot1v = PlayerChar.CurrentVehicle
                    slot1v.EngineRunning = False
                    slot1v.IsPersistent = True
                    slot1v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot1v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot1.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot1ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot1ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bodyshell"), slot1, slot1r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot1v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot1v, VehicleSeat.Driver)
                    slot1v.IsPersistent = False
                    slot1v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            ElseIf selectedItem Is islot2 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot2v = PlayerChar.CurrentVehicle
                    slot2v.EngineRunning = False
                    slot2v.IsPersistent = True
                    slot2v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot2v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot2.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot2ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot2ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bonnet"), slot2, slot2r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot2v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot2v, VehicleSeat.Driver)
                    slot2v.IsPersistent = False
                    slot2v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            ElseIf selectedItem Is islot3 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot3v = PlayerChar.CurrentVehicle
                    slot3v.EngineRunning = False
                    slot3v.IsPersistent = True
                    slot3v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot3v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot3.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot3ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot3ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bonnet"), slot3, slot3r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot3v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot3v, VehicleSeat.Driver)
                    slot3v.IsPersistent = False
                    slot3v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            ElseIf selectedItem Is islot4 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot4v = PlayerChar.CurrentVehicle
                    slot4v.EngineRunning = False
                    slot4v.IsPersistent = True
                    slot4v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot4v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot4.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot4ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot4ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bodyshell"), slot4, slot4r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot4v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot4v, VehicleSeat.Driver)
                    slot4v.IsPersistent = False
                    slot4v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            ElseIf selectedItem Is islot5 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot5v = PlayerChar.CurrentVehicle
                    slot5v.EngineRunning = False
                    slot5v.IsPersistent = True
                    slot5v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot5v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot5.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot5ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot5ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bodyshell"), slot5, slot5r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot5v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot5v, VehicleSeat.Driver)
                    slot5v.IsPersistent = False
                    slot5v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            ElseIf selectedItem Is islot6 Then
                Game.FadeScreenOut(300)
                Wait(300)
                If PlayerChar.IsInVehicle AndAlso selectedItem.RightLabel = "Empty" AndAlso AllowClass.Contains(PlayerChar.LastVehicle.ClassType) Then
                    slot6v = PlayerChar.CurrentVehicle
                    slot6v.EngineRunning = False
                    slot6v.IsPersistent = True
                    slot6v.LockStatus = VehicleLockStatus.CannotBeTriedToEnter
                    selectedItem.SetRightLabel(slot6v.FriendlyName)
                    If PlayerChar.IsInVehicle Then PlayerChar.Task.WarpOutOfVehicle(PlayerChar.LastVehicle)
                    Config = ScriptSettings.Load("scripts\CarCarrierTrailerMod.ini")
                    Dim u As String = Config.GetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), slot6.Z.ToString())
                    If u = Nothing Then
                        Config.SetValue(Of String)("VEHCOORDS", PlayerChar.LastVehicle.Model.Hash.ToString(), 0.0)
                        Config.Save()
                        u = "0.0"
                    End If
                    If u.Contains("-") Then
                        UpdateSlot6ZCoords("-", Convert.ToSingle(u.Remove(0, 1)))
                    Else
                        UpdateSlot6ZCoords("+", Convert.ToSingle(u))
                    End If
                    AttachTo(PlayerChar.LastVehicle, TRL, TRL.GetBoneIndex("bodyshell"), slot6, slot6r)
                ElseIf Not PlayerChar.IsInVehicle AndAlso Not selectedItem.RightLabel = "Empty" Then
                    slot6v.Detach()
                    selectedItem.SetRightLabel("Empty")
                    PlayerChar.Task.WarpIntoVehicle(slot6v, VehicleSeat.Driver)
                    slot6v.IsPersistent = False
                    slot6v.LockStatus = VehicleLockStatus.Unlocked
                End If
                Wait(300)
                Game.FadeScreenIn(300)
            End If
        End If
    End Sub

    Public Sub OnKeyDown(o As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Config.GetValue(Of Keys)("CONTROL", "SECONDARY", Keys.B) AndAlso e.Modifiers = Config.GetValue(Of Keys)("CONTROL", "PRIMARY", Keys.Shift) Then
            If Not _MenuPool.IsAnyMenuOpen Then
                MainMenu.Visible = Not MainMenu.Visible
            End If
        End If
    End Sub

    Public Sub OnAbort() Handles Me.Aborted
        If Not TRL = Nothing Then TRL.Delete()
        If Not TRK = Nothing Then TRK.Delete()
        If Not slot1v = Nothing Then slot1v.Delete()
        If Not slot2v = Nothing Then slot2v.Delete()
        If Not slot3v = Nothing Then slot3v.Delete()
        If Not slot4v = Nothing Then slot4v.Delete()
        If Not slot5v = Nothing Then slot5v.Delete()
        If Not slot6v = Nothing Then slot6v.Delete()
        Game.FadeScreenIn(300)
    End Sub
End Class
