using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Enquiry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class EnquiryController : Controller
    {
    

        private readonly AppDbContext _context;
        

        public EnquiryController(AppDbContext context)
        {
            _context = context;
           
        }


        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var enquiries = from e in _context.Enquiries
                            select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                enquiries = enquiries.Where(e => e.FirstName.Contains(searchString)
                                               || e.LastName.Contains(searchString)
                                               || e.Phone.Contains(searchString)
                                               || e.EmailId.Contains(searchString)
                                               || e.Message.Contains(searchString));
            }

            var enquiryVMs = await enquiries
                .Select(e => new EnquiryVM
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Phone = e.Phone,
                    EmailId = e.EmailId,
                    Message = e.Message,
                 
                    IsReadStatus = e.IsRead ?"Read" : "Unread"
                })
                .ToListAsync();

            return View(enquiryVMs);
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
