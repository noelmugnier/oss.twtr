using System.Linq.Expressions;
using System.Reflection;
using FluentMigrator;
using LinqToDB.Mapping;

namespace OSS.Twtr.Infrastructure;

public abstract class DbMigration : Migration
{
    protected string GetColumnName<T>(Expression<Func<T, object>> action) where T : class
    {
        var type = typeof(T);
        
        MemberExpression memberExpression;
        if (action.Body is UnaryExpression unaryExpression)
            memberExpression = (MemberExpression)(unaryExpression.Operand);
        else
            memberExpression = (MemberExpression)(action.Body);

        var name = ((PropertyInfo)memberExpression.Member).Name;
        var property = type.GetProperty(name)!;
        var result = ((ColumnAttribute)property.GetCustomAttributes(true).FirstOrDefault(a => a is ColumnAttribute))?.Name ?? name;
        return result;
    }

    protected string GetTableName<T>()
    {
        var type = typeof(T);
        var result = ((TableAttribute)type.GetCustomAttributes(true).FirstOrDefault(a => a is TableAttribute))?.Name ?? type.Name;
        return result;
    }
}