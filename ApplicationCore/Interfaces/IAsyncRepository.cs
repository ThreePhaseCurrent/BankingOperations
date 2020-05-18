using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingSystem.ApplicationCore.Interfaces
{
  /// <typeparam name="T">Entity</typeparam>
  public interface IAsyncRepository<T>
  {
    Task<T> GetById(int? id);
    Task<List<T>> GetAll();

    Task<List<T>> ListAsync(ISpecification<T> specification);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpdateUserAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> SingleBy(ISpecification<T> specification);
  }
}
