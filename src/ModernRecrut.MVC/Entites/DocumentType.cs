using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModernRecrut.MVC.Models;


namespace ModernRecrut.MVC.Entites
{
        public enum DocumentType
        {
            CV = 1,
            LettreDeMotivation = 2,
            Diplome = 3
        }
}
