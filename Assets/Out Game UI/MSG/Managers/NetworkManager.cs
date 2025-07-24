using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class NetworkManager : MonoBehaviourPunCallbacks, INetworkManager
    {

        #region Fields and Properties
        [Inject] private IOutGameUIManager _outGameUIManager;

        #endregion


        #region Unity Methods

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }


        #endregion


        #region Photon Callbacks

        //public override void OnConnectedToMaster()
        //{
        //    _outGameUIManager.Hide("Loading Panel");
        //    _outGameUIManager.Show("Log in Panel");
        //}

        #endregion
    }
}

