using SistemaBancarioSimples.Service;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SistemaBancarioSimples
{
    public partial class MainWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;

        public MainWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;

            Title = $"Sistema Bancário - Conta ID: {contaId}";

            // Carrega o saldo inicial
            _ = AtualizarSaldoAsync();
        }

        private async Task AtualizarSaldoAsync()
        {
            try
            {
                var conta = await _contaService.GetContaAsync(_contaId);
                SaldoTextBlock.Text = conta.Saldo.ToString("C"); // Atualiza o texto grande
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro ao atualizar: {ex.Message}";
                MensagemTextBlock.Foreground = Brushes.Red;
            }
        }

        // --- EVENTOS DE ABERTURA DE JANELAS ---

        private void DepositarButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DepositoWindow(_contaService, _contaId);
            window.ShowDialog(); // Trava a tela até fechar
            _ = AtualizarSaldoAsync(); // Atualiza o saldo na volta
        }

        private void SacarButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SaqueWindow(_contaService, _contaId);
            window.ShowDialog();
            _ = AtualizarSaldoAsync();
        }

        // Arquivo: MainWindow.xaml.cs

        private void RendimentoButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Cria e abre a janela
            var window = new RendimentoWindow(_contaService, _contaId);

            // 2. O 'ShowDialog' trava o código aqui até o usuário fechar a janela de rendimento
            window.ShowDialog();

            // 3. ASSIM QUE FECHAR, essa linha roda e busca o saldo novo no banco
            _ = AtualizarSaldoAsync();
        }

        private void TransacaoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TransacaoWindow(_contaService, _contaId);
            window.ShowDialog();
            _ = AtualizarSaldoAsync();
        }

        private void VerHistoricoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new HistoricoWindow(_contaService, _contaId);
            window.ShowDialog();
        }
    }
}