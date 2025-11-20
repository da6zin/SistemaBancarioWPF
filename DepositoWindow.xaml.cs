using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SistemaBancarioSimples.Helpers;

namespace SistemaBancarioSimples
{
    public partial class DepositoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;


        public DepositoWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;
            txtValor.Focus();
        }

        private async void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            string valorTexto = txtValor.Text.Replace(".", ",");

            if (!MoedaHelper.TentarConverter(txtValor.Text, out decimal valor))
            {
                MessageBox.Show("Valor inválido.");
                return;
            }

            try
            {
                await _contaService.DepositarAsync(_contaId, valor);

                MessageBox.Show($"Depósito de {valor:C} realizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
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

        private void Valor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = MoedaHelper.EhTextoInvalido(e.Text);
        }
    }
}