using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Blogs;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace TamVaxti.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;
        public BlogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> BlogCountAsync()
        {
            return await _context.Blogs.CountAsync();
        }



        public async Task CreateAsync(BlogCreateVM blogCreateVM)
        {
            await _context.Blogs.AddAsync(new Blog { Title = blogCreateVM.Title, Description = blogCreateVM.Description, Image = blogCreateVM.Image ,Uname= blogCreateVM.Uname,Date = blogCreateVM.Date });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blog = await GetByIdAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(Blog blog, BlogEditVM blogEdit)
        {
            blog.Title = blogEdit.BlogTitle;
            blog.Description = blogEdit.BlogDescription;
            blog.Image = blogEdit.Image;

            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistAsync(string name)
        {
            return _context.Blogs.AnyAsync(m => m.Title == name);
        }

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _context.Blogs.Where(m => !m.SoftDeleted).ToListAsync();
        }
        public async Task UpdateViewCountAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.Hits++; // Increment Hits instead of ViewCount
                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Update comment count for the blog post
            var blog = await _context.Blogs.FindAsync(comment.BlogId);
            if (blog != null)
            {
                blog.CommentCount++;
                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCommentCountAsync(int blogId)
        {
            return await _context.Comments.CountAsync(c => c.BlogId == blogId);
        }
        public async Task<Comment> GetCommentById(int commentId)
        {
                return await _context.Comments.SingleOrDefaultAsync(m => m.Id == commentId);
        }
        public async Task<List<Comment>> GetCommentOfBlog(int blogId)
        {
            return await _context.Comments.Where(m => m.Status).ToListAsync();
        }
        public async Task<List<Comment>> GetAllCommentOfBlogs()
        {
            return await _context.Comments.Include(b=>b.Blog).ToListAsync();
        }
        public async Task UpdateComments(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(int blogId)
        {
            return await _context.Comments
                                 .Where(c => c.BlogId == blogId && c.Status)
                                 .ToListAsync();
        }

        public async Task<List<BlogVM>> GetAllOrderByAsync()
        {
            var blogs = await GetAllAsync();
            return blogs.Select(m => new BlogVM { Id = m.Id, Title = m.Title, Description = m.Description, Image=m.Image }).ToList().OrderByDescending(m => m.Id).ToList();
        }

        public async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs.Where(m => !m.SoftDeleted).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Blog>> SkipTakeAsync(int skipCount)
        {
            return await _context.Blogs.Skip(skipCount).Take(3).ToListAsync();
        }

        public async Task<List<Blog>> TakeAsync()
        {
            return await _context.Blogs
                                 .Where(m => !m.SoftDeleted)
                                 .OrderByDescending(m => m.Date)
                                 .Take(10)  // Adjust the number as needed
                                 .ToListAsync();
        }

        public async Task<List<Blog>> GetRecentBlogsAsync(int count)
        {
            return await _context.Blogs
                                 .Where(m => !m.SoftDeleted)
                                 .OrderByDescending(m => m.Date)
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<List<Blog>> GetPopularBlogsAsync(int count)
        {
            return await _context.Blogs
                                 .Where(m => !m.SoftDeleted)
                                 .OrderByDescending(m => m.Hits)
                                 .Take(count)
                                 .ToListAsync();
        }
    }

}

