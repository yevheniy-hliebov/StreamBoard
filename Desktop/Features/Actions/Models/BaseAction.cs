using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Variables.Services;
using System.Reflection;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Models
{
    public abstract class BaseAction : ObservableObject
    {
        private static string GenerateId() => Guid.NewGuid().ToString();

        [JsonPropertyName("id")]
        public string Id { get; protected set; } = GenerateId();

        public void RegenerateId()
        {
            Id = GenerateId();
        }

        [JsonIgnore]
        public abstract ActionMetadata Metadata { get; }

        [JsonIgnore]
        public string CategoryName
        {
            get
            {
                var categoryAttribute = GetType().GetCustomAttribute<ActionCategoryAttribute>(true);
                return categoryAttribute?.Name ?? "Uncategorized";
            }
        }

        [JsonIgnore]
        public virtual string Label => Metadata.Name;

        [JsonIgnore]
        public string FullLabel => $"{CategoryName} | {Label}";

        public abstract Task ExecuteAsync(ActionExecutionContext context);
               
        public abstract BaseAction Copy();
        public BaseAction CopyWithNewId()
        {
            var copied = Copy();
            copied.RegenerateId();
            return copied;
        }

        public static string ResolveVariable(string value, ActionExecutionContext context)
        {
            var variableService = App.ServiceProvider.GetRequiredService<IVariableService>();

            return variableService.Resolve(value, context);
        }
    }
}
