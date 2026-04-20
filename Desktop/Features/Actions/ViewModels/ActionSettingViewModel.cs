using StreamBoard.Core;
using StreamBoard.Features.Actions.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Actions.ViewModels
{
    public abstract class ActionSettingViewModel : ObservableObject
    {
        public string Label { get; }
        public string? Hint { get; }
        protected object TargetAction { get; }
        protected PropertyInfo Property { get; }

        protected ActionSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
        {
            Label = label;
            Hint = hint;
            TargetAction = targetAction;
            Property = property;
        }
    }

    public class StringSettingViewModel : ActionSettingViewModel
    {
        public StringSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class IntSettingViewModel : ActionSettingViewModel
    {
        public IntSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public int Value
        {
            get => Property.GetValue(TargetAction) is int val ? val : 0;
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class BoolSettingViewModel : ActionSettingViewModel
    {
        public BoolSettingViewModel(string label, string? hint, object targetAction, PropertyInfo property)
            : base(label, hint, targetAction, property) { }

        public bool Value
        {
            get => Property.GetValue(TargetAction) is bool val && val;
            set
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
            }
        }
    }

    public class DropdownSettingViewModel : ActionSettingViewModel
    {
        private readonly IOptionsProvider _provider;
        private readonly BaseAction _action;

        public ObservableCollection<string> Options { get; } = new();

        public DropdownSettingViewModel(
            string label,
            string? hint,
            BaseAction targetAction,
            PropertyInfo property,
            IOptionsProvider provider)
            : base(label, hint, targetAction, property)
        {
            _action = targetAction;
            _provider = provider;

            _action.PropertyChanged += OnActionPropertyChanged;

            RefreshOptions();
        }

        public string Value
        {
            get => Property.GetValue(TargetAction) as string ?? "";
            set
            {
                if (Value != value)
                {
                    Property.SetValue(TargetAction, value);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectOptionCommand => new RelayCommand<string>(option =>
        {
            if (option != null)
            {
                Value = option;
            }
        });

        private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshOptions();
        }

        private void RefreshOptions()
        {
            var newOptions = _provider.GetOptions(_action) ?? [];

            if (!Options.SequenceEqual(newOptions))
            {
                Options.Clear();
                foreach (var opt in newOptions)
                {
                    Options.Add(opt);
                }

                OnPropertyChanged(nameof(Options));
            }
        }
    }

    public class SearchSettingViewModel : ActionSettingViewModel
    {
        private readonly IAsyncSearchProvider _provider;
        private readonly string? _displayPropertyName; // Зберігаємо назву цільової властивості
        private string _searchText = string.Empty;
        private CancellationTokenSource? _cts;

        public ObservableCollection<SearchResult> SearchResults { get; } = new();

        public SearchSettingViewModel(
            string label,
            string? hint,
            object targetAction,
            PropertyInfo property,
            IAsyncSearchProvider provider,
            string? displayPropertyName = null) // Додано параметр
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
                System.Diagnostics.Debug.WriteLine($"[SearchSetting] Error: {ex.Message}");
            }
        }
    }
}