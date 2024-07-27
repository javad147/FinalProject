using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;

namespace TamVaxti.Services
{
    public class ContactUsService : IContactUsService
    {

        private readonly AppDbContext _context; // Assuming you're using Entity Framework

        public ContactUsService(AppDbContext context)
        {
            _context = context;
        }

        public void SaveEnquiry(Enquiry enquiry)
        {
            var maxLength = 500;

            if (enquiry.Message.Length > maxLength)
            {
                enquiry.Message = enquiry.Message.Substring(0, maxLength);
            }
            _context.Enquiries.Add(enquiry); // Add the enquiry to the database context
            _context.SaveChanges(); // Save changes to the database
        }
    }
}
