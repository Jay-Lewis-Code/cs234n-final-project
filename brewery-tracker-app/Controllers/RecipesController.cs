using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using brew_schedule_data.Models;
using brew_schedule_data.ServerData;
using brewery_tracker_app.Models; // Using statement for the DTO

namespace brewery_tracker_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly BitsContext _context;

        public RecipesController(BitsContext context)
        {
            _context = context;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduledBrewDto>>> GetScheduledBrews()
        {
            var today = DateTime.Today;

            var recipes = await _context.Recipes
                .Include(r => r.Style)
                .Include(r => r.Batches)
                .Select(r => new ScheduledBrewDto
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.Name,
                    Version = r.Version,
                    Style = r.Style != null ? r.Style.Name : null,
                    Ibu = r.Batches.Any() ? r.Batches.FirstOrDefault().Ibu : null, // Taking from a representative batch
                    Abv = r.EstimatedAbv, // Using EstimatedAbv from Recipe
                    LastBrewed = r.Batches
                                  .Where(b => b.StartDate.HasValue && b.StartDate.Value < today)
                                  .OrderByDescending(b => b.StartDate)
                                  .Select(b => b.StartDate)
                                  .FirstOrDefault(),
                    ScheduledDates = r.Batches
                                      .Where(b => b.ScheduledStartDate.HasValue && b.ScheduledStartDate.Value >= today)
                                      .Select(b => b.ScheduledStartDate)
                                      .ToList()
                })
                .OrderBy(r => r.RecipeName)
                .ToListAsync();

            return recipes;
        }


        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDto>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Where(r => r.RecipeId == id)
                .Select(r => new RecipeDetailDto
                {
                    RecipeId = r.RecipeId,
                    Name = r.Name,
                    Version = r.Version,
                    Style = r.Style.Name,
                    Volume = r.Volume,
                    Brewer = r.Brewer,
                    EstimatedAbv = r.EstimatedAbv,
                    Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                    {
                        Name = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Ingredient.UnitType.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, UpdateRecipeDto updateDto)
        {
            var recipeToUpdate = await _context.Recipes.FindAsync(id);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            // Apply updates from the DTO to the entity we fetched
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                recipeToUpdate.Name = updateDto.Name;
            }
            if (updateDto.Version.HasValue)
            {
                recipeToUpdate.Version = updateDto.Version.Value;
            }
            if (updateDto.Volume.HasValue)
            {
                recipeToUpdate.Volume = updateDto.Volume.Value;
            }
            if (!string.IsNullOrEmpty(updateDto.Brewer))
            {
                recipeToUpdate.Brewer = updateDto.Brewer;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
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

        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<RecipeDetailDto>> PostRecipe(CreateRecipeDto createDto)
        {
            var recipe = new Recipe
            {
                Name = createDto.Name,
                StyleId = createDto.StyleId,
                Volume = createDto.Volume,
                Brewer = createDto.Brewer,
                Version = createDto.Version,
                EstimatedAbv = createDto.EstimatedAbv,
                BoilTime = createDto.BoilTime,
                Date = DateTime.UtcNow // Set creation date
            };

            foreach (var ingredientDto in createDto.Ingredients)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    IngredientId = ingredientDto.IngredientId,
                    Quantity = ingredientDto.Quantity
                };
                recipe.RecipeIngredients.Add(recipeIngredient);
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // To return the full details, we can call our own GetRecipe method
            // This is a clean way to ensure the returned object is identical to a GET request
            var actionResult = await GetRecipe(recipe.RecipeId);
            
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.RecipeId }, actionResult.Value);
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Ensure related batches are handled before deleting a recipe
            var batches = await _context.Batches.Where(b => b.RecipeId == id).ToListAsync();
            if (batches.Any())
            {
                return BadRequest("Cannot delete a recipe that has associated batches.");
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}