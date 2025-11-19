using Microsoft.EntityFrameworkCore;
using SistemaBancarioSimples.Model;
using System.IO;
using System;

namespace SistemaBancarioSimples.Data
{
    public class BancoContext : DbContext
    {
        public DbSet<ContaBancaria> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        private readonly string _dbPath;

        // 1. Construtor Padrão (Usado em tempo de execução na UI)
        public BancoContext()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Join(folder, "banco_simples.db");
        }

        // 2. NOVO: Construtor que aceita opções (Usado pela Fábrica para Migrações)
        // Isso resolve o erro "No database provider has been configured"
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
            // Não precisamos definir _dbPath ou chamar OnConfiguring aqui, pois as opções
            // já foram configuradas e passadas pelo IDesignTimeDbContextFactory.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Aplica a configuração padrão SOMENTE se nenhuma opção já tiver sido fornecida
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ... (configurações existentes)
        }
    }
}