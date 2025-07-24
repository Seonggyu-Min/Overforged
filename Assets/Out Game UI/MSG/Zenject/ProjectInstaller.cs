using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    /// <summary>
    /// 프로젝트 전체에 종속되는 MonoInstaller
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _firebaseManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IFirebaseManager>()
                .To<FirebaseManager>()
                .FromComponentInNewPrefab(_firebaseManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
