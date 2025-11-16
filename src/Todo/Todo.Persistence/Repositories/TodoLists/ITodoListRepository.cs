using SharedCore.Persistence.Repositories;
using Todo.Domain.Entities.TodoLists;

namespace Todo.Persistence.Repositories.TodoLists;

/// <summary>
/// The to do list repository.
/// </summary>
/// <seealso cref="IRepository{TEntity}"/>
public interface ITodoListRepository : IRepository<TodoList>;