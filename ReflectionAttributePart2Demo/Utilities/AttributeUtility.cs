using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ReflectionAttributePart2Demo.Utilities
{
    public static class AttributeUtility
    {
        /// <summary>
        /// Return the requested Attribute for the passed propertyName
        /// </summary>
        /// <typeparam name="TClass">class to be searched</typeparam>
        /// <typeparam name="TAttribute">attribute that is being returned</typeparam>
        /// <param name="propertyName">PropertyName that we're going to look for an Attribute on</param>
        /// <returns>the Attribute if it exists</returns>
        public static TAttribute? GetAttribute<TClass, TAttribute>(string propertyName)
        {
            Type modelType = typeof(TClass);

            // first check for standard attribute
            var attribute = (TAttribute?)modelType.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase)?
                .GetCustomAttributes(typeof(TAttribute), true)
                .SingleOrDefault();

            if (attribute == null)
            {
                // if the standard attribute is null, look for a metadata attribute
                var metadataTypeAttribute = (MetadataTypeAttribute?)modelType.GetCustomAttributes(typeof(MetadataTypeAttribute), true)
                    .FirstOrDefault();

                if (metadataTypeAttribute != null)
                {
                    var property = metadataTypeAttribute
                        .MetadataClassType
                        .GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);

                    if (property != null)
                    {
                        attribute = (TAttribute?)property.GetCustomAttributes(typeof(TAttribute), true)
                            .FirstOrDefault();
                    }
                }
            }

            return attribute;
        }

        /// <summary>
        /// Get the name of a property from the passed expression
        /// </summary>
        /// <typeparam name="TClass">class to be searched</typeparam>
        /// <param name="expression">Expression that points to a class property.</param>
        /// <returns>Property name that the expression is referencing</returns>
        public static string GetPropertyNameFromExpression<TClass>(Expression<Func<TClass, object>> expression)
        {
            List<string> propertyList = new List<string>();

            //unless it's a root property the expression's NodeType will always be Convert
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = expression.Body as UnaryExpression;

                    propertyList = (unaryExpression?.Operand ?? Expression.Empty())
                        .ToString()
                        .Split(".")
                        .Skip(1) // ignore the root property
                        .ToList();
                    break;

                default:
                    propertyList = expression.Body
                        .ToString()
                        .Split(".")
                        .Skip(1) // ignore the root property
                        .ToList();
                    break;
            }

            // return the last matching property
            return propertyList.LastOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Return the requested Attribute for the passed expression
        /// </summary>
        /// <typeparam name="TClass">class to be searched</typeparam>
        /// <typeparam name="TAttribute">attribute that is being returned</typeparam>
        /// <param name="expression">Expression that points to a class property.</param>
        /// <returns>the Attribute if it exists</returns>
        public static TAttribute? GetAttribute<TClass, TAttribute>(Expression<Func<TClass, object>> expression)
        {
            var propertyName = GetPropertyNameFromExpression(expression);

            return GetAttribute<TClass, TAttribute>(propertyName);
        }

        /// <summary>
        /// based on the passed expression, lookup the PropertyName and then lookup the DisplayAttribute.Name value
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetDisplayName<TClass>(Expression<Func<TClass, object>> expression)
        {
            // get the property name using the expression
            var propertyName = GetPropertyNameFromExpression(expression);

            // lookup the DisplayAttribute.Name value
            return GetDisplayName<TClass>(propertyName);
        }

        /// <summary>
        /// based on the passed property name, lookup the DisplayAttribute.Name value
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetDisplayName<TClass>(string propertyName)
        {
            // get the Display attribute for the property name
            var attr = GetAttribute<TClass, DisplayAttribute>(propertyName);

            if (attr != null)
            {
                return attr.Name ?? propertyName;
            }

            // by default, just return the propertyName
            return propertyName;
        }
    }
}
