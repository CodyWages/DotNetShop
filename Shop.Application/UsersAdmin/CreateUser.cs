using Shop.Domain.Infrastructure;

namespace Shop.Application.UsersAdmin
{
    public class CreateUser
    {
        private IUserManager _userManager;

        public CreateUser(IUserManager userManager) 
        { 
            _userManager = userManager; 
        }

        public class Request 
        {
            public string Username { get; set; } 
            public string Password { get; set; }    
        }

        public async Task<bool> Do(Request request)
        {
            await _userManager.CreateManagerUser(request.Username, request.Password);
            return true;
        }
    }
}
