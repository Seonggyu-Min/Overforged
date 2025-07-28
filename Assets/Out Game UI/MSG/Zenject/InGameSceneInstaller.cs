using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    /// <summary>
    /// 인게임 씬에 종속된 MonoInstaller
    /// </summary>
    public class InGameSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _gameManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IGameManager>()
                .To<GameManager>()
                .FromComponentInNewPrefab(_gameManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
