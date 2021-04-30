using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using System;
using ChloesBeauty.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors
        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
        public PersonController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool PersonExists(int id)
        {
            //Devolvemos true si existe alguna persona con ese id, false en caso contrario
            return _context.Persons.Any(e => e.PersonId == id);
        }

        #endregion Private Methods

        #region Public Methods

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePerson(int id)
        {
            //Buscamos los datos de la persona con ese id
            var person = await _context.Persons.FindAsync(id);
            //Si es null devolvemos un 404
            if (person == null)
            {
                return NotFound(false);
            }

            //Si se ha encontrado ponemos el campo deleted a 1 para hacer un borrado lógico y guardamos los datos
            person.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            // Buscamos en la base de datos una persona con el id que se recibe por parámetro
            var person = await _context.Persons.FindAsync(id);
            // Si es null devolvemos un 404
            if (person == null)
            {
                return NotFound();
            }
            // Si se ha encontrado devolvemos la persona
            return person;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            // Devolvemos todas las personas que no se hayan borrado
            return await _context.Persons.Where(p => !p.Deleted).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            // Buscamos si existe una persona con el email recibido por parámetros
            var personFound = await _context.Persons.Where(p => p.Email == person.Email).FirstOrDefaultAsync();
            // Si existe devolvemos un 400
            if (personFound != null)
                return BadRequest(false);

            // Si no existe modificamos los datos necesarios y guardamos los cambios
            person.Deleted = false;
            person.ModifiedDate = DateTime.Now;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            // Ahora creamos un usuario y le ponemos una contraseña temporal para que sea el usuario el que la cambie
            var user = new User
            {
                UserName = person.Email,
                Password = Functions.Encrypt("Temporal"),
                PersonId = person.PersonId,
                IsActive = true,
                ModifiedDate = DateTime.Now
            };
            // Guardamos los datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Creamos un rol de cliente para la nueva persona
            var userRole = new UsersRole
            {
                UserId = person.PersonId,
                RoleId = (byte)Enums.Roles.Client,
                ModifiedDate = DateTime.Now
            };

            // Guardamos los cambios
            _context.UsersRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            //Si el id no corresponde con el id de la persona devolvemos un 400
            if (id != person.PersonId)
            {
                return BadRequest();
            }

            person.ModifiedDate = DateTime.Now;
            // Ponemos el flag del estado a modificado
            _context.Entry(person).State = EntityState.Modified;

            try
            {
                //Guardamos los datos para que se actualice
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si ha habido algún problema comprobamos si la cita existe para manejar el error
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        #endregion Public Methods
    }
}