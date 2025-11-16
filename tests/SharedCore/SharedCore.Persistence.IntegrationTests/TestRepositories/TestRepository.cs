using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.Repositories;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.IntegrationTests.TestRepositories;

internal class TestRepository : Repository<TestEntity>
{
    public TestRepository(DbContext context,
        IOptionsSnapshot<QueryParametersSettings> queryParametersOptions)
        : base(context, queryParametersOptions)
    {
    }
}