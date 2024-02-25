using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Services
{
    public class NotesService : INotesService
    {
        private readonly IAsyncRepository<Note> _noteRepository;

        public NotesService(IAsyncRepository<Note> noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<Note> Ajouter(RequeteNote requeteNote)
        {
            Note note = new Note()
            {
                NoteDetail = requeteNote.NoteDetail,
                NomEmeteur = requeteNote.NomEmeteur,
                PostulationId = requeteNote.PostulationId,
            };
            return await _noteRepository.AddAsync(note);
        }

        public async Task<bool> Modifier(Note note)
        {
            await _noteRepository.EditAsync(note);
            return true;
        }

        public async Task<Note> ObtenirSelonId(int id)
        {
            return await _noteRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Note>> ObtenirTout()
        {
            return await _noteRepository.ListAsync();
        }

        public Task Supprimer(Note note)
        {
            return _noteRepository.DeleteAsync(note);
        }
    }
}
