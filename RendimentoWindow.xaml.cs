using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SistemaBancarioSimples.Helpers;

namespace SistemaBancarioSimples
{
    public partial class RendimentoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;
        private decimal _saldoInicial = 0;

        // Variável para guardar o lucro calculado até o usuário decidir aplicar
        private decimal _lucroParaAplicar = 0;

        public RendimentoWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;
            _ = CarregarSaldoAsync();
        }

        //teste
        private async System.Threading.Tasks.Task CarregarSaldoAsync()
        {
            try
            {
                var conta = await _contaService.GetContaAsync(_contaId);
                _saldoInicial = conta.Saldo;
                txtSaldoAtual.Text = _saldoInicial.ToString("C");
            }
            catch
            {
                txtSaldoAtual.Text = "Erro";
            }
        }

        private void Simular_Click(object sender, RoutedEventArgs e)
        {
            decimal aporteMensal = 0;

            // LÓGICA INTELIGENTE:
            // Se tiver texto, tenta converter com o Helper. 
            // Se o Helper falhar (texto inválido), avisa.
            // Se estiver vazio, ignora e mantém 0.
            if (!string.IsNullOrWhiteSpace(txtAporteMensal.Text))
            {
                if (!MoedaHelper.TentarConverter(txtAporteMensal.Text, out aporteMensal))
                {
                    MessageBox.Show("Valor do aporte inválido.");
                    return;
                }
            }

            // Validação dos Meses (Número Inteiro - Lógica separada pois não é moeda)
            if (!int.TryParse(txtMeses.Text, out int meses) || meses <= 0)
            {
                MessageBox.Show("Digite uma quantidade válida de meses.");
                return;
            }

            // Cálculo (1% ao mês)
            decimal taxaMensal = 0.01M;
            decimal saldoFuturo = _saldoInicial;
            decimal totalInvestido = _saldoInicial;

            for (int i = 0; i < meses; i++)
            {
                saldoFuturo += aporteMensal;
                totalInvestido += aporteMensal;
                saldoFuturo += (saldoFuturo * taxaMensal);
            }

            // Armazena o lucro na variável global da janela
            _lucroParaAplicar = saldoFuturo - totalInvestido;

            // Atualiza a tela
            txtResultadoLucro.Text = _lucroParaAplicar.ToString("C");
            txtResultadoTotal.Text = saldoFuturo.ToString("C");

            // MOSTRA O BOTÃO DE APLICAR se houver lucro
            if (_lucroParaAplicar > 0)
            {
                btnAplicar.Visibility = Visibility.Visible;
            }
        }

        // --- NOVO MÉTODO: Aplica o dinheiro na conta ---
        private async void Aplicar_Click(object sender, RoutedEventArgs e)
        {
            if (_lucroParaAplicar <= 0) return;

            try
            {
                // Reutiliza o método de depósito para adicionar o dinheiro
                // Isso é ótimo porque já gera histórico de transação automaticamente!
                await _contaService.DepositarAsync(_contaId, _lucroParaAplicar);

                MessageBox.Show($"Parabéns! O rendimento de {_lucroParaAplicar:C} foi creditado na sua conta.",
                                "Resgate Realizado", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close(); // Fecha a janela para voltar ao menu principal
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao aplicar rendimento: {ex.Message}");
            }
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Valor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = MoedaHelper.EhTextoInvalido(e.Text);
        }

        private void Numero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}