using System.Reflection;
using StreamBoard.Features.Decks.Models;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using StreamBoard.Features.Decks.Attributes;

namespace StreamBoard.Features.Decks.Serialization
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
            if (typeInfo.Type != typeof(DeckAction)) return;

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
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DeckAction)));

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
