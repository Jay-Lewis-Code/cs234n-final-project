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
    public class BatchTests
    {
        BitsContext dbContext = null!;
        Batch? b;
        List<Batch>? batches;
        DateTime _today;

        [SetUp]
        public void Setup()
        {
            dbContext = new BitsContext();
            // Reset DB state for each test using your stored procedure
            dbContext.Database.ExecuteSqlRaw("CALL reset_bits_for_brew_tests()");
            _today = new DateTime(2024, 11, 1); // fixed "today" for DateTime to avoid errors
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
            batches = dbContext.Batches
                .Include(b => b.Recipe)
                    .ThenInclude(r => r.Style)
                .OrderBy(b => b.BatchId)
                .ToList();

            Assert.IsNotNull(batches);
            Assert.AreEqual(5, batches!.Count);

            // Based on seed data in reset_bits_for_brew_tests()
            Assert.AreEqual(1, batches[0].BatchId);
            Assert.AreEqual("West Coast IPA", batches[0].Recipe!.Name);

            Assert.AreEqual(5, batches[4].BatchId);
            Assert.AreEqual("Oatmeal Stout", batches[4].Recipe!.Name);

            PrintAll(batches);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            // Batch 1: West Coast IPA, start 2024-09-01
            b = dbContext.Batches
                .Include(b => b.Recipe)
                    .ThenInclude(r => r.Style)
                .SingleOrDefault(b => b.BatchId == 1);

            Assert.IsNotNull(b);
            Assert.AreEqual(1, b!.RecipeId);
            Assert.AreEqual("West Coast IPA", b.Recipe!.Name);
            Assert.AreEqual("IPA", b.Recipe!.Style!.Name);
            Assert.AreEqual(new DateTime(2024, 09, 01), b.StartDate);

            Console.WriteLine(b);
        }

        [Test]

        // Get all batches for a specific recipe as shown on the HTML table
        // Had to get some AI help for the MySQL query
        public void GetByRecipeTest()
        {
            // All batches for the West Coast IPA recipe (recipe_id = 1)
            batches = dbContext.Batches
                .Include(b => b.Recipe)
                .Where(b => b.RecipeId == 1)
                .OrderBy(b => b.BatchId)
                .ToList();

            Assert.IsNotNull(batches);
            Assert.AreEqual(3, batches!.Count); // 2 past + 1 future scheduled

            Assert.IsTrue(batches.All(x => x.RecipeId == 1));
            Assert.AreEqual("West Coast IPA", batches[0].Recipe!.Name);

            PrintAll(batches);
        }

        // Scheduled brews = future batches with a ScheduledStartDate and no StartDate
        // Had to get some AI help for the MySQL query
        [Test]
        public void GetScheduledBrewsTest()
        {
            var scheduled = dbContext.Batches
                .Include(b => b.Recipe)
                    .ThenInclude(r => r.Style)
                .Where(b =>
                    b.ScheduledStartDate != null &&
                    b.ScheduledStartDate > _today &&
                    b.StartDate == null)
                .OrderBy(b => b.ScheduledStartDate)
                .ToList();

            Assert.AreEqual(2, scheduled.Count);

            Assert.AreEqual("West Coast IPA", scheduled[0].Recipe!.Name);
            Assert.AreEqual(new DateTime(2024, 12, 15), scheduled[0].ScheduledStartDate);
            Assert.AreEqual("IPA", scheduled[0].Recipe!.Style!.Name);

            Assert.AreEqual("Oatmeal Stout", scheduled[1].Recipe!.Name);
            Assert.AreEqual(new DateTime(2025, 01, 05), scheduled[1].ScheduledStartDate);
            Assert.AreEqual("Stout", scheduled[1].Recipe!.Style!.Name);

            foreach (var b in scheduled)
            {
                Console.WriteLine(
                    $"\tBatch ID: {b.BatchId}, " +
                    $"Recipe: {b.Recipe!.Name}, " +
                    $"Style: {b.Recipe!.Style!.Name}, " +
                    $"Scheduled: {b.ScheduledStartDate}, " +
                    $"Start: {b.StartDate}, " +
                    $"Finish: {b.FinishDate}");
            }
        }


        // Determines the "Last brewed on" part of the table we display on HTML page.
        // Had to get some AI help for the MySQL query
        [Test]
        public void GetLastBrewPerRecipeTest()
        {
            var lastBrewPerRecipe = dbContext.Batches
                .Where(b => b.StartDate != null && b.StartDate <= _today)
                .GroupBy(b => b.RecipeId)
                .Select(g => new
                {
                    RecipeId = g.Key,
                    LastBrewed = g.Max(b => b.StartDate)
                })
                .ToList();

            var ipaLast = lastBrewPerRecipe.Single(x => x.RecipeId == 1);
            var stoutLast = lastBrewPerRecipe.Single(x => x.RecipeId == 2);

            Assert.AreEqual(new DateTime(2024, 10, 15), ipaLast.LastBrewed);
            Assert.AreEqual(new DateTime(2024, 08, 20), stoutLast.LastBrewed);

            Console.WriteLine($"IPA last brewed:   {ipaLast.LastBrewed:d}");
            Console.WriteLine($"Stout last brewed: {stoutLast.LastBrewed:d}");
        }

        // ---------- Simple C(R)UD-style tests ----------

        [Test]
        public void CreateTest()
        {
            b = new Batch
            {
                RecipeId = 1,                  // West Coast IPA
                EquipmentId = 1,
                Volume = 20,
                ScheduledStartDate = new DateTime(2025, 02, 01),
                EstimatedFinishDate = new DateTime(2025, 02, 15)
            };

            dbContext.Batches.Add(b);
            dbContext.SaveChanges();

            var newId = b.BatchId;
            var fromDb = dbContext.Batches
                .Include(x => x.Recipe)
                .SingleOrDefault(x => x.BatchId == newId);

            Assert.IsNotNull(fromDb);
            Assert.AreEqual(1, fromDb!.RecipeId);
            Assert.AreEqual(new DateTime(2025, 02, 01), fromDb.ScheduledStartDate);
        }

        [Test]
        public void UpdateTest()
        {
            // Update the future IPA batch (batch_id = 4)
            b = dbContext.Batches.Find(4);
            Assert.IsNotNull(b);

            b!.Volume = 25;
            b.EstimatedFinishDate = new DateTime(2024, 12, 30);

            dbContext.Batches.Update(b);
            dbContext.SaveChanges();

            var fromDb = dbContext.Batches.Find(4);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(25, fromDb!.Volume);
            Assert.AreEqual(new DateTime(2024, 12, 30), fromDb.EstimatedFinishDate);
        }

        [Test]
        public void DeleteTest()
        {
            // Delete the future Stout batch (batch_id = 5)
            b = dbContext.Batches.Find(5);
            Assert.IsNotNull(b);

            dbContext.Batches.Remove(b!);
            dbContext.SaveChanges();

            var fromDb = dbContext.Batches.Find(5);
            Assert.IsNull(fromDb);
        }

        // Utility method to get all Batches information
        public void PrintAll(List<Batch> batches)
        {
            foreach (var batch in batches)
            {
                Console.WriteLine(
                    $"Batch {batch.BatchId}: RecipeId={batch.RecipeId}, " +
                    $"Scheduled={batch.ScheduledStartDate}, Start={batch.StartDate}, Finish={batch.FinishDate}");
            }
        }
    }
}