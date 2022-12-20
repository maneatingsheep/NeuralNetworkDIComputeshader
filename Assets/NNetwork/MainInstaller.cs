using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private InputDataManager _inputDataManager;
    [SerializeField] private ModelManager _modelManager;
    [SerializeField] private ComputeShader _computeshader;
    [SerializeField] private SettingsConfig settingsConfig;
    [SerializeField] private XorNetOutputView _xorNetOutputView;
    

    override public void InstallBindings() {
        Container.Bind<InputDataManager>().FromInstance(_inputDataManager).NonLazy();
        
        Container.Bind<ModelManager>().FromInstance(_modelManager);
        Container.Bind<NeuralNetwork>().AsSingle().NonLazy();
        Container.Bind<Trainer>().AsSingle().NonLazy();
        Container.Bind<ComputeShader>().FromInstance(_computeshader);
        Container.Bind<SettingsConfig>().FromScriptableObject(settingsConfig).AsSingle().NonLazy();
        Container.Bind<IVerifier>().To(typeof(XorVerifier)).AsSingle().NonLazy();
        Container.Bind<XorNetOutputView>().FromInstance(_xorNetOutputView);
    }
}
