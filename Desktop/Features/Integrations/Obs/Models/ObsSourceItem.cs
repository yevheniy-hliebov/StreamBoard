namespace StreamTabula.Features.Integrations.Obs.Models
{
    public class ObsSourceItem
    {
        public string Name { get; set; } = string.Empty;
        public bool IsGroup { get; set; }
        public bool IsNestedScene { get; set; }
        public bool IsInGroup { get; set; }
        public string? ParentGroupName { get; set; }
    }
}
