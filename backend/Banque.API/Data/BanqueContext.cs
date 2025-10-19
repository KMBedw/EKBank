using Microsoft.EntityFrameworkCore;
using Banque.API.Models;
namespace Banque.API.Data
{
    public class BanqueContext:DbContext
    {
        public BanqueContext(DbContextOptions<BanqueContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Compte> Comptes => Set<Compte>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
