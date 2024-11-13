using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<TablesController> _logger;

        public TablesController(BeanSceneContext context, ILogger<TablesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Table>>> GetTables()
        {
            try
            {
                var tables = await _context.Tables.ToListAsync();
                return Ok(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tables");
                return StatusCode(500, new { message = "Error retrieving tables", error = ex.Message });
            }
        }

        // GET: api/tables/area/{area}
        [HttpGet("area/{area}")]
        public async Task<ActionResult<IEnumerable<Table>>> GetTablesByArea(string area)
        {
            try
            {
                var tables = await _context.Tables
                    .Where(t => t.Area == area)
                    .ToListAsync();

                if (!tables.Any())
                {
                    return NotFound(new { message = $"No tables found in area: {area}" });
                }

                return Ok(tables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tables by area");
                return StatusCode(500, new { message = "Error retrieving tables", error = ex.Message });
            }
        }

        // POST: api/tables
        [HttpPost]
        public async Task<ActionResult<Table>> CreateTable(TableCreateDto dto)
        {
            try
            {
                var existingTable = await _context.Tables
                    .FirstOrDefaultAsync(t => t.TableID == dto.TableID);

                if (existingTable != null)
                {
                    return Conflict(new { message = "Table with this ID already exists" });
                }

                var table = new Table
                {
                    TableID = dto.TableID,
                    Area = dto.Area,
                    Capacity = dto.Capacity
                };

                _context.Tables.Add(table);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTables), new { id = table.TableID }, table);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating table");
                return StatusCode(500, new { message = "Error creating table", error = ex.Message });
            }
        }

        // PUT: api/tables/M1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(string id, TableUpdateDto dto)
        {
            try
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { message = "Table not found" });
                }

                table.Area = dto.Area ?? table.Area;
                if (dto.Capacity > 0)
                {
                    table.Capacity = dto.Capacity;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Table updated successfully", table });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating table");
                return StatusCode(500, new { message = "Error updating table", error = ex.Message });
            }
        }

        // DELETE: api/tables/M1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(string id)
        {
            try
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { message = "Table not found" });
                }

                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Table successfully deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting table");
                return StatusCode(500, new { message = "Error deleting table", error = ex.Message });
            }
        }
    }

    public class TableCreateDto
    {
        [Required]
        [StringLength(4, MinimumLength = 2)]
        public string TableID { get; set; } = default!;

        [Required]
        public string Area { get; set; } = default!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }

    public class TableUpdateDto
    {
        public string Area { get; set; } = default!;
        
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}