using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBancarioSimples.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Armazenar senha com hash

        // Relacionamento 1 para 1: Um Usuário tem uma Conta Bancária
        public ContaBancaria Conta { get; set; }
        public int ContaBancariaId { get; set; }
    }
}
