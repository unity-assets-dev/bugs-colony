using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class MenuButton: MonoBehaviour, ILayoutButton {
    [SerializeField] private Button _button;

    private void OnValidate() {
        _button = GetComponent<Button>();
        name = $"[{GetType().Name}]";
    }

    public void OnLayoutShow() {
        
    }
    
    public void OnLayoutHide() => _button.onClick.RemoveAllListeners();

    public void AddListener(Action command) => _button.onClick.AddListener(() => command?.Invoke());

    public void RemoveListener(Action command) => _button.onClick.RemoveAllListeners();

    public void RemoveAllListeners() => _button.onClick.RemoveAllListeners();
}