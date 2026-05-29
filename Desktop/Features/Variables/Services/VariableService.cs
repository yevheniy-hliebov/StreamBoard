using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using StreamTabula.Core;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Variables.Models;

namespace StreamTabula.Features.Variables.Services
{
    public interface IVariableService
    {
        ObservableCollection<Variable> GlobalVariables { get; }
        ObservableCollection<Variable> TemporaryVariables { get; }

        void SetVariable(string name, VariableScope scope, string value, ActionExecutionContext? context = null);

        string? GetVariableValue(string name, ActionExecutionContext? context = null);

        string Resolve(string template, ActionExecutionContext? context = null);

        void Initialize();
        void DeleteGlobalVariable(string name);
        void DeleteTemporaryVariable(string name);
    }

    public class VariableService : ObservableObject, IVariableService
    {
        private readonly VariablesJsonStorage _storage;

        private static readonly Regex VariableRegex = new(@"\{([a-zA-Z0-9_:]+)\}", RegexOptions.Compiled);
        private static readonly Regex ExactVariableRegex = new(@"^\{([a-zA-Z0-9_:]+)\}$", RegexOptions.Compiled);
        private const int MaxRecursionDepth = 10;

        public ObservableCollection<Variable> GlobalVariables { get; } = [];
        public ObservableCollection<Variable> TemporaryVariables { get; } = [];

        private readonly Dictionary<string, Variable> _globalCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Variable> _temporaryCache = new(StringComparer.OrdinalIgnoreCase);

        private readonly object _lockObject = new();

        public VariableService(VariablesJsonStorage storage)
        {
            _storage = storage;
            Initialize();
        }

        private void Initialize()
        {
            GlobalVariables.Clear();
            _globalCache.Clear();

            TemporaryVariables.Clear();
            _temporaryCache.Clear();

            foreach (var variable in _storage.Current.GlobalVariables)
            {
                variable.Scope = VariableScope.Global;
                GlobalVariables.Add(variable);
                _globalCache[variable.Name] = variable;
            }
        }

        public void SetVariable(string name, VariableScope scope, string value, ActionExecutionContext? context = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            string safeValue = value ?? string.Empty;

            if (scope == VariableScope.Runtime)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context), "Context is required for Runtime variables.");

                context.RuntimeVariables[name] = safeValue;
            }
            else if (scope == VariableScope.Temporary)
            {
                lock (_lockObject)
                {
                    if (_temporaryCache.TryGetValue(name, out var tempVar))
                    {
                        tempVar.Value = safeValue;
                    }
                    else
                    {
                        var newTemp = new Variable
                        {
                            Name = name,
                            Scope = VariableScope.Temporary,
                            Value = safeValue
                        };

                        _temporaryCache[name] = newTemp;

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            TemporaryVariables.Add(newTemp);
                        });
                    }
                }
            }
            else
            {
                lock (_lockObject)
                {
                    if (_globalCache.TryGetValue(name, out var globalVar))
                    {
                        globalVar.Value = safeValue;
                    }
                    else
                    {
                        var newGlobal = new Variable
                        {
                            Name = name,
                            Scope = VariableScope.Global,
                            Value = safeValue
                        };

                        _globalCache[name] = newGlobal;

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            GlobalVariables.Add(newGlobal);
                        });
                    }

                    SaveToStorage();
                }
            }
        }

        public string? GetVariableValue(string name, ActionExecutionContext? context = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            VariableScope? explicitScope = null;
            string targetName = name;

            if (name.Contains(':'))
            {
                var parts = name.Split(':', 2);
                var scopeString = parts[0].ToLowerInvariant();
                targetName = parts[1];

                explicitScope = scopeString switch
                {
                    "global" => VariableScope.Global,
                    "temp" or "temporary" => VariableScope.Temporary,
                    "runtime" => VariableScope.Runtime,
                    _ => null
                };
            }

            if (explicitScope == VariableScope.Runtime)
            {
                return (context != null && context.RuntimeVariables.TryGetValue(targetName, out var val)) ? val?.ToString() : null;
            }

            if (explicitScope == VariableScope.Temporary)
            {
                lock (_lockObject)
                {
                    return _temporaryCache.TryGetValue(targetName, out var val) ? val.Value : null;
                }
            }

            if (explicitScope == VariableScope.Global)
            {
                lock (_lockObject)
                {
                    return _globalCache.TryGetValue(targetName, out var val) ? val.Value : null;
                }
            }

            if (context != null && context.RuntimeVariables.TryGetValue(targetName, out var runtimeValue))
            {
                return runtimeValue?.ToString();
            }

            lock (_lockObject)
            {
                if (_temporaryCache.TryGetValue(targetName, out var tempVar))
                {
                    return tempVar.Value;
                }
                if (_globalCache.TryGetValue(targetName, out var globalVar))
                {
                    return globalVar.Value;
                }
            }

            return null;
        }

        public string Resolve(string template, ActionExecutionContext? context = null)
        {
            return ResolveRecursive(template, context, 0);
        }

        private string ResolveRecursive(string template, ActionExecutionContext? context, int depth)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            if (depth > MaxRecursionDepth) return "[RECURSION_LIMIT_EXCEEDED]";

            var exactMatch = ExactVariableRegex.Match(template);
            if (exactMatch.Success)
            {
                string varName = exactMatch.Groups[1].Value;
                string? val = GetVariableValue(varName, context);

                if (val != null && val.Contains('{') && val.Contains('}'))
                {
                    return ResolveRecursive(val, context, depth + 1);
                }

                return val ?? template;
            }

            return VariableRegex.Replace(template, match =>
            {
                string varName = match.Groups[1].Value;
                string? val = GetVariableValue(varName, context);

                if (val == null) return match.Value;

                if (val.Contains('{') && val.Contains('}'))
                {
                    return ResolveRecursive(val, context, depth + 1);
                }

                return val;
            });
        }

        public void DeleteGlobalVariable(string name)
        {
            lock (_lockObject)
            {
                if (_globalCache.Remove(name, out var variable))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        GlobalVariables.Remove(variable);
                    });
                    SaveToStorage();
                }
            }
        }

        public void DeleteTemporaryVariable(string name)
        {
            lock (_lockObject)
            {
                if (_temporaryCache.Remove(name, out var variable))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        TemporaryVariables.Remove(variable);
                    });
                }
            }
        }

        private void SaveToStorage()
        {
            _storage.Current.GlobalVariables = GlobalVariables.ToList();
            _storage.Save();
        }

        void IVariableService.Initialize()
        {
            Initialize();
        }
    }
}