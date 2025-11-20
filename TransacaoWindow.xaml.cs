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
            // 1. Tratamento robusto para valor (troca ponto por vírgula se necessário)
            string valorTexto = txtValorTransferencia.Text.Replace(".", ",");

            if (!decimal.TryParse(valorTexto, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Valor inválido. Digite um valor numérico positivo.");
                return;
            }

            // 2. Pega o NOME DO USUÁRIO (já que seu modelo usa Usuario e não Numero da Conta)
            string usernameDestino = txtContaDestino.Text;

            if (string.IsNullOrWhiteSpace(usernameDestino))
            {
                MessageBox.Show("Por favor, digite o nome do usuário que receberá a transferência.");
                return;
            }

            try
            {
                // 3. AQUI A MÁGICA ACONTECE: Chama o serviço real
                // Note que passamos 'usernameDestino' (string) em vez de número de conta
                await _contaService.TransferirAsync(_contaOrigemId, usernameDestino, valor);

                // 4. Feedback e Fechamento
                MessageBox.Show($"Transferência de {valor:C} para {usernameDestino} realizada com sucesso!", "Sucesso");
                this.Close();
            }
            catch (Exception ex)
            {
                // Se o usuário não existir ou não tiver saldo, o erro aparecerá aqui
                MessageBox.Show($"Não foi possível transferir: {ex.Message}", "Erro");
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}