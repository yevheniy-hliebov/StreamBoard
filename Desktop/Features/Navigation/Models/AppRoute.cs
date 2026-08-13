using StreamTabula.Controls.Icons;
using StreamTabula.Features.Integrations.Common.Models;

namespace StreamTabula.Features.Navigation.Models;

public record AppRoute(
    string Name,
    Type PageType,
    FluentIconType? FluentIcon = null,
    IntegrationIconType? IntegrationIcon = null,
    string? ParentName = null,
    bool IsFooter = false,
    bool AddSeparatorAfter = false
);