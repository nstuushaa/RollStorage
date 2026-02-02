using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RollStorage.Models;

namespace RollStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RollsController : ControllerBase
    {
        private readonly RollContext _context;

        public RollsController(RollContext context)
        {
            _context = context;
        }

        // GET: api/Rolls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roll>>> GetRolls()
        {
            return await _context.Rolls.ToListAsync();
        }

        // GET: api/Rolls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Roll>> GetRoll(int id)
        {
            var roll = await _context.Rolls.FindAsync(id);

            if (roll == null)
            {
                return NotFound();
            }

            return roll;
        }

        // Добавление нового рулона на склад
        [HttpPost]
        public async Task<ActionResult<Roll>> PostRoll(Roll roll)
        {
            _context.Rolls.Add(roll);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoll", new { id = roll.Id }, roll);
        }


        private bool RollExists(int id)
        {
            return _context.Rolls.Any(e => e.Id == id);
        }
    }
}
