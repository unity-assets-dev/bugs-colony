public class StatesRouter: IStateRouter {
    private readonly AppStates _states;

    public StatesRouter(AppStates states) => _states = states;

    public bool HistoryIsEmpty => _states.HistoryIsEmpty;
    
    public void ChangeState<T>() where T : class, IScreenState  => _states.ChangeState<T>();
    public void ReloadState<T>() where T : class, IScreenState => _states.ReloadState<T>();
    
    public IScreenState ReturnBack() => _states.ReturnBack();
}