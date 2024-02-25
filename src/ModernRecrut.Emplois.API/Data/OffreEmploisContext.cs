using Microsoft.EntityFrameworkCore;
using ModernRecrut.Emplois.API.Models;

namespace ModernRecrut.Emplois.API.Data
{
	public class OffreEmploisContext : DbContext
	{
        public OffreEmploisContext(DbContextOptions<OffreEmploisContext> options) : base(options)
        {
        }

        public DbSet<OffreEmploi>? OffreEmploi { get; set; }
    }
}
