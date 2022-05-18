﻿using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Server
{
    public class Commands : BaseScript
    {
        [Command("spawn_car")]
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

        [Command("lobby")]
        public void CommandSelectLobby(Player src, string[] args)
        {
            try
            {
                int lobby = int.Parse(args[0].Trim());

                Debug.WriteLine($"Player: {src.Name} - {lobby}");

                TriggerEvent("escape_ls:LobbySelected", int.Parse(src.Handle.Trim()), lobby);
            } catch
            {
                Debug.WriteLine("Invalid lobby entered");
                return;
            }
        }

        [Command("teleport")]
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
    }
}