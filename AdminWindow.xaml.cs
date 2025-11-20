using SistemaBancarioSimples.DTOs;
using SistemaBancarioSimples.Service;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SistemaBancarioSimples
{
    public partial class AdminWindow : Window
    {
        private readonly IContaService _contaService;
        private List<ContaDTO> _todasContas; // Guarda a lista completa na memória

        public AdminWindow(IContaService contaService)
        {
            InitializeComponent();
            _contaService = contaService;
            CarregarDados();
        }

        private async void CarregarDados()
        {
            // Busca todos os clientes do banco
            _todasContas = await _contaService.GetTodasContasAsync();

            // Joga na tela
            GridClientes.ItemsSource = _todasContas;
        }

        // FILTRO EM TEMPO REAL
        private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBusca.Text.ToLower();

            // Filtra a lista original por Nome ou Número da conta
            var listaFiltrada = _todasContas.Where(c =>
                c.NomeTitular.ToLower().Contains(texto) ||
                c.Numero.Contains(texto)
            ).ToList();

            GridClientes.ItemsSource = listaFiltrada;
        }

        // QUANDO CLICA EM UM CLIENTE
        private async void GridClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verifica qual linha foi clicada
            if (GridClientes.SelectedItem is ContaDTO contaSelecionada)
            {
                // Busca o histórico daquela conta específica
                var historico = await _contaService.GetHistoricoAsync(contaSelecionada.Id);
                GridHistorico.ItemsSource = historico;
            }
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}