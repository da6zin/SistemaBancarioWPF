using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;
using SistemaBancarioSimples.Data;

namespace SistemaBancarioSimples.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BancoContext>
    {
        public BancoContext CreateDbContext(string[] args)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Join(folder, "banco_simples.db");
            var optionsBuilder = new DbContextOptionsBuilder<BancoContext>();

            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new BancoContext(optionsBuilder.Options);
        }
    }
}