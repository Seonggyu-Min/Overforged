using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace SCR
{
    public class Player : MonoBehaviourPun, IPunObservable
    {
        private Rigidbody _rigidbody;
        private Collider _collider;
        private Animator animator;
        [SerializeField] private AudioSource audioSource;
        public Rigidbody Rigidbody { get => _rigidbody; }
        public Collider Collider { get => _collider; }
        public Animator Animator { get => animator; }
        public AudioSource AudioSource { get => audioSource; }

        private PlayerController playerController;
        private PlayerPhysical playerPhysical;
        [SerializeField] private PlayerSFX sfx;
        [SerializeField] private AudioSource walkSfx;
        [SerializeField] PlayerInput playerInput;
        public AudioSource WalkSfx { get => walkSfx; }
        public PlayerController PlayerController { get => playerController; }
        public PlayerPhysical PlayerPhysical { get => playerPhysical; }
        public PlayerSFX SFX { get => sfx; }

        private const byte PlayAnimationEventCode = 1;

        private bool firstTime;
        private int team;

        public GameObject HoldObject;
        public GameObject Hammer;
        public GameObject Tongs;
        public List<AudioClip> AudioList { get => audioList; }
        private List<AudioClip> audioList;

        public Action Trigger;


        [Header("물건 드는 위치")]
        [SerializeField] private Transform holdingPos;
        public Transform HoldingPos { get => holdingPos; }

        [Header("팀 색상")]
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Image image;

        [Header("캐릭터 설정")]
        private int characterNum;
        [SerializeField] private CharacterInfo character;
        [SerializeField] private TeamColor teamColor;
        [SerializeField] private List<MeshRenderer> _bodyrenderer;
        [SerializeField] private GameObject _normalhead;
        [SerializeField] private List<GameObject> _head;
        [SerializeField] private GameObject isMineUI;


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(characterNum);
                stream.SendNext(team);
            }
            else if (stream.IsReading)
            {
                characterNum = (int)stream.ReceiveNext();
                team = (int)stream.ReceiveNext();
            }

        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            animator = GetComponent<Animator>();
            if (photonView.IsMine)
                playerController = gameObject.AddComponent<PlayerController>();
            playerPhysical = gameObject.AddComponent<PlayerPhysical>();
            PhotonNetwork.NetworkingClient.EventReceived += AnymSyncFun;
            firstTime = true;
            walkSfx.clip = sfx.Move;
            walkSfx.loop = true;
            walkSfx.Stop();
            SetSound();
            playerInput.enabled = false;
        }

        private void Start()
        {

            if (photonView.IsMine)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MIN.CustomPropertyKeys.CharacterId, out object character))
                {
                    SetCharacter((int)character);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MIN.CustomPropertyKeys.TeamColor, out object team))
                {
                    SetTeam((int)team);
                }
                isMineUI.SetActive(true);

                MIN.CameraController controller = Camera.main.GetComponent<MIN.CameraController>();
                controller.SetPlayer(transform);
            }
            else
            {
                isMineUI.SetActive(false);
            }

        }

        public void OnPlayerInput()
        {
            playerInput.enabled = true;
        }

        public void SetTeam(int team)
        {
            this.team = team;
            _renderer.material.color = teamColor.Color[team];

            Color newColor = teamColor.Color[team];
            newColor.a = image.color.a;
            image.color = newColor;
        }

        public void SetCharacter(int num)
        {
            characterNum = num;
            playerPhysical.SetPhysical(character.characters[num].walkSpeed,
                                    character.characters[num].dashForce,
                                    character.characters[num].workSpeed * 0.2f);
            _normalhead.SetActive(false);
            _head[num].SetActive(true);
            foreach (Renderer renderer in _bodyrenderer)
                renderer.material = character.characters[num].color;
        }

        public void FixedUpdate()
        {
            if (HoldObject != null)
            {
                HoldObject.transform.localPosition = new Vector3(0, 0, 0);
            }
            if (Tongs != null)
            {
                Tongs.transform.localPosition = new Vector3(0, 0, 0);
            }

        }

        public void Update()
        {
            if (firstTime)
            {
                if (!photonView.IsMine)
                {
                    SetCharacter(characterNum);
                    SetTeam(team);
                }
            }
        }

        // 이벤트를 받는다
        public void AnymSyncFun(EventData obj)
        {
            if (obj.Code == PlayAnimationEventCode)
            {
                object[] data = (object[])obj.CustomData;
                int targetPhotonView = (int)data[0];
                if (targetPhotonView == photonView.ViewID)
                {
                    string animatorParameter = (string)data[1];
                    string parameterType = (string)data[2];
                    object parameterValue = (object)data[3];
                    switch (parameterType)
                    {
                        case "Trigger":
                            animator.SetTrigger(animatorParameter);
                            break;
                        case "Bool":
                            animator.SetBool(animatorParameter, (bool)parameterValue);
                            break;
                        case "Float":
                            animator.SetFloat(animatorParameter, (float)parameterValue);
                            break;
                        case "Int":
                            animator.SetInteger(animatorParameter, (int)parameterValue);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // 이벤트를 보낸다
        public void SendPlayAnimationEvent(int photonViewID, string animatorParameter, string parameterType, object parameterValue = null)
        {
            if (photonView.IsMine)
            {
                object[] content = new object[] { photonViewID, animatorParameter, parameterType, parameterValue };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PlayAnimationEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }
        }

        private void SetSound()
        {
            audioList = new()
            {
                SFX.Idle,
                SFX.Move,
                SFX.Dash,
                SFX.Throw,
                SFX.ChangeState,
                SFX.Hold,
                SFX.Put,
                SFX.Hammering,
                SFX.CutDown,
                SFX.ShowOff,
                SFX.RandonHit()
            };
        }
        [PunRPC]
        public void PlaySound(Vector3 pos, int audioClip)
        {
            AudioSource.transform.position = pos;
            AudioSource.PlayOneShot(AudioList[audioClip]);
        }

        public enum CharSFXType
        {
            Idle,
            Move,
            Dash,
            Throw,
            ChangeState,
            Hold,
            Put,
            Hammering,
            CutDown,
            ShowOff,
            Hit
        }
        /// <summary>
        /// 물건을 들었을 때의 함수
        /// </summary>
        /// <param name="itemId"></param>
        [PunRPC]
        private void UseTongs(bool use, int itemId)
        {
            PhotonView photonView = PhotonView.Find(itemId);
            photonView.gameObject.SetActive(use);
            if (!use)
            {
                Tongs = photonView.gameObject;
                Tongs.transform.SetParent(HoldingPos);
                Tongs.transform.localPosition = new Vector3(0, 0, 0);
                Tongs.transform.rotation = Quaternion.identity;
            }
            else
            {
                Tongs.transform.SetParent(null);
                Tongs = null;
            }
        }

        /// <summary>
        /// 물건을 들었을 때의 함수
        /// </summary>
        /// <param name="itemId"></param>
        [PunRPC]
        private void PickUpObject(int itemId)
        {
            PhotonView photonView = PhotonView.Find(itemId);
            HoldObject = photonView.gameObject;
            HoldObject.transform.SetParent(HoldingPos);
            HoldObject.transform.localPosition = new Vector3(0, 0, 0);
            HoldObject.transform.rotation = transform.rotation;
            HoldObject.GetComponent<Collider>().isTrigger = true;
            HoldObject.GetComponent<Rigidbody>().useGravity = false;
            PlayerPhysical.IsHold = true;
        }

        /// <summary>
        /// 물건을 내려 놓을 때의 함수
        /// </summary>
        /// <param name="isThrow"></param>
        [PunRPC]
        private void LayDownObject(bool isThrow = false, bool isPut = false)
        {
            PlayerPhysical.IsHold = false;
            if (isPut)
            {
                HoldObject = null;
                return;
            }

            HoldObject.transform.SetParent(null);
            HoldObject.GetComponent<Collider>().isTrigger = false;
            HoldObject.GetComponent<Rigidbody>().useGravity = true;
            if (isThrow)
            {
                float throwPower = 5f;
                if (PlayerPhysical.IsDash) throwPower *= 2;
                HoldObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower, ForceMode.Impulse);
            }

            HoldObject = null;
        }

        public void StartTrigger()
        {
            Trigger?.Invoke();
        }
    }
}