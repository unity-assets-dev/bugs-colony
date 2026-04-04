using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public static class StatesExtensions {

    private static MonoBehaviour _player;

    private static MonoBehaviour Player {
        get {
            if (_player == null) _player = Object.FindAnyObjectByType<AppEntry>();
            
            return _player;
        }
    }

    public static IButtonCommand CreateCommand<TState>(this IStateRouter router, Action onExecuted = null) where TState : class, IScreenState {
        if(router == null) throw new ArgumentNullException(nameof(router));
        return IButtonCommand.Create(() => {
            router.ChangeState<TState>();
            onExecuted?.Invoke();
        });
    }

    public static IButtonCommand CreateBackCommand(this IStateRouter router, Action onExecuted = null) =>
        IButtonCommand.Create(() => {
            router.ReturnBack();
            onExecuted?.Invoke();
        });

    public static Coroutine PlayCoroutine(this IEnumerator routine) => Player.StartCoroutine(routine);
    
    public static void StopCoroutine(this IEnumerator routine) {
        if(routine != null) 
            Player.StopCoroutine(routine);
    }

    private static IEnumerator DelayCommand(float time, Action onDelay) {
        yield return new WaitForSeconds(time);
        onDelay?.Invoke();
    }
    
    public static void Delay<TState>(this IStateRouter router, float delay)  where TState: class, IScreenState {
        DelayCommand(delay, router.ChangeState<TState>).PlayCoroutine();
    }

}

public static class Delay {
    public static Coroutine Execute(float time, Action onComplete) {
        return Run().PlayCoroutine();
        IEnumerator Run() {
            yield return new WaitForSeconds(time);
            
            onComplete?.Invoke();
        }
    }
}