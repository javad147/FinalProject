namespace TamVaxti.ViewModels.Enquiry
{
    public class EnquiryVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IsReadStatus { get; set; }

        public string ShortMessage
        {
            get
            {
                if (string.IsNullOrEmpty(Message))
                {
                    return string.Empty;
                }
                return Message.Length >= 20 ? Message.Substring(0, 20) + "..." : Message;
            }
        }

    }
}
