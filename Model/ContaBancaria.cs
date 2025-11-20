using SistemaBancarioSimples.Model;

public class ContaBancaria
{
    public int Id { get; set; }
    public decimal Saldo { get; set; } = 0m;

    // NOVO: Guarda o número da conta (ex: "4829-1")
    public string Numero { get; set; }

    public ICollection<Transacao> Transacoes { get; set; }
    public Usuario Usuario { get; set; }
}