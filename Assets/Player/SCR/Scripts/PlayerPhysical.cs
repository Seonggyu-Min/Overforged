using UnityEngine;
namespace SCR
{
    public class PlayerPhysical : MonoBehaviour
    {
        private Player player;
        public float Speed { get => speed; }
        private float speed;
        public float DashForce { get => dashForce; }
        private float dashForce;
        public float WorkSpeed { get => workSpeed; }
        private float workSpeed;
        private float rayLength;
        private LayerMask HoldingObjLayer;
        public bool IsDash;
        public bool IsHold;
        public bool UseTongs;
        private Vector3 centralPos;

        void Awake()
        {
            player = GetComponent<Player>();
            rayLength = 1f;
            HoldingObjLayer = LayerMask.NameToLayer("item");
        }

        void Update()
        {
            centralPos = transform.position;
            centralPos.y = 0.3f;
            Debug.DrawRay(centralPos, transform.forward * rayLength, Color.yellow);
        }

        public void SetPhysical(float speed, float dashForce, float workSpeed)
        {
            this.speed = speed;
            this.dashForce = dashForce;
            this.workSpeed = workSpeed;
        }

        public GameObject GetActionObj()
        {
            RaycastHit hit;

            if (Physics.Raycast(centralPos, transform.forward, out hit, rayLength, HoldingObjLayer))
            {
                Debug.Log(hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
            return null;
        }
    }
}
