using System.Threading.Tasks;
using SistemaBancarioSimples.Model;

namespace SistemaBancarioSimples.Service
{
    public interface IAuthService
    {
        Task<Usuario> CadastrarAsync(string username, string password);
        Task<Usuario> LoginAsync(string username, string password);
    }
}