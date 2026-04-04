using Zenject;

public class RouteService {
    
    private readonly DiContainer _container;

    public RouteService(DiContainer container) {
        _container = container;
    }
    
    public T Take<T>() => _container.Resolve<T>();
}
