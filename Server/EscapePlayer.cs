using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace escape_ls.Server
{
    public class EscapePlayer
    {
        public Player Player { get; }
        public bool HasEscaped { get; set; }
        public int Lobby { get; set; }

        public EscapePlayer(Player player)
        {
            this.Player = player;
            this.HasEscaped = false;
            this.Lobby = -1;
        }
    }

    public class EscapePlayerList : List<EscapePlayer>
    {
        public EscapePlayerList()
        {
        }

        public EscapePlayerList(IEnumerable<EscapePlayer> collection) : base(collection)
        {
        }

        public EscapePlayerList(int capacity) : base(capacity)
        {
        }

        public EscapePlayer FindPlayer(Player player)
        {
            return this.Find(ep => ep.Player == player);
        }
    }
}
