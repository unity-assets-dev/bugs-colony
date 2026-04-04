using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class KitInstallerBase : MonoInstaller<KitInstallerBase> {
    
    private Type[] _domainTypes;
    //private IPresenterView[] _presenterViews;

    public override void InstallBindings() {
        _domainTypes = AppDomain
            .CurrentDomain
            .GetAssembliesTypes();
        
        /*_presenterViews = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IPresenterView>()
            .ToArray();*/

        BindSources();

        BindStates();

        BindInstances();
    }

    private void BindSources() {
        
        BindScreens();

        BindServices();
    }

    protected abstract void BindServices();

    private void BindStates() {
        BindPresenters();

        BindEachAsSingle<IUIKitModel>();
        
        BindEachAsSingle<IScreenState>();
        
        Container.BindAsSingle<AppStates>();
    }

    private void BindPresenters() {
        //_presenterViews.Each(Container.BindAsSingleFromInstanceType);
        
        TypeOf<IPresenter>().EachNonAlloc(t => {
            if (t.TryGetAttribute<PresenterOfAttribute>(out var attribute))
                attribute.Types.EachNonAlloc(typeInAttribute => {
                    Container
                        .BindInterfacesAndSelfTo(t)
                        .WhenInjectedInto(typeInAttribute);
                });
        });
    }

    private void BindEachAsSingle<TInterface>() {
        TypeOf<TInterface>().EachNonAlloc(type => {
            Container
                .BindInterfacesAndSelfTo(type)
                .AsSingle();
        });
    }

    private void BindScreens() {
        FindObjectsByType<MenuScreen>(FindObjectsInactive.Include, FindObjectsSortMode.None).EachNonAlloc(screen => {
            screen.Hide();
            Container
                .BindInterfacesAndSelfTo(screen.GetType())
                .FromInstance(screen)
                .AsSingle();
        });
    }

    private void BindInstances() {
        OnBindTargetInstances();
        Container.BindAsSingleFromInstanceMono<AppEntry>(); //bootstrap
    }

    protected abstract void OnBindTargetInstances();

    protected Type[] TypeOf<TType>() {
        var interfaceType = typeof(TType);

        return _domainTypes
                .Where(type => !type.IsAbstract && type.IsClass && interfaceType.IsAssignableFrom(type))
                .ToArray();
    }
    
}