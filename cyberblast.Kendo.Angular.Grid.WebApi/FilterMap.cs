using cyberblast.Kendo.Angular.Grid.WebApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace cyberblast.Kendo.Angular.Grid.WebApi
{
    public static class FilterMap
    {
        public static Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(this GridState state, Dictionary<string, string> fieldmap = null) {
            if (state.Filter != null && state.Filter.Filters != null && state.Filter.Filters.Count > 0) {
                var entity = Expression.Parameter(typeof(TEntity), "entity");
                var body = ResolveCompositeFilterDescriptor(state.Filter, entity, fieldmap);
                return Expression.Lambda<Func<TEntity, bool>>(body, entity);
            }
            return null;
        }

        private static Expression ResolveCompositeFilterDescriptor(CompositeFilterDescriptor cfd, ParameterExpression entity, Dictionary<string, string> fieldmap = null) {
            Expression result = null;
            foreach (var sub in cfd.Filters) {
                if (sub.ContainsKey("filters")) {
                    // sub is composite
                    var composite = sub.ToObject<CompositeFilterDescriptor>();
                    if (composite != null) {
                        var compEx = ResolveCompositeFilterDescriptor(composite, entity, fieldmap);
                        CombineExpressions(ref result, compEx, cfd.Logic);
                    }
                } else {
                    // sub is filter
                    var filter = sub.ToObject<FilterDescriptor>();
                    if (filter != null) {
                        var filterEx = ResolveFilterDescriptor(filter, entity, fieldmap);
                        CombineExpressions(ref result, filterEx, cfd.Logic);
                    }
                }
            }
            return result;
        }

        private static Expression ResolveFilterDescriptor(FilterDescriptor filter, ParameterExpression entity, Dictionary<string, string> fieldmap = null) {
            // map filter field to different properties
            string fieldName;
            if (fieldmap != null && fieldmap.ContainsKey(filter.Field)) {
                fieldName = fieldmap[filter.Field];
            } else {
                fieldName = filter.Field;
            }

            // use dot to resolve 1 to many relations
            if (fieldName.Contains(".")) {
                var field = fieldName.Split('.');
                var prop = Expression.Property(entity, field[0]);
                var propertyType = ((PropertyInfo)prop.Member).PropertyType;

                var parameter = Expression.Parameter(propertyType.GenericTypeArguments[0]);
                var body = ExpressionBody(parameter, field[1], filter);
                var subLambda = Expression.Lambda(body, parameter);

                // we are looking for any item with a matching property -> Any
                return Expression.Call(typeof(Enumerable), "Any", new[] { propertyType.GenericTypeArguments[0] }, prop, subLambda);
            } else {
                return ExpressionBody(entity, fieldName, filter);
            }
        }

        private static Expression ExpressionBody(ParameterExpression parameter, string fieldName, FilterDescriptor filter) {
            var prop = Expression.Property(parameter, fieldName);
            var propertyType = ((PropertyInfo)prop.Member).PropertyType;
            var valueExpression = GetValueExpression(propertyType, filter.Value);
            return Operator[filter.Operator](prop, valueExpression);
        }

        private static void CombineExpressions(ref Expression result, Expression partial, string logic) {
            if (result == null) {
                result = partial;
            } else {
                var combine = Combiner[logic];
                result = combine(result, partial);
            }
        }
        
        private static Expression GetValueExpression(Type propertyType, object value) {
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(value.GetType())) {
                // cannot convert, try raw usage
                return Expression.Constant(value);
            }

            var propertyValue = converter.ConvertFrom(value);
            var constant = Expression.Constant(propertyValue);
            return Expression.Convert(constant, propertyType);
        }

        private static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> Combiner =
        new Dictionary<string, Func<Expression, Expression, BinaryExpression>> {
            { "and",  Expression.AndAlso },
            { "or",  Expression.OrElse }
        };

        private static readonly Dictionary<string, Func<MemberExpression, Expression, Expression>> Operator =
        new Dictionary<string, Func<MemberExpression, Expression, Expression>>
        {
            { "eq", Expression.Equal },
            { "gt", Expression.GreaterThan },
            { "gte", Expression.GreaterThanOrEqual },
            { "lt", Expression.LessThan },
            { "lte", Expression.LessThanOrEqual },
            { "contains", Contains }
        };

        private static readonly MethodInfo StringContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static Func<MemberExpression, Expression, Expression> Contains = (MemberExpression member, Expression value) => Expression.Call(member, ((PropertyInfo)member.Member).PropertyType == typeof(string) ? StringContains : ((PropertyInfo)member.Member).PropertyType.GetMethod("Contains", new[] { (member.Member as PropertyInfo).PropertyType.GenericTypeArguments[0] }), value);
    }
}
