using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using brew_schedule_data.Models;
using brew_schedule_data.ServerData;
using brewery_tracker_app.Models;

namespace brewery_tracker_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly BitsContext _context;

        public BatchController(BitsContext context)
        {
            _context = context;
        }

        // GET: api/Batch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchDto>>> GetBatches()
        {
            return await _context.Batches
                .Select(b => new BatchDto
                {
                    BatchId = b.BatchId,
                    ScheduledStartDate = b.ScheduledStartDate,
                    Volume = b.Volume,
                    RecipeId = b.RecipeId,
                    RecipeName = b.Recipe.Name,
                    Version = b.Recipe.Version,
                    Style = b.Recipe.Style.Name,
                    Ibu = b.Ibu,
                    Abv = b.Abv
                })
                .OrderByDescending(b => b.ScheduledStartDate)
                .ToListAsync();
        }

        // GET: api/Batch/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BatchDto>> GetBatch(int id)
        {
            var batch = await _context.Batches
                .Where(b => b.BatchId == id)
                .Select(b => new BatchDto
                {
                    BatchId = b.BatchId,
                    ScheduledStartDate = b.ScheduledStartDate,
                    Volume = b.Volume,
                    RecipeId = b.RecipeId,
                    RecipeName = b.Recipe.Name,
                    Version = b.Recipe.Version,
                    Style = b.Recipe.Style.Name,
                    Ibu = b.Ibu,
                    Abv = b.Abv
                })
                .FirstOrDefaultAsync();

            if (batch == null)
            {
                return NotFound();
            }

            return batch;
        }

        // PUT: api/Batch/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBatch(int id, UpdateBatchDto updateDto)
        {
            var batchToUpdate = await _context.Batches.FindAsync(id);

            if (batchToUpdate == null)
            {
                return NotFound();
            }

            // Apply updates from the DTO to the entity we fetched
            if (updateDto.ScheduledStartDate.HasValue)
            {
                batchToUpdate.ScheduledStartDate = updateDto.ScheduledStartDate.Value;
            }
            if (updateDto.Volume.HasValue)
            {
                batchToUpdate.Volume = updateDto.Volume.Value;
            }
            if (updateDto.Notes != null)
            {
                batchToUpdate.Notes = updateDto.Notes;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BatchExists(id))
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

        // POST: api/Batch
        [HttpPost]
        public async Task<ActionResult<BatchDto>> PostBatch(CreateBatchDto createDto)
        {
            var batch = new Batch
            {
                RecipeId = createDto.RecipeId,
                EquipmentId = createDto.EquipmentId,
                Volume = createDto.Volume,
                ScheduledStartDate = createDto.ScheduledStartDate
            };

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            // This creates the full DTO to return to the client
            var newBatchDto = new BatchDto
            {
                BatchId = batch.BatchId,
                ScheduledStartDate = batch.ScheduledStartDate,
                Volume = batch.Volume,
                RecipeId = batch.RecipeId
            };

            return CreatedAtAction(nameof(GetBatch), new { id = batch.BatchId }, newBatchDto);
        }

        // DELETE: api/Batch/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
            {
                return NotFound();
            }

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BatchExists(int id)
        {
            return _context.Batches.Any(e => e.BatchId == id);
        }
    }
}