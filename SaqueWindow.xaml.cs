using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SistemaBancarioSimples
{
    public partial class SaqueWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;

        public SaqueWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;
            txtValor.Focus();
        }

        private async void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            string valorTexto = txtValor.Text.Replace(".", ",");

            if (!decimal.TryParse(valorTexto, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Por favor, insira um valor válido.");
                return;
            }

            try
            {
                await _contaService.SacarAsync(_contaId, valor);

                MessageBox.Show($"Saque de {valor:C} realizado. Retire o dinheiro.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Não foi possível sacar: {ex.Message}", "Saldo Insuficiente", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao sacar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Valor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}