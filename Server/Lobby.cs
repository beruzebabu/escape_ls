using System;
using System.Collections.Generic;
using System.Text;

namespace escape_ls.Server
{
    public class Lobby
    {
        public int Id { get; set; }
        public int Difficulty { get; set; }
        public EscapePlayer Creator { get; set; }
        public EscapePlayerList LobbyPlayers { get; set; }
        public DateTime Timestamp { get; set; }

        public Lobby(int id, int difficulty, EscapePlayer creator)
        {
            this.Id = id;
            this.Difficulty = difficulty;
            this.Creator = creator;
            this.LobbyPlayers = new EscapePlayerList();
            this.LobbyPlayers.Add(creator);
            this.Timestamp = DateTime.Now;
        }
    }

    public class LobbyList : List<Lobby>
    {
        public Lobby FindLobbyById(int id)
        {
            return this.Find(l => l.Id == id);
        }
    }
}
