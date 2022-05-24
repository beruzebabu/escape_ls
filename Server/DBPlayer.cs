using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace escape_ls.Server
{
    public class DBPlayer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string Identifier { get; set; }
    }
}
