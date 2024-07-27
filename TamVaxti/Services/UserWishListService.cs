using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services;

public class UserWishListService : IUserWishList
{
    private readonly AppDbContext _context;

    public UserWishListService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddToWishList(UserWishList userWishList)
    {
        await _context.UserWishList.AddAsync(userWishList);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IList<UserWishList>> GetUserSavedWishList(string userId)
    {
        var value = await _context.UserWishList
            .Include(uwl => uwl.Product) // Include product details
            .Where(uwl => uwl.UserId == userId)
            .ToListAsync();
        return value;
    }

    public async Task<IList<int>> GetUserSavedWishListProducts(string userId)
    {
        var value = (await _context.UserWishList.Where(uwl => uwl.UserId == userId).ToListAsync())
            .DistinctBy(x => x.ProductId).Select(x => x.ProductId)
            .ToArray();
        return value;
    }

    public async Task<bool> RemoveProductFromWishList(UserWishList userWishList)
    {
       var product  = await _context.UserWishList.FirstAsync(x=>x.ProductId == userWishList.ProductId && x.UserId == userWishList.UserId);
        _context.UserWishList.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> WishListExists(UserWishList userWishList)
    {
        return await _context.UserWishList.AnyAsync(x =>
            x.UserId == userWishList.UserId && x.ProductId == userWishList.ProductId);
    }
}