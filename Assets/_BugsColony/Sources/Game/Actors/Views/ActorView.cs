using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ActorView : MonoBehaviour, IActorView {
    
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _destination;
    
    private readonly HashSet<IInteractionHandler> _subscriptions = new();
    
    public Transform Transform => transform;
    public IActor Actor { get; set; }

    public IDisposable Subscribe(IInteractionHandler subscription) {
        _subscriptions.Add(subscription);
        return new DisposableSubscription(() => _subscriptions.Remove(subscription));
    }
    
    private void OnCollisionEnter(Collision other) {
        if (other.collider.TryGetComponent<IActorView>(out var view)) {
            _subscriptions.EachNonAlloc(s => s.ConsumeTarget(view.Actor));
        }
    }

    private void OnDisable() {
        _subscriptions.Clear();
    }
    
    private void OnValidate() {
        _agent = GetComponent<NavMeshAgent>();
        OnValidateNext();
    }

    public void SetDestination(Vector3 destination) {
        _agent.SetDestination(destination);
    }

    protected virtual void OnValidateNext() {}

    [Button]
    private void MoveToDestination() {
        SetDestination(_destination.position);
    }
    
    private class DisposableSubscription : IDisposable {
        private readonly Action _onDispose;

        public DisposableSubscription(Action onDispose) => _onDispose = onDispose;

        public void Dispose() => _onDispose?.Invoke();
    }
}