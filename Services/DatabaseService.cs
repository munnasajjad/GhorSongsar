using GhorSongsar.Models;
using SQLite;

namespace GhorSongsar.Services;

public class DatabaseService
{
    private const string DbName = "ghorsongsar.db3";

    private SQLiteAsyncConnection? _connection;

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
            return _connection;

        var path = Path.Combine(FileSystem.AppDataDirectory, DbName);
        var connection = new SQLiteAsyncConnection(
            path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await connection.CreateTableAsync<Account>();
        await connection.CreateTableAsync<Transaction>();
        _connection = connection;
        return connection;
    }

    public async Task<Account?> GetAccountAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Account>().FirstOrDefaultAsync();
    }

    public async Task SaveAccountAsync(Account account)
    {
        var db = await GetConnectionAsync();
        await db.InsertOrReplaceAsync(account);
    }

    public async Task InsertTransactionAsync(Transaction transaction)
    {
        var db = await GetConnectionAsync();
        await db.InsertAsync(transaction);
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        var db = await GetConnectionAsync();
        await db.UpdateAsync(transaction);
    }

    public async Task DeleteTransactionAsync(Transaction transaction)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAsync(transaction);
    }

    public async Task<Transaction?> GetTransactionAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.Table<Transaction>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Transaction>> GetTransactionsAsync(DateTime from, DateTime to)
    {
        var db = await GetConnectionAsync();
        return await db.Table<Transaction>()
            .Where(t => t.Date >= from && t.Date < to)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetRecentAsync(int count)
    {
        var db = await GetConnectionAsync();
        return await db.Table<Transaction>()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Take(count)
            .ToListAsync();
    }
}