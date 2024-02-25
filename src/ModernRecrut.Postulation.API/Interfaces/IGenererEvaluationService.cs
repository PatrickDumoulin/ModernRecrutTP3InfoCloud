using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface IGenererEvaluationService
    {
        public Note GenererEvaluation(decimal pretentionSalariale);
    }
}
