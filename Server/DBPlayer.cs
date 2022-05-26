using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace escape_ls.Server
{
    public class DBPlayer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string Identifier { get; set; }

        public static async Task<List<DBPlayer>> GetDBPlayersAsync(SQLiteAsyncConnection _connection)
        {
            return await _connection.QueryAsync<DBPlayer>("SELECT * FROM DBPlayer");
        }

        public static async Task<List<DBPlayer>> GetDBPlayerByIdAsync(SQLiteAsyncConnection _connection, int id)
        {
            return await _connection.QueryAsync<DBPlayer>("SELECT * FROM DBPlayer WHERE Id = ?", id);
        }

        public static async Task<List<DBPlayer>> GetDBPlayerByIdentifierAsync(SQLiteAsyncConnection _connection, string identifier)
        {
            return await _connection.QueryAsync<DBPlayer>("SELECT * FROM DBPlayer WHERE Identifier = ?", identifier);
        }
    }
}
