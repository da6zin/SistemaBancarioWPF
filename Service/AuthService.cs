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

        public async Task<Usuario> CadastrarAsync(string username, string password)
        {
            // 1. Verificar se o usuário já existe
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("Nome de usuário já está em uso.");
            }

            // 2. Criar a Conta Bancária
            var novaConta = new ContaBancaria { Saldo = 0m };
            _context.Contas.Add(novaConta);
            await _context.SaveChangesAsync(); // Salva para obter o Id da conta

            // 3. Criar o Usuário
            var novoUsuario = new Usuario
            {
                Username = username,
                // Em produção, use BCrypt ou Argon2. Aqui, usamos a senha como hash simplificado.
                PasswordHash = password,
                ContaBancariaId = novaConta.Id
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return novoUsuario;
        }

        public async Task<Usuario> LoginAsync(string username, string password)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Conta) // Inclui os dados da conta
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null)
            {
                return null; // Usuário não encontrado
            }

            // Em produção, verifique o hash: VerifyHash(password, usuario.PasswordHash)
            if (usuario.PasswordHash != password)
            {
                return null; // Senha incorreta
            }

            return usuario; // Login bem-sucedido
        }
    }
}