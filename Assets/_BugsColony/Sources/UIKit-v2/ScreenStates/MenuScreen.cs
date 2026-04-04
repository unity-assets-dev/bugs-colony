using System;
using UnityEngine;


public abstract class MenuScreen : ScreenLayout, IStateScreen {
    
    [SerializeField] private ScreenAnimator _animator;

    private void OnValidate() {
        _animator ??= GetComponent<ScreenAnimator>();
        name = $"{GetType().Name}";
        
        OnValidateNext();
    }

    protected virtual void OnValidateNext() {}
    
    public void Show(Action onComplete = null) {
        ShowElements();
        
        _animator?.Show(() => {
            OnScreenShow();
            onComplete?.Invoke();
        });
    }
    
    protected virtual void OnScreenShow() {}
    
    public void Hide(Action onComplete = null) {
        HideElements();
        
        _animator?.Hide(() => {
            OnScreenHide();
            onComplete?.Invoke();
        });
    }
    
    protected virtual void OnScreenHide() {}
}