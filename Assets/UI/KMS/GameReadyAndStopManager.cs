using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using Zenject;
using System;
using MIN;


public class GameReadyAndStopManager : MonoBehaviourPunCallbacks
{
    [Inject] IGameManager _gameManager;

    public static GameReadyAndStopManager Instance;

    private string localSceneLoaded = "LocalSceneLoaded";

    public bool IsGameStopped { get; private set; }

    [SerializeField] RawImage ready;
    [SerializeField] RawImage go;

    [SerializeField] RawImage TimeUp;
    public Action OnGameBegin;
    public Action OnTimeOver;



    // 플레이어가 게임에 들어온 직후, 내가 게임에 들어왔다는 커스텀 프로퍼티를 설정한다.
    // 씬 진입 직후에 로컬 플레이어의 커스텀 프로퍼티를 true로 설정?
    // 커스텀 프로퍼티가 모두 켜지고 마스터가 그걸 확인하면, Ready UI가 모두에게 뜬다.


    //0. 우선 씬에 입장하면 기본적으로 인풋 금지, 타이머 중지상태.
    //1. 아래 조건 만족되면 UI활성화.
    //2. 일정 시간 지난 후 인풋 및 타이머 활성화함.
    void Awake()
    {
        IsGameStopped = true;
        Instance = this;
    }
    void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { localSceneLoaded, true } });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (changedProps[localSceneLoaded] == null) return;
        int count = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(localSceneLoaded, out object islocalLoaded))
            {
                if ((bool)islocalLoaded) count++;
            }
        }
        if (count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC("StartReadyUI", RpcTarget.All);
        }
    }

    public void TimesUp()
    {
        StartCoroutine(TimesUpRoutine());
    }

    [PunRPC]
    public void StartReadyUI()
    {
        StartCoroutine(ReadyGoRoutine());
    }

    private IEnumerator ReadyGoRoutine()
    {
        yield return new WaitForSeconds(1);
        ready.enabled = true;
        yield return new WaitForSeconds(3);
        ready.enabled = false;
        go.enabled = true;
        IsGameStopped = false;
        OnGameBegin?.Invoke();
        yield return new WaitForSeconds(1.5f);
        go.enabled = false;
    }
    private IEnumerator TimesUpRoutine()
    {
        IsGameStopped = true;
        TimeUp.enabled = true;
        yield return new WaitForSeconds(5);
        TimeUp.enabled = false;
        IsGameStopped = false;
        _gameManager.SetGameEnd();
    }
}
