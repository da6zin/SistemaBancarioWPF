namespace SistemaBancarioSimples.DTOs
{
    public class ContaDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public decimal Saldo { get; set; }
        public string NomeTitular { get; set; }
    }
}