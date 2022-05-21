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
        public LobbyList lobbies = new LobbyList();

        public ServerMain()
        {
            Debug.WriteLine($"Escape Los Santos {Helpers.GetAssemblyVersion()} server loaded");
            EventHandlers["playerJoining"] += new Action<Player, string>(OnPlayerJoining);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            EventHandlers["escape_ls:LobbySelected"] += new Action<int, int>(OnLobbySelected);
            EventHandlers["escape_ls:DifficultySelected"] += new Action<int, int>(OnDifficultySelected);
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
                    if (ep.Lobby == default(Lobby))
                    {
                        Lobby existingLobby = lobbies.FindLobbyById(lobby);
                        if (existingLobby == default(Lobby))
                        {
                            Lobby newLobby = new Lobby(lobby, -1);
                            ep.Lobby = newLobby;
                            lobbies.Add(newLobby);

                            TriggerClientEvent("chat:addMessage", new
                            {
                                color = new int[] { 255, 255, 255 },
                                multiline = false,
                                args = new[] { "Gamemode", $"Lobby set to ^2{lobby}^7, ^*please set a difficulty by typing ^3/difficulty <number>" },
                            });
                        } else
                        {
                            ep.Lobby = existingLobby;

                            TriggerClientEvent("chat:addMessage", new
                            {
                                color = new int[] { 255, 255, 255 },
                                multiline = false,
                                args = new[] { "Gamemode", $"Lobby set to ^2{lobby}^7, ^*difficulty is {existingLobby.Difficulty}" },
                            });

                            player.TriggerEvent("escape_ls:toggleJoinScreen");
                            player.TriggerEvent("escape_ls:startEscape");
                        }

                        SetPlayerRoutingBucket(player.Handle, lobby);

                        Debug.WriteLine($"{player.Name} set lobby to {lobby}");
                    }
                } catch
                {
                    Debug.WriteLine($"Couldn't assign lobby {lobby} for playerid {playerId}");
                }
            }
        }

        private void OnDifficultySelected(int playerId, int difficulty)
        {
            if (difficulty > 0)
            {
                try
                {
                    Player player = Players[playerId];
                    EscapePlayer ep = escapePlayerList.FindPlayer(player);
                    if (ep.Lobby != default(Lobby) && ep.Lobby.Difficulty < 0)
                    {
                        ep.Lobby.Difficulty = difficulty;

                        Debug.WriteLine($"{player.Name} set difficulty to {difficulty}");

                        player.TriggerEvent("escape_ls:toggleJoinScreen");
                        player.TriggerEvent("escape_ls:startEscape");
                    }
                }
                catch
                {
                    Debug.WriteLine($"Couldn't assign lobby difficulty for playerid {playerId}");
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
                        args = new[] { "Gamemode", $"{p.Name} has escaped Los Santos!" },
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

                EscapePlayer ep = escapePlayerList.FindPlayer(p);

                if (ep.HasEscaped == false 
                    && GetPlayerWantedLevel(p.Handle) < 1 
                    && ep.Lobby != default(Lobby) 
                    && ep.Lobby.Difficulty > 0)
                {
                    SetPlayerWantedLevel(p.Handle, ep.Lobby.Difficulty, false);
                    p.TriggerEvent("escape_ls:setWantedLevel", ep.Lobby.Difficulty);
                }
            }

            return Delay(1000);
        }
    }
}