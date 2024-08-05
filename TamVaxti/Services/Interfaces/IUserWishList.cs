using TamVaxti.Models;

namespace TamVaxti.Services.Interfaces
{
    public interface IUserWishList
    {
        Task<bool> WishListExists(UserWishList userWishList);

        Task<bool> AddToWishList(UserWishList userWishList);

        Task<IList<UserWishList>> GetUserSavedWishList(string userId);

        Task<IList<long>> GetUserSavedWishListProducts(string userId);

        Task<bool> RemoveProductFromWishList(UserWishList userWishList);
    }
}
