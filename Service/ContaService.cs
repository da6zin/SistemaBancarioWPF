using SistemaBancarioSimples.Data;
using SistemaBancarioSimples.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SistemaBancarioSimples.Service
{
    public class ContaService : IContaService
    {
        // Padrão de Injeção de Dependência (Princípio D - Inversão de Dependência)
        private readonly BancoContext _context;

        public ContaService(BancoContext context)
        {
            _context = context;
        }

        public async Task<ContaBancaria> GetContaAsync(int id)
        {
            // Busca a conta e garante que a conta inicial exista
            return await _context.Contas.FirstOrDefaultAsync(c => c.Id == id)
                   ?? throw new InvalidOperationException("Conta não encontrada.");
        }

        public async Task DepositarAsync(int contaId, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do depósito deve ser positivo.");

            var conta = await GetContaAsync(contaId);

            // Lógica de negócio
            conta.Saldo += valor;

            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor,
                Tipo = TransacaoTipo.Deposito
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
        }

        public async Task SacarAsync(int contaId, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do saque deve ser positivo.");

            var conta = await GetContaAsync(contaId);

            if (conta.Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente.");

            // Lógica de negócio
            conta.Saldo -= valor;

            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor,
                Tipo = TransacaoTipo.Saque
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
        }


        // Certifique-se de ter: using Microsoft.EntityFrameworkCore;

        public async Task<List<Transacao>> GetHistoricoAsync(int contaId)
        {
            // Busca na tabela de Transações onde o ContaId for igual ao do usuário
            return await _context.Transacoes
                                 .Where(t => t.ContaBancariaId == contaId)
                                 .OrderByDescending(t => t.DataHora) // Ordena do mais recente para o antigo
                                 .ToListAsync();
        }


        public async Task TransferirAsync(int contaOrigemId, string usernameDestino, decimal valor)
        {
            // 1. Busca a conta de origem
            var contaOrigem = await _context.Contas.FindAsync(contaOrigemId);

            // 2. BUSCA A CONTA DE DESTINO PELO NOME DO USUÁRIO
            // O ".Include" carrega os dados do Usuário junto com a Conta
            var contaDestino = await _context.Contas
                                             .Include(c => c.Usuario)
                                             .FirstOrDefaultAsync(c => c.Usuario.Username == usernameDestino);

            // --- VALIDAÇÕES ---
            if (contaDestino == null)
            {
                throw new Exception($"Usuário '{usernameDestino}' não encontrado.");
            }

            if (contaOrigem.Id == contaDestino.Id)
            {
                throw new Exception("Você não pode transferir para si mesmo.");
            }

            if (contaOrigem.Saldo < valor)
            {
                throw new InvalidOperationException("Saldo insuficiente.");
            }

            // --- OPERAÇÃO MATEMÁTICA ---
            contaOrigem.Saldo -= valor;
            contaDestino.Saldo += valor;

            // --- CRIA O EXTRATO (HISTÓRICO) ---

            // Saída (Para quem enviou)
            var transacaoSaida = new Transacao
            {
                ContaBancariaId = contaOrigem.Id,
                Valor = valor * -1, // Negativo para indicar saída
                Tipo = TransacaoTipo.Transferencia,
                DataHora = DateTime.Now
            };

            // Entrada (Para quem recebeu)
            var transacaoEntrada = new Transacao
            {
                ContaBancariaId = contaDestino.Id,
                Valor = valor, // Positivo
                Tipo = TransacaoTipo.Transferencia,
                DataHora = DateTime.Now
            };

            _context.Transacoes.Add(transacaoSaida);
            _context.Transacoes.Add(transacaoEntrada);

            await _context.SaveChangesAsync();
        }
    }
}
