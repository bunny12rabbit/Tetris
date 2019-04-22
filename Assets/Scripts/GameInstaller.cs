using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameController gameController;
    public AudioController audioController;
    public UIController uIController;
    public ScoreSystem scoreSystem;
    public UserInput userInput;

    public override void InstallBindings()
    {
        Container.Bind<TimeController>().AsSingle();
        Container.BindInterfacesAndSelfTo<UserInput>().FromInstance(userInput).AsSingle();
        Container.BindInterfacesAndSelfTo<AudioController>().FromInstance(audioController).AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreSystem>().FromInstance(scoreSystem).AsSingle();
        Container.BindInterfacesAndSelfTo<GameController>().FromInstance(gameController).AsSingle();
        Container.BindInterfacesAndSelfTo<UIController>().FromInstance(uIController).AsSingle();
    }
}
