using System;
using System.Linq.Expressions;

namespace Question
{
    public class Sorting<T>
    {
        public Expression<Func<T, object>> Field { get; }
        public SortingDirection Direction { get; }

        public Sorting(Expression<Func<T, object>> field, SortingDirection direction)
        {
            Field = field;
            Direction = direction;
        }
    }
}