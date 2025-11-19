using SistemaBancarioSimples.Service;
using System.Windows;
using System.Threading.Tasks;

namespace SistemaBancarioSimples
{
    /// <summary>
    /// Lógica interna para HistoricoWindow.xaml
    /// </summary>
    public partial class HistoricoWindow : Window
    {
        private readonly IContaService _contaService;
        private readonly int _contaId;

        public HistoricoWindow(IContaService contaService, int contaId)
        {
            InitializeComponent();
            _contaService = contaService;
            _contaId = contaId;

            // Carrega os dados quando a janela é aberta
            Loaded += HistoricoWindow_Loaded;
        }

        private async void HistoricoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarHistoricoAsync();
        }

        private async Task CarregarHistoricoAsync()
        {
            try
            {
                var historico = await _contaService.GetHistoricoAsync(_contaId);
                // Define a fonte de dados do DataGrid
                TransacoesDataGrid.ItemsSource = historico;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
