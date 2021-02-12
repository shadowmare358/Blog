using Blog.Models.Comments;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.Data
{
    //For removing disambiguity MsCore/Linq
    public static class QueryableExtensions
    {
    
        public static IIncludableQueryable<TEntity, TProperty> IncludeCore<TEntity, TProperty>(this System.Linq.IQueryable<TEntity> source, System.Linq.Expressions.Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath);
        }
        public static System.Linq.IQueryable<T> AsNoTracking<T>(this System.Linq.IQueryable<T> source) where T : class
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(source);
        }


    }
}
