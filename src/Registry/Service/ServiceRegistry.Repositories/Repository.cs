using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class Repository<TAggregateRoot, TId> : IRepository<TAggregateRoot, TId>
		where TAggregateRoot : AggregateRoot<TId>
	{
		// private static Lazy<Expression<Func<TAggregateRoot, bool>>> equalExpression = new Lazy<Expression<Func<TAggregateRoot, bool>>>(() =>
		//{
		//    var arg = Expression.Parameter(typeof(TId), "id");
		//    var exp = Expression.Equal(arg, Expression.Constant(id, typeof(TId)));
		//    var exp2 = Expression.Lambda<Func<TAggregateRoot, bool>>(exp, arg);
		//    return exp2;
		//});

		public async Task AddAsync(TAggregateRoot aggregateRoot)
		{
			using (var ctx = new ConfigContext())
			{
				ctx.Set<TAggregateRoot>().Add(aggregateRoot);
				await ctx.SaveChangesAsync();
			}
		}

		public async Task AddRangeAsync(IEnumerable<TAggregateRoot> aggregateRoot)
		{
			using (var ctx = new ConfigContext())
			{
				ctx.Set<TAggregateRoot>().AddRange(aggregateRoot);
				await ctx.SaveChangesAsync();
			}
		}

		private static class WhereExpressionBuilder
		{
			// ReSharper disable StaticMemberInGenericType
			private static readonly ParameterExpression ArgumentExpress = Expression.Parameter(typeof(TAggregateRoot), "it");
			private static readonly MemberExpression PropertyExpress;
			// ReSharper restore StaticMemberInGenericType

			static WhereExpressionBuilder()
			{
				PropertyExpress = Expression.Property(ArgumentExpress, "Id");
			}

			public static Expression<Func<TAggregateRoot, bool>> GetEqual(TId value)
			{
				var findValue = Expression.Constant(value, typeof(TId));
				var compareExp = Expression.Equal(PropertyExpress, findValue);
				var exp = Expression.Lambda<Func<TAggregateRoot, bool>>(compareExp, ArgumentExpress);
				return exp;
			}
		}

		public Task<TAggregateRoot> GetByIdAsync(TId id)
		{
			var exp2 = WhereExpressionBuilder.GetEqual(id);
			return GetAsync(exp2);
		}

		public Task<TAggregateRoot> GetAsync(Expression<Func<TAggregateRoot, bool>> express)
		{
			using (var ctx = new ConfigContext())
			{
				var result = ctx.Set<TAggregateRoot>()
					.FirstOrDefault(express);

				return Task.FromResult(result);
			}
		}

		public Task<TAggregateRoot[]> GetAllAsync()
		{
			using (var ctx = new ConfigContext())
			{
				var result = ctx.Set<TAggregateRoot>()
					.ToArray();

				return Task.FromResult(result);
			}
		}

		public Task<TAggregateRoot[]> GetAllAsync(Expression<Func<TAggregateRoot, bool>> express)
		{
			using (var ctx = new ConfigContext())
			{
				var result = ctx.Set<TAggregateRoot>()
					.Where(express)
					.ToArray();

				return Task.FromResult(result);
			}
		}

		public async Task RemoveAsync(TId id)
		{
			using (var ctx = new ConfigContext())
			{
				var result = ctx.Set<TAggregateRoot>()
					.FirstOrDefault(it => it.Id.Equals(id));

				if (result != null)
				{
					ctx.Set<TAggregateRoot>().Remove(result);
					await ctx.SaveChangesAsync();
				}
			}
		}

		public async Task RemoveRangeAsync(IEnumerable<TId> ids)
		{
			using (var ctx = new ConfigContext())
			{
				var result = ctx.Set<TAggregateRoot>()
					.Where(it => ids.Contains(it.Id))
					.ToArray();

				if (result.Length > 0)
				{
					ctx.Set<TAggregateRoot>().RemoveRange(result);
					await ctx.SaveChangesAsync();
				}
			}
		}

		public async Task UpdateAsync(TAggregateRoot aggregateRoot)
		{
			using (var ctx = new ConfigContext())
			{
				ctx.Set<TAggregateRoot>().Attach(aggregateRoot);
				ctx.Entry(aggregateRoot).State = EntityState.Modified;


				await ctx.SaveChangesAsync();
			}
		}

		public async Task UpdateRangeAsync(IEnumerable<TAggregateRoot> aggregateRoots)
		{
			using (var ctx = new ConfigContext())
			{
				foreach (var item in aggregateRoots)
				{
					ctx.Set<TAggregateRoot>().Attach(item);
					ctx.Entry(item).State = EntityState.Modified;
				}
				await ctx.SaveChangesAsync();
			}
		}

		public void Dispose()
		{

		}
	}
}