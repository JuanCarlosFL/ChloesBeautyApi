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
        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
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
            try
            {
                //Comprobamos si el nombre viene vacío y si es así devolvemos un bad request
                if (String.IsNullOrEmpty(model.Name))
                    return BadRequest(false);

                //Comprobamos si el usuario ya existe
                var userFound = await _context.Users.Where(u => u.UserName == model.Email).FirstOrDefaultAsync();

                //Si existe devolvemos un bad request
                if (userFound != null)
                    return BadRequest(false);

                //Creamos los datos not null de la tabla persona
                var person = new Person
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    Points = 0,
                    Deleted = false,
                    ModifiedDate = DateTime.Now
                };

                //Añadimos la persona y guardamos los datos
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                //Creamos los datos obligatorios de la tabla user
                var user = new User
                {
                    UserName = model.Email,
                    Password = Functions.Encrypt(model.Password),
                    PersonId = person.PersonId,
                    IsActive = true,
                    ModifiedDate = DateTime.Now
                };

                //Añadimos el usuario y guardamos los datos
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                //Guardamos en la tabla userRole el rol del usuario que acabamos de crear
                var userRole = new UsersRole
                {
                    UserId = person.PersonId,
                    RoleId = (byte)Enums.Roles.Client,
                    ModifiedDate = DateTime.Now
                };

                //Añadimos los datos y los guardamos
                _context.UsersRoles.Add(userRole);
                await _context.SaveChangesAsync();

                //Devolvemos un 200 ya que ha ido todo correctamente
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAsync()
        {
            //Creamos una lista de usuarios vacía
            var users = new List<User>();

            try
            {
                //Traemos los usuarios de la base de datos incluyendo sus datos de la tabla person y los almacenamos en la lista
                users = await _context.Users.Include(u => u.Person).ToListAsync();
            }
            catch (Exception ex)
            {
                //Si hay algún error devolvemos un bad request con el mensaje de error
                return BadRequest(ex);
            }
            //Si todo ha ido bien devolvemos la lista de usuarios
            return Ok(users);
        }

        [HttpGet]
        [Route("Get/{username}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAsync(string username)
        {
            //Instanciamos un usuario
            var user = new User();

            try
            {
                //Si el usuario no viene vacío
                if (!String.IsNullOrEmpty(username))
                {
                    //Devolvemos el usuario inluyendo sus datos de la tabla person que coincida con el username
                    user = await _context.Users.Include(u => u.Person).Where(u => u.UserName == username).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                //Si hay algún error devolvemos un bad request con el mensaje de error
                return BadRequest(ex);
            }
            //Si todo ha ido bien devolvemos el usuario
            return Ok(user);
        }

        [HttpGet]
        [Route("GetRole/{username}")]
        public ActionResult<IEnumerable<UsersRole>> GetRole(string username)
        {
            //Como un usuario puede tener varios roles creamos una lista de strings
            IEnumerable<string> roles = new List<string>();

            try
            {
                //Si el usuario no viene vacío
                if (!String.IsNullOrEmpty(username))
                {
                    //Buscamos el usario incluyendo los datos de las tablas role y userroles y devolvemos los nombres de los roles que tenga
                    roles = _context.Users
                        .Include(u => u.UsersRoles)
                        .ThenInclude(r => r.Role)
                        .Where(u => u.UserName == username)
                        .Select(r => r.UsersRoles.Select(r => r.Role.Name))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //Si hay algún error devolvemos un bad request con el mensaje de error
                return BadRequest(ex);
            }
            //Si el usuario no se ha encontrado devolvemos un 404 sino delvovemos un 200 con los datos
            return roles == null ? NotFound() : Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> IsValidLoginAsync([FromBody] UserViewModel user)
        {
            //Si el nombre del usuario viene vacío devolvemos un 400
            if (String.IsNullOrEmpty(user.UserName))
            {
                return BadRequest();
            }

            //Buscamos en la bbdd un usuario que coincida con el nombre recibido
            var userFound = await _context.Users.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();

            //Si no es null y concide la contraseña recibida con la de la bbdd devolvemos un 200 con los datos del usuario
            if ((userFound != null) && (Functions.Decrypt(userFound.Password) == user.Password))
                return Ok(true);
            else
                //Sino devolvemos un 401
                return Unauthorized();
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<ActionResult<bool>> RecoverPassword(RecoverPasswordViewModel model)
        {
            //Si el usuario vivene vacío devolvemos un 400
            if (String.IsNullOrEmpty(model.UserName))
                return BadRequest(false);

            //Buscamos un usuario en la bbdd que coincida con el nombre recibido
            var userFound = await _context.Users.Where(u => u.UserName == model.UserName).FirstOrDefaultAsync();

            //si el usuario es nulo es que no existe y devolvemos un 400 con el mensaje
            if (userFound == null)
                return BadRequest($"El usuario {model.UserName} no existe");

            //Almacenamos la nueva contraseña y la fecha de modificación
            userFound.Password = Functions.Encrypt(model.Password);
            userFound.ModifiedDate = DateTime.Now;

            //Guardamos los datos
            await _context.SaveChangesAsync();
            //Devolvemos un 200 con un booleano a true
            return Ok(true);
        }

        #endregion Public Methods
    }
}