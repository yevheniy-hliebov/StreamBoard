using StreamBoard.Core;

namespace StreamBoard.Features.Integrations.Common.Models
{
    public class IntegrationStateModel : ObservableObject
    {
        private string _name = string.Empty;
        private ConnectionState _state;

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
    }
}