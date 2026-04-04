using System;
using Zenject;

public class AppStates : ScreenStates {
    
    public AppStates(IScreenState[] states) {
        states.EachNonAlloc(AddState);
    }
    
}

public interface IStateScreen {
    void Show(Action onComplete);
    void Hide(Action onComplete);

    bool TryGetExtension<T>(out T extension) where T : class, ILayoutElement;
}

public abstract class ScreenState<T> where T : class, IStateScreen {
    private bool _screenHandle;
    private readonly ShadowRouter _router = new ShadowRouter();
    protected T Screen { get; }
    
    [Inject] private IPresenter[] _presenters = Array.Empty<IPresenter>();
    
    protected ScreenState(T screen) => Screen = screen;

    protected void KeepScreenHandle(bool screenHandle) => _screenHandle = !screenHandle;
    
    public void EnterState() {
        if(!_screenHandle) 
            Screen.Show(OnStateEntered);
        else OnStateEntered();
    }

    private void OnStateEntered() {
        OnStateEnter();

        _presenters.EachNonAlloc(presenter => presenter.OnEnter(Screen));
    }

    protected virtual void OnStateEnter() {}

    public void ExitState() {
        if(!_screenHandle)
            Screen.Hide(OnStateExited);
        else OnStateExited();
    }

    private void OnStateExited() {
        _presenters.EachNonAlloc(presenter => presenter.BeforeExit(Screen));
        OnStateExit();
    }
    
    public void BindRouter(IStateRouter router) {
        _router.Cover(router);
    }

    protected IStateRouter Router => _router;

    protected virtual void OnStateExit() {}

    private class ShadowRouter : IStateRouter {
        private IStateRouter _router;
        public void Cover(IStateRouter router) => _router = router;

        public bool HistoryIsEmpty => _router.HistoryIsEmpty;
        public void ChangeState<T1>() where T1 : class, IScreenState => _router?.ChangeState<T1>();

        public void ReloadState<T1>() where T1 : class, IScreenState => _router?.ReloadState<T1>();

        public IScreenState ReturnBack() => _router?.ReturnBack();
    }
}