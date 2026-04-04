using System;

public interface IScreenCommand {
    bool State { get; set; }
    
    event Action OnStateChange;
    
    void Execute();

    public static IScreenCommand CreateCommand(Action command) => new ActionCommand(command);

    private class ActionCommand : IScreenCommand {
        private readonly Action _command;
        private bool _state;

        public bool State {
            get => _state;
            set {
                if (value != _state) {
                    _state = value;
                    OnStateChange?.Invoke();
                }
            }
        }
        
        public event Action OnStateChange;

        public ActionCommand(Action command) => _command = command;

        public void Execute() {
            _command?.Invoke();
        }
    }
}