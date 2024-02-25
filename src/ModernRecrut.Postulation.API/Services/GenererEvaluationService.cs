using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Services
{
    public class GenererEvaluationService : IGenererEvaluationService
    {
        public Note GenererEvaluation(decimal pretentionSalariale) 
        {
            string appreciation;

            if (pretentionSalariale < 20000m)
                appreciation = "Salaire inférieur à la norme";
            else if (20000m <= pretentionSalariale && pretentionSalariale <= 39999m)
                appreciation = "Salaire proche mais inférieur à la norme";
            else if (40000m <= pretentionSalariale && pretentionSalariale <= 79999m)
                appreciation = "Salaire dans la norme";
            else if (80000m <= pretentionSalariale && pretentionSalariale <= 99999m)
                appreciation = "Salaire proche mais supérieur à la norme";
            else
                appreciation = "Salaire supérieur à la norme";
           
            Note note = new Note()
            {
                NomEmeteur = "ApplicationPostulation",
                NoteDetail = appreciation
            };

            return note;
        }
    }
}
