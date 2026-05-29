using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Variables.Models;
using StreamTabula.Features.Variables.Services;

namespace StreamTabula.Features.Actions.Library.Variables
{
    [ActionDiscriminator("set_global_variable")]
    public class SetGlobalVariableAction : VariablesBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Set Global Variable",
            DialogTitle: "Configure Global Variable",
            Icon: FluentIconType.Globe
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _variableName = "";

        [InputField("Variable Name", Hint = "e.g., my_counter (without braces)")]
        [JsonPropertyName("variable_name")]
        public string VariableName
        {
            get => _variableName;
            set
            {
                _variableName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        private string _variableValue = "";

        [InputField("Value", Hint = "Enter text, number, or {other_variable}")]
        [JsonPropertyName("value")]
        public string Value
        {
            get => _variableValue;
            set
            {
                _variableValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrWhiteSpace(VariableName)
            ? Metadata.Name
            : $"Set Global: {VariableName}";

        public override Task ExecuteAsync(ActionExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(VariableName)) return Task.CompletedTask;

            string resolvedValue = ResolveVariable(Value, context);

            var variableService = App.ServiceProvider.GetRequiredService<IVariableService>();
            variableService.SetVariable(VariableName, VariableScope.Global, resolvedValue, context);

            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new SetGlobalVariableAction
        {
            Id = this.Id,
            VariableName = this.VariableName,
            Value = this.Value
        };
    }
}