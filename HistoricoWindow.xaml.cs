using SistemaBancarioSimples.Service;
using System.Windows;

namespace SistemaBancarioSimples
{
    public partial class HistoricoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;

        public HistoricoWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;

            // Chama o carregamento assim que a janela abre
            CarregarHistorico();
        }

        private async void CarregarHistorico()
        {
            try
            {
                // AGORA SIM: Usamos o método específico que criamos
                var transacoes = await _contaService.GetHistoricoAsync(_contaId);

                // Joga a lista direto no Grid
                TransacoesDataGrid.ItemsSource = transacoes;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}");
            }
        }

        private void FecharButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}