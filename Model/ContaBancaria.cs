using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBancarioSimples.Model
{
    public class ContaBancaria
    {
        // A chave primária será a chave estrangeira do Usuário (relacionamento 1:1)
        public int Id { get; set; }

        public decimal Saldo { get; set; } = 0m;

        public ICollection<Transacao> Transacoes { get; set; }

        // Propriedade de navegação
        public Usuario Usuario { get; set; }
    }
}
