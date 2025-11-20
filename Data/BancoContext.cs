using Microsoft.EntityFrameworkCore;
using SistemaBancarioSimples.Model;

namespace SistemaBancarioSimples.Data
{
    public class BancoContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ContaBancaria> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        // 1. CONSTRUTOR VAZIO (Para quando você faz 'new BancoContext()')
        public BancoContext()
        {
        }

        // 2. CONSTRUTOR COM OPÇÕES (O que estava faltando!)
        // Aceita configurações externas se necessário
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Verifica se já foi configurado externamente (pelo construtor 2)
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=banco_final.db");
            }
        }
    }
}