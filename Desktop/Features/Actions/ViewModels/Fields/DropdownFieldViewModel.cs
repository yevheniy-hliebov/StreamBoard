using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using StreamTabula.Features.Actions.Models;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Actions.ViewModels;

public class DropdownFieldViewModel : ActionFieldViewModel
{
    private readonly IOptionsProvider _provider;
    private readonly BaseAction _action;

    public ObservableCollection<DropdownOption> Options { get; } = new();

    public DropdownFieldViewModel(
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

    public object? Value
    {
        get => Property.GetValue(TargetAction);
        set
        {
            if (!Equals(Value, value))
            {
                Property.SetValue(TargetAction, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedDisplayOption));
            }
        }
    }

    public string? SelectedDisplayOption
    {
        get
        {
            var currentVal = Value;
            if (currentVal == null || (currentVal is string s && string.IsNullOrWhiteSpace(s)))
                return null;

            var selectedOpt = Options.FirstOrDefault(o => Equals(o.Value, currentVal));
            return selectedOpt?.DisplaySelectedOption ?? currentVal.ToString();
        }
    }

    public ICommand SelectOptionCommand => new RelayCommand<DropdownOption>(option =>
    {
        if (option != null)
        {
            Value = option.Value;
        }
    });

    private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BaseAction.Label))
            return;

        if (e.PropertyName == Property.Name)
            return;

        RefreshOptions();
    }

    private void RefreshOptions()
    {
        var rawOptions = _provider.GetOptions(_action) ?? [];

        Options.Clear();

        foreach (var opt in rawOptions)
        {
            if (opt is DropdownOption dropOption)
            {
                Options.Add(dropOption);
            }
            else if (opt is string strOption)
            {
                Options.Add(new DropdownOption(strOption));
            }
            else if (opt != null)
            {
                Options.Add(new DropdownOption(opt));
            }
        }

        OnPropertyChanged(nameof(Options));
        OnPropertyChanged(nameof(SelectedDisplayOption));
    }
}