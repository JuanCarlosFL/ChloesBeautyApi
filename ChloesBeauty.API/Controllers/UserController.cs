using ChloesBeauty.API.Helpers;
using ChloesBeauty.API.Models;
using ChloesBeauty.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors

        public UserController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        [HttpPost]
        [Route("CreateUser")]
        public async Task<ActionResult<bool>> CreateUserAsync([FromBody] RegisterViewModel model)
        {
            if (String.IsNullOrEmpty(model.Name))
                return BadRequest(false);

            var userFound = await _context.Users.Where(u => u.UserName == model.Email).FirstOrDefaultAsync();

            if (userFound != null)
                return BadRequest(false);

            var person = new Person
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Points = 0,
                Deleted = false,
                ModifiedDate = DateTime.Now
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = model.Email,
                Password = Functions.Encrypt(model.Password),
                PersonId = person.PersonId,
                IsActive = true,
                ModifiedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAsync()
        {
            var users = new List<User>();

            try
            {
                users = await _context.Users.Include(u => u.Person).ToListAsync();
            }
            catch (Exception ex)
            {
                // to do
            }

            return Ok(users);
        }

        [HttpGet]
        [Route("Get/{username}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAsync(string username)
        {
            var user = new User();

            try
            {
                if (!String.IsNullOrEmpty(username))
                {
                    user = await _context.Users.Include(u => u.Person).Where(u => u.UserName == username).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                // to do
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> IsValidLoginAsync([FromBody] UserViewModel user)
        {
            if (String.IsNullOrEmpty(user.UserName))
            {
                return BadRequest();
            }

            var userFound = await _context.Users.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();

            if ((userFound != null) && (Functions.Decrypt(userFound.Password) == user.Password))
                return Ok(true);
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<ActionResult<bool>> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (String.IsNullOrEmpty(model.UserName))
                return BadRequest(false);

            var userFound = await _context.Users.Where(u => u.UserName == model.UserName).FirstOrDefaultAsync();

            if (userFound == null)
                return BadRequest($"El usuario {model.UserName} no existe");

            userFound.Password = Functions.Encrypt(model.Password);

            await _context.SaveChangesAsync();

            return Ok(true);
        }

        #endregion Public Methods
    }
}