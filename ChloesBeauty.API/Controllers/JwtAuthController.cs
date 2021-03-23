using ChloesBeauty.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChloesBeauty.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtAuthController : ControllerBase
    {
        #region Private Fields

        private readonly IConfiguration _configuration;

        #endregion Private Fields

        #region Public Constructors

        public JwtAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Methods

        [AllowAnonymous]
        [HttpPost]
        [Route("RequestToken")]
        public JsonResult RequestToken(UserViewModel user)
        {
            DateTime utcNow = DateTime.UtcNow;

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            DateTime expiredDateTime = utcNow.AddDays(1);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey")));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            string token = jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(claims: claims, expires: expiredDateTime, notBefore: utcNow, signingCredentials: signingCredentials));

            return new JsonResult(new { token });
        }

        #endregion Public Methods
    }
}