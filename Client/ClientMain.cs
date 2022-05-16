using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Client
{
    public class ClientMain : BaseScript
    {
        public ClientMain()
        {
            Debug.WriteLine($"Escape Los Santos {Helpers.GetAssemblyVersion()} client loaded");
            EventHandlers["escape_ls:setWantedLevel"] += new Action<int>(SetWantedLevelEvent);

            int blip = AddBlipForArea(0, 1500, 50, 10000, 13000);
            SetBlipColour(blip, 6);
            SetBlipAlpha(blip, 64);

            TriggerEvent("chat:addMessage", new
            {
                color = new int[] { 255, 255, 255 },
                multiline = false,
                args = new[] { "Gamemode", $"Escape Los Santos by getting out of the ^*^1RED^7 zone marked on the map!" },
            });
        }

        [Tick]
        public Task OnTick()
        {
            return Task.FromResult(0);
        }

        private void SetWantedLevelEvent(int level)
        {
            SetPlayerWantedLevel(LocalPlayer.Handle, level, false);
            SetWantedLevelMultiplier(1.5f);
            SetWantedLevelDifficulty(LocalPlayer.Handle, 1.0f);
            SetPlayerWantedLevelNow(LocalPlayer.Handle, false);
        }
    }
}