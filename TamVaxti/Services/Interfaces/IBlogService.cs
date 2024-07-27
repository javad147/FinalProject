using TamVaxti.Models;
using TamVaxti.ViewModels.Blogs;

namespace TamVaxti.Services.Interfaces
{
    public interface IBlogService
    {
        Task<List<Blog>> GetAllAsync();
        Task<List<BlogVM>> GetAllOrderByAsync();
        Task<List<Blog>> TakeAsync();
        Task<List<Blog>> SkipTakeAsync(int skipCount);
        Task<int> BlogCountAsync();
        Task<Blog> GetByIdAsync(int id);
        Task<bool> ExistAsync(string name);
        Task CreateAsync(BlogCreateVM blogCreateVM);
        Task DeleteAsync(int id);
        Task UpdateViewCountAsync(int id);
        Task EditAsync(Blog blog, BlogEditVM blogEdit);
        Task AddCommentAsync(Comment comment);
        Task<int> GetCommentCountAsync(int blogId);
        Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(int blogId);
        Task<List<Blog>> GetPopularBlogsAsync(int count);
        Task<List<Blog>> GetRecentBlogsAsync(int count);
    }
}
