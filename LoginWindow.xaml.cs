using SistemaBancarioSimples.Data;
using SistemaBancarioSimples.Model;
using SistemaBancarioSimples.Service;
using System.Windows;
using System;

namespace SistemaBancarioSimples
{
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _authService;
        private readonly IContaService _contaService;

        public LoginWindow()
        {
            InitializeComponent();

            // Inicialização dos serviços (Injeção de Dependência simples)
            var context = new BancoContext();
            _authService = new AuthService(context);
            _contaService = new ContaService(context);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MensagemTextBlock.Text = "Preencha todos os campos.";
                return;
            }

            try
            {
                Usuario usuarioLogado = await _authService.LoginAsync(username, password);

                if (usuarioLogado != null)
                {
                    // Login bem-sucedido: Abre a Janela Principal
                    MensagemTextBlock.Text = "Login realizado com sucesso!";

                    // Passa o ID da conta do usuário logado para a MainWindow
                    MainWindow mainWindow = new MainWindow(_contaService, usuarioLogado.ContaBancariaId);
                    mainWindow.Show();
                    this.Close(); // Fecha a janela de login
                }
                else
                {
                    MensagemTextBlock.Text = "Usuário ou senha inválidos.";
                }
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro no Login: {ex.Message}";
            }
        }

        private async void CadastrarButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MensagemTextBlock.Text = "Preencha todos os campos.";
                return;
            }

            try
            {
                await _authService.CadastrarAsync(username, password);
                MensagemTextBlock.Text = "Cadastro realizado com sucesso! Faça login.";
            }
            catch (InvalidOperationException ex)
            {
                MensagemTextBlock.Text = ex.Message;
            }
            catch (Exception ex)
            {
                MensagemTextBlock.Text = $"Erro no Cadastro: {ex.Message}";
            }
        }
    }
}