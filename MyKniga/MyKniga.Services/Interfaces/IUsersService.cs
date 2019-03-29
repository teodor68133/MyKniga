namespace MyKniga.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<string> GetPublisherIdByUserNameAsync(string userName);
    }
}