using Contact_management_devextreme.DTOs;
using Contact_management_devextreme.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Contact_management_devextreme.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ContactDbContext context;
        private readonly JWTSettings setting;
        private readonly IRefreshTokenGenerator tokenGenerator;

        public UserController(ContactDbContext dbContext, IOptions<JWTSettings> options, IRefreshTokenGenerator generator) 
        {
            context = dbContext;
            setting = options.Value;
            tokenGenerator = generator;
        }

        [NonAction]
        public TokenResponse Authenticate(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenhandler = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(username);
            return tokenResponse;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCred users)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var _user = context.Users.FirstOrDefault(o => o.Username == users.username && o.Password == users.password);
            if(_user == null)
            {
                return Unauthorized();
            }

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(setting.securitykey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, users.username),
                    }
                    ),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescription);
            string finaltoken = tokenhandler.WriteToken(token);

            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = tokenGenerator.GenerateToken(users.username);
            return Ok(tokenResponse);
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.JWTToken);
            var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;


            //var username = principal.Identity.Name;
            var _reftable = context.RefreshTokens.FirstOrDefault(o => o.Username == username && o.RefreshToken == token.RefreshToken);
            if (_reftable == null)
            {
                return Unauthorized();
            }
            TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
            return Ok(_result);
            //var tokenhandler = new JwtSecurityTokenHandler();
            //SecurityToken securityToken;
            //var principal = tokenhandler.ValidateToken(token.JWTToken, new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.securitykey)),
            //    ValidateIssuer = false,
            //    ValidateAudience = false
            //}, out securityToken);
            //var _token = securityToken as JwtSecurityToken;
            //if(_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)) 
            //{
            //    return Unauthorized();
            //}
            //var username = principal.Identity.Name;
            //var _reftable = context.RefreshTokens.FirstOrDefault(o => o.Username == username &&  o.RefreshToken == token.RefreshToken);
            //if(_reftable == null)
            //{
            //    return Unauthorized();
            //}

            //TokenResponse _result = Authenticate(username, principal.Claims.ToArray());
            //return Ok(_result);
        }

    }

}
