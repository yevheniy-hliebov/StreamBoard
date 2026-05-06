using StreamTabula.Core;
using StreamTabula.Features.Actions.Services;

namespace StreamTabula.Features.Actions.ViewModels
{
    public class ActionLibraryViewModel : ObservableObject
    {
        private readonly ActionRegistry _registry;

        public IEnumerable<ActionCategoryViewModel> _allCategories => _registry.Categories;

        private List<ActionCategoryViewModel> _filteredCategories;
        public List<ActionCategoryViewModel> FilteredCategories
        {
            get => _filteredCategories;
            set => SetProperty(ref _filteredCategories, value);
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterCategories(value);
                }
            }
        }

        public ActionLibraryViewModel(ActionRegistry registry)
        {
            _registry = registry;

            _filteredCategories = _allCategories.ToList();
        }

        private void FilterCategories(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                FilteredCategories = _allCategories.ToList();
                return;
            }

            var result = new List<ActionCategoryViewModel>();
            var cleanQuery = query.Trim();

            foreach (var category in _allCategories)
            {
                bool categoryMatches = category.Name.Contains(cleanQuery, StringComparison.OrdinalIgnoreCase);

                if (categoryMatches)
                {
                    result.Add(category);
                }
                else
                {
                    var matchingActions = category.Actions
                        .Where(a => a.Metadata.Name.Contains(cleanQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matchingActions.Count > 0)
                    {
                        var tempCategory = new ActionCategoryViewModel(
                            category.Name,
                            category.Icon,
                            category.IntegrationIcon
                        );

                        tempCategory.IsExpanded = true;

                        foreach (var action in matchingActions)
                        {
                            tempCategory.Actions.Add(action);
                        }

                        result.Add(tempCategory);
                    }
                }
            }

            FilteredCategories = result;
        }
    }
}
