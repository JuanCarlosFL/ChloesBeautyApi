using ChloesBeauty.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        // Inyectamos en el constructor la dependencia para poder acceder a los datos guardados en el appsettings.json
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
            //Generamos una lista claims que nos serviran para la metadata del token
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            // Damos un día de validez al token
            DateTime expiredDateTime = utcNow.AddDays(1);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //Usamos una llave secreta que está almacenada en el appsettings
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey")));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            //Generamos el token con la metadata, fecha expiración, etc..
            string token = jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(claims: claims, expires: expiredDateTime, notBefore: utcNow, signingCredentials: signingCredentials));
            // Y lo devolvemos
            return new JsonResult(new { token });
        }

        #endregion Public Methods
    }
}