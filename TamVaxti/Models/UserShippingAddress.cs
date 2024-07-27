using TamVaxti.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserShippingAddress
{
    [Key]
    public int ShippingAddressId { get; set; }

    [ForeignKey("AspNetUsers")]
    public string UserId { get; set; }

    public string Flat { get; set; }

    [Required]
    [StringLength(255)]
    public string Address { get; set; }

    [Required]
    [StringLength(20)]
    public string ZipCode { get; set; }

    [Required]
    [StringLength(100)]
    public string Country { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; }

    [Required]
    [StringLength(100)]
    public string Region { get; set; }
    public AppUser User { get; set; }
}
