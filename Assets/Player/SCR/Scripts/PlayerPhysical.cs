using SHG;
using UnityEngine;
using Photon.Pun;
using MIN;
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
        public Vector3 RespawnPoint;
        public bool IsRespawning;
        public bool IsDash;
        public bool IsHold;
        public bool UseTongs;
        private Vector3 centralPos;
        private RaycastHit hitInteractable2;
        private Collider hitInteractable;
        public ToolTransferArgs TransferArgs { get => transferArgs; }
        private ToolTransferArgs transferArgs;
        public bool CanTransfer { get => canTransfer; }
        private bool canTransfer;
        int teamId;
        void Awake()
        {
            player = GetComponent<Player>();
            rayLength = 1.5f;
            HoldingObjLayer = LayerMask.GetMask("Item", "InteractionObject");
            IsDash = false;
            IsHold = false;
            UseTongs = false;
            IsRespawning = false;
            // if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object teamcolor))
            // {
            //     teamId = int.Parse(teamcolor.ToString());
            // }
            teamId = PhotonNetwork.LocalPlayer.ActorNumber;

        }

        void Update()
        {
            centralPos = transform.position;
            centralPos.y = transform.position.y + 0.2f;
            CheckInteractable();
        }

        void FixedUpdate()
        {
            Debug.DrawRay(centralPos, transform.forward * rayLength, Color.yellow);
        }

        public void SetPhysical(float speed, float dashForce, float workSpeed)
        {
            this.speed = speed;
            this.dashForce = dashForce;
            this.workSpeed = workSpeed;
        }
        bool TryFind(out Collider col, float radius)
        {
            col = null;
            float distance = float.MaxValue;

            Collider[] hitColliders = Physics.OverlapSphere(
              position: this.transform.position,
              radius: radius,
              layerMask: HoldingObjLayer.value
              );
            foreach (var hit in hitColliders)
            {
                var dir = hit.transform.position - this.transform.position;
                var dot = Vector3.Dot(dir.normalized, this.transform.forward);
                //dot = Mathf.Clamp(dot, -1f, 1f);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                if (angle < 80f)
                {
                    float dist = dir.sqrMagnitude;
                    if (dist < distance)
                    {
                        distance = dist;
                        col = hit;
                    }
                }
            }
            return col != null;
        }
        public GameObject GetActionObj()
        {
            Collider hit;

            if (TryFind(out hit, rayLength))
            {
                Debug.Log(hit.gameObject.name);
                return hit.gameObject;
            }
            return null;
        }

        public GameObject GetActionObj2()
        {
            RaycastHit hit;

            if (Physics.Raycast(centralPos, transform.forward, out hit, rayLength, HoldingObjLayer))
            {
                Debug.Log(hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
            return null;
        }

        private void CheckInteractable()
        {
            if (TryFind(out hitInteractable, rayLength))
            {
                if (IsHold)
                {
                    transferArgs = new ToolTransferArgs
                    {
                        ItemToGive = player.HoldObject.GetComponent<Item>(),
                        PlayerNetworkId = teamId
                    };
                }
                else
                {
                    transferArgs = new ToolTransferArgs
                    {
                        ItemToGive = null,
                        PlayerNetworkId = teamId
                    };
                }
                IInteractableTool interactable = hitInteractable.gameObject.GetComponent<IInteractableTool>();
                if (interactable == null)
                {
                    Item item = hitInteractable.gameObject.GetComponent<Item>();
                    if (item != null) item.Hightlight(Color.green);
                    return;
                }
                canTransfer = interactable.CanTransferItem(transferArgs);
                if (interactable is SmithingToolComponent smithingTool)
                {
                    if (smithingTool.CanWork())
                    {
                        smithingTool.HighlightInstantly(Color.green);
                    }
                    else if (canTransfer)
                    {
                        smithingTool.HighlightInstantly(Color.blue);
                    }
                    else
                    {
                        smithingTool.HighlightInstantly(Color.yellow);
                    }
                }
            }
        }
    }
}
