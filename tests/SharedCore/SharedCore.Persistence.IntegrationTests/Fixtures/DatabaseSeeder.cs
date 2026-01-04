using SharedCore.Common.ApplicationContext;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.IntegrationTests.TestServices;

namespace SharedCore.Persistence.IntegrationTests.Fixtures;

internal class DatabaseSeeder
{
    private readonly TestDbContext _context;
    private readonly ICurrentUser _currentUser;

    public const int NumberOfRecordsOfUser = 100;
    public const int NumberOfRecordsOfOtherUser = 100;
    public static readonly Guid CurrentUserKnownTestEntityId = Guid.CreateVersion7();
    public static readonly Guid OtherUserKnownTestEntityId = Guid.CreateVersion7();

    public DatabaseSeeder(TestDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var entities = new List<TestEntity>();

        var currentUserId = _currentUser.UserId!;

        foreach (var number in Enumerable.Range(1, NumberOfRecordsOfUser))
        {
            var code = number.ToString("000");

            entities.Add(new TestEntity
            {
                OwnerId = currentUserId,
                Code = code,
                Children = new List<TestChildEntity>
                {
                    new() { Code = $"ChildCode-{code}-1" },
                    new() { Code = $"ChildCode-{code}-2" }
                }
            });
        }

        foreach (var number in Enumerable.Range(NumberOfRecordsOfUser + 1, NumberOfRecordsOfOtherUser))
        {
            var code = number.ToString("000");

            entities.Add(new TestEntity
            {
                OwnerId = "OtherOwnerId",
                Code = code,
                Children = new List<TestChildEntity>
                {
                    new() { Code = $"ChildCode-{code}-1" },
                    new() { Code = $"ChildCode-{code}-2" }
                }
            });
        }

        _context.TestEntities.AddRange(entities);

        _context.Entry(entities.First()).Property(e => e.Id).CurrentValue = CurrentUserKnownTestEntityId;
        _context.Entry(entities.Last()).Property(e => e.Id).CurrentValue = OtherUserKnownTestEntityId;

        return _context.SaveChangesAsync(cancellationToken);
    }
}