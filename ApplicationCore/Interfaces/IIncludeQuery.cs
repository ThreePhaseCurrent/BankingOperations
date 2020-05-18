using System;
using System.Collections.Generic;

using BankingSystem.ApplicationCore.Helpers.Query;

namespace BankingSystem.ApplicationCore.Interfaces
{
  public interface IIncludeQuery
  {
    Dictionary<IIncludeQuery, string> PathMap { get; }
    IncludeVisitor Visitor { get; }
    HashSet<string> Paths { get; }
  }

  public interface IIncludeQuery<TEntity, out TPreviousProperty> : IIncludeQuery { }
}
