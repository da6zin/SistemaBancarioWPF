using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBancarioSimples.Model
{
    public class Transacao
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public TransacaoTipo Tipo { get; set; }

        // --- ADICIONE ESTA LINHA AQUI: ---
        public string Descricao { get; set; }
        // ---------------------------------

        public int ContaBancariaId { get; set; }
        public ContaBancaria Conta { get; set; }
    }

    public enum TransacaoTipo
    {
        Deposito,
        Saque,
        Transferencia
    }
}
