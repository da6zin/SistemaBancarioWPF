using SistemaBancarioSimples.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SistemaBancarioSimples.Helpers; // Importa nosso novo ajudante

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

            if (!MoedaHelper.TentarConverter(txtValor.Text, out decimal valor))
            {
                MessageBox.Show("Valor inválido.");
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
            // Olha como ficou legível! Não tem mais Regex espalhado no código.
            e.Handled = MoedaHelper.EhTextoInvalido(e.Text);
        }
    }
}