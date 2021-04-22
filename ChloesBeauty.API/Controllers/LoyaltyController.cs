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

        public LoyaltyController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool LoyaltyExists(int id)
        {
            return _context.Loyalties.Any(e => e.LoyaltyId == id);
        }

        #endregion Private Methods

        #region Public Methods

        // DELETE: api/Loyalty/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteLoyalty(int id)
        {
            var loyalty = await _context.Loyalties.FindAsync(id);
            if (loyalty == null)
            {
                return NotFound(false);
            }

            loyalty.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        // GET: api/Loyalty
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoyaltiesViewModel>>> GetLoyalties()
        {
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
            return await _context.Loyalties
                .Include(l => l.Treatment)
                .Where(l => !l.Deleted)
                .ToListAsync();
        }

        // GET: api/Loyalty/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Loyalty>> GetLoyalty(int id)
        {
            var loyalty = await _context.Loyalties.FindAsync(id);

            if (loyalty == null)
            {
                return NotFound();
            }

            return loyalty;
        }

        // POST: api/Loyalty To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Loyalty>> PostLoyalty(Loyalty loyalty)
        {
            try
            {
                loyalty.Deleted = false;
                loyalty.ModifiedDate = DateTime.Now;
                _context.Loyalties.Add(loyalty);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetLoyalty", new { id = loyalty.LoyaltyId }, loyalty);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        // PUT: api/Loyalty/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoyalty(int id, Loyalty loyalty)
        {
            if (id != loyalty.LoyaltyId)
            {
                return BadRequest();
            }

            loyalty.Deleted = false;
            loyalty.ModifiedDate = DateTime.Now;
            _context.Entry(loyalty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
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