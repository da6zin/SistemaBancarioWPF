using SistemaBancarioSimples.Service;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SistemaBancarioSimples.DTOs;

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

            _ = AtualizarSaldoAsync();
        }

        private async Task AtualizarSaldoAsync()
        {
            try
            {
                ContaDTO conta = await _contaService.GetContaAsync(_contaId);

                if (conta != null)
                {
                    SaldoTextBlock.Text = conta.Saldo.ToString("C");

                    string numero = string.IsNullOrEmpty(conta.Numero) ? "S/N" : conta.Numero;

                    txtNumeroConta.Text = $"Conta: {numero} | Olá, {conta.NomeTitular}";
                }
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro: {ex.Message}";
            }
        }


        private void DepositarButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DepositoWindow(_contaService, _contaId);
            window.ShowDialog();
            _ = AtualizarSaldoAsync();
        }

        private void SacarButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SaqueWindow(_contaService, _contaId);
            window.ShowDialog();
            _ = AtualizarSaldoAsync();
        }

        private void RendimentoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new RendimentoWindow(_contaService, _contaId);

            window.ShowDialog();

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


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Deseja realmente sair da sua conta?", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();

                this.Close();
            }
        }

        private async void ExcluirContaButton_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Tem certeza que deseja encerrar sua conta?\nEssa ação é irreversível e todos os seus dados serão apagados.",
                "Encerrar Conta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    await _contaService.ExcluirContaAsync(_contaId);

                    MessageBox.Show("Sua conta foi encerrada. Sentiremos sua falta!", "Conta Excluída");

                    LoginWindow login = new LoginWindow();
                    login.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir conta: {ex.Message}");
                }
            }
        }
    }
}