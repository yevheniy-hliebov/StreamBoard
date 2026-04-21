using System.Collections.ObjectModel;
using System.Reflection;
using StreamBoard.Features.Actions.Models;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class SearchFieldViewModel : ActionFieldViewModel
    {
        private readonly IAsyncSearchProvider _provider;
        private readonly string? _displayPropertyName;
        private string _searchText = string.Empty;
        private CancellationTokenSource? _cts;

        public ObservableCollection<SearchResult> SearchResults { get; } = new();

        public SearchFieldViewModel(
            string label,
            string? hint,
            object targetAction,
            PropertyInfo property,
            IAsyncSearchProvider provider,
            string? displayPropertyName = null)
            : base(label, hint, targetAction, property)
        {
            _provider = provider;
            _displayPropertyName = displayPropertyName;

            if (!string.IsNullOrEmpty(_displayPropertyName))
            {
                var nameProp = TargetAction.GetType().GetProperty(_displayPropertyName);
                if (nameProp != null && nameProp.CanRead)
                {
                    _searchText = nameProp.GetValue(TargetAction) as string ?? "";
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();

                var selected = SearchResults.FirstOrDefault(r => r.DisplayName == value);
                if (selected != null)
                {
                    if (Property.CanWrite)
                    {
                        Property.SetValue(TargetAction, selected.Id);
                    }

                    if (!string.IsNullOrEmpty(_displayPropertyName))
                    {
                        var nameProp = TargetAction.GetType().GetProperty(_displayPropertyName);
                        if (nameProp != null && nameProp.CanWrite)
                        {
                            nameProp.SetValue(TargetAction, selected.DisplayName);
                        }
                    }
                    return;
                }

                DebounceSearch(value);
            }
        }

        private async void DebounceSearch(string query)
        {
            _cts?.Cancel();
            if (string.IsNullOrWhiteSpace(query))
            {
                SearchResults.Clear();
                return;
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Delay(500, token);

                var results = await _provider.SearchAsync(query);

                if (!token.IsCancellationRequested)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SearchResults.Clear();
                        foreach (var res in results) SearchResults.Add(res);
                    });
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SearchField] Error: {ex.Message}");
            }
        }
    }
}