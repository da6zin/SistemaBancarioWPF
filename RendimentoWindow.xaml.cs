using System.Windows;

namespace SistemaBancarioSimples
{
    public partial class RendimentoWindow : Window
    {
        public RendimentoWindow()
        {
            InitializeComponent();
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}