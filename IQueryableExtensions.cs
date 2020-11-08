using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Question
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<Post> OrderByCommon(this IQueryable<Post> queryable, PostOrder orderBy)
            => orderBy switch
            {
                PostOrder.TitleAsc => queryable.OrderBy(x => x.Title),
                PostOrder.TitleDesc => queryable.OrderByDescending(x => x.Title),
                PostOrder.ScoreAsc => queryable.OrderBy(x => x.Score).ThenBy(x => x.Title),
                PostOrder.ScoreDesc => queryable.OrderByDescending(x => x.Score).ThenBy(x => x.Title),
                _ => throw new NotSupportedException(),
            };

        public static IOrderedQueryable<TModel> PartitionBy<TModel, TKey>(
            this IQueryable<TModel> queryable, 
            Expression<Func<TModel, TKey>> fieldSelector, 
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy,
            int skip, int take)
        {
            // key
            ParameterExpression key = Expression.Parameter(typeof(TKey), "key");
            // x
            ParameterExpression x = Expression.Parameter(typeof(TModel), "x");
            // fieldSelector(x) == key
            Expression predicateBody = Expression.Equal(
                Expression.Invoke(fieldSelector, new Expression[] { x }),
                key
            );
            // orderBy(queryable).Where(x => fieldSelector(x) == key)
            MethodCallExpression whereCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Where),  
                new Type[] { typeof(TModel) },  
                orderBy(queryable).Expression,  
                Expression.Lambda<Func<TModel, bool>>(
                    predicateBody, 
                    new ParameterExpression[] { x })
            );
            // orderBy(queryable).Where(x => fieldSelector(x) == key).Skip(skip)
            MethodCallExpression skipCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Skip),  
                new Type[] { typeof(TModel) },  
                whereCallExpression,  
                Expression.Constant(skip)
            );
            // orderBy(queryable).Where(x => fieldSelector(x) == key).Skip(skip).Take(take)
            MethodCallExpression takeCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Take),  
                new Type[] { typeof(TModel) },  
                skipCallExpression,  
                Expression.Constant(take)
            );
            // key => orderBy(queryable).Where(x => fieldSelector(x) == key).Skip(skip).Take(take)
            var selectManyExpr = Expression.Lambda<Func<TKey, IEnumerable<TModel>>>(
                takeCallExpression,
                new ParameterExpression[] { key });

            var filteredQueryable = queryable
                .Select(fieldSelector)
                .Distinct()
                .SelectMany<TKey, TModel, TModel>(selectManyExpr, (a, b) => b);
            return orderBy(filteredQueryable);
        }

        public static IOrderedQueryable<TModel> PartitionBy<TModel, TKey>(
            this IQueryable<TModel> queryable, 
            Expression<Func<TModel, TKey>> groupBy, 
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy,
            Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> limitBy)
        {
            // key
            ParameterExpression key = Expression.Parameter(typeof(TKey), "key");
            // x
            ParameterExpression x = Expression.Parameter(typeof(TModel), "x");
            // groupBy(x) == key
            Expression predicateBody = Expression.Equal(
                Expression.Invoke(groupBy, new Expression[] { x }),
                key
            );
            // orderBy(queryable).Where(x => groupBy(x) == key)
            MethodCallExpression whereCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Where),  
                new Type[] { typeof(TModel) },  
                orderBy(queryable).Expression,  
                Expression.Lambda<Func<TModel, bool>>(
                    predicateBody, 
                    new ParameterExpression[] { x })
            );
            // key => orderBy(queryable).Where(x => groupBy(x) == key).Skip(skip).Take(take)
            var selectManyExpr = Expression.Lambda<Func<TKey, IEnumerable<TModel>>>(
                Expression.Invoke(limitBy, new Expression[] { whereCallExpression }),
                new ParameterExpression[] { key });

            var filteredQueryable = queryable
                .Select(groupBy)
                .Distinct()
                .SelectMany<TKey, TModel, TModel>(selectManyExpr, (a, b) => b);
            return orderBy(filteredQueryable);
        }
    }
}