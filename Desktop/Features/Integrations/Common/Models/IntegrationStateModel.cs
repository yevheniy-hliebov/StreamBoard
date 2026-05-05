using StreamTabula.Core;

namespace StreamTabula.Features.Integrations.Common.Models
{
    public class IntegrationStateModel : ObservableObject
    {
        private string _name = string.Empty;
        private ConnectionState _state;
        private Type? _targetPageType;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ConnectionState State
        {
            get => _state;
            set
            {
                if (SetProperty(ref _state, value))
                {
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public Type? TargetPageType
        {
            get => _targetPageType;
            set => SetProperty(ref _targetPageType, value);
        }
    }
}