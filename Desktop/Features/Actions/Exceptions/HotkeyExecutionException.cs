namespace StreamTabula.Features.Actions.Exceptions
{
    public class HotkeyExecutionException : Exception
    {
        public HotkeyExecutionException(string message) : base(message) { }
        public HotkeyExecutionException(string message, Exception inner) : base(message, inner) { }
    }
}
