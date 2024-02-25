using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernRecrut.Documents.API.Entites
{
    [Table("DocumentDetails")]
    public class DocumentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string ID { get; set; }

        public string? DocumentName { get; set; }
        public byte[]? DocumentData { get; set; }
        public  DocumentType DocumentType {get; set;}
    }
}
