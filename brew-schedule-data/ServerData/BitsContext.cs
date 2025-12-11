using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using brew_schedule_data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace brew_schedule_data.ServerData;

public partial class BitsContext : DbContext
{
    public BitsContext()
    {
    }

    public BitsContext(DbContextOptions<BitsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientInventoryAddition> IngredientInventoryAdditions { get; set; }

    public virtual DbSet<IngredientInventorySubtraction> IngredientInventorySubtractions { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<InventoryTransactionType> InventoryTransactionTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<Style> Styles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierAddress> SupplierAddresses { get; set; }

    public virtual DbSet<UnitType> UnitTypes { get; set; }

    /// <summary>
    /// Asynchronously checks the health of the database connection and query functionality.
    /// </summary>
    /// <returns>
    /// A tuple indicating the status of the connection and query execution.
    /// <para><c>CanConnect</c>: True if a connection to the database can be established.</para>
    /// <para><c>CanQuery</c>: True if a simple query can be successfully executed against the database.</para>
    /// </returns>
    public async Task<(bool CanConnect, bool CanQuery)> CheckDatabaseHealthAsync()
    {
        try
        {
            var canConnect = await Database.CanConnectAsync();
            if (!canConnect)
            {
                return (false, false);
            }

            // A simple query to ensure tables can be accessed.
            _ = await Styles.FirstOrDefaultAsync();
            return (true, true);
        }
        catch
        {
            return (false, false);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigDB.GetMySqlConnectionString();
        if (!optionsBuilder.IsConfigured)
        {
            // Originally had the connection created via Scaffold
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PRIMARY");

            entity.ToTable("batch");

            entity.HasIndex(e => e.RecipeId, "batch_recipe_FK");

            entity.HasIndex(e => e.EquipmentId, "batch_recipe_FK_idx");

            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.Abv).HasColumnName("abv");
            entity.Property(e => e.ActualEfficiency).HasColumnName("actual_efficiency");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.Carbonation).HasColumnName("carbonation");
            entity.Property(e => e.CarbonationTemp).HasColumnName("carbonation_temp");
            entity.Property(e => e.CarbonationUsed)
                .HasMaxLength(100)
                .HasColumnName("carbonation_used");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.EstimatedFinishDate)
                .HasColumnType("datetime")
                .HasColumnName("estimated_finish_date");
            entity.Property(e => e.FermentationStages).HasColumnName("fermentation_stages");
            entity.Property(e => e.Fg).HasColumnName("fg");
            entity.Property(e => e.FinishDate)
                .HasColumnType("datetime")
                .HasColumnName("finish_date");
            entity.Property(e => e.ForcedCarbonation).HasColumnName("forced_carbonation");
            entity.Property(e => e.Ibu).HasColumnName("ibu");
            entity.Property(e => e.IbuMethod)
                .HasMaxLength(50)
                .HasColumnName("ibu_method");
            entity.Property(e => e.KegPrimingFactor).HasColumnName("keg_priming_factor");
            entity.Property(e => e.Notes)
                .HasMaxLength(2000)
                .HasColumnName("notes");
            entity.Property(e => e.Og).HasColumnName("og");
            entity.Property(e => e.PrimaryAge).HasColumnName("primary_age");
            entity.Property(e => e.PrimaryTemp).HasColumnName("primary_temp");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.ScheduledStartDate)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_start_date");
            entity.Property(e => e.SecondaryAge).HasColumnName("secondary_age");
            entity.Property(e => e.SecondaryTemp).HasColumnName("secondary_temp");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.TasteNotes)
                .HasMaxLength(2000)
                .HasColumnName("taste_notes");
            entity.Property(e => e.TasteRating).HasColumnName("taste_rating");
            entity.Property(e => e.Temp).HasColumnName("temp");
            entity.Property(e => e.TertiaryAge).HasColumnName("tertiary_age");
            entity.Property(e => e.UnitCost)
                .HasPrecision(10)
                .HasColumnName("unit_cost");
            entity.Property(e => e.Volume).HasColumnName("volume");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Batches)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("batch_recipe_FK");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PRIMARY");

            entity.ToTable("ingredient");

            entity.HasIndex(e => e.IngredientTypeId, "ingredient_ingredient_type_FK_idx");

            entity.HasIndex(e => e.UnitTypeId, "ingredient_unit_type_FK_idx");

            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.IngredientTypeId).HasColumnName("ingredient_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(2000)
                .HasColumnName("notes");
            entity.Property(e => e.OnHandQuantity).HasColumnName("on_hand_quantity");
            entity.Property(e => e.ReorderPoint).HasColumnName("reorder_point");
            entity.Property(e => e.UnitCost)
                .HasPrecision(10)
                .HasColumnName("unit_cost");
            entity.Property(e => e.UnitTypeId).HasColumnName("unit_type_id");
            entity.Property(e => e.Version).HasColumnName("version");

            entity.HasOne(d => d.UnitType).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.UnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_unit_type_FK");
        });

        modelBuilder.Entity<IngredientInventoryAddition>(entity =>
        {
            entity.HasKey(e => e.IngredientInventoryAdditionId).HasName("PRIMARY");

            entity.ToTable("ingredient_inventory_addition");

            entity.HasIndex(e => e.IngredientId, "ingredient_inventory_addition_ingredient_FK_idx");

            entity.HasIndex(e => e.SupplierId, "ingredient_invertory_addition_supplier_FK_idx");

            entity.Property(e => e.IngredientInventoryAdditionId).HasColumnName("ingredient_inventory_addition_id");
            entity.Property(e => e.EstimatedDeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("estimated_delivery_date");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.QuantityRemaining).HasColumnName("quantity_remaining");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.UnitCost)
                .HasPrecision(10)
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientInventoryAdditions)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_inventory_addition_ingredient_FK");

            entity.HasOne(d => d.Supplier).WithMany(p => p.IngredientInventoryAdditions)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_invertory_addition_supplier_FK");
        });

        modelBuilder.Entity<IngredientInventorySubtraction>(entity =>
        {
            entity.HasKey(e => e.IngredientInventorySubtractionId).HasName("PRIMARY");

            entity.ToTable("ingredient_inventory_subtraction");

            entity.HasIndex(e => e.IngredientId, "ingredient_inventory_subtraction_ingredient_FK");

            entity.HasIndex(e => e.BatchId, "ingredient_purchased_batch_FK");

            entity.Property(e => e.IngredientInventorySubtractionId).HasColumnName("ingredient_inventory_subtraction_id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");

            entity.HasOne(d => d.Batch).WithMany(p => p.IngredientInventorySubtractions)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("ingredient_purchased_batch_FK");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientInventorySubtractions)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingredient_inventory_subtraction_ingredient_FK");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.InventoryTransactionId).HasName("PRIMARY");

            entity.ToTable("inventory_transaction");

            entity.HasIndex(e => e.AccountId, "inventory_transaction_account_idx");

            entity.HasIndex(e => e.AppUserId, "inventory_transaction_app_user_FK_idx");

            entity.HasIndex(e => e.BatchId, "inventory_transaction_batch_FK_idx");

            entity.HasIndex(e => e.ProductContainerSizeId, "inventory_transaction_product_container_size_FK_idx");

            entity.HasIndex(e => e.InventoryTransctionTypeId, "inventory_transaction_transaction_type_FK_idx");

            entity.Property(e => e.InventoryTransactionId).HasColumnName("inventory_transaction_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.InventoryTransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("inventory_transaction_date");
            entity.Property(e => e.InventoryTransctionTypeId).HasColumnName("inventory_transction_type_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .HasColumnName("notes");
            entity.Property(e => e.ProductContainerSizeId).HasColumnName("product_container_size_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Batch).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_transaction_batch_FK");

            entity.HasOne(d => d.InventoryTransctionType).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.InventoryTransctionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_transaction_transaction_type_FK");
        });

        modelBuilder.Entity<InventoryTransactionType>(entity =>
        {
            entity.HasKey(e => e.InventoryTransactionTypeId).HasName("PRIMARY");

            entity.ToTable("inventory_transaction_type");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.InventoryTransactionTypeId).HasColumnName("inventory_transaction_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => new { e.BatchId, e.ProductContainerSizeId }).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.BatchId, "keg_batch_FK_idx");

            entity.HasIndex(e => e.ProductContainerSizeId, "product_product_container_size_FK");

            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.ProductContainerSizeId).HasColumnName("product_container_size_id");
            entity.Property(e => e.IngredientCost)
                .HasPrecision(10, 4)
                .HasColumnName("ingredient_cost");
            entity.Property(e => e.QuantityRacked).HasColumnName("quantity_racked");
            entity.Property(e => e.QuantityRemaining).HasColumnName("quantity_remaining");
            entity.Property(e => e.RackedDate)
                .HasColumnType("datetime")
                .HasColumnName("racked_date");
            entity.Property(e => e.SellByDate)
                .HasColumnType("datetime")
                .HasColumnName("sell_by_date");
            entity.Property(e => e.SuggestedPrice)
                .HasPrecision(10, 4)
                .HasColumnName("suggested_price");

            entity.HasOne(d => d.Batch).WithMany(p => p.Products)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_batch_FK");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PRIMARY");

            entity.ToTable("recipe");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.HasIndex(e => e.EquipmentId, "recipe_equipment_FK_idx");

            entity.HasIndex(e => e.MashId, "recipe_mash_FK_idx");

            entity.HasIndex(e => e.StyleId, "recipe_style_type_FK_idx");

            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.ActualEfficiency)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_efficiency");
            entity.Property(e => e.BoilTime)
                .HasDefaultValueSql("'0'")
                .HasColumnName("boil_time");
            entity.Property(e => e.BoilVolume)
                .HasDefaultValueSql("'0'")
                .HasColumnName("boil_volume");
            entity.Property(e => e.Brewer)
                .HasMaxLength(100)
                .HasColumnName("brewer");
            entity.Property(e => e.CarbonationTemp).HasColumnName("carbonation_temp");
            entity.Property(e => e.CarbonationUsed)
                .HasMaxLength(100)
                .HasColumnName("carbonation_used");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Efficiency)
                .HasDefaultValueSql("'0'")
                .HasColumnName("efficiency");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.EstimatedAbv)
                .HasDefaultValueSql("'0'")
                .HasColumnName("estimated_abv");
            entity.Property(e => e.EstimatedColor)
                .HasDefaultValueSql("'0'")
                .HasColumnName("estimated_color");
            entity.Property(e => e.EstimatedFg)
                .HasDefaultValueSql("'0'")
                .HasColumnName("estimated_fg");
            entity.Property(e => e.EstimatedOg)
                .HasDefaultValueSql("'0'")
                .HasColumnName("estimated_og");
            entity.Property(e => e.FermentationStages)
                .HasDefaultValueSql("'1'")
                .HasColumnName("fermentation_stages");
            entity.Property(e => e.ForcedCarbonation).HasColumnName("forced_carbonation");
            entity.Property(e => e.KegPrimingFactor).HasColumnName("keg_priming_factor");
            entity.Property(e => e.MashId).HasColumnName("mash_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StyleId).HasColumnName("style_id");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Volume).HasColumnName("volume");

            entity.HasOne(d => d.Style).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.StyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe_style_FK");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.RecipeIngredientId).HasName("PRIMARY");

            entity.ToTable("recipe_ingredient");

            entity.HasIndex(e => e.IngredientId, "recipe_ingredient_ingredient_idx");

            entity.HasIndex(e => e.RecipeId, "recipe_ingredient_recipe_FK");

            entity.HasIndex(e => e.UseDuringId, "recipe_ingredient_use_during_FK_idx");

            entity.Property(e => e.RecipeIngredientId).HasColumnName("recipe_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("'0'")
                .HasColumnName("time");
            entity.Property(e => e.UseDuringId).HasColumnName("use_during_id");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe_ingredient_ingredient_FK");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recipe_ingredient_recipe_FK");
        });

        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.StyleId).HasName("PRIMARY");

            entity.ToTable("style");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.StyleId).HasColumnName("style_id");
            entity.Property(e => e.AbvMax).HasColumnName("abv_max");
            entity.Property(e => e.AbvMin).HasColumnName("abv_min");
            entity.Property(e => e.CarbMax).HasColumnName("carb_max");
            entity.Property(e => e.CarbMin).HasColumnName("carb_min");
            entity.Property(e => e.CategoryLetter)
                .HasMaxLength(50)
                .HasColumnName("category_letter");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.CategoryNumber)
                .HasMaxLength(50)
                .HasColumnName("category_number");
            entity.Property(e => e.ColorMax).HasColumnName("color_max");
            entity.Property(e => e.ColorMin).HasColumnName("color_min");
            entity.Property(e => e.Examples)
                .HasMaxLength(2000)
                .HasColumnName("examples");
            entity.Property(e => e.FgMax).HasColumnName("fg_max");
            entity.Property(e => e.FgMin).HasColumnName("fg_min");
            entity.Property(e => e.IbuMax).HasColumnName("ibu_max");
            entity.Property(e => e.IbuMin).HasColumnName("ibu_min");
            entity.Property(e => e.Ingredients)
                .HasMaxLength(2000)
                .HasColumnName("ingredients");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(5000)
                .HasColumnName("notes");
            entity.Property(e => e.OgMax).HasColumnName("og_max");
            entity.Property(e => e.OgMin).HasColumnName("og_min");
            entity.Property(e => e.Profile)
                .HasMaxLength(5000)
                .HasColumnName("profile");
            entity.Property(e => e.StyleGuide)
                .HasMaxLength(50)
                .HasColumnName("style_guide");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.Version).HasColumnName("version");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(50)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactFirstName)
                .HasMaxLength(50)
                .HasColumnName("contact_first_name");
            entity.Property(e => e.ContactLastName)
                .HasMaxLength(50)
                .HasColumnName("contact_last_name");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(50)
                .HasColumnName("contact_phone");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .HasColumnName("note");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .HasColumnName("website");
        });

        modelBuilder.Entity<SupplierAddress>(entity =>
        {
            entity.HasKey(e => new { e.SupplierId, e.AddressId, e.AddressTypeId }).HasName("PRIMARY");

            entity.ToTable("supplier_address");

            entity.HasIndex(e => e.AddressId, "supplier_address_address_FK_idx");

            entity.HasIndex(e => e.AddressTypeId, "supplier_address_address_type_FK_idx");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AddressTypeId).HasColumnName("address_type_id");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierAddresses)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("supplier_address_supplier_FK");
        });

        modelBuilder.Entity<UnitType>(entity =>
        {
            entity.HasKey(e => e.UnitTypeId).HasName("PRIMARY");

            entity.ToTable("unit_type");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.UnitTypeId).HasColumnName("unit_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
