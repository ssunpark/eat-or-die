using System.Threading.Tasks;

public interface IAuthenticator
{
    Task<AuthResultWrapper> CreateAccountAsync(string email, string password);
    Task<AuthResultWrapper> SignInAsync(string email, string password);
}