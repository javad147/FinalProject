namespace TamVaxti.Models
{
    public class Position : BaseEntity
    {
        public string PostionName { get; set; }
        public ICollection<Expert> Experts { get; set; }
    }
}
