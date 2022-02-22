namespace Universe.Sp.CQRS.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.SharePoint;

    public class SpMapper
    {
        private List<PropertyInfo> GetProperties<TEntity>() where TEntity : class, new()
        {
            var type = typeof(TEntity);
            return GetProperties(type);
        }

        private List<PropertyInfo> GetProperties(Type incomingtype)
        {
            var type = incomingtype;

            var properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
                                    | BindingFlags.GetProperty | BindingFlags.SetProperty);


            var q = properties.ToList();
            q = q.Where(a => a.PropertyType.Name != "SPListItem" && a.Name != "Id" && a.Name != "ListUrl").ToList();

            return q.ToList();
        }

        public void Map<TEntity>(TEntity entitySp, SPListItem item) where TEntity : class, new()
        {
            var properties = GetProperties<TEntity>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(entitySp);

                item[name] = value;
            }
        }
    }
}