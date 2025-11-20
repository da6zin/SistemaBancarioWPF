using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SistemaBancarioSimples
{
    public partial class DepositoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;

        // Construtor recebe as dependências
        public DepositoWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;

            // Foca no campo de valor ao abrir
            txtValor.Focus();
        }

        private async void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            // Tratamento de Ponto/Vírgula
            string valorTexto = txtValor.Text.Replace(".", ",");

            if (!decimal.TryParse(valorTexto, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Por favor, insira um valor válido maior que zero.");
                return;
            }

            try
            {
                // Chama o serviço
                await _contaService.DepositarAsync(_contaId, valor);

                MessageBox.Show($"Depósito de {valor:C} realizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Fecha a janela após o sucesso
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao depositar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Permite apenas números e vírgula/ponto
        private void Valor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}