using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ModernRecrut.Postulation.API.Services
{
    public class PostulationsService : IPostulationsService
    {
        private readonly IAsyncRepository<Models.Postulation> _postulationRepository;
        private readonly IGenererEvaluationService _genrateEvaluationService;
        private readonly INotesService _notesService;

        public PostulationsService(
                IAsyncRepository<Models.Postulation> postulationRepository,
                IGenererEvaluationService genererEvaluationService,
                INotesService notesService)
        {
            _postulationRepository = postulationRepository;
            _genrateEvaluationService = genererEvaluationService;
            _notesService = notesService;
        }

        public async Task<Models.Postulation?> Ajouter(RequetePostulation requetePostulation)
        {
            Models.Postulation postulationAAjouter = new Models.Postulation()
            {
                CandidatId = requetePostulation.CandidatId,
                OffreDEmploiId = requetePostulation.OffreDemploiId,
                PretentionSalariale = requetePostulation.PretentionSalariale,
                DateDisponibilite = requetePostulation.DateDisponibilite
            };
            // Postulation existante
            var postulations = await ObtenirTout(); 
            if (postulations.Any(p => p.CandidatId == requetePostulation.CandidatId && p.OffreDEmploiId == requetePostulation.OffreDemploiId))
                return null;

            Models.Postulation postulation= await _postulationRepository.AddAsync(postulationAAjouter);

            // Ajouter une note
            Note note = _genrateEvaluationService.GenererEvaluation(postulation.PretentionSalariale);
            RequeteNote requeteNote = new RequeteNote()
            {
                NoteDetail = note.NoteDetail,
                NomEmeteur = note.NomEmeteur,
                PostulationId = postulation.Id
            };

            Note notePostulation = await _notesService.Ajouter(requeteNote);

            return postulation;
        }

        public async Task<bool> Modifier(Models.Postulation postulation)
        {
            // Postulation existante
            Models.Postulation postulationExistante = await _postulationRepository.GetByIdAsync(postulation.Id);
            if (postulationExistante == null)
                return false;

            // Check si modification de PretentionSalariale 
            if(postulationExistante.PretentionSalariale != postulation.PretentionSalariale)
            {
                IEnumerable<Note> notes = await _notesService.ObtenirTout();
                Note noteAModifier = notes.FirstOrDefault(n => n.PostulationId == postulation.Id && n.NomEmeteur == "ApplicationPostulation");

                if(noteAModifier != null)
                {
                    Note note = _genrateEvaluationService.GenererEvaluation(postulation.PretentionSalariale);
                    if (noteAModifier.NoteDetail != note.NoteDetail)
                    {
                        noteAModifier.NoteDetail = note.NoteDetail;
                        await _notesService.Modifier(noteAModifier);
                    }
                }
            }

            await _postulationRepository.EditAsync(postulation);

            return true;
        }

        public async Task<Models.Postulation?> ObtenirSelonId(int id)
        {
            return await _postulationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Models.Postulation>> ObtenirTout()
        {
            return await _postulationRepository.ListAsync();
        }

        public Task Supprimer(Models.Postulation postulation)
        {
            return _postulationRepository.DeleteAsync(postulation);
        }
    }
}
