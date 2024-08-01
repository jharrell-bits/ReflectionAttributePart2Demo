using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ReflectionAttributeDemo.Utilities
{
    public static class AttributeHelper
    {
        /// <summary>
        /// Return the requested Attribute for the passed propertyName
        /// </summary>
        /// <typeparam name="TModel">class to be searched</typeparam>
        /// <typeparam name="TAttribute">attribute that is being returned</typeparam>
        /// <param name="propertyName">PropertyName that we're going to look for an Attribute on</param>
        /// <returns>the Attribute if it exists</returns>
        public static TAttribute? GetAttribute<TModel, TAttribute>(string propertyName)
        {
            Type type = typeof(TModel);

            var attr = (TAttribute?)type.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase)?
                .GetCustomAttributes(typeof(TAttribute), true)
                .SingleOrDefault();

            if (attr == null)
            {
                // if the attribute is null, look for a custom attribute (accounts for user created attributes)
                var metadataType = (MetadataTypeAttribute?)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
                    .FirstOrDefault();

                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType
                        .GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);

                    if (property != null)
                    {
                        attr = (TAttribute?)property.GetCustomAttributes(typeof(TAttribute), true)
                            .SingleOrDefault();
                    }
                }
            }

            return attr;
        }

        /// <summary>
        /// Get the name of a property from the passed expression
        /// </summary>
        /// <typeparam name="TModel">class to be searched</typeparam>
        /// <param name="expression">Expression that points to a class property.</param>
        /// <returns>Property name that the expression is referencing</returns>
        public static string GetPropertyNameFromExpression<TModel>(Expression<Func<TModel, object>> expression)
        {
            IEnumerable<string> propertyList = new List<string>();

            //unless it's a root property the expression's NodeType will always be Convert
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = expression.Body as UnaryExpression;

                    propertyList = (unaryExpression?.Operand ?? Expression.Empty())
                        .ToString()
                        .Split(".")
                        .Skip(1); // ignore the root property
                    break;

                default:
                    propertyList = expression.Body
                        .ToString()
                        .Split(".")
                        .Skip(1); // ignore the root property
                    break;
            }

            // return the property
            return propertyList.Last();
        }

        /// <summary>
        /// Return the requested Attribute for the passed expression
        /// </summary>
        /// <typeparam name="TModel">class to be searched</typeparam>
        /// <typeparam name="TAttribute">attribute that is being returned</typeparam>
        /// <param name="expression">Expression that points to a class property.</param>
        /// <returns>the Attribute if it exists</returns>
        public static TAttribute? GetAttribute<TModel, TAttribute>(Expression<Func<TModel, object>> expression)
        {
            var propertyName = GetPropertyNameFromExpression<TModel>(expression);

            return GetAttribute<TModel, TAttribute>(propertyName);
        }
    }
}
