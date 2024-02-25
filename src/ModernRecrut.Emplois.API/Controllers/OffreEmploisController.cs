using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Emplois.API.DTO;
using ModernRecrut.Emplois.API.Interfaces;
using ModernRecrut.Emplois.API.Models;


namespace ModernRecrut.Emplois.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OffreEmploisController : ControllerBase
	{
		private readonly IOffreEmploisService _offreEmploisService;

        public OffreEmploisController(IOffreEmploisService offreEmploisService)
        {
			_offreEmploisService = offreEmploisService;
        }

        // GET: api/<OffreEmploisController>
        [HttpGet]
		public async Task<IEnumerable<OffreEmploi>> Get()
		{
			return await _offreEmploisService.ObtenirTout();
		}


		// GET 200 / 404
		// GET api/<OffreEmploisController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult> Get(int id)
		{
			OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(id);

			if(offreEmploi == null)
				return NotFound();

			return Ok(offreEmploi);
		}
		

		// 201 // 400
		// POST api/<OffreEmploisController>
		[HttpPost]
		public async Task<ActionResult> Post([FromBody] RequeteOffreEmploi requeteOffreEmploi)
		{
			if (ModelState.IsValid)
			{
				OffreEmploi? offreEmploi  = await _offreEmploisService.Ajouter(requeteOffreEmploi);
				
				if(offreEmploi == null)
					return BadRequest();
					
				return CreatedAtAction(nameof(Get), new { id = offreEmploi?.Id }, offreEmploi);
			}

			return BadRequest();
		}


		// 204 // 400 // 400
		// PUT api/<OffreEmploisController>/5
		[HttpPut("{id}")]
		public async Task<ActionResult> Put(int id, [FromBody] OffreEmploi offreEmploi)
		{
			if(id != offreEmploi.Id)
			{
				return BadRequest();
			}

			if (ModelState.IsValid)
			{
				bool modification = await _offreEmploisService.Modifier(offreEmploi);
				if (!modification)
					return BadRequest();

				return NoContent();
			}
			return BadRequest();
		}

		// DELETE api/<OffreEmploisController>/5
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(id);

			if (offreEmploi == null)
				return NotFound();

			await _offreEmploisService.Supprimer(offreEmploi);

			return NoContent();
		}
	}
}
