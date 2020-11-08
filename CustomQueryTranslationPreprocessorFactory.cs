using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace Question
{
    public class CustomExpressionModifier : ExpressionVisitor
    {
        private static readonly MethodInfo? orderByAllMethod
            = typeof(IQueryableExtensions).GetMethod(nameof(IQueryableExtensions.OrderByAll));

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.GetGenericMethodDefinition() == orderByAllMethod)
            {
                
                return base.VisitMethodCall(node);
            }
            return base.VisitMethodCall(node);
        }
    }

    public class CustomQueryTranslationPreprocessor : RelationalQueryTranslationPreprocessor
    {
        public CustomQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext) { }
        public override Expression Process(Expression query) => base.Process(Preprocess(query));
        private Expression Preprocess(Expression query)
        {
            query = new CustomExpressionModifier().Visit(query);               
            return query;
        }
    }

    public class CustomQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
    {
        public CustomQueryTranslationPreprocessorFactory(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
        {
            Dependencies = dependencies;
            RelationalDependencies = relationalDependencies;
        }
        protected QueryTranslationPreprocessorDependencies Dependencies { get; }
        protected RelationalQueryTranslationPreprocessorDependencies RelationalDependencies;
        public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
            => new CustomQueryTranslationPreprocessor(Dependencies, RelationalDependencies, queryCompilationContext);
    }
}