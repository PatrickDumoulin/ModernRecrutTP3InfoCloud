using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Postulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulationsController : ControllerBase
    {
        private readonly IPostulationsService _postulationsService;

        public PostulationsController(IPostulationsService postulationsService)
        {
            _postulationsService = postulationsService;
        }


        // GET: api/<PostulationsController>
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Models.Postulation>>> Get()
        public async Task<IEnumerable<Models.Postulation>> Get()
        {
            return await _postulationsService.ObtenirTout();
        }

        // GET api/<PostulationsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            Models.Postulation postulation = await _postulationsService.ObtenirSelonId(id);

            if(postulation == null)
                return NotFound();
            
            return Ok(postulation);
        }


        // 201 // 400
        // POST api/<PostulationsController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RequetePostulation requetePostulation)
        {
            if (ModelState.IsValid)
            {
                Models.Postulation postulation = await _postulationsService.Ajouter(requetePostulation);
                if (postulation == null)
                    return BadRequest();

                return CreatedAtAction(nameof(Get), new { id = postulation?.Id }, postulation);
            }

            return BadRequest();
        }

        // PUT api/<PostulationsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Models.Postulation postulation)
        {
            if(id != postulation.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                bool modification = await _postulationsService.Modifier(postulation);
                if(!modification)
                    return BadRequest();

                return NoContent();
            }
            return BadRequest();
        }

        // DELETE api/<PostulationsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            Models.Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if(postulation == null)
                return NotFound();

            await _postulationsService.Supprimer(postulation);

            return NoContent();
        }
    }
}
