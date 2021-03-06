﻿namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(string);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			return context.Source as string;
		}
		public object Deserialize(SerializationContext context)
		{
			return context.LocalValue.String;
		}
	}
}