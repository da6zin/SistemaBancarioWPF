using SistemaBancarioSimples.Service;
using System;
using System.Windows;

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
            if (!decimal.TryParse(txtValorTransferencia.Text, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Valor inválido.");
                return;
            }

            string contaDestino = txtContaDestino.Text;

            try
            {
                // Você precisará criar esse método 'TransferirAsync' no seu Service
                // await _contaService.TransferirAsync(_contaOrigemId, contaDestino, valor);

                MessageBox.Show($"Transferência de {valor:C} para conta {contaDestino} realizada!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}