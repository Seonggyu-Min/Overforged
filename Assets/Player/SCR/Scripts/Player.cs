using UnityEngine;

namespace SCR
{
    public class Player : MonoBehaviour
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
            playerController = gameObject.AddComponent<PlayerController>();
            playerPhysical = gameObject.AddComponent<PlayerPhysical>();
            SetTeam(1);
        }

        public void SetTeam(int team)
        {
            this.team = team;
            // team을 받아서 스카프의 색을 변경
            // _renderer.material.color = Color[team];
            // 캐릭터의 정보를 받아와서 스탯을 바꿔준다.
            playerPhysical.SetPhysical(3f, 5f, 1f);
        }
    }
}