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
        public DateTime DataHora { get; set; } = DateTime.Now;
        public TransacaoTipo Tipo { get; set; } // Enum: Deposito ou Saque
        public decimal Valor { get; set; }

        // Chave estrangeira para a conta
        public int ContaBancariaId { get; set; }
        public ContaBancaria ContaBancaria { get; set; } // Propriedade de navegação
    }

    public enum TransacaoTipo
    {
        Deposito,
        Saque
    }
}
