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

        public async Task<IEnumerable<Transacao>> GetHistoricoAsync(int contaId)
        {
            // Retorna as transações em ordem decrescente de data
            return await _context.Transacoes
                                 .Where(t => t.ContaBancariaId == contaId)
                                 .OrderByDescending(t => t.DataHora)
                                 .ToListAsync();
        }
    }
}
