using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using ParkyAPI.Data;
using Microsoft.Extensions.Options;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _dBContext;
        private readonly AppSettings _appSettings;

        public UserRepository(ApplicationDBContext dBContext, IOptions<AppSettings> appSettings)
        {
            this._dBContext = dBContext;
            this._appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _dBContext.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            //user not exists
            if (user == null)
            {
                return null;
            }

            //if user found generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";
            return user;

        }

        public bool IsUniqueUser(string username)
        {
            var user = _dBContext.Users.SingleOrDefault(q => q.Username == username);

            //Returns null if user not found
            if (user == null)
                return true;

            return false;
        }

        public User Register(string username, string password)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Role = "User"
            };

            _dBContext.Users.Add(user);
            _dBContext.SaveChanges();

            user.Password = "";

            return user;
        }
    }
}
