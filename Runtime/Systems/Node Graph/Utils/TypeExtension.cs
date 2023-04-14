using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Konfus.Systems.Node_Graph
{
    public static class TypeExtension
    {
        public static bool IsReallyAssignableFrom(this Type type, Type otherType)
        {
            if (type.IsAssignableFrom(otherType))
                return true;
            if (otherType.IsAssignableFrom(type))
                return true;

            try
            {
                ParameterExpression v = Expression.Variable(otherType);
                UnaryExpression expr = Expression.Convert(v, type);
                return expr.Method != null && expr.Method.Name != "op_Implicit";
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        // https://www.codeproject.com/Tips/5267157/How-To-Get-A-Collection-Element-Type-Using-Reflect
        /// <summary>
        ///     Indicates whether or not the specified type is a collection.
        /// </summary>
        /// <param name="type">The type to query</param>
        /// <returns>True if the type is a list, otherwise false</returns>
        public static bool IsCollection(this Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            if (typeof(IList).IsAssignableFrom(type))
                return true;
            foreach (Type it in type.GetInterfaces())
                if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                    return true;
            return false;
        }

        // https://www.codeproject.com/Tips/5267157/How-To-Get-A-Collection-Element-Type-Using-Reflect
        /// <summary>
        ///     Retrieves the collection element type from this type
        /// </summary>
        /// <param name="type">The type to query</param>
        /// <returns>
        ///     The element type of the collection or null if the type was not a collection
        /// </returns>
        public static Type GetCollectionElementType(this Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            // first try the generic way
            // this is easy, just query the IEnumerable<T> interface for its generic parameter
            Type etype = typeof(IEnumerable<>);
            foreach (Type bt in type.GetInterfaces())
                if (bt.IsGenericType && bt.GetGenericTypeDefinition() == etype)
                    return bt.GetGenericArguments()[0];

            // now try the non-generic way

            // if it's a dictionary we always return DictionaryEntry
            if (typeof(IDictionary).IsAssignableFrom(type))
                return typeof(DictionaryEntry);

            // if it's a list we look for an Item property with an int index parameter
            // where the property type is anything but object
            if (typeof(IList).IsAssignableFrom(type))
                foreach (PropertyInfo prop in type.GetProperties())
                    if ("Item" == prop.Name && typeof(object) != prop.PropertyType)
                    {
                        ParameterInfo[] ipa = prop.GetIndexParameters();
                        if (1 == ipa.Length && typeof(int) == ipa[0].ParameterType) return prop.PropertyType;
                    }

            // if it's a collection, we look for an Add() method whose parameter is 
            // anything but object
            if (typeof(ICollection).IsAssignableFrom(type))
                foreach (MethodInfo meth in type.GetMethods())
                    if ("Add" == meth.Name)
                    {
                        ParameterInfo[] pa = meth.GetParameters();
                        if (1 == pa.Length && typeof(object) != pa[0].ParameterType)
                            return pa[0].ParameterType;
                    }

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return typeof(object);
            return null;
        }

        /// <summary>
        ///     Try to instantiate a type using the default constructor
        ///     Returns false if a parameter constructor doesn't exist
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <param name="instance">Returns the instantiated value. null if constructor isn't found</param>
        /// <returns>Whether the instantiation is successful</returns>
        public static bool TryInstantiate(this Type type, out object instance)
        {
            if (!type.HasDefaultConstructor())
            {
                instance = null;
                return false;
            }

            instance = Activator.CreateInstance(type);
            return true;
        }

        /// <summary>
        ///     Checks whether the given type has a parameterless constructor
        /// </summary>
        /// <param name="t">To to check</param>
        /// <returns>True if parameterless constructor is found</returns>
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static FieldInfo[] GetInstanceFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static PropertyInfo[] GetInstanceProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MemberInfo[] GetInstanceFieldsAndProperties(this Type type)
        {
            return type.GetInstanceFields().Cast<MemberInfo>()
                .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .ToArray();
        }

        public static MethodInfo[] GetInstanceMethodsByAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.HasCustomAttribute<T>()).ToArray();
        }
    }
}