using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    override public void InstallBindings() {
        //Container.Bind
        print("123");
    }
}
