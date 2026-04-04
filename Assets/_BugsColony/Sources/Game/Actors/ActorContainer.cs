using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActorContainer: IEnumerable<IActor> {
    
    private readonly HashSet<IActor> _actors = new();
    
    private readonly HashSet<IActor> _income = new();
    private readonly HashSet<IActor> _outcome = new();
    

    public void AddActor(IActor actor) => _income.Add(actor);

    public void RemoveActor(IActor actor) => _outcome.Add(actor);

    public void OnUpdateFrame(float dt) {
        _actors.EachNonAlloc(actor => actor.OnUpdateFrame(dt));
        
        MergeQueues();
    }

    private void MergeQueues() {

        foreach (var actor in _outcome) _actors.Remove(actor);
        foreach (var actor in _income) _actors.Add(actor);
        
        _outcome.Clear();
        _income.Clear();
    }

    public IActor[] Dispose() {
        _income.Clear();
        _outcome.UnionWith(_actors);
        var result = _outcome.ToArray();
        MergeQueues();

        return result;
    }

    public IEnumerable<T> Get<T>() => _actors.OfType<T>();
    public IEnumerator<IActor> GetEnumerator() => _actors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}