using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaBancarioSimples.Model;
using System.Threading.Tasks;

namespace SistemaBancarioSimples.Service
{
    public interface IContaService
    {
        Task<ContaBancaria> GetContaAsync(int id);
        Task DepositarAsync(int contaId, decimal valor);
        Task SacarAsync(int contaId, decimal valor);
        Task<List<Transacao>> GetHistoricoAsync(int contaId);

        Task TransferirAsync(int contaOrigemId, string usernameDestino, decimal valor);
    }
}
