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
                // Busca os dados da conta (incluindo o Numero agora)
                var conta = await _contaService.GetContaAsync(_contaId);

                // Atualiza o Saldo
                SaldoTextBlock.Text = conta.Saldo.ToString("C");

                // NOVO: Atualiza o Número da Conta
                // Se o número for nulo (contas antigas), mostra um padrão
                string numero = string.IsNullOrEmpty(conta.Numero) ? "Sem Número" : conta.Numero;
                txtNumeroConta.Text = $"Conta: {numero}";
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


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Confirmação opcional (Bancos geralmente não pedem, mas é bom ter)
            var result = MessageBox.Show("Deseja realmente sair da sua conta?", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 2. Cria a nova tela de login
                // (Lembre-se: seu construtor do LoginWindow() já inicializa o BancoContext, então não precisa passar nada)
                LoginWindow login = new LoginWindow();
                login.Show();

                // 3. Fecha a janela do banco
                this.Close();
            }
        }
    }
}