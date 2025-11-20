using Microsoft.EntityFrameworkCore;
using SistemaBancarioSimples.Model;

namespace SistemaBancarioSimples.Data
{
    public class BancoContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ContaBancaria> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        public BancoContext()
        {
        }

        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=banco_final.db");
            }
        }
    }
}