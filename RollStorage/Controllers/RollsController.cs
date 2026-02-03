using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RollStorage.Models;
using RollStorage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RollStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RollsController : ControllerBase
    {
        private readonly RollService _rollService;

        public RollsController(RollService rollService)
        {
            _rollService = rollService;
        }

        // GET: api/Rolls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roll>>> GetRolls()
        {
            var rolls = await _rollService.GetAllRollsAsync();
            return Ok(rolls);
        }

        // GET: api/Rolls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Roll>> GetRoll(int id)
        {
            var rolls = await _rollService.GetRollByIdAsync(id);
            if (rolls == null)
                return NotFound();
            return Ok(rolls);
        }

        // Добавление нового рулона на склад
        [HttpPost]
        public async Task<ActionResult<Roll>> PostRoll(Roll roll)
        {
            await _rollService.AddRollAsync(roll);
            return CreatedAtAction(nameof(GetRoll), new { id = roll.Id }, roll);
        }

        //Удаление рулона с указанным id со склада
        [HttpPut]
        public async Task<ActionResult<Roll>> PutRoll(int id, [FromBody] DateTime removeAt)
        {
            var roll = await _rollService.GetRollByIdAsync(id);
            if (roll == null)
                return NotFound();

            roll.RemoveAt = removeAt;
            await _rollService.UpdateRollAsync(roll);
            return Ok(roll);
        }

        //Получение статистики по рулонам
        [HttpGet("statistics")]
        public async Task<ActionResult<RollStatisticsDto>> GetRollStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var statistics = await _rollService.GetStatisticsAsync(startDate, endDate);

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
