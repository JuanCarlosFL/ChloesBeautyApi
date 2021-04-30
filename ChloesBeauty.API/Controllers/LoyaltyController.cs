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
    public class LoyaltyController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors
        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
        public LoyaltyController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool LoyaltyExists(int id)
        {
            //Devolvemos true si existe alguna oferta con ese id, false en caso contrario
            return _context.Loyalties.Any(e => e.LoyaltyId == id);
        }

        #endregion Private Methods

        #region Public Methods

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteLoyalty(int id)
        {
            //Buscamos los datos de la oferta con ese id
            var loyalty = await _context.Loyalties.FindAsync(id);
            //Si es null devolvemos un 404
            if (loyalty == null)
            {
                return NotFound(false);
            }
            //Si se ha encontrado ponemos el campo deleted a 1 para hacer un borrado lógico y guardamos los datos
            loyalty.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoyaltiesViewModel>>> GetLoyalties()
        {
            // Este método se usa para devolver el nombre del tratamiento y los puntos para la lista de ofertas del lado cliente
            return await _context.Loyalties
                .Include(l => l.Treatment)
                .Where(l => !l.Deleted)
                .Select(l => new LoyaltiesViewModel { Name = l.Treatment.Name, Points = l.Points })
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetAllLoyalties")]
        public async Task<ActionResult<IEnumerable<Loyalty>>> GetAllLoyalties()
        {
            // Este método devuelve una lista de todas las ofertas que no están borradas incluyendo los datos del tratamiento en la oferta
            return await _context.Loyalties
                .Include(l => l.Treatment)
                .Where(l => !l.Deleted)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Loyalty>> GetLoyalty(int id)
        {
            // Buscamos en la base de datos una oferta con el id que se recibe por parámetro
            var loyalty = await _context.Loyalties.FindAsync(id);
            // Si es null devolvemos un 404
            if (loyalty == null)
            {
                return NotFound();
            }
            // Si se ha encontrado devolvemos la oferta
            return loyalty;
        }

        [HttpPost]
        public async Task<ActionResult<Loyalty>> PostLoyalty(Loyalty loyalty)
        {
            try
            {
                // modificamos los datos necesarios y guardamos los datos
                loyalty.Deleted = false;
                loyalty.ModifiedDate = DateTime.Now;
                _context.Loyalties.Add(loyalty);
                await _context.SaveChangesAsync();

                return Ok(true);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoyalty(int id, Loyalty loyalty)
        {
            //Si el id no corresponde con el id de la oferta devolvemos un 400
            if (id != loyalty.LoyaltyId)
            {
                return BadRequest();
            }

            loyalty.ModifiedDate = DateTime.Now;
            // Ponemos el flag del estado a modificado
            _context.Entry(loyalty).State = EntityState.Modified;

            try
            {
                //Guardamos los datos para que se actualice
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si ha habido algún problema comprobamos si la cita existe para manejar el error
                if (!LoyaltyExists(id))
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