using System;

public interface IPresenter {
    void OnEnter(IStateScreen screen);
    void BeforeExit(IStateScreen screen);
}

public class PresenterOfAttribute: Attribute {
    public Type[] Types { get; private set; }

    public PresenterOfAttribute(params Type[] presenterType) => Types = presenterType;
}
