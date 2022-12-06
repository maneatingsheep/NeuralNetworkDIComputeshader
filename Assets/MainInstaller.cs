using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private InputDataManager _inputDataManager;
    [SerializeField] private InputDisplayView _inputDisplayView;
    [SerializeField] private NetworkOutputDisaply _networkOutputDisaply;
    [SerializeField] private ModelManager _modelManager;
    [SerializeField] private ComputeShader _computeshader;
    

    override public void InstallBindings() {
        Container.Bind<InputDataManager>().FromInstance(_inputDataManager);
        Container.Bind<InputDisplayView>().FromInstance(_inputDisplayView);
        Container.Bind<NetworkOutputDisaply>().FromInstance(_networkOutputDisaply);
        Container.Bind<ModelManager>().FromInstance(_modelManager);
        Container.Bind<NeuralNetwork>().AsSingle().NonLazy();
        Container.Bind<Trainer>().AsSingle().NonLazy();
        Container.Bind<ComputeShader>().FromInstance(_computeshader);
    }
}
