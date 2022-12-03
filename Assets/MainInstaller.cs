using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private InputDataManager _inputDataManager;

    override public void InstallBindings() {
        Container.Bind<InputDataManager>().FromInstance(_inputDataManager);
    }
}
