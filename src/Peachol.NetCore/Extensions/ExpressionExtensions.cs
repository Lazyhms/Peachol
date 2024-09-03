namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) where T : class
        => first.Compose(second, Expression.AndAlso);

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) where T : class
        => first.Compose(second, Expression.OrElse);

    private static Expression<TDelegate> Compose<TDelegate>(this Expression<TDelegate> first, Expression<TDelegate> second, Func<Expression, Expression, Expression> merge) where TDelegate : class
    {
        var visitor = new ParameterVisitor<TDelegate>();
        var expression = merge(visitor.Visit(first.Body)!, visitor.Visit(second.Body)!);
        return Expression.Lambda<TDelegate>(expression, first.Parameters);
    }

    private sealed class ParameterVisitor<T>() : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression parameter)
            => Expression.Parameter(typeof(T), "filter");
    }
}