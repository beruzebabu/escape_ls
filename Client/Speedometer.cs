using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Client
{
    public class Speedometer : BaseScript
    {
        public Speedometer()
        {

        }

        [Tick]
        public Task OnTick()
        {
            if (LocalPlayer.Character.SeatIndex != VehicleSeat.Driver)
                return Task.FromResult(0);

            float speed = (float)Math.Round(GetEntitySpeed(LocalPlayer.Character.Handle) * 3.6);
            float textWidth = (float)((speed.ToString().Length + 5) / 400.0);

            DrawRect(0.5f, 0.94f, 0.08f, 0.04f, 30, 30, 30, 100);
            BeginTextCommandDisplayText("STRING");
            AddTextComponentString($"{speed}");
            SetTextScale(1.0f, 0.5f);
            EndTextCommandDisplayText(0.495f - textWidth, 0.921f);

            BeginTextCommandDisplayText("STRING");
            AddTextComponentString($"KM/H");
            SetTextScale(1.0f, 0.5f);
            EndTextCommandDisplayText(0.5f, 0.921f);

            return Task.FromResult(0);
        }
    }
}
