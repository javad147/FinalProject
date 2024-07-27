namespace TamVaxti.Models
{
    public class Expert : BaseEntity
    {
        public string FullName { get; set; }
        public string ExpertOpinion { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public ICollection<ExpertImage> ExpertImages { get; set; } 

    }
}
