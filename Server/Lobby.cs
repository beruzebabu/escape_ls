using System;
using System.Collections.Generic;
using System.Text;

namespace escape_ls.Server
{
    public class Lobby
    {
        public int Id { get; set; }
        public int Difficulty { get; set; }

        public Lobby(int id, int difficulty)
        {
            this.Id = id;
            this.Difficulty = difficulty;
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
