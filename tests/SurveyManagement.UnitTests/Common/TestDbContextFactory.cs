using Microsoft.EntityFrameworkCore;
using SurveyManagement.Infrastructure.Persistence;

namespace SurveyManagement.UnitTests.Common;

public static class TestDbContextFactory
{
    // A fresh, empty in-memory database per test so tests never affect each other
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
