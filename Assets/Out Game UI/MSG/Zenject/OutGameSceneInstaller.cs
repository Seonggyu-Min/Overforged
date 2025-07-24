using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    /// <summary>
    /// 인게임 씬이 아닌 아웃게임 씬에 종속된 MonoInstaller
    /// </summary>
    public class OutGameSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _outGameUIManagerPrefab;
        [SerializeField] private GameObject _networkManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IOutGameUIManager>()
                .To<OutGameUIManager>()
                .FromComponentInNewPrefab(_outGameUIManagerPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<INetworkManager>()
                .To<NetworkManager>()
                .FromComponentInNewPrefab(_networkManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
