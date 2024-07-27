namespace TamVaxti.Models
{
    public class Enquiry
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedOn { get; set; } // Nullable DateTime
        public DateTime? UpdatedOn { get; set; } // Nullable DateTime
        public bool IsRead { get; set; } // Bit type maps to boolean

    }
}
