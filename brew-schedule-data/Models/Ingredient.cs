using System;
using System.Collections.Generic;

namespace brew_schedule_data.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;

    public int? Version { get; set; }

    public int IngredientTypeId { get; set; }

    public double OnHandQuantity { get; set; }

    public int UnitTypeId { get; set; }

    public decimal UnitCost { get; set; }

    public double ReorderPoint { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<IngredientInventoryAddition> IngredientInventoryAdditions { get; set; } = new List<IngredientInventoryAddition>();

    public virtual ICollection<IngredientInventorySubtraction> IngredientInventorySubtractions { get; set; } = new List<IngredientInventorySubtraction>();

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual UnitType UnitType { get; set; } = null!;
}
