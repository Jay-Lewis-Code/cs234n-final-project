namespace brewery_tracker_app.Models
{
    public class ScheduledBrewDto
    {
        public int RecipeId { get; set; }
        public string? RecipeName { get; set; }
        public int? Version { get; set; }
        public string? Style { get; set; }
        public double? Ibu { get; set; }
        public double? Abv { get; set; }
        public DateTime? LastBrewed { get; set; }

        // This property will hold future scheduled brew dates for a recipe
        public List<DateTime?> ScheduledDates { get; set; } = new List<DateTime?>();
    }

    public class BatchDto
    {
        public int BatchId { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public double Volume { get; set; }

        // Related Recipe Info
        public int RecipeId { get; set; }
        public string? RecipeName { get; set; }
        public int? Version { get; set; }
        public string? Style { get; set; }
        public double? Ibu { get; set; }
        public double? Abv { get; set; }
    }

    // DTO for creating a batch
    public class CreateBatchDto
    {
        public int RecipeId { get; set; }
        public int EquipmentId { get; set; }
        public double Volume { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
    }

    public class UpdateBatchDto
    {
        // Only include fields that are safe to update
        public DateTime? ScheduledStartDate { get; set; }
        public double? Volume { get; set; }
        public string? Notes { get; set; }
    }

    public class RecipeIngredientDto
    {
        public string? Name { get; set; }
        public double Quantity { get; set; }
        public string? Unit { get; set; }
    }

    public class RecipeDetailDto
    {
        public int RecipeId { get; set; }
        public string? Name { get; set; }
        public int? Version { get; set; }
        public string? Style { get; set; }
        public double Volume { get; set; }
        public string? Brewer { get; set; }
        public double? EstimatedAbv { get; set; }
        public double? EstimatedIbu { get; set; }

        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }

    // Same logic as UpdateBatchDto update by only the desired fields 
    public class UpdateRecipeDto
    {
        // Only include fields that are safe to update
        public string? Name { get; set; }
        public int? Version { get; set; }
        public double? Volume { get; set; }
        public string? Brewer { get; set; }
    }

    // DTO for the list of ingredients in a new recipe
    public class CreateRecipeIngredientDto
    {
        public int IngredientId { get; set; }
        public double Quantity { get; set; }
    }

    // DTO for creating a new recipe
    public class CreateRecipeDto
    {
        public string Name { get; set; } = null!;
        public int StyleId { get; set; }
        public double Volume { get; set; }
        public string Brewer { get; set; } = null!;
        public int? Version { get; set; }
        public double? EstimatedAbv { get; set; }
        public double? BoilTime { get; set; }

        public List<CreateRecipeIngredientDto> Ingredients { get; set; } = new();
    }
}