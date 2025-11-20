using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaBancarioSimples.Model;
using System.Threading.Tasks;
using SistemaBancarioSimples.DTOs;

namespace SistemaBancarioSimples.Service
{
    public interface IContaService
    {
        Task<ContaDTO> GetContaAsync(int id);
        Task DepositarAsync(int contaId, decimal valor);
        Task SacarAsync(int contaId, decimal valor);
        Task<List<Transacao>> GetHistoricoAsync(int contaId);
        Task TransferirAsync(int contaOrigemId, string usernameDestino, decimal valor);
        Task ExcluirContaAsync(int contaId);

    }
}
