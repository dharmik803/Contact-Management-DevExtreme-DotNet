using Contact_management_devextreme.Models;
using System.Security.Cryptography;
using Contact_management_devextreme.Models;

namespace Contact_management_devextreme
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly ContactDbContext _context;

        public RefreshTokenGenerator(ContactDbContext context)
        {
            _context = context;
        }
        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using(var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken = Convert.ToBase64String(randomnumber);

                var _user = _context.RefreshTokens.FirstOrDefault(o => o.Username == username);
                if(_user != null)
                {
                    _user.RefreshToken = RefreshToken;
                    _context.SaveChanges();
                }
                else
                {
                    RefreshTokens refreshTokens = new RefreshTokens()
                    {
                        Username = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = RefreshToken,
                        IsActive = true
                    };
                }
                return RefreshToken;
            }
        }
    }
}
