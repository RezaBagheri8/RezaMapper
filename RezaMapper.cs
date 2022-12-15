using System.Linq.Expressions;

namespace EfCore.Api.Services;

public static class RezaMapper
{
    public static TDestination Map<TSource, TDestination>(TSource source)
        where TSource : class
        where TDestination : class
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var destinationProperties = typeof(TDestination).GetProperties();

        List<MemberBinding> memberBindings = new List<MemberBinding>();
        var destinationObject = Expression.New(typeof(TDestination));

        foreach (var property in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(r => String.Equals(r.Name, property.Name, StringComparison.CurrentCultureIgnoreCase));
            
            if (destinationProperty is not null && property.PropertyType == destinationProperty.PropertyType)
            {
                ConstantExpression memberValue = Expression.Constant(property.GetValue(source));

                MemberAssignment memberAssignment = Expression.Bind(destinationProperty, memberValue);

                memberBindings.Add(memberAssignment);
            }
        }

        MemberInitExpression memberInitExpression = Expression.MemberInit(destinationObject, memberBindings);

        return Expression.Lambda<Func<TDestination>>(memberInitExpression).Compile().Invoke();
    }
}