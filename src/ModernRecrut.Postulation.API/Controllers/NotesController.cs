using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Postulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        // GET: api/<NotesController>
        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _notesService.ObtenirTout();
        }

        // GET api/<NotesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            Note note = await _notesService.ObtenirSelonId(id);

            if(note == null)
                return NotFound();

            return Ok(note);
        }


        // 201 // 400
        // POST api/<NotesController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RequeteNote requeteNote)
        {
            if (ModelState.IsValid)
            {
                Note note = await _notesService.Ajouter(requeteNote);

                if (note == null)
                    return BadRequest();

                return CreatedAtAction(nameof(Get), new { id = note?.Id }, note);
            }

            return BadRequest();
        }

        // PUT api/<NotesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Note note)
        {
            if(id != note.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                bool modification = await _notesService.Modifier(note);
                if(!modification)
                    return BadRequest();

                return NoContent();
            }

            return BadRequest();
        }

        // DELETE api/<NotesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Note note = await _notesService.ObtenirSelonId(id);

            if (note == null)
                return NotFound();

            await _notesService.Supprimer(note);

            return NoContent();
        }
    }
}
