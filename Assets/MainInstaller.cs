using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private InputDataManager _inputDataManager;
    [SerializeField] private InputDisplayView _inputDisplayView;
    

    override public void InstallBindings() {
        Container.Bind<InputDataManager>().FromInstance(_inputDataManager);
        Container.Bind<InputDisplayView>().FromInstance(_inputDisplayView);
    }
}
