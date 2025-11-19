using SistemaBancarioSimples.Data;
using SistemaBancarioSimples.Model;
using SistemaBancarioSimples.Service;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SistemaBancarioSimples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContaService _contaService;
        // VARIÁVEL DE INSTÂNCIA: Armazena o ID da conta do usuário logado
        private readonly int _contaId;

        // REMOVA esta linha se ela ainda existir:
        // private const int ContaId = 1; 

        // NOVO CONSTRUTOR: Recebe o ID da conta
        public MainWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId; // Inicializa a variável

            Title = $"Sistema Bancário - Conta ID: {contaId}";
            AtualizarSaldoAsync();
        }

        private async Task AtualizarSaldoAsync()
        {
            try
            {
                var conta = await _contaService.GetContaAsync(_contaId);
                // Formatação simples para mostrar o valor em Reais (R$)
                SaldoTextBlock.Text = conta.Saldo.ToString("C");
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro ao carregar saldo: {ex.Message}";
            }
        }

        private async void DepositarButton_Click(object sender, RoutedEventArgs e)
        {
            await RealizarTransacaoAsync(TransacaoTipo.Deposito);
        }

        private async void SacarButton_Click(object sender, RoutedEventArgs e)
        {
            await RealizarTransacaoAsync(TransacaoTipo.Saque);
        }

        private async Task RealizarTransacaoAsync(TransacaoTipo tipo)
        {
            if (!decimal.TryParse(ValorTextBox.Text, out decimal valor) || valor <= 0)
            {
                MensagemTextBlock.Text = "Por favor, insira um valor válido.";
                return;
            }

            try
            {
                MensagemTextBlock.Text = ""; // Limpa a mensagem anterior

                if (tipo == TransacaoTipo.Deposito)
                {
                    await _contaService.DepositarAsync(_contaId, valor);
                    MensagemTextBlock.Text = $"Depósito de {valor:C} realizado com sucesso!";
                }
                else
                {
                    await _contaService.SacarAsync(_contaId, valor);
                    MensagemTextBlock.Text = $"Saque de {valor:C} realizado com sucesso!";
                }

                // Atualiza a interface
                ValorTextBox.Clear();
                await AtualizarSaldoAsync();
            }
            catch (InvalidOperationException ex)
            {
                MensagemTextBlock.Text = $"Erro na Transação: {ex.Message}";
            }
            catch (ArgumentException ex)
            {
                MensagemTextBlock.Text = $"Erro de Valor: {ex.Message}";
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro Inesperado: {ex.Message}";
            }
        }

        // Permite apenas números e um ponto/vírgula
        private void ValorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+"); // Restringe a entrada a números e pontuação decimal
            e.Handled = regex.IsMatch(e.Text);
        }

        private void VerHistoricoButton_Click(object sender, RoutedEventArgs e)
        {
            // Cria e exibe a janela de histórico
            var historicoWindow = new HistoricoWindow(_contaService, _contaId);
            historicoWindow.Show();
        }
    }
}