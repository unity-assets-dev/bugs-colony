using System;

public interface ILayoutButton : ILayoutElement {
    void AddListener(Action command);
    void RemoveListener(Action command);
    void RemoveAllListeners();
}

public interface IButtonCommand
{
    bool State { get; set; }
    void Execute();

    public static IButtonCommand Create(Action onExecuted) => new DefaultCommand(onExecuted);

    private class DefaultCommand : IButtonCommand {
        private readonly Action _onExecuted;
        public bool State { get; set; } = true;

        public DefaultCommand(Action onExecuted) => _onExecuted = onExecuted;

        public void Execute() {
            if (State) _onExecuted();
        }
    }
}