using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Server
{
    public class Commands : BaseScript
    {
        [Command("spawn_car", Restricted = true)]
        public void CommandSpawnCar(Player src, string[] args)
        {
            if (args.Length < 1)
            {
                Debug.WriteLine("Missing vehicle model argument");
                return;
            }

            string model = args[0].Trim();

            Debug.WriteLine($"Spawning {model}");

            uint hash = (uint)GetHashKey(model);
            int vehicle = CreateVehicle(hash, src.Character.Position.X, src.Character.Position.Y, src.Character.Position.Z, src.Character.Heading, true, false);
            SetPedIntoVehicle(src.Character.Handle, vehicle, -1);
        }

        [Command("coords")]
        public void CommandGetCoordinates(Player src)
        {
            Debug.WriteLine($"{src.Character.Position}");
        }

        [Command("teleport", Restricted = true)]
        public void CommandTeleportPlayer(Player src, string[] args)
        {
            if (args.Length < 3)
            {
                Debug.WriteLine("Missing coordinate arguments");
                return;
            }

            try
            {
                float x = float.Parse(args[0].Trim());
                float y = float.Parse(args[1].Trim());
                float z = float.Parse(args[2].Trim());

                SetEntityCoords(src.Character.Handle, x, y, z, true, false, false, false);
            }
            catch
            {
                Debug.WriteLine("Invalid coordinate arguments");
                return;
            }
        }

        [Command("lobby")]
        public void CommandSelectLobby(Player src, string[] args)
        {
            try
            {
                int lobby = int.Parse(args[0].Trim());

                TriggerEvent("escape_ls:LobbySelected", int.Parse(src.Handle.Trim()), lobby);
            }
            catch
            {
                Debug.WriteLine("Invalid lobby entered");
                return;
            }
        }

        [Command("difficulty")]
        public void CommandSelectDifficulty(Player src, string[] args)
        {
            try
            {
                int difficulty = int.Parse(args[0].Trim());

                TriggerEvent("escape_ls:DifficultySelected", int.Parse(src.Handle.Trim()), difficulty);
            }
            catch
            {
                Debug.WriteLine("Invalid difficulty entered");
                return;
            }
        }

        [Command("restart_lobby")]
        public void CommandRestartLobby(Player src)
        {
            try
            {
                TriggerEvent("escape_ls:LobbyRestarted", int.Parse(src.Handle.Trim()));
            }
            catch
            {
                Debug.WriteLine("Failed to restart lobby");
                return;
            }
        }
    }
}
