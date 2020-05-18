using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationCore.Entity;

using BankingSystem.ApplicationCore.Interfaces;

using Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    /// <summary>
    ///   Реализация интерфейса
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IAsyncRepository<T> where T : class
    {
        private readonly BankOperationsIdentityContext _context;
        private readonly BankOperationsContext Context;

        public EfRepository(BankOperationsContext context, BankOperationsIdentityContext usContext)
        {
            Context = context;
            _context = usContext;
        }

        public async Task<T> GetById(int? id) => await Context.Set<T>().FindAsync(id);

        public async Task<List<T>> GetAll() => await Context.Set<T>().ToListAsync();

        public async Task AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateUserAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<List<T>> ListAsync(ISpecification<T> spec) => await ApplySpecification(spec).ToListAsync();

        public async Task<T> SingleBy(ISpecification<T> specification) =>
                        await ApplySpecification(specification).SingleOrDefaultAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
                        SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), spec);
    }
}
