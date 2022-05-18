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
            EventHandlers["escape_ls:LobbySelected"] += new Action<int, int>(OnLobbySelected);
        }

        private void OnPlayerJoining([FromSource] Player player, string oldId)
        {
            Debug.WriteLine($"Player {player.Name} joined.");
            EscapePlayer escapePlayer = new EscapePlayer(player);
            escapePlayerList.Add(escapePlayer);

            EscapePlayer joinedPlayer = escapePlayerList.FindPlayer(player);
            Debug.WriteLine($"{joinedPlayer.Player.Name} added to EscapePlayerList");

            player.TriggerEvent("escape_ls:toggleJoinScreen");
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Debug.WriteLine($"Player {player.Name} dropped (Reason: {reason}).");
            EscapePlayer droppedPlayer = escapePlayerList.FindPlayer(player);
            if (escapePlayerList.Remove(droppedPlayer))
                Debug.WriteLine($"{player.Name} dropped from EscapePlayerList");
        }

        private void OnLobbySelected(int playerId, int lobby)
        {
            if (lobby > 0)
            {
                try
                {
                    Player player = Players[playerId];
                    EscapePlayer ep = escapePlayerList.FindPlayer(player);
                    if (ep.Lobby < 0)
                    {
                        ep.Lobby = lobby;
                        SetPlayerRoutingBucket(player.Handle, lobby);

                        Debug.WriteLine($"{player.Name} set lobby to {lobby}");

                        player.TriggerEvent("escape_ls:toggleJoinScreen");
                    }
                } catch
                {
                    Debug.WriteLine($"Couldn't assign lobby {lobby} for playerid {playerId}");
                }
            }
        }

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