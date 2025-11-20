namespace SistemaBancarioSimples.DTOs
{
    public class ContaDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public decimal Saldo { get; set; }

        // Veja que NÃO temos a senha nem o objeto Usuario completo aqui.
        // Apenas o que a tela precisa mostrar.
        public string NomeTitular { get; set; }
    }
}