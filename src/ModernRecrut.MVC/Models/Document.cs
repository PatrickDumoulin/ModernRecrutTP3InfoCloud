using ModernRecrut.MVC.Entites;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models
{
    public class Document
    {
        [
            DisplayName("Fichier"),
            Required(ErrorMessage ="Le fichier est obligatoire")
        ]
        public IFormFile DocumentDetails { get; set; }

        [
            DisplayName("Type de document"),
            Required(ErrorMessage ="Le type de document est obligatoire")
        ]
        public DocumentType DocumentType { get; set; }
    }
}
