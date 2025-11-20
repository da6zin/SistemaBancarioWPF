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

            // Carrega o saldo inicial
            _ = AtualizarSaldoAsync();
        }

        private async Task AtualizarSaldoAsync()
        {
            try
            {
                // Agora a variável 'conta' é um DTO, não mais o Model do banco!
                ContaDTO conta = await _contaService.GetContaAsync(_contaId);

                if (conta != null)
                {
                    SaldoTextBlock.Text = conta.Saldo.ToString("C");

                    string numero = string.IsNullOrEmpty(conta.Numero) ? "S/N" : conta.Numero;

                    // BÔNUS: Agora podemos mostrar o nome do titular também!
                    txtNumeroConta.Text = $"Conta: {numero} | Olá, {conta.NomeTitular}";
                }
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro: {ex.Message}";
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

        private async void ExcluirContaButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Confirmação de segurança
            var resultado = MessageBox.Show(
                "Tem certeza que deseja encerrar sua conta?\nEssa ação é irreversível e todos os seus dados serão apagados.",
                "Encerrar Conta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // 2. Chama o serviço para apagar tudo
                    await _contaService.ExcluirContaAsync(_contaId);

                    MessageBox.Show("Sua conta foi encerrada. Sentiremos sua falta!", "Conta Excluída");

                    // 3. Redireciona para o Login (já que o usuário atual não existe mais)
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