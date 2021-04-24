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

        public AppointmentController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        #endregion Private Methods

        #region Public Methods

        // DELETE: api/Appointment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentsViewModel>>> GetAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Availability)
                .Include(t => t.Treatment)
                .Include(p => p.Person)
                .Where(a => !a.Deleted && a.Availability.Date.Date >= DateTime.Today)
                .Select(a => new AppointmentsViewModel { AppointmentDate = a.Availability.Date, PersonName = $"{a.Person.Name} {a.Person.Surname}" , TreatmentName = a.Treatment.Name} )
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        // GET: api/Appointment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TreatmentsByPersonViewModel>>> GetAppointmentsByPerson(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Treatment)
                .Include(a => a.Availability)
                .Where(a => a.PersonId == id && !a.Deleted)
                .Select(a => new TreatmentsByPersonViewModel { Name = a.Treatment.Name, Date = a.Availability.Date })
                .ToListAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // POST: api/Appointment To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            appointment.Deleted = false;
            appointment.ModifiedDate = DateTime.Now;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }

        // PUT: api/Appointment/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
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