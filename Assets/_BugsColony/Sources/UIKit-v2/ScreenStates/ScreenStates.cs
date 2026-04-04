using System;
using System.Collections.Generic;
using System.Linq;

public interface IScreenState {
    void EnterState();
    void ExitState();

    void BindRouter(IStateRouter router);
}

public interface IStateRouter {
    bool HistoryIsEmpty { get; }
    void ChangeState<T>() where T : class, IScreenState;
    void ReloadState<T>() where T : class, IScreenState;
    IScreenState ReturnBack();
}

public abstract class ScreenStates: IStateRouter {
    
    private readonly List<IScreenState> _states = new();
    private readonly Stack<IScreenState> _history = new();
    
    private IScreenState _currentState;
    
    public bool HistoryIsEmpty => _history.Count == 0;
    
    public void AddState(IScreenState state) {
        state.BindRouter(this);
        _states.Add(state);
    }

    public void ChangeState<T>() where T : class, IScreenState {
        if(_currentState != null) _history.Push(_currentState);
        
        ChangeTo(_states.OfType<T>().FirstOrDefault());
    }

    public void ReloadState<T>() where T : class, IScreenState {
        ChangeTo(_states.OfType<T>().FirstOrDefault());
    }

    private void ChangeTo(IScreenState state, bool reload = false) {
        if (reload || state != _currentState) {
            _currentState?.ExitState();
            _currentState = state;
            _currentState?.EnterState();
        }
    }

    public IScreenState ReturnBack() {
        ChangeTo(_history.Pop());
        return _currentState;
    }

    public void Dispose() {
        if(_currentState != null) 
            ChangeTo(null);
    }
}
