using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Question
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderByAll<T>(this IQueryable<T> queryable, IEnumerable<Sorting<T>> sortings)
        {
            if (!sortings.Any())
                throw new NotSupportedException();

            var sorting = sortings.First();
            IOrderedQueryable<T> orderedQueryable = sorting.Direction switch
            {
                SortingDirection.Asc => queryable.OrderBy(sorting.Field),
                SortingDirection.Desc => queryable.OrderByDescending(sorting.Field),
                _ => throw new NotSupportedException(),
            };
            sortings = sortings.Skip(1);
            while (sortings.Any())
            {
                var nextSorting = sortings.First();
                orderedQueryable = sorting.Direction switch
                {
                    SortingDirection.Asc => orderedQueryable.ThenBy(nextSorting.Field),
                    SortingDirection.Desc => orderedQueryable.ThenByDescending(nextSorting.Field),
                    _ => throw new NotSupportedException(),
                };
                sortings = sortings.Skip(1);
            }
            return orderedQueryable;
        }

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
            // queryable.Where(x => fieldSelector(x) == key)
            MethodCallExpression whereCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Where),  
                new Type[] { typeof(TModel) },  
                orderBy(queryable).Expression,  
                Expression.Lambda<Func<TModel, bool>>(
                    predicateBody, 
                    new ParameterExpression[] { x })
            );
            // queryable.Where(x => fieldSelector(x) == key).Skip(skip)
            MethodCallExpression skipCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Skip),  
                new Type[] { typeof(TModel) },  
                whereCallExpression,  
                Expression.Constant(skip)
            );
            // queryable.Where(x => fieldSelector(x) == key).Skip(skip).Take(take)
            MethodCallExpression takeCallExpression = Expression.Call(  
                typeof(Queryable),  
                nameof(Queryable.Take),  
                new Type[] { typeof(TModel) },  
                skipCallExpression,  
                Expression.Constant(take)
            );
            // key => queryable.Where(x => fieldSelector(x) == key).Skip(skip).Take(take)
            var selectManyExpr = Expression.Lambda<Func<TKey, IEnumerable<TModel>>>(
                takeCallExpression,
                new ParameterExpression[] { key });

            var filteredQueryable = queryable
                .Select(fieldSelector)
                .Distinct()
                .SelectMany<TKey, TModel, TModel>(selectManyExpr, (a, b) => b);
            return orderBy(filteredQueryable);
        }
    }
}