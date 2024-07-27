namespace TamVaxti.Models
{
    public class UserWishList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } // Navigation property
    }

}
