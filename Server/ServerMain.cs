using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Server
{
    public class ServerMain : BaseScript
    {
        public EscapePlayerList escapePlayerList = new EscapePlayerList();

        public ServerMain()
        {
            Debug.WriteLine($"Escape Los Santos {Helpers.GetAssemblyVersion()} server loaded");
            EventHandlers["playerJoining"] += new Action<Player, string>(OnPlayerJoining);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
        }

        private void OnPlayerJoining([FromSource] Player player, string oldId)
        {
            Debug.WriteLine($"Player {player.Name} joined.");
            EscapePlayer escapePlayer = new EscapePlayer(player);
            escapePlayerList.Add(escapePlayer);

            EscapePlayer joinedPlayer = escapePlayerList.FindPlayer(player);
            Debug.WriteLine($"{joinedPlayer.Player.Name} added to EscapePlayerList");
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Debug.WriteLine($"Player {player.Name} dropped (Reason: {reason}).");
            EscapePlayer droppedPlayer = escapePlayerList.FindPlayer(player);
            if (escapePlayerList.Remove(droppedPlayer))
                Debug.WriteLine($"{player.Name} dropped from EscapePlayerList");
        }


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
            } catch {
                Debug.WriteLine("Invalid coordinate arguments");
                return;
            }
        }

/*        [Tick]
        public Task OnTick()
        {
            foreach (Player p in Players)
            {
                if (escapePlayerList.FindPlayer(p) == default(EscapePlayer))
                {
                    escapePlayerList.Add(new EscapePlayer(p));
                }
            }

            foreach (EscapePlayer escapePlayer in escapePlayerList)
            {
                if (string.IsNullOrEmpty(escapePlayer.Player.Name))
                {
                    escapePlayerList.Remove(escapePlayer);
                    break;
                }
            }

            return Task.FromResult(0);
        }*/

        [Tick]
        public Task CheckPlayersEscaped()
        {
            foreach (Player p in Players)
            {
                if (p.Character == null)
                    continue;

                if ((Math.Abs(p.Character.Position.X) > 5000.0 || Math.Abs(p.Character.Position.Y) > 8000.0 || p.Character.Position.Y < -5000.0) && escapePlayerList.FindPlayer(p).HasEscaped == false)
                {
                    escapePlayerList.FindPlayer(p).HasEscaped = true;
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new int[] { 255, 255, 255 },
                        multiline = false,
                        args = new[] { "Server", $"{p.Name} has escaped Los Santos!" },
                    });
                }
            }

            return Delay(100);
        }

        [Tick]
        public Task PlayerWantedSweep()
        {
            foreach (Player p in Players)
            {
                if (p.Character == null)
                    continue;

                if (escapePlayerList.FindPlayer(p).HasEscaped == false && GetPlayerWantedLevel(p.Handle) < 1)
                {
                    SetPlayerWantedLevel(p.Handle, 2, false);
                    p.TriggerEvent("escape_ls:setWantedLevel", 2);
                }
            }

            return Delay(1000);
        }
    }
}