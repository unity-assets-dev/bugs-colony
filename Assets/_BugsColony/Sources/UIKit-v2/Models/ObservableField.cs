using System;
using System.Collections.Generic;

public interface IObservableField<T> {
    T Value { get; set; }
    void AddListener(Action<T> listener);
    void RemoveListener(Action<T> listener);
    void RemoveAllListeners();
}

public class ObservableField<T>: IObservableField<T> {
    
    private readonly bool _notifySubscription = true;
    private readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;
    
    private Action<T> _listeners;
    private T _value;


    public T Value {
        get => _value;
        set {
            if (!_comparer.Equals(_value, value)) {
                _value = value;
                _listeners?.Invoke(value);
            }
        }
    }

    public void AddListener(Action<T> listener) {
        _listeners += listener;

        if (_notifySubscription) {
            listener?.Invoke(_value);
        }
    }

    public void RemoveListener(Action<T> listener) => _listeners -= listener;
    public void RemoveAllListeners() => _listeners.GetInvocationList().EachNonAlloc(l => RemoveListener(l as Action<T>));

    public ObservableField(T defaults = default(T)) => _value = defaults;

    public ObservableField(T defaults, bool notifySubscription) : this(defaults) {
        _notifySubscription = notifySubscription;
    }
}