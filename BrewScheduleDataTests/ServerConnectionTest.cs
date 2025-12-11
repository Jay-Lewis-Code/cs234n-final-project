using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using brew_schedule_data.Models;
using brew_schedule_data.ServerData;
using Microsoft.EntityFrameworkCore;


namespace BrewScheduleDataTests;

[TestFixture]
public class ServerConnectionTest
{
    BitsContext dbContext;

    [SetUp]
    public void Setup()
    {
        dbContext = new BitsContext();
        dbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        dbContext?.Dispose();
    }

    [Test]
    public async Task Database_Health_Check_Should_Succeed()
    {
        var (canConnect, canQuery) = await dbContext.CheckDatabaseHealthAsync();

        Assert.That(canConnect, Is.True, "Database should be connectable.");
        Assert.That(canQuery, Is.True, "Database should be queryable.");
    }

}