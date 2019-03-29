namespace MyKniga.Services
{
    using System.Threading.Tasks;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class UsersService : BaseService, IUsersService
    {
        public UsersService(MyKnigaDbContext context) : base(context)
        {
        }

        public async Task<string> GetPublisherIdByUserNameAsync(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            return user?.PublisherId;
        }
    }
}