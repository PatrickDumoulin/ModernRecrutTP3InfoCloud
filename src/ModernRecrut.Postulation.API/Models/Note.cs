using ModernRecrut.Postulation.API.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModernRecrut.Postulation.API.Models
{
    public class Note : BaseEntity
    {
        public string NoteDetail {  get; set; }
        public string NomEmeteur { get; set; }

        // FK postulation
        [ForeignKey("Postulation")]
        public int PostulationId { get; set; }

        [JsonIgnore]
        public virtual Postulation? Postulation { get; set; }
    }
}
