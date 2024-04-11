using Microsoft.AspNetCore.Http;
using Npgsql;
using CLOD.FavoriteUrl.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CLOD.FavoriteUrl.Services
{
    internal class UrlService : IUrlService
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = "Server=127.0.0.1;Port=5432;Database=blazor;User Id=edonis;Password=edonis19;";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InsertUrl(CLOD.FavoriteUrl.Models.FavoriteUrl favoriteUrl)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                // Gestisci il caso in cui l'utente non sia autenticato
                // Ad esempio, lanciando un'eccezione o restituendo un errore
                throw new Exception("Utente non autenticato.");
            }

            const string query = @"
                INSERT INTO public.favoritesurls (url, description, createduserid, createddate)
                VALUES (@Url, @Description, @CreatedUserId, @CreatedDate);
            ";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Url", favoriteUrl.Url);
            cmd.Parameters.AddWithValue("@Description", favoriteUrl.Description);
            cmd.Parameters.AddWithValue("@CreatedUserId", userId);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateUrl(CLOD.FavoriteUrl.Models.FavoriteUrl updatedFavoriteUrl)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                // Gestisci il caso in cui l'utente non sia autenticato
                // Ad esempio, lanciando un'eccezione o restituendo un errore
                throw new Exception("Utente non autenticato.");
            }

            const string query = @"
                UPDATE public.favoritesurls
                SET url = @Url, description = @Description ,editeddate = @EditDate ,editeduserid = @EditUserId
                WHERE id = @Id;
            ";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Url", updatedFavoriteUrl.Url);
            cmd.Parameters.AddWithValue("@Description", updatedFavoriteUrl.Description);
            cmd.Parameters.AddWithValue("@Id", updatedFavoriteUrl.Id);
            cmd.Parameters.AddWithValue("@EditUserId", userId);
            cmd.Parameters.AddWithValue("@EditDate", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<CLOD.FavoriteUrl.Models.FavoriteUrl> GetFavoriteById(int id)
        {
            const string query = @"
                SELECT id, url, description
                FROM public.favoritesurls
                WHERE id = @Id;
            ";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CLOD.FavoriteUrl.Models.FavoriteUrl
                {
                    Id = reader.GetInt32(0),
                    Url = reader.GetString(1),
                    Description = reader.GetString(2)
                };
            }

            return null; // Se non viene trovato nessun preferito con l'ID specificato
        }

        public async Task<List<CLOD.FavoriteUrl.Models.FavoriteUrl>> GetAllFavorites()
        {
            List<CLOD.FavoriteUrl.Models.FavoriteUrl> favorites = new List<CLOD.FavoriteUrl.Models.FavoriteUrl>();

            try
            {
                const string query = @"
            SELECT id, url, description
            FROM public.favoritesurls;
        ";

                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(query, connection);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    favorites.Add(new CLOD.FavoriteUrl.Models.FavoriteUrl
                    {
                        Id = reader.GetInt32(0),
                        Url = reader.GetString(1),
                        Description = reader.GetString(2)
                    });
                }
            }
            catch (Exception ex)
            {
                // Logga l'errore o visualizzalo per il debug
                Console.WriteLine($"Errore durante il recupero dei preferiti: {ex.Message}");
            }

            return favorites;
        }

        public async Task DeleteFavorite(int id)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                // Gestisci il caso in cui l'utente non sia autenticato
                // Ad esempio, lanciando un'eccezione o restituendo un errore
                throw new Exception("Utente non autenticato.");
            }

            const string query = @"
        DELETE FROM public.favoritesurls
        WHERE id = @Id AND createduserid = @UserId;
    ";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UserId", userId);

            await cmd.ExecuteNonQueryAsync();
        }


    }
}
