using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using ChloesBeauty.API.ViewModels;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors

        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
        public AppointmentController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool AppointmentExists(int id)
        {
            //Devolvemos true si existe alguna cita con ese id, false en caso contrario
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        #endregion Private Methods

        #region Public Methods

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            //Buscamos los datos de la cita con ese id
            var appointment = await _context.Appointments.FindAsync(id);
            //Si es null devolvemos un 404
            if (appointment == null)
            {
                return NotFound();
            }
            //Si se ha encontrado ponemos el campo deleted a 1 para hacer un borrado lógico y guardamos los datos
            appointment.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentsViewModel>>> GetAppointments()
        {
            // Devolvemos todas las citas haciendo inner join con las tablas de disponibilidad, tratamientos y personas
            return await _context.Appointments
                .Include(a => a.Availability)
                .Include(t => t.Treatment)
                .Include(p => p.Person)
                //Buscamos las que no estén borradas y la fecha sea mayor o igual que la fecha actual
                .Where(a => !a.Deleted && a.Availability.Date.Date >= DateTime.Today)
                //Devolvemos sólo la fecha de la cita, el nombre de la persona y el nombre del tratamiento
                .Select(a => new AppointmentsViewModel { AppointmentDate = a.Availability.Date, PersonName = $"{a.Person.Name} {a.Person.Surname}" , TreatmentName = a.Treatment.Name} )
                //Ordenado por la fecha de la cita
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TreatmentsByPersonViewModel>>> GetAppointmentsByPerson(int id)
        {
            //Devolvemos la fecha de la cita y el nombre del tratamiento de una cita por su id si no está borrada
            var appointment = await _context.Appointments
                .Include(a => a.Treatment)
                .Include(a => a.Availability)
                .Where(a => a.PersonId == id && !a.Deleted)
                .Select(a => new TreatmentsByPersonViewModel { Name = a.Treatment.Name, Date = a.Availability.Date })
                .ToListAsync();

            if (appointment == null)
            {
                //Si no se ha encontrado devolvemos un 404
                return NotFound();
            }
            //Si todo ha ido bien devolvemos los datos de la cita
            return appointment;
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            try
            {
                //Buscamos los datos de la tabla persona que tenga el personId recibido
                var person = await _context.Persons.Where(p => p.PersonId == appointment.PersonId).FirstOrDefaultAsync();
                //Buscamos los datos del tratamiento que tenga el treatmentId recibido
                var treatment = await _context.Treatments.Where(t => t.TreatmentId == appointment.TreatmentId).FirstOrDefaultAsync();
                //Sumamos a los puntos del usuario los puntos del tratamiento seleccionado
                person.Points += (int)treatment.Points;

                // modificamos los datos necesarios y guardamos los datos
                appointment.Deleted = false;
                appointment.ModifiedDate = DateTime.Now;
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                //Devolvemos un 200 
                return Ok(true);
            }
            catch (Exception ex)
            {
                //Si hay algún error devolvemos un bad request con el mensaje de error
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            //Si el id no corresponde con el id de la cita devolvemos un 400
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }
            appointment.ModifiedDate = DateTime.Now;
            // Ponemos el flag del estado a modificado
            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                //Guardamos los datos para que se actualice
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si ha habido algún problema comprobamos si la cita existe para manejar el error
                if (!AppointmentExists(id))
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