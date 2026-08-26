using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Actions.Attributes;
using System.Reflection;
using System.Text.Json;
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
        public ActionMetadata Metadata
        {
            get
            {
                var attr = GetType().GetCustomAttribute<ActionInfoAttribute>();
                return attr != null
                    ? new ActionMetadata(attr.Name, attr.DialogTitle, attr.Icon)
                    : new ActionMetadata(GetType().Name, "Settings", Controls.Icons.FluentIconType.Settings);
            }
        }

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
        public virtual string Label
        {
            get
            {
                var props = GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() != null
                             && p.PropertyType == typeof(string)
                             && p.Name != nameof(Id));

                var values = props.Select(p => p.GetValue(this) as string)
                                  .Where(v => !string.IsNullOrEmpty(v))
                                  .ToList();

                if (values.Count == 0) return Metadata.Name;
                return $"{Metadata.Name} ({string.Join(", ", values)})";
            }
        }

        [JsonIgnore]
        public string FullLabel => $"{CategoryName} | {Label}";

        public abstract Task ExecuteAsync(object? data = null);


        public virtual BaseAction Copy()
        {
            var json = JsonSerializer.Serialize(this, GetType());
            return (BaseAction)JsonSerializer.Deserialize(json, GetType())!;
        }

        public BaseAction CopyWithNewId()
        {
            var copied = Copy();
            copied.RegenerateId();
            return copied;
        }
    }
}
