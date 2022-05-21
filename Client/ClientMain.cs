using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Client
{
    public class ClientMain : BaseScript
    {
        public bool showJoinScreen = false;

        public ClientMain()
        {
            Debug.WriteLine($"Escape Los Santos {Helpers.GetAssemblyVersion()} client loaded");
            EventHandlers["escape_ls:toggleJoinScreen"] += new Action(ToggleJoinScreen);
            EventHandlers["escape_ls:setWantedLevel"] += new Action<int>(SetWantedLevelEvent);
            EventHandlers["escape_ls:startEscape"] += new Action(StartEscape);

            int blip = AddBlipForArea(0, 1500, 50, 10000, 13000);
            SetBlipColour(blip, 6);
            SetBlipAlpha(blip, 64);

            SetRandomTrains(true);
            SetRandomBoats(true);

            TriggerEvent("chat:addMessage", new
            {
                color = new int[] { 255, 255, 255 },
                multiline = false,
                args = new[] { "Gamemode", $"^*Escape Los Santos by getting out of the ^1RED^7 zone marked on the map!" },
            });

            TriggerEvent("chat:addMessage", new
            {
                color = new int[] { 255, 255, 255 },
                multiline = false,
                args = new[] { "Gamemode", $"^*Join a lobby by typing ^3/lobby <number>" },
            });
        }

        [Tick]
        public Task OnTick()
        {
            SetParkedVehicleDensityMultiplierThisFrame(1.0f);
            SetRandomVehicleDensityMultiplierThisFrame(0.8f);

            if (showJoinScreen)
            {
                DrawJoinScreen();
            }

            return Task.FromResult(0);
        }

        [Tick]
        public Task OtherPlayersCheck()
        {
            foreach (Player otherPlayer in Players)
            {
                if (otherPlayer.Handle == LocalPlayer.Handle)
                    continue;

                int blip = GetBlipFromEntity(otherPlayer.Character.Handle);

                if (blip == 0)
                {
                    int localPlayerBlip = AddBlipForEntity(otherPlayer.Character.Handle);
                    SetBlipAsFriendly(localPlayerBlip, true);
                }
            }

            return Delay(1000);
        }

        public void ToggleJoinScreen()
        {
            showJoinScreen = !showJoinScreen;

            if (showJoinScreen)
            {
                FreezeEntityPosition(LocalPlayer.Character.Handle, true);
            }
            else
            {
                FreezeEntityPosition(LocalPlayer.Character.Handle, false);
            }
        }

        public void DrawJoinScreen()
        {
            FreezeEntityPosition(LocalPlayer.Character.Handle, true);
        }

        public async void StartEscape()
        {
            RequestModel((uint)GetHashKey("s_m_m_prisguard_01"));
            RequestModel((uint)GetHashKey("s_m_y_prisoner_01"));

            while (!HasModelLoaded((uint)GetHashKey("s_m_y_prisoner_01")))
            {
                await Delay(50);
            }

            int prisonGuardPed = CreatePed(0, (uint)GetHashKey("s_m_m_prisguard_01"), 1643, 2538, 45.5f, 120, true, true);
            GiveWeaponToPed(prisonGuardPed, (uint)GetHashKey("weapon_pistol"), 12, false, true);
            SetCurrentPedWeapon(prisonGuardPed, (uint)GetHashKey("weapon_pistol"), true);

            int prisonerPed = CreatePed(0, (uint)GetHashKey("s_m_y_prisoner_01"), 1645, 2538, 45.5f, 120, true, true);
            GiveWeaponToPed(prisonerPed, (uint)GetHashKey("weapon_switchblade"), 12, false, true);
            SetCurrentPedWeapon(prisonerPed, (uint)GetHashKey("weapon_switchblade"), true);

            ApplyDamageToPed(prisonGuardPed, 101, false);
            ApplyDamageToPed(prisonerPed, 101, false);
        }

        private void SetWantedLevelEvent(int level)
        {
            if (LocalPlayer.Character.Position.X > 1630 && LocalPlayer.Character.Position.X < 1660 && LocalPlayer.Character.Position.Y > 2510 && LocalPlayer.Character.Position.Y < 2545)
                return;

            SetPlayerWantedLevel(LocalPlayer.Handle, level, false);
            SetWantedLevelMultiplier(1.5f);
            SetWantedLevelDifficulty(LocalPlayer.Handle, 1.0f);
            SetPlayerWantedLevelNow(LocalPlayer.Handle, false);
        }
    }
}