using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceRegistry.Domain.Repositories
{
	public interface IRepository<TAggregateRoot, in TId> : IDisposable
		where TAggregateRoot : class, IAggregateRoot<TId>
	{
		Task AddAsync(TAggregateRoot aggregateRoot);

		Task AddRangeAsync(IEnumerable<TAggregateRoot> aggregateRoot);

		Task<TAggregateRoot> GetByIdAsync(TId id);

		Task<TAggregateRoot> GetAsync(Expression<Func<TAggregateRoot, bool>> express);

		Task<TAggregateRoot[]> GetAllAsync();

		Task<TAggregateRoot[]> GetAllAsync(Expression<Func<TAggregateRoot, bool>> express);

		Task RemoveAsync(TId id);

		Task RemoveRangeAsync(IEnumerable<TId> id);

		Task UpdateAsync(TAggregateRoot aggregateRoot);

		Task UpdateRangeAsync(IEnumerable<TAggregateRoot> aggregateRoot);
	}
}
