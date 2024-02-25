using ModernRecrut.Postulation.API.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModernRecrut.Postulation.API.Models
{
    public class Postulation : BaseEntity
    {
        public string CandidatId {  get; set; }
        public int OffreDEmploiId { get; set; }
        public decimal PretentionSalariale { get; set; }
        public DateTime DateDisponibilite { get; set; }

        [JsonIgnore]
        public virtual ICollection<Note>? Notes { get; set; }
    }
}
