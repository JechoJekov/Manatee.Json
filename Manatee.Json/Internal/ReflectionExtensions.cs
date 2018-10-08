using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;
using Manatee.Json.Serialization.Internal.Serializers;

namespace Manatee.Json.Internal
{
	internal static class ReflectionExtensions
	{
		// Note: These methods assume that if a generic type is passed, the type is open.
		public static bool InheritsFrom(this Type tDerived, Type tBase)
		{
			if (tDerived._IsSubtypeOf(tBase)) return true;
			var interfaces = tDerived.GetTypeInfo().ImplementedInterfaces.SelectMany(_GetAllInterfaces);
			return interfaces.Contains(tBase);
		}
		private static IEnumerable<Type> _GetAllInterfaces(Type type)
		{
			if (type.GetTypeInfo().IsGenericType)
				yield return type.GetGenericTypeDefinition();
			yield return type;
		}
		private static bool _IsSubtypeOf(this Type tDerived, Type tBase)
		{
			var currentType = tDerived.GetTypeInfo().BaseType;
			while (currentType != null)
			{
				if (currentType.GetTypeInfo().IsGenericType)
					currentType = currentType.GetGenericTypeDefinition();
				if (currentType == tBase) return true;
				currentType = currentType.GetTypeInfo().BaseType;
			}
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetTypeArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}
		public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo type)
		{
			var properties = new List<PropertyInfo>();
			while (type != null)
			{
				properties.AddRange(type.DeclaredProperties);
				type = type.BaseType?.GetTypeInfo();
			}
			return properties;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumericType(this Type value)
		{
			return IsFloat(value) || IsInteger(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFloat(this Type type)
		{
			return type == typeof(decimal) ||
			       type == typeof(double) ||
			       type == typeof(float);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInteger(this Type type)
		{
			return type == typeof(int) ||
			       type == typeof(uint) ||
			       type == typeof(short) ||
			       type == typeof(ushort) ||
			       type == typeof(byte) ||
			       type == typeof(long) ||
			       type == typeof(ulong);
		}

		public static object Default(this Type type)
		{
			return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
		}

		public static void SetMember(this object obj, JsonPointer pointer, object value)
		{
			if (obj == null) return;
			if (pointer.Count == 0)
				throw new ArgumentException("Pointer must have at least one segment.");

			SerializationInfo member;
			var segment = pointer[0];
			if (int.TryParse(segment, out var index))
			{
				member = ReflectionCache.GetMembers(obj.GetType(), PropertySelectionStrategy.ReadWriteOnly, false)
					.Single(m => m.MemberInfo is PropertyInfo pi && pi.GetIndexParameters().Length == 1);
				if (member == null) return;

				var indexer = (PropertyInfo) member.MemberInfo;
				if (pointer.Count == 1)
				{
					indexer.SetValue(obj, value, new object[] {index});
					return;
				}

				var local = indexer.GetValue(obj, new object[] { index });
				SetMember(local, new JsonPointer(pointer.Skip(1)), value);
				return;
			}

			member = ReflectionCache.GetMembers(obj.GetType(), PropertySelectionStrategy.ReadWriteOnly, true)
				.Single(m => m.SerializationName == segment);
			if (member == null) return;

			if (member.MemberInfo is PropertyInfo asProperty)
			{
				if (pointer.Count == 1)
				{
					asProperty.SetValue(obj, value);
					return;
				}

				var local = asProperty.GetValue(obj);
				SetMember(local, new JsonPointer(pointer.Skip(1)), value);
				return;
			}

			if (member.MemberInfo is FieldInfo asField)
			{
				if (pointer.Count == 1)
				{
					asField.SetValue(obj, value);
					return;
				}

				var local = asField.GetValue(obj);
				SetMember(local, new JsonPointer(pointer.Skip(1)), value);
				return;
			}
		}
	}
}