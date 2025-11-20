using SistemaBancarioSimples.Service;
using System;
using System.Windows;
using SistemaBancarioSimples.Helpers;
using System.Windows.Input;

namespace SistemaBancarioSimples
{
    public partial class TransacaoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaOrigemId;

        public TransacaoWindow(IContaService contaService, int contaOrigemId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaOrigemId = contaOrigemId;
        }

        private async void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            // 1. LIMPEZA: Valida o valor usando o Helper
            if (!MoedaHelper.TentarConverter(txtValorTransferencia.Text, out decimal valor))
            {
                MessageBox.Show("Valor inválido. Digite um número positivo.");
                return;
            }

            // 2. Valida o Usuário (Lógica normal de string)
            string usernameDestino = txtContaDestino.Text;
            if (string.IsNullOrWhiteSpace(usernameDestino))
            {
                MessageBox.Show("Por favor, digite o nome do usuário de destino.");
                return;
            }

            try
            {
                await _contaService.TransferirAsync(_contaOrigemId, usernameDestino, valor);

                MessageBox.Show($"Transferência de {valor:C} para {usernameDestino} realizada!", "Sucesso");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na transferência: {ex.Message}");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Adicione: PreviewTextInput="Valor_PreviewTextInput" no seu TextBox de Valor no XAML da Transação
        private void Valor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = MoedaHelper.EhTextoInvalido(e.Text);
        }
    }
}