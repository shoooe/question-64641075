using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace Question
{
    public sealed class MethodCallTranslatorPlugin : NpgsqlMethodCallTranslatorProvider
    {
        public MethodCallTranslatorPlugin(
            RelationalMethodCallTranslatorProviderDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource)
            : base(dependencies, typeMappingSource)
        {
            ISqlExpressionFactory expressionFactory = dependencies.SqlExpressionFactory;
            this.AddTranslators(new List<IMethodCallTranslator>
            {
                new OrderByAllMethodCallTranslator(expressionFactory)
            });
        }
    }
}