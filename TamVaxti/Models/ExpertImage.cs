namespace TamVaxti.Models
{
    public class ExpertImage : BaseEntity
    {
        public string ExpertImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
        public int ExpertId { get; set; }
        public Expert Expert { get; set; }
    }
}
