using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace SCR
{
    public class Player : MonoBehaviourPun
    {
        private Rigidbody _rigidbody;
        private Collider _collider;
        private Animator animator;
        private AudioSource audioSource;
        public Rigidbody Rigidbody { get => _rigidbody; }
        public Collider Collider { get => _collider; }
        public Animator Animator { get => animator; }
        public AudioSource AudioSource { get => audioSource; }

        private PlayerController playerController;
        private PlayerPhysical playerPhysical;
        public PlayerController PlayerController { get => playerController; }
        public PlayerPhysical PlayerPhysical { get => playerPhysical; }


        private const byte PlayAnimationEventCode = 1;

        private int team;

        public GameObject HoldObject;
        public GameObject Hammer;
        public GameObject Tongs;


        [Header("물건 드는 위치")]
        [SerializeField] private Transform holdingPos;
        public Transform HoldingPos { get => holdingPos; }

        [Header("팀 색상")]
        [SerializeField] private MeshRenderer _renderer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            if (photonView.IsMine)
                playerController = gameObject.AddComponent<PlayerController>();
            playerPhysical = gameObject.AddComponent<PlayerPhysical>();
            PhotonNetwork.NetworkingClient.EventReceived += AnymSyncFun;
        }

        public void SetTeam(int team, Color color)
        {
            this.team = team;
            _renderer.material.color = color;
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

        public void SetCharacter(Character character)
        {
            playerPhysical.SetPhysical(character.walkSpeed,
                                    character.dashForce,
                                    character.workSpeed);
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
    }
}