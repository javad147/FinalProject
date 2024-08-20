using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Models;

public static class UserManagerExtensions
{
    public static async Task<AppUser> FindByPhoneNumberAsync(this UserManager<AppUser> userManager, string phoneNumber)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }
    public static async Task<AppUser> FindByPhoneNumberAsync(this UserManager<AppUser> userManager, string phoneNumber,string customerId)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Id != customerId);
    }

    public static async Task<AppUser> FindByEmailAsync(this UserManager<AppUser> userManager, string email, string customerId)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email && u.Id != customerId);
    }
}