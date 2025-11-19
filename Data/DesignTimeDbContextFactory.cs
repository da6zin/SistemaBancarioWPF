using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;
using SistemaBancarioSimples.Data;

namespace SistemaBancarioSimples.Data
{
    // Esta classe diz ao EF Core como criar uma instância do BancoContext APENAS para migrações
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BancoContext>
    {
        public BancoContext CreateDbContext(string[] args)
        {
            // Replicamos a lógica de criação do caminho do arquivo SQLite
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Join(folder, "banco_simples.db");

            var optionsBuilder = new DbContextOptionsBuilder<BancoContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            // Passamos as opções configuradas para o construtor base do DbContext
            return new BancoContext(optionsBuilder.Options);
        }
    }
}