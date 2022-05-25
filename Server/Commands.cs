using CitizenFX.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static CitizenFX.Core.Native.API;

namespace escape_ls.Server
{
    public class Commands : BaseScript
    {
        private string _databasePath { get; set; }
        private SQLiteAsyncConnection _connection { get; set; }
        public Commands()
        {
            _databasePath = Path.Combine(Directory.GetCurrentDirectory(), "escapeDB.db");

            this._connection = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        }

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

        [Command("leaderboard")]
        public async void CommandShowLeaderboard(Player src)
        {
            try
            {
                List<DBPlayer> dbPlayers = await DBPlayer.GetDBPlayerByIdentifierAsync(_connection, src.Identifiers["fivem"]);

                if (dbPlayers.Count < 1)
                    return;

                List<EscapeRun> escapeRuns = await EscapeRun.GetFastestEscapeRunAsync(_connection);

                if (escapeRuns.Count < 1)
                    return;

                src.TriggerEvent("chat:addMessage", new
                {
                    color = new int[] { 255, 255, 255 },
                    multiline = false,
                    args = new[] { "Gamemode", $"The current record is set by ^2{escapeRuns[0].DBPlayerId}^7, with a time to escape of ^*^1{escapeRuns[0].TimeTaken} seconds^7 at difficulty ^*^1{escapeRuns[0].Difficulty}" },
                });
            }
            catch
            {
                Debug.WriteLine("Failed to show leaderboard");
                return;
            }
        }
    }
}
