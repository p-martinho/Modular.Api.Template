using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using SharedCore.Common.ApplicationContext;
using SharedCore.Common.Authorization;
using SharedCore.Persistence.IntegrationTests.TestEntities;
using SharedCore.Persistence.Repositories;
using SharedCore.Persistence.Repositories.Settings;

namespace SharedCore.Persistence.IntegrationTests.TestRepositories;

internal class TestQueryRepository : QueryRepository<TestEntity>
{
    private readonly ICurrentUser _currentUser;

    public TestQueryRepository(DbContext context,
        IOptionsSnapshot<QueryParametersSettings> queryParametersOptions,
        ICurrentUser currentUser)
        : base(context, queryParametersOptions)
    {
        _currentUser = currentUser;
    }

    protected override Func<IQueryable<TestEntity>, IIncludableQueryable<TestEntity, object>>
        GetDefaultAggregateIncludes()
    {
        return q => q.Include(e => e.Children);
    }

    protected override Expression<Func<TestEntity, bool>> GetDefaultPermissionsFilter()
    {
        return e => _currentUser.IsInRole(UserRoles.Admin) ||
                    e.OwnerId == _currentUser.UserId;
    }
}