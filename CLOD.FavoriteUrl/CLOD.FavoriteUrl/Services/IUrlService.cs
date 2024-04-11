
namespace CLOD.FavoriteUrl.Services
{
    public interface IUrlService
    {

        Task InsertUrl(CLOD.FavoriteUrl.Models.FavoriteUrl favoriteUrl);
        Task UpdateUrl(CLOD.FavoriteUrl.Models.FavoriteUrl updatedFavoriteUrl);
        Task<CLOD.FavoriteUrl.Models.FavoriteUrl> GetFavoriteById(int id);

        Task<List<CLOD.FavoriteUrl.Models.FavoriteUrl>> GetAllFavorites();
        Task DeleteFavorite(int id);
    }
}

