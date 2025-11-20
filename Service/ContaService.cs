using SistemaBancarioSimples.Data;
using SistemaBancarioSimples.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SistemaBancarioSimples.DTOs;
using SistemaBancarioSimples.Helpers;

namespace SistemaBancarioSimples.Service
{
    public class ContaService : IContaService
    {
        private readonly BancoContext _context;

        public ContaService(BancoContext context)
        {
            _context = context;
        }

        // OBTER DADOS DA CONTA
        public async Task<ContaDTO> GetContaAsync(int id)
        {
            var contaModel = await _context.Contas
                                           .Include(c => c.Usuario)
                                           .FirstOrDefaultAsync(c => c.Id == id);
            return ContaMapper.ParaDTO(contaModel);
        }


        // DEPOSITAR
        public async Task DepositarAsync(int contaId, decimal valor)
        {
            var conta = await _context.Contas.FindAsync(contaId);
            if (conta == null) throw new Exception("Conta não encontrada.");

            conta.Saldo += valor;

            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor,
                Tipo = TransacaoTipo.Deposito,
                DataHora = DateTime.Now,
                Descricao = "Depósito em Dinheiro"
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
        }

        // SACAR
        public async Task SacarAsync(int contaId, decimal valor)
        {
            var conta = await _context.Contas.FindAsync(contaId);
            if (conta.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");

            conta.Saldo -= valor;

            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor * -1,
                Tipo = TransacaoTipo.Saque,
                DataHora = DateTime.Now,
                Descricao = "Saque em Conta"
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
        }

        // HISTÓRICO DE TRANSAÇÕES
        public async Task<List<Transacao>> GetHistoricoAsync(int contaId)
        {
            return await _context.Transacoes
                                 .Where(t => t.ContaBancariaId == contaId)
                                 .OrderByDescending(t => t.DataHora)
                                 .ToListAsync();
        }


        // TRANSFERÊNCIA
        public async Task TransferirAsync(int contaOrigemId, string usernameDestino, decimal valor)
        {
            var contaOrigem = await _context.Contas.FindAsync(contaOrigemId);
            var contaDestino = await _context.Contas
                                             .Include(c => c.Usuario)
                                             .FirstOrDefaultAsync(c => c.Usuario.Username == usernameDestino);

            if (contaDestino == null) throw new Exception("Destinatário não encontrado.");
            if (contaOrigem.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");

            contaOrigem.Saldo -= valor;
            contaDestino.Saldo += valor;

            var tSaida = new Transacao
            {
                ContaBancariaId = contaOrigem.Id,
                Valor = valor * -1,
                Tipo = TransacaoTipo.Transferencia,
                DataHora = DateTime.Now,
                Descricao = $"Enviado para {usernameDestino}"
            };

            var tEntrada = new Transacao
            {
                ContaBancariaId = contaDestino.Id,
                Valor = valor,
                Tipo = TransacaoTipo.Transferencia,
                DataHora = DateTime.Now,
                Descricao = $"Recebido de {contaOrigem.Usuario?.Username ?? "Desconhecido"}"
            };

            _context.Transacoes.Add(tSaida);
            _context.Transacoes.Add(tEntrada);

            await _context.SaveChangesAsync();
        }


        // EXCLUIR CONTA
        public async Task ExcluirContaAsync(int contaId)
        {
            var conta = await _context.Contas
                                      .Include(c => c.Usuario) 
                                      .FirstOrDefaultAsync(c => c.Id == contaId);

            if (conta == null) throw new Exception("Conta não encontrada.");

            _context.Contas.Remove(conta);

            if (conta.Usuario != null)
            {
                _context.Usuarios.Remove(conta.Usuario);
            }

            await _context.SaveChangesAsync();
        }


        // LISTAR TODAS AS CONTAS (ADMIN)
        public async Task<List<ContaDTO>> GetTodasContasAsync()
        {
            var contas = await _context.Contas
                                       .Include(c => c.Usuario)
                                       .ToListAsync();

            return contas.Select(c => ContaMapper.ParaDTO(c)).ToList();
        }
    }
}
