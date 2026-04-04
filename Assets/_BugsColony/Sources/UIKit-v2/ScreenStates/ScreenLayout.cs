using System;
using System.Linq;
using UnityEngine;

public abstract class ScreenLayout : MonoBehaviour {
    
    private ILayoutElement[] _elements = Array.Empty<ILayoutElement>();
    
    private void Awake() => _elements = GetComponentsInChildren<ILayoutElement>(true);

    protected bool TryGetElement<T>(out T element) where T : class, ILayoutElement {
        element = _elements.OfType<T>().FirstOrDefault();
        
        return element != null;
    }

    public bool TryGetExtension<T>(out T extension) where T : class, ILayoutElement {
        extension = _elements.OfType<T>().FirstOrDefault();
        return extension != null;
    }

    protected void ShowElements() => _elements.EachNonAlloc(e => e.OnLayoutShow());
    protected void HideElements() => _elements.EachNonAlloc(e => e.OnLayoutHide());

    public void OnButtonClick<T>(Action command) where T : class, ILayoutButton {
        if (TryGetElement<T>(out var button)) {
            button.AddListener(command);
        }
    }

    public void OnButtonClick<T>(IScreenCommand command) where T : class, ILayoutButton => OnButtonClick<T>(command.Execute);
}