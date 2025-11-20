using System.Text.RegularExpressions;

namespace SistemaBancarioSimples.Helpers
{
    // "static" significa que não precisamos dar "new MoedaHelper()" para usar.
    // Podemos chamar direto: MoedaHelper.Metodo()
    public static class MoedaHelper
    {
        /// <summary>
        /// Tenta converter uma string para decimal, tratando pontos e vírgulas.
        /// Retorna TRUE se conseguiu e devolve o valor na variável 'resultado'.
        /// </summary>
        public static bool TentarConverter(string texto, out decimal resultado)
        {
            resultado = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            // A lógica centralizada: troca ponto por vírgula
            string textoAjustado = texto.Replace(".", ",");

            // Tenta converter e verifica se é positivo
            bool sucesso = decimal.TryParse(textoAjustado, out decimal valor);

            if (sucesso && valor > 0)
            {
                resultado = valor;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifica se o caractere digitado é válido para dinheiro (apenas números, ponto e vírgula).
        /// Usado no evento PreviewTextInput.
        /// </summary>
        public static bool EhTextoInvalido(string textoDigitado)
        {
            // A lógica do Regex centralizada aqui
            Regex regex = new Regex("[^0-9,.]+");
            return regex.IsMatch(textoDigitado);
        }
    }
}