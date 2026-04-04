using BugsColony.App.States;
using UnityEngine;

public class BootstrapState: ScreenState<BootstrapScreen>, IScreenState {
    
    public BootstrapState(BootstrapScreen screen) : base(screen) {}

    protected override void OnStateEnter() {
        Screen.Show();
        
        Router.Delay<GamePlayState>(.25f);
    }

    protected override void OnStateExit() {
        Screen.Hide();
    }
}