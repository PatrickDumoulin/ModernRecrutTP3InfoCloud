using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Models
{
    public class Note
    {
		[JsonPropertyName("id")]
		public int Id { get; set; }
        public string NoteDetail {  get; set; }
        public string NomEmeteur { get; set; }

        // FK postulation
        [ForeignKey("Postulation")]
        public int PostulationId { get; set; }

        [JsonIgnore]
        public virtual Postulation? Postulation { get; set; }
    }
}
