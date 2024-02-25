using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Entites
{
    public class DocumentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        public string? DocumentName { get; set; }
        public byte[]? DocumentData { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
