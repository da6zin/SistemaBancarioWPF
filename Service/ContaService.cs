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
        // Padrão de Injeção de Dependência (Princípio D - Inversão de Dependência)
        private readonly BancoContext _context;

        public ContaService(BancoContext context)
        {
            _context = context;
        }


        public async Task<ContaDTO> GetContaAsync(int id)
        {
            var contaModel = await _context.Contas
                                           .Include(c => c.Usuario) // Traz o usuário para pegarmos o nome
                                           .FirstOrDefaultAsync(c => c.Id == id);

            // Usa o Mapper para converter antes de entregar para quem pediu
            return ContaMapper.ParaDTO(contaModel);
        }

        // DEPOSITAR
        public async Task DepositarAsync(int contaId, decimal valor)
        {
            var conta = await _context.Contas.FindAsync(contaId);
            if (conta == null) throw new Exception("Conta não encontrada.");

            // 1. ATUALIZA O SALDO (Isso estava faltando antes?)
            conta.Saldo += valor;

            // 2. CRIA A TRANSAÇÃO (Usando a classe genérica e Enum)
            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor,
                Tipo = TransacaoTipo.Deposito, // Enum
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

            // 1. TIRA O SALDO
            conta.Saldo -= valor;

            // 2. CRIA A TRANSAÇÃO
            var transacao = new Transacao
            {
                ContaBancariaId = contaId,
                Valor = valor * -1, // Negativo
                Tipo = TransacaoTipo.Saque,
                DataHora = DateTime.Now,
                Descricao = "Saque em Conta"
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


        // TRANSFERIR
        public async Task TransferirAsync(int contaOrigemId, string usernameDestino, decimal valor)
        {
            var contaOrigem = await _context.Contas.FindAsync(contaOrigemId);
            var contaDestino = await _context.Contas
                                             .Include(c => c.Usuario)
                                             .FirstOrDefaultAsync(c => c.Usuario.Username == usernameDestino);

            if (contaDestino == null) throw new Exception("Destinatário não encontrado.");
            if (contaOrigem.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");

            // 1. MOVE O DINHEIRO
            contaOrigem.Saldo -= valor;
            contaDestino.Saldo += valor;

            // 2. REGISTRA SAÍDA (Origem)
            var tSaida = new Transacao
            {
                ContaBancariaId = contaOrigem.Id,
                Valor = valor * -1,
                Tipo = TransacaoTipo.Transferencia,
                DataHora = DateTime.Now,
                Descricao = $"Enviado para {usernameDestino}"
            };

            // 3. REGISTRA ENTRADA (Destino)
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

        public async Task ExcluirContaAsync(int contaId)
        {
            // 1. Busca a conta e INCLUI o Usuário dono dela
            var conta = await _context.Contas
                                      .Include(c => c.Usuario) // Importante para apagar o login também
                                      .FirstOrDefaultAsync(c => c.Id == contaId);

            if (conta == null) throw new Exception("Conta não encontrada.");

            // 2. (Opcional) Regra de Negócio: Não deixar apagar se tiver saldo positivo
            // if (conta.Saldo > 0) throw new InvalidOperationException("Saque todo o saldo antes de encerrar a conta.");

            // 3. Remove a Conta
            _context.Contas.Remove(conta);

            // 4. Remove o Usuário (Login) associado
            if (conta.Usuario != null)
            {
                _context.Usuarios.Remove(conta.Usuario);
            }

            // 5. Salva e confirma a exclusão no banco
            await _context.SaveChangesAsync();
        }

    }
}
