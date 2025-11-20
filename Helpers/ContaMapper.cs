using SistemaBancarioSimples.Model;
using SistemaBancarioSimples.DTOs;

namespace SistemaBancarioSimples.Helpers
{
    public static class ContaMapper
    {
        public static ContaDTO ParaDTO(ContaBancaria contaModel)
        {
            if (contaModel == null) return null;

            return new ContaDTO
            {
                Id = contaModel.Id,
                Numero = contaModel.Numero,
                Saldo = contaModel.Saldo,
                // Aqui pegamos só o nome do usuário, protegendo a senha
                NomeTitular = contaModel.Usuario != null ? contaModel.Usuario.Username : "Desconhecido"
            };
        }
    }
}