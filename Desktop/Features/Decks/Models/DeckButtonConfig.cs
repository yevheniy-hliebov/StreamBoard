using StreamTabula.Core;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Decks.ViewModels;
using StreamTabula.Features.Variables.Models;
using StreamTabula.Features.Variables.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckButtonConfig : ObservableObject
    {
        private const string DefaultBackgroundColor = "#FF2D2D2D";

        private IVariableService? _variableService;

        private string _name = string.Empty;
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _backgroundColor = DefaultBackgroundColor;
        [JsonPropertyName("background_color")]
        public string BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        [JsonPropertyName("actions")]
        public ObservableCollection<BaseAction> Actions { get; set; } = [];

        [JsonIgnore]
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        [JsonIgnore]
        public bool HasData => Actions.Count > 0 ||
                               !string.IsNullOrWhiteSpace(Name) ||
                               !string.IsNullOrWhiteSpace(LegacyImagePath) ||
                               BackgroundColor != DefaultBackgroundColor;

        private DynamicImageModel _dynamicImage = new();
        [JsonPropertyName("dynamic_image")]
        public DynamicImageModel DynamicImage
        {
            get => _dynamicImage;
            set
            {
                if (SetProperty(ref _dynamicImage, value ?? new DynamicImageModel()))
                {
                    EvaluateDisplayImage();
                    OnPropertyChanged(nameof(DisplayImage));
                }
            }
        }

        [JsonPropertyName("image_path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LegacyImagePath
        {
            get => null;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(DynamicImage.DefaultImage))
                {
                    DynamicImage.DefaultImage = value;
                }
            }
        }

        private string? _displayImage;
        [JsonIgnore]
        public string? DisplayImage
        {
            get => string.IsNullOrWhiteSpace(_displayImage) ? DynamicImage?.DefaultImage : _displayImage;
            private set => SetProperty(ref _displayImage, value);
        }

        // Викликається після десеріалізації або створення кнопки, щоб передати сервіс
        public void Initialize(IVariableService variableService)
        {
            if (_variableService != null)
            {
                _variableService.VariableChanged -= OnVariableChanged;
            }

            _variableService = variableService;
            _variableService.VariableChanged += OnVariableChanged;

            EvaluateDisplayImage();
        }

        private void OnVariableChanged(object? sender, VariableChangedEventArgs e)
        {
            if (string.Equals(e.Name, DynamicImage.TriggerVariable, StringComparison.OrdinalIgnoreCase))
            {
                EvaluateDisplayImage(e.Value);
            }
        }

        // Метод можна викликати публічно з редактора після збереження нових умов
        public void EvaluateDisplayImage(string? currentValue = null)
        {
            if (string.IsNullOrWhiteSpace(DynamicImage.TriggerVariable) || DynamicImage.Conditions.Count == 0)
            {
                DisplayImage = DynamicImage.DefaultImage;
                return;
            }

            if (currentValue == null && _variableService != null)
            {
                currentValue = _variableService.GetVariableValue(DynamicImage.TriggerVariable);
            }

            string safeValue = currentValue ?? string.Empty;

            if (DynamicImage.TriggerCondition == TriggerConditionVariable.equal)
            {
                var match = DynamicImage.Conditions.FirstOrDefault(c => string.Equals(c.Value, safeValue, StringComparison.OrdinalIgnoreCase));
                if (match != null && !string.IsNullOrWhiteSpace(match.ImagePath))
                {
                    DisplayImage = match.ImagePath;
                    return;
                }
            }
            else if (DynamicImage.TriggerCondition == TriggerConditionVariable.range)
            {
                if (double.TryParse(safeValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double numValue))
                {
                    foreach (var condition in DynamicImage.Conditions)
                    {
                        if (double.TryParse(condition.ValueFrom, NumberStyles.Any, CultureInfo.InvariantCulture, out double from) &&
                            double.TryParse(condition.ValueTo, NumberStyles.Any, CultureInfo.InvariantCulture, out double to))
                        {
                            if (numValue >= from && numValue <= to && !string.IsNullOrWhiteSpace(condition.ImagePath))
                            {
                                DisplayImage = condition.ImagePath;
                                return;
                            }
                        }
                    }
                }
            }

            // Якщо жодна умова не виконана, показуємо дефолтне зображення
            DisplayImage = DynamicImage.DefaultImage;
        }

        public void ResetAppearance()
        {
            Name = string.Empty;
            BackgroundColor = DefaultBackgroundColor;
            DynamicImage = new DynamicImageModel();
            EvaluateDisplayImage();
        }

        ~DeckButtonConfig()
        {
            if (_variableService != null)
            {
                _variableService.VariableChanged -= OnVariableChanged;
            }
        }
    }
}
