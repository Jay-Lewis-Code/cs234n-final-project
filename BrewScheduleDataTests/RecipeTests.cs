using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using brew_schedule_data.Models;
using brew_schedule_data.ServerData;

namespace BrewScheduler.Tests
{
    [TestFixture]
    public class RecipeTests
    {
        BitsContext dbContext = null!;
        Recipe? r;
        List<Recipe>? recipes;

        [SetUp]
        public void Setup()
        {
            dbContext = new BitsContext();
            // Reset DB state for each test
            dbContext.Database.ExecuteSqlRaw("CALL reset_bits_for_brew_tests()");
        }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }

        // ---------- Read / LINQ tests ----------

        [Test]
        public void GetAllTest()
        {
            recipes = dbContext.Recipes
                .Include(r => r.Style)
                .OrderBy(r => r.Name)
                .ToList();

            Assert.IsNotNull(recipes);
            Assert.AreEqual(2, recipes!.Count);

            // Based on your seeding in reset_bits_for_brew_tests()
            Assert.AreEqual("Oatmeal Stout", recipes[0].Name);
            Assert.AreEqual("Stout", recipes[0].Style!.Name);

            Assert.AreEqual("West Coast IPA", recipes[1].Name);
            Assert.AreEqual("IPA", recipes[1].Style!.Name);

            PrintAll(recipes);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            // Recipe seeded in proc: (1, 'West Coast IPA', version 7, style IPA, volume 20, brewer 'Test Brewer')
            r = dbContext.Recipes
                .Include(r => r.Style)
                .SingleOrDefault(r => r.RecipeId == 1);

            Assert.IsNotNull(r);
            Assert.AreEqual("West Coast IPA", r!.Name);
            Assert.AreEqual(7, r.Version);
            Assert.AreEqual("IPA", r.Style!.Name);
            Console.WriteLine(r);
        }

        [Test]
        public void GetByStyleTest()
        {
            // All IPA recipes (Style.Name == "IPA")
            recipes = dbContext.Recipes
                .Include(r => r.Style)
                .Where(r => r.Style!.Name == "IPA")
                .OrderBy(r => r.Name)
                .ToList();

            Assert.AreEqual(1, recipes!.Count);
            Assert.AreEqual("West Coast IPA", recipes[0].Name);
            Assert.AreEqual("IPA", recipes[0].Style!.Name);

            PrintAll(recipes);
        }

        [Test]
        public void GetWithBatchesTest()
        {
            // reset_bits_for_brew_tests() seeds 3 batches for recipe_id 1
            r = dbContext.Recipes
                .Include(r => r.Batches)
                .SingleOrDefault(r => r.RecipeId == 1);

            Assert.IsNotNull(r);
            Assert.AreEqual("West Coast IPA", r!.Name);
            Assert.AreEqual(3, r.Batches!.Count);

            Console.WriteLine(r);
            foreach (var b in r.Batches)
            {
                Console.WriteLine(
                    $"\tBatch ID: {b.BatchId}, " +
                    $"RecipeId: {b.RecipeId}, " +
                    $"Scheduled: {b.ScheduledStartDate}, " +
                    $"Start: {b.StartDate}, " +
                    $"Finish: {b.FinishDate}");
            }
        }

        // ---------- Simple C(R)UD-style tests (optional) ----------

        [Test]
        public void CreateTest()
        {
            r = new Recipe
            {
                Name = "Test Pale Ale",
                Version = 1,
                StyleId = 1,          // IPA style from seed data
                Volume = 20,
                Brewer = "Unit Test Brewer"
            };

            dbContext.Recipes.Add(r);
            dbContext.SaveChanges();

            var fromDb = dbContext.Recipes.Find(r.RecipeId);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual("Test Pale Ale", fromDb!.Name);
        }

        [Test]
        public void UpdateTest()
        {
            r = dbContext.Recipes.Find(2); // Oatmeal Stout
            Assert.IsNotNull(r);

            r!.Name = "Updated Oatmeal Stout";
            dbContext.Recipes.Update(r);
            dbContext.SaveChanges();

            var fromDb = dbContext.Recipes.Find(2);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual("Updated Oatmeal Stout", fromDb!.Name);
        }

        [Test]
        public void DeleteTest()
        {
            r = dbContext.Recipes
                .Include(r => r.Batches)
                .SingleOrDefault(r => r.RecipeId == 2); // Oatmeal Stout
            Assert.IsNotNull(r);

            // Delete any dependent Batch records first
            if (r!.Batches != null && r.Batches.Any())
            {
                dbContext.Batches.RemoveRange(r.Batches);
                dbContext.SaveChanges();
            }

            dbContext.Recipes.Remove(r!);
            dbContext.SaveChanges();

            var fromDb = dbContext.Recipes.Find(2);
            Assert.IsNull(fromDb);
        }

        // Utility method to see everything in recipes list
        public void PrintAll(List<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                Console.WriteLine(recipe);
            }
        }
    }
}