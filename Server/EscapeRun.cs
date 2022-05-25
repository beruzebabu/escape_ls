using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace escape_ls.Server
{
    public class EscapeRun
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int DBPlayerId { get; set; }
        public double TimeTaken { get; set; }
        public int Difficulty { get; set; }

        public static async Task<List<EscapeRun>> GetEscapeRunsAsync(SQLiteAsyncConnection _connection)
        {
            return await _connection.QueryAsync<EscapeRun>("SELECT * FROM EscapeRun");
        }

        public static async Task<List<EscapeRun>> GetFastestEscapeRunAsync(SQLiteAsyncConnection _connection)
        {
            return await _connection.QueryAsync<EscapeRun>("SELECT * FROM EscapeRun ORDER BY TimeTaken ASC LIMIT 1");
        }

        public static async Task<List<EscapeRun>> GetEscapeRunsByDBPlayerAsync(SQLiteAsyncConnection _connection, DBPlayer dbPlayer)
        {
            return await _connection.QueryAsync<EscapeRun>("SELECT * FROM EscapeRun WHERE DBPlayerId = ?", dbPlayer.Id);
        }

        public static async Task<List<EscapeRun>> GetFastestEscapeRunByDBPlayerAsync(SQLiteAsyncConnection _connection, DBPlayer dbPlayer)
        {
            return await _connection.QueryAsync<EscapeRun>("SELECT * FROM EscapeRun WHERE DBPlayerId = ? ORDER BY TimeTaken ASC LIMIT 1", dbPlayer.Id);
        }
    }
}
