using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SOInstaller", menuName = "Create SO Installer")]
public class SoInstaller : ScriptableObjectInstaller<SoInstaller>
{
    public GameConfig config;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameConfig>().FromInstance(config).AsSingle();
    }
}
