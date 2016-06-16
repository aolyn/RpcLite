using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceRegistry.Domain.Repositories
{
	public interface IRepository<TAggregateRoot, TId>
		where TAggregateRoot : class, IAggregateRoot<TId>
	{
		Task AddAsync(TAggregateRoot aggregateRoot);

		Task AddRangeAsync(IEnumerable<TAggregateRoot> aggregateRoot);

		Task<TAggregateRoot> GetByIdAsync(TId id);

		Task<TAggregateRoot> GetAsync(Expression<Func<TAggregateRoot, bool>> express);

		Task<IList<TAggregateRoot>> GetAllAsync();

		Task<IList<TAggregateRoot>> GetAllAsync(Expression<Func<TAggregateRoot, bool>> express);

		Task RemoveAsync(TId id);

		Task RemoveRangeAsync(IEnumerable<TId> id);

		Task UpdateAsync(TAggregateRoot aggregateRoot);

		Task UpdateRangeAsync(IEnumerable<TAggregateRoot> aggregateRoot);
	}
}
