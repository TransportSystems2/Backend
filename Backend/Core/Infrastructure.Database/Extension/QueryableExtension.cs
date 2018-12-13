using System;
using System.Linq;
using System.Linq.Expressions;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Extension
{
    public static class QueryableExtension
    {
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> queryableList, Expression<Func<T, TKey>> keySelector, OrderingKind type)
        {
            switch(type)
            {
                case OrderingKind.Asc:
                    {
                        return queryableList.OrderBy(keySelector);
                    }

                case OrderingKind.Desc:
                    {
                        return queryableList.OrderByDescending(keySelector);
                    }
            }

            return (IOrderedQueryable<T>)queryableList;
        }
    }
}