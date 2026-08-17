using StreamTabula.Core.Mvvm;

namespace StreamTabula.Features.Integrations.Common.Models;

public class IntegrationStatusModel : ObservableObject
{
    private string _name = string.Empty;
    private ConnectionStatus _status;
    private Type? _targetPageType;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ConnectionStatus Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                OnPropertyChanged(nameof(Status));
            }
        }
    }

    public Type? TargetPageType
    {
        get => _targetPageType;
        set => SetProperty(ref _targetPageType, value);
    }
}