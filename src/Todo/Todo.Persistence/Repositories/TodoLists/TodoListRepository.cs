using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using SharedCore.Common.ApplicationContext;
using SharedCore.Common.Authorization;
using SharedCore.Persistence.Repositories;
using SharedCore.Persistence.Repositories.Settings;
using Todo.Domain.Entities.TodoLists;

namespace Todo.Persistence.Repositories.TodoLists;

/// <summary>
/// The to do list repository.
/// </summary>
/// <seealso cref="Repository{TEntity}"/>
/// <seealso cref="ITodoListRepository"/>
internal class TodoListRepository : Repository<TodoList>, ITodoListRepository
{
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListRepository"/> class.
    /// </summary>
    /// <param name="context">The DB context.</param>
    /// <param name="queryParametersOptions">The query parameters options.</param>
    /// <param name="currentUser">The current user.</param>
    public TodoListRepository(TodoDbContext context,
        IOptionsSnapshot<QueryParametersSettings> queryParametersOptions,
        ICurrentUser currentUser)
        : base(context, queryParametersOptions)
    {
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    protected override Func<IQueryable<TodoList>, IIncludableQueryable<TodoList, object>> GetDefaultAggregateIncludes()
    {
        return q => q.Include(e => e.Items);
    }

    /// <inheritdoc />
    protected override Expression<Func<TodoList, bool>> GetDefaultPermissionsFilter()
    {
        return e => _currentUser.IsInRole(UserRoles.Admin) ||
                    e.OwnerId == _currentUser.UserId;
    }
}