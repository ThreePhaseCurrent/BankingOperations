using System;
using System.Collections.Generic;
using System.Linq;

using BankingSystem.ApplicationCore.Interfaces;

namespace BankingSystem.ApplicationCore.Helpers.Query
{
  public class IncludeQuery<TEntity, TPreviousProperty> : IIncludeQuery<TEntity, TPreviousProperty>
  {
    public IncludeQuery(Dictionary<IIncludeQuery, string> pathMap) => PathMap = pathMap;

    public Dictionary<IIncludeQuery, string> PathMap { get; } = new Dictionary<IIncludeQuery, string>();
    public IncludeVisitor Visitor { get; } = new IncludeVisitor();

    public HashSet<string> Paths => PathMap.Select(selector: x => x.Value).ToHashSet();
  }
}
