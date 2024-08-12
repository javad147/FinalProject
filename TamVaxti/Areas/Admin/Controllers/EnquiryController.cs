using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Enquiry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class EnquiryController : Controller
    {
    

        private readonly AppDbContext _context;
        

        public EnquiryController(AppDbContext context)
        {
            _context = context;
           
        }


        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            // Store the current search and status filter in ViewData to retain them on the view.
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = statusFilter;

            // Query to get all enquiries from the database.
            var enquiries = from e in _context.Enquiries
                            select e;

            // Apply search filter if searchString is provided.
            if (!string.IsNullOrEmpty(searchString))
            {
                enquiries = enquiries.Where(e => e.FirstName.Contains(searchString)
                                               || e.LastName.Contains(searchString)
                                               || e.Phone.Contains(searchString)
                                               || e.EmailId.Contains(searchString)
                                               || e.Message.Contains(searchString));
            }

            // Apply status filter if statusFilter is provided.
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isRead = statusFilter == "Read";
                enquiries = enquiries.Where(e => e.IsRead == isRead);
            }

            // Project the filtered and searched enquiries into EnquiryVM objects.
            var enquiryVMs = await enquiries
                .Select(e => new EnquiryVM
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Phone = e.Phone,
                    EmailId = e.EmailId,
                    Message = e.Message,
                    IsReadStatus = e.IsRead ? "Read" : "Unread"
                }).OrderByDescending(x => x.Id)
                .ToListAsync();

            // Return the view with the list of EnquiryVM objects.
            return View(enquiryVMs);
        }


        [HttpGet]
        public JsonResult GetEnquiryDetails(int enquiryId)
        {
            var enquiry = _context.Enquiries
                .Where(e => e.Id == enquiryId)
                .Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.Phone,
                    e.EmailId,
                    e.Message,
                    IsReadStatus = e.IsRead ? "Read" : "Unread"
                })
                .FirstOrDefault();

            if (enquiry != null)
            {
                return Json(new { success = true, data = enquiry });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public JsonResult UpdateEnquiryStatus(int enquiryId, string isReadStatus)
        {
            var enquiry = _context.Enquiries.Find(enquiryId);

            if (enquiry != null)
            {
                enquiry.IsRead = isReadStatus == "Read";
                enquiry.UpdatedOn = DateTime.Now; 
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


    }
}
