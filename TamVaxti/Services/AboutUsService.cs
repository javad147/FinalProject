using TamVaxti.Data;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.AboutUs;
using TamVaxti.ViewModels.Company;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class AboutUsService : IAboutUsService

    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public AboutUsService(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = appDbContext;
            _webHostEnvironment = webHostEnvironment;

        }

        public async Task CreateAsync(AboutUsCreateVM model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Handle About Us
                var about = new About
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedOn = DateTime.UtcNow,
                };

                if (model.ImageFile != null)
                {
                    about.Image = await SaveFile(model.ImageFile);
                }

                _context.About.Add(about);
                await _context.SaveChangesAsync();

                // Handle AboutUsHistory
                if (model.AboutHistory != null)
                {
                    foreach (var history in model.AboutHistory)
                    {
                        var historyEntry = new AboutUsHistory
                        {
                            AboutId = about.Id, // Set the foreign key
                            Title = history.Title,
                            Description = history.Description,
                            CreatedOn = DateTime.UtcNow,
                        };

                        if (history.ImageFile != null)
                        {

                            historyEntry.Image = await SaveFile(model.ImageFile);
                        }

                        _context.AboutUsHistory.Add(historyEntry);
                    }
                }

                // Handle AboutUsTeam
                if (model.AboutUsTeam != null)
                {
                    foreach (var team in model.AboutUsTeam)
                    {
                        var teamEntry = new AboutUsTeam
                        {
                            AboutId = about.Id, // Set the foreign key
                            Title = team.Title,
                            Role = team.Role,
                            CreatedOn = DateTime.UtcNow,
                        };

                        if (team.ImageFile != null)
                        {

                            teamEntry.Image = await SaveFile(model.ImageFile);

                        }

                        _context.AboutUsTeam.Add(teamEntry);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AboutUsCreateVM> GetAboutUsByIdAsync(int id)
        {
            var aboutUs = await _context.About
                .Include(a => a.AboutUsHistories)
                .Include(a => a.AboutUsTeams)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aboutUs == null)
            {
                return null;
            }

            var aboutUsCreateVM = new AboutUsCreateVM
            {
                Title = aboutUs.Title,
                Description = aboutUs.Description,
                ImageName = aboutUs.Image, // Set ImageName
                AboutHistory = aboutUs.AboutUsHistories.Select(h => new AboutUsHistoryVM
                {
                    Id = h.Id,
                    Title = h.Title,
                    Description = h.Description,
                    ImageName = h.Image // Set ImageName
                }).ToList(),
                AboutUsTeam = aboutUs.AboutUsTeams.Select(t => new AboutUsTeamVM
                {
                    Id = t.Id,
                    Title = t.Title,
                    Role = t.Role,
                    ImageName = t.Image // Set ImageName
                }).ToList()
            };

            return aboutUsCreateVM;
        }

        //public async Task UpdateAsync(int id, AboutUsCreateVM model)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // Handle About Us
        //        var about = await _context.Abouts.FindAsync(id);
        //        if (about == null)
        //        {
        //            throw new Exception("About Us not found.");
        //        }

        //        about.Title = model.Title;
        //        about.Description = model.Description;
        //        about.UpdatedOn = DateTime.UtcNow;

        //        if (model.ImageFile != null)
        //        {
        //            about.Image = await SaveFile(model.ImageFile);
        //        }

        //        _context.Abouts.Update(about);
        //        await _context.SaveChangesAsync();

        //        // Handle AboutUsHistory
        //        var existingHistories = _context.AboutUsHistory.Where(h => h.AboutId == id).ToList();

        //        // Remove histories that are no longer in the model
        //        var modelHistoryIds = model.AboutHistory.Where(h => h.Id > 0).Select(h => h.Id).ToList();
        //        var historiesToRemove = existingHistories.Where(e => !modelHistoryIds.Contains(e.Id)).ToList();
        //        _context.AboutUsHistory.RemoveRange(historiesToRemove);

        //        // Update existing histories and add new ones
        //        foreach (var history in model.AboutHistory)
        //        {
        //            if (history.Id > 0)
        //            {
        //                // Update existing history
        //                var existingHistory = existingHistories.FirstOrDefault(h => h.Id == history.Id);
        //                if (existingHistory != null)
        //                {
        //                    existingHistory.Title = history.Title;
        //                    existingHistory.Description = history.Description;

        //                    if (history.ImageFile != null)
        //                    {
        //                        existingHistory.Image = await SaveFile(history.ImageFile);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Add new history
        //                var historyEntry = new AboutUsHistory
        //                {
        //                    AboutId = about.Id, // Set the foreign key
        //                    Title = history.Title,
        //                    Description = history.Description,
        //                    CreatedOn = DateTime.UtcNow,
        //                };

        //                if (history.ImageFile != null)
        //                {
        //                    historyEntry.Image = await SaveFile(history.ImageFile);
        //                }

        //                _context.AboutUsHistory.Add(historyEntry);
        //            }
        //        }

        //        await _context.SaveChangesAsync();

        //        // Handle AboutUsTeam
        //        var existingTeams = _context.AboutUsTeam.Where(t => t.AboutId == id).ToList();

        //        // Remove teams that are no longer in the model
        //        var modelTeamIds = model.AboutUsTeam.Where(t => t.Id > 0).Select(t => t.Id).ToList();
        //        var teamsToRemove = existingTeams.Where(e => !modelTeamIds.Contains(e.Id)).ToList();
        //        _context.AboutUsTeam.RemoveRange(teamsToRemove);

        //        // Update existing teams and add new ones
        //        foreach (var team in model.AboutUsTeam)
        //        {
        //            if (team.Id > 0)
        //            {
        //                // Update existing team
        //                var existingTeam = existingTeams.FirstOrDefault(t => t.Id == team.Id);
        //                if (existingTeam != null)
        //                {
        //                    existingTeam.Title = team.Title;
        //                    existingTeam.Role = team.Role;

        //                    if (team.ImageFile != null)
        //                    {
        //                        existingTeam.Image = await SaveFile(team.ImageFile);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Add new team
        //                var teamEntry = new AboutUsTeam
        //                {
        //                    AboutId = about.Id, // Set the foreign key
        //                    Title = team.Title,
        //                    Role = team.Role,
        //                    CreatedOn = DateTime.UtcNow,
        //                };

        //                if (team.ImageFile != null)
        //                {
        //                    teamEntry.Image = await SaveFile(team.ImageFile);
        //                }

        //                _context.AboutUsTeam.Add(teamEntry);
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}


        public async Task<bool> UpdateAsync(int id, AboutUsCreateVM model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            bool result = true;
            try
            {
                // Handle About Us
                var about = await _context.About.FindAsync(id);
                if (about == null)
                {
                    throw new Exception("About Us not found.");
                }

                about.Title = model.Title;
                about.Description = model.Description;
                about.UpdatedOn = DateTime.Now;

                // If new file is uploaded and an existing URL already exists, delete the existing and create the new one.
                if (model.ImageFile != null)
                {
                    if (model.ImageName != null)
                    {
                        RemoveFile(model.ImageName);
                    }
                    model.ImageName = await SaveFile(model.ImageFile);
                }

                about.Image = model.ImageName;
                _context.About.Update(about);
                await _context.SaveChangesAsync();

                // Handle AboutUsHistory
                var existingHistories = _context.AboutUsHistory.Where(h => h.AboutId == id).ToList();

                foreach (var history in model.AboutHistory ?? new List<AboutUsHistoryVM>())
                {
                    var historyEntry = existingHistories.FirstOrDefault(h => h.Id == history.Id);

                    if (historyEntry == null)
                    {
                        // Add new history entry
                        historyEntry = new AboutUsHistory
                        {
                            AboutId = about.Id,
                            Title = history.Title,
                            Description = history.Description,
                            CreatedOn = DateTime.Now,
                        };

                        if (history.ImageFile != null)
                        {
                            historyEntry.Image = await SaveFile(history.ImageFile);
                        }

                        _context.AboutUsHistory.Add(historyEntry);
                    }
                    else
                    {
                        // Update existing history entry
                        historyEntry.Title = history.Title;
                        historyEntry.Description = history.Description;
                        historyEntry.UpdatedOn = DateTime.Now;

                        if (history.ImageFile != null)
                        {
                            historyEntry.Image = await SaveFile(history.ImageFile);
                        }

                        _context.AboutUsHistory.Update(historyEntry);
                    }
                }

                // Remove histories that are not in the model
                var historyIds = model.AboutHistory?.Select(h => h.Id).ToList() ?? new List<int>();
                var historiesToRemove = existingHistories.Where(h => !historyIds.Contains(h.Id)).ToList();
                _context.AboutUsHistory.RemoveRange(historiesToRemove);

                // Handle AboutUsTeam
                var existingTeams = _context.AboutUsTeam.Where(t => t.AboutId == id).ToList();

                if (model.AboutUsTeam != null)
                {
                    foreach (var team in model.AboutUsTeam)
                    {
                        var teamEntry = existingTeams.FirstOrDefault(t => t.Id == team.Id);

                        if (teamEntry == null)
                        {
                            // Add new team entry
                            teamEntry = new AboutUsTeam
                            {
                                AboutId = about.Id,
                                Title = team.Title,
                                Role = team.Role,
                                CreatedOn = DateTime.Now,
                            };

                            if (team.ImageFile != null)
                            {
                                teamEntry.Image = await SaveFile(team.ImageFile);
                            }

                            _context.AboutUsTeam.Add(teamEntry);
                        }
                        else
                        {
                            // Update existing team entry
                            teamEntry.Title = team.Title;
                            teamEntry.Role = team.Role;
                            teamEntry.UpdatedOn = DateTime.Now;

                            if (team.ImageFile != null)
                            {
                                teamEntry.Image = await SaveFile(team.ImageFile);
                            }

                            _context.AboutUsTeam.Update(teamEntry);
                        }
                    }

                    // Remove teams that are not in the model
                    var teamIds = model.AboutUsTeam.Select(t => t.Id).ToList();
                    var teamsToRemove = existingTeams.Where(t => !teamIds.Contains(t.Id)).ToList();
                    _context.AboutUsTeam.RemoveRange(teamsToRemove);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result = false;

                // Log the exception or handle it as needed
                //throw new Exception("An error occurred while updating records.", ex);
            }
            
            return result;
        }



        public List<AboutUsVM> GetAllAboutUs()
        {
            return _context.About
                .Select(a => new AboutUsVM
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Image = a.Image,
                }).ToList();
        }


        private async Task<string> SaveFile(IFormFile file)
        {
            var fileName = "";
            try
            {
                // Write Upload code over here.
                if (file != null)
                {
                    fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "about", fileName);
                    await FileExtensions.SaveFileToLocalAsync(file, filePath);
                }

            }
            catch { }
            return fileName;
        }

        private bool RemoveFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "about", fileName);
                FileExtensions.DeleteFileFromLocal(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //public AboutUsVM GetAboutUsById(int id)
        //{
        //    var aboutUs = _context.AboutUs.Find(id);
        //    if (aboutUs == null) return null;

        //    return new AboutUsVM
        //    {
        //        Id = aboutUs.Id,
        //        Name = aboutUs.Name,
        //        Description = aboutUs.Description,
        //        ImageUrl = aboutUs.ImageUrl,
        //        CreatedOn = aboutUs.CreatedOn,
        //        UpdatedOn = aboutUs.UpdatedOn
        //    };
        //}

        //public void AddAboutUs(AboutUsVM model)
        //{
        //    var aboutUs = new AboutUs
        //    {
        //        Name = model.Name,
        //        Description = model.Description,
        //        ImageUrl = model.ImageUrl,
        //        CreatedOn = DateTime.Now,
        //        UpdatedOn = DateTime.Now
        //    };

        //    _context.AboutUs.Add(aboutUs);
        //    _context.SaveChanges();
        //}
        //public void UpdateAboutUs(AboutUsVM model)
        //{
        //    var aboutUs = _context.AboutUs.Find(model.Id);
        //    if (aboutUs == null) return;

        //    aboutUs.Name = model.Name;
        //    aboutUs.Description = model.Description;
        //    aboutUs.ImageUrl = model.ImageUrl;
        //    aboutUs.UpdatedOn = DateTime.Now;

        //    _context.SaveChanges();
        //}

        //public void DeleteAboutUs(int id)
        //{
        //    var aboutUs = _context.AboutUs.Find(id);
        //    if (aboutUs == null) return;

        //    _context.AboutUs.Remove(aboutUs);
        //    _context.SaveChanges();
        //}
    }

}
