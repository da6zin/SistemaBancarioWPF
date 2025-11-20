using System.Text.RegularExpressions;

namespace SistemaBancarioSimples.Helpers
{
    public static class MoedaHelper
    {
        public static bool TentarConverter(string texto, out decimal resultado)
        {
            resultado = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            string textoAjustado = texto.Replace(".", ",");

            bool sucesso = decimal.TryParse(textoAjustado, out decimal valor);

            if (sucesso && valor > 0)
            {
                resultado = valor;
                return true;
            }

            return false;
        }
        public static bool EhTextoInvalido(string textoDigitado)
        {
            Regex regex = new Regex("[^0-9,.]+");
            return regex.IsMatch(textoDigitado);
        }
    }
}