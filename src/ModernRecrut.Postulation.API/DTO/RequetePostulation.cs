namespace ModernRecrut.Postulation.API.DTO
{
    public class RequetePostulation
    {
        public string CandidatId { get; set; }
        public int OffreDemploiId { get; set; }
        public decimal PretentionSalariale { get; set; }
        public DateTime DateDisponibilite { get; set; }
    }
}
