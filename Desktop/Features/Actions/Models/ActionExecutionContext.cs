namespace StreamTabula.Features.Actions.Models
{
    public class ActionExecutionContext
    {
        public Guid ExecutionId { get; } = Guid.NewGuid();

        public Dictionary<string, object> RuntimeVariables { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
