using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SistemaBancarioSimples.Helpers;

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
            if (!MoedaHelper.TentarConverter(txtValor.Text, out decimal valor))
            {
                MessageBox.Show("Por favor, insira um valor válido para saque.");
                return;
            }

            try
            {
                await _contaService.SacarAsync(_contaId, valor);

                MessageBox.Show($"Saque de {valor:C} realizado. Retire o dinheiro.", "Sucesso");
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Não foi possível sacar: {ex.Message}", "Atenção");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao sacar: {ex.Message}");
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