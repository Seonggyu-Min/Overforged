using Photon.Pun;
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

        private int team;

        public GameObject HoldObject;

        [Header("물건 드는 위치")]
        [SerializeField] private Transform holdingPos;

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
        }

        public void SetTeam(int team, Color color)
        {
            this.team = team;
            _renderer.material.color = color;
        }

        public void SetCharacter(Character character)
        {
            playerPhysical.SetPhysical(character.walkSpeed,
                                    character.dashForce,
                                    character.workSpeed);
        }
    }
}