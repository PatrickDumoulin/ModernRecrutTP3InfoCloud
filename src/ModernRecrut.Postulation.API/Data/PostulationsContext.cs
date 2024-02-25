using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Data
{
    public class PostulationsContext : DbContext
    {
        public PostulationsContext(DbContextOptions<PostulationsContext> options) : base(options)
        {
        }

        public DbSet<Models.Postulation> Postulation {  get; set; }
        public DbSet<Note> Note {  get; set; }
    }
}
