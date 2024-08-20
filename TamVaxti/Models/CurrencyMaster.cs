using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class CurrencyMaster
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
    }
}
