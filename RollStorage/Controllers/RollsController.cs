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

        // Получение списка рулонов со склада с фильтрацией 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roll>>> GetRolls([FromQuery] RollFiltersDto filter)
        {
            try
            {
                var rolls = await _rollService.GetAllRollsAsync(filter);
                return rolls;
            }
            catch (ArgumentException ex) 
            { 
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Получение по id
        [HttpGet("{id}")]
        public async Task<ActionResult<Roll>> GetRoll(int id)
        {
            try
            {
                var roll = await _rollService.GetRollByIdAsync(id);
                return Ok(roll);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Добавление нового рулона на склад
        [HttpPost]
        public async Task<ActionResult<Roll>> PostRoll(CreateRollDto createRoll)
        {
            var roll = await _rollService.AddRollAsync(createRoll);
            return CreatedAtAction(nameof(GetRoll), new { id = roll.Id }, roll);
        }

        //Удаление рулона с указанным id со склада
        [HttpPut]
        public async Task<ActionResult<Roll>> PutRoll(int id, [FromBody] DateTime removeAt)
        {
            try
            {
                var result = await _rollService.RemoveRollAsync(id, removeAt);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
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
