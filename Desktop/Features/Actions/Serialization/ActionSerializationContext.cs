using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;

namespace StreamTabula.Features.Actions.Serialization
{
    public static class ActionSerializationContext
    {
        private static List<JsonDerivedType>? _cachedDerivedTypes;

        public static DefaultJsonTypeInfoResolver GetResolver()
        {
            return new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ResolveActionTypes }
            };
        }

        private static void ResolveActionTypes(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Type != typeof(BaseAction)) return;

            var options = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "type",
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
            };

            _cachedDerivedTypes ??= FindAllActionTypes();

            foreach (var derivedType in _cachedDerivedTypes)
            {
                options.DerivedTypes.Add(derivedType);
            }

            typeInfo.PolymorphismOptions = options;
        }

        private static List<JsonDerivedType> FindAllActionTypes()
        {
            var derivedTypes = new List<JsonDerivedType>();
            var assembly = Assembly.GetExecutingAssembly();

            var actionTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseAction)));

            foreach (var type in actionTypes)
            {
                var attr = type.GetCustomAttribute<ActionDiscriminatorAttribute>();

                string discriminator = attr?.Discriminator
                                       ?? type.Name.Replace("Action", "").ToLower();

                derivedTypes.Add(new JsonDerivedType(type, discriminator));
            }

            return derivedTypes;
        }
    }
}
