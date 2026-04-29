using StreamBoard.Core.Models;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Navigation.Models
{
    public record AppRoute(
        string Name,
        Type PageType,
        FluentIconType? FluentIcon = null,
        IntegrationIconType? IntegrationIcon = null,
        string? ParentName = null,
        bool IsFooter = false,
        bool AddSeparatorAfter = false
    );
}