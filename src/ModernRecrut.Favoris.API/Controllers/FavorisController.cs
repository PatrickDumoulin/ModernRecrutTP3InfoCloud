using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModernRecrut.Favoris.API.DTO;
using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Models;
using ModernRecrut.Favoris.API.Services;
using System.Net;


namespace ModernRecrut.Favoris.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavorisController : ControllerBase
    {

        #region Attributs
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheTools _cacheTools;
        #endregion

        #region Constructor
        public FavorisController(IMemoryCache memoryCache, ICacheTools cacheTools)
        {
            _memoryCache = memoryCache;
            _cacheTools = cacheTools;
        }

        #endregion

        // 201 // 400
        // GET api/<FavorisController>/5
        [HttpGet("{cle}")]
        public ActionResult Get(string cle)
        {
            // Validationde l'adresse IP
#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            if(!IPAddress.TryParse(cle, out IPAddress ip))
                return BadRequest();
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.

            if(_memoryCache.TryGetValue(cle, out List<OffreEmploi> offresEmploi)){
                return Ok(offresEmploi);
            }

            //return Ok(offresEmploi);
            return NotFound("Liste des Favoris non trouvée");
        }

        // POST api/<FavorisController>
        [HttpPost]
        public ActionResult Post([FromBody] RequeteFavori requeteFavori)
        {
            // Retrouver si cache avec cette cle // 
            string cle = requeteFavori.Cle;
            OffreEmploi offreEmploi = requeteFavori.OffreEmploi;

#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            if (offreEmploi == null || !IPAddress.TryParse(cle, out IPAddress ip)) 
                return BadRequest();
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.

            if (!_memoryCache.TryGetValue(cle, out List<OffreEmploi> offresEmploi))
                offresEmploi = new List<OffreEmploi>();

            // Teste si l'offre d'emploi est déja dans le cache
            if (offresEmploi.Any(o => o.Id == offreEmploi.Id))
                return BadRequest();

            // Ajout de l'offre d'emploi au favoris
            offresEmploi.Add(offreEmploi);

            // Configure la durée de mise en cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                Size = _cacheTools.CalculTailleListOffresEmploi(offresEmploi)
            }; 

            _memoryCache.Set(cle, offresEmploi, cacheEntryOptions);

            return Ok();
        }

        // DELETE api/<FavorisController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string cle, int id)
        {
            // Validationde l'adresse IP
#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
            if(!IPAddress.TryParse(cle, out IPAddress ip))
                return BadRequest();
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.

            if (!_memoryCache.TryGetValue(cle, out List<OffreEmploi> offresEmploi))
                return BadRequest();

            OffreEmploi? offreEmploi = offresEmploi.SingleOrDefault(o => o.Id == id);
            
            if (offreEmploi == null)
                return BadRequest();

            // Retrait de l'offre d'emploi de la liste des offres d'emplois
            offresEmploi.Remove(offreEmploi);
            
            // Suppression de l'enregistrement en mémoire si la liste ne contient plus d'offre d'emploi
            if(offresEmploi.Count == 0)
            {
                _memoryCache.Remove(cle);
                return Ok();
            }

            // Configure la durée de mise en cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                    SlidingExpiration = TimeSpan.FromHours(6),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    Size = _cacheTools.CalculTailleListOffresEmploi(offresEmploi)
            };

            // Mise en Cache
            _memoryCache.Set(cle, offresEmploi, cacheEntryOptions);

            return Ok();
        }
    }
}
