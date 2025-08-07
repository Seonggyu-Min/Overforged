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
        private RaycastHit hitInteractable;
        public ToolTransferArgs TransferArgs { get => transferArgs; }
        private ToolTransferArgs transferArgs;
        public bool CanTransfer { get => canTransfer; }
        private bool canTransfer;
        int teamId;
        void Awake()
        {
            player = GetComponent<Player>();
            rayLength = 0.8f;
            HoldingObjLayer = LayerMask.GetMask("Item", "InteractionObject");
            IsDash = false;
            IsHold = false;
            UseTongs = false;
            IsRespawning = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object teamcolor))
            {
                teamId = int.Parse(teamcolor.ToString());
            }

        }

        void Update()
        {
            centralPos = transform.position;
            centralPos.y = 0.7f;
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

        private void CheckInteractable()
        {
            if (Physics.Raycast(centralPos, transform.forward, out hitInteractable, rayLength, HoldingObjLayer))
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
                IInteractableTool interactable = hitInteractable.collider.gameObject.GetComponent<IInteractableTool>();
                if (interactable == null)
                {
                    Item item = hitInteractable.collider.gameObject.GetComponent<Item>();
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
