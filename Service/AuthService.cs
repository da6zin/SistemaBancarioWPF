using SistemaBancarioSimples.Data;
using SistemaBancarioSimples.Model;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SistemaBancarioSimples.Service
{
    public class AuthService : IAuthService
    {
        private readonly BancoContext _context;

        public AuthService(BancoContext context)
        {
            _context = context;
        }

        // CADASTRAR NOVO USUÁRIO
        public async Task<Usuario> CadastrarAsync(string username, string password)
        {
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("Nome de usuário já está em uso.");
            }

            Random random = new Random();
            string numeroContaGerado = random.Next(10000, 99999).ToString();

            var novaConta = new ContaBancaria
            {
                Saldo = 0,
                Numero = numeroContaGerado
            };

            _context.Contas.Add(novaConta);
            await _context.SaveChangesAsync();


            var novoUsuario = new Usuario
            {
                Username = username,
                PasswordHash = password,
                ContaBancariaId = novaConta.Id
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return novoUsuario;
        }


        // LOGIN DE USUÁRIO
        public async Task<Usuario> LoginAsync(string username, string password)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Conta)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null)
            {
                return null;
            }

            if (usuario.PasswordHash != password)
            {
                return null;
            }

            return usuario;
        }
    }
}