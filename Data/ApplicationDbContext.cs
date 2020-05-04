using Microsoft.EntityFrameworkCore;
using ProjetoVSCApi.Models;

namespace ProjetoVSCApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}