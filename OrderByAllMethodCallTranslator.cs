using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Question
{
    public class OrderByAllMethodCallTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory expressionFactory;

        private static readonly MethodInfo? orderByAllMethod
            = typeof(IQueryableExtensions).GetMethod(nameof(IQueryableExtensions.OrderByAll));

        public OrderByAllMethodCallTranslator(ISqlExpressionFactory expressionFactory)
        {
            this.expressionFactory = expressionFactory;
        }

        public SqlExpression? Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method != orderByAllMethod || !arguments.Any())
                return null;
            return null;
            // var queryableInstance = arguments[0];
            // var orderingExpression = new OrderingExpression()
            // var args = new List<SqlExpression> { arguments[1], arguments[2] }; // cut the first parameter from extension function
            // if (method == _encryptMethod)
            // {
            //     return _expressionFactory.Function(instance, "EncryptByPassPhrase", args, typeof(byte[]));
            // }
            // if (method == _decryptMethod)
            // {
            //     return _expressionFactory.Function(instance, "DecryptByPassPhrase", args, typeof(byte[]));
            // }

            // if (method == _decryptByKeyMethod)
            // {
            //     return _expressionFactory.Function(instance, "DecryptByKey", args, typeof(byte[]));
            // }

            // return null;
        }
    }
}