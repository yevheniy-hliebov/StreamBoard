using StreamTabula.Components.Controls;
using StreamTabula.Core;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Views.Components.PropertyEditor;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Decks.ViewModels;

public class DynamicImageViewModel : ObservableObject
{
    public DynamicImageModel DynamicImage { get; }

    public List<DropdownOption> TriggerConditions { get; } =
    [
        new DropdownOption(TriggerConditionVariable.equal, "Change the image at a certain value of the variable."),
        new DropdownOption(TriggerConditionVariable.range, "Change the image over a range of values ​​of the variable.")
    ];

    public ObservableCollection<DynamicImageCondition> Conditions { get; }

    public bool IsRangeMode => DynamicImage.TriggerCondition == TriggerConditionVariable.range;
    public bool IsEqualMode => DynamicImage.TriggerCondition == TriggerConditionVariable.equal;

    public string DefaultImage
    {
        get => DynamicImage.DefaultImage;
        set
        {
            if (DynamicImage.DefaultImage != value)
            {
                DynamicImage.DefaultImage = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand AddConditionCommand { get; }
    public ICommand PickDefaultImageCommand { get; }
    public ICommand PickConditionImageCommand { get; }

    private DropdownOption _selectedTriggerCondition = null!;
    public DropdownOption SelectedTriggerCondition
    {
        get => _selectedTriggerCondition;
        set
        {
            if (SetProperty(ref _selectedTriggerCondition, value) && value?.Value is TriggerConditionVariable condition)
            {
                DynamicImage.TriggerCondition = condition;

                OnPropertyChanged(nameof(IsRangeMode));
                OnPropertyChanged(nameof(IsEqualMode));
            }
        }
    }

    public string TriggerVariable
    {
        get => DynamicImage.TriggerVariable;
        set
        {
            if (DynamicImage.TriggerVariable != value)
            {
                DynamicImage.TriggerVariable = value;
                OnPropertyChanged();
            }
        }
    }

    public DynamicImageViewModel(DynamicImageModel dynamicImage)
    {
        DynamicImage = dynamicImage;

        Conditions = new ObservableCollection<DynamicImageCondition>(DynamicImage.Conditions);

        AddConditionCommand = new RelayCommand(_ => AddCondition());

        PickDefaultImageCommand = new RelayCommand(_ => PickDefaultImage());
        PickConditionImageCommand = new RelayCommand<DynamicImageCondition>(PickConditionImage);

        SelectedTriggerCondition = TriggerConditions.FirstOrDefault(x =>
            (TriggerConditionVariable)x.Value == DynamicImage.TriggerCondition)
            ?? TriggerConditions.First();
    }

    private void AddCondition()
    {
        var newCondition = new DynamicImageCondition();
        DynamicImage.Conditions.Add(newCondition);
        Conditions.Add(newCondition);
    }

    private void PickDefaultImage()
    {
        var dialog = new ImageLibraryDialog();

        if (dialog.ShowDialog() == true && dialog.Result != null)
        {
            DefaultImage = dialog.Result.Path;
        }
    }

    private void PickConditionImage(DynamicImageCondition? condition)
    {
        if (condition == null) return;

        var dialog = new ImageLibraryDialog();

        if (dialog.ShowDialog() == true && dialog.Result != null)
        {
            condition.ImagePath = dialog.Result.Path;
        }
    }
}