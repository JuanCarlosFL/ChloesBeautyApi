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
    public class TreatmentController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors
        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
        public TreatmentController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods
        //Devolvemos true si existe algún tratamiento con ese id, false en caso contrario
        private bool TreatmentExists(int id)
        {
            return _context.Treatments.Any(e => e.TreatmentId == id);
        }

        #endregion Private Methods

        #region Public Methods

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTreatment(int id)
        {
            //Buscamos los datos del tratamiento con ese id
            var treatment = await _context.Treatments.FindAsync(id);
            //Si es null devolvemos un 404
            if (treatment == null)
            {
                return NotFound();
            }
            //Si se ha encontrado ponemos el campo deleted a 1 para hacer un borrado lógico y guardamos los datos
            treatment.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Treatment>> GetTreatment(int id)
        {
            // Buscamos en la base de datos un tratamiento con el id que se recibe por parámetro
            var treatment = await _context.Treatments.FindAsync(id);
            // Si es null devolvemos un 404
            if (treatment == null)
            {
                return NotFound();
            }
            // Si se ha encontrado devolvemos el tratamiento
            return treatment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Treatment>>> GetTreatments()
        {
            // Devolvemos todos los tratamientos que no se hayan borrado
            return await _context.Treatments.Where(t => !t.Deleted).ToListAsync();
        }

        [HttpGet]
        [Route("GetTreatmentsForAppointment")]
        // Este método devuelve los datos que necesitamos para la modal de seleccionar cita del cliente
        public async Task<ActionResult<IEnumerable<TreatmentsForAppointmentViewModel>>> GetTreatmentsForAppointment()
        {
            // Buscamos todos los tratamientos
            return await _context.Treatments
                // Que no estén borrados
                .Where(t => !t.Deleted)
                // Seleccionamos el id, nombre, duración, precio y puntos
                .Select(t => new TreatmentsForAppointmentViewModel { Id = t.TreatmentId, Name = t.Name, Duration = t.Duration, Price = t.Price, Points = t.Points })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Treatment>> PostTreatment(Treatment treatment)
        {
            // modificamos los datos necesarios y guardamos los datos
            treatment.Deleted = false;
            treatment.ModifiedDate = DateTime.Now;
            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        // PUT: api/Treatment/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTreatment(int id, Treatment treatment)
        {
            //Si el id no corresponde con el id del tratamiento devolvemos un 400
            if (id != treatment.TreatmentId)
            {
                return BadRequest();
            }

            treatment.ModifiedDate = DateTime.Now;
            // Ponemos el flag del estado a modificado
            _context.Entry(treatment).State = EntityState.Modified;

            try
            {
                //Guardamos los datos para que se actualice
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si ha habido algún problema comprobamos si la cita existe para manejar el error
                if (!TreatmentExists(id))
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