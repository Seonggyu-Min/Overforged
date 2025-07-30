using SHG;
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
        private RaycastHit hitInteractable;
        public ToolTransferArgs TransferArgs { get => transferArgs; }
        private ToolTransferArgs transferArgs;
        public bool CanTransfer { get => canTransfer; }
        private bool canTransfer;
        void Awake()
        {
            player = GetComponent<Player>();
            rayLength = 0.8f;
            HoldingObjLayer = LayerMask.GetMask("Item", "InteractionObject");
            IsDash = false;
            IsHold = false;
            UseTongs = false;
        }

        void Update()
        {
            centralPos = transform.position;
            centralPos.y = 0.2f;
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
                        PlayerNetworkId = 1
                    };
                }
                else
                {
                    transferArgs = new ToolTransferArgs
                    {
                        ItemToGive = null,
                        PlayerNetworkId = 1
                    };
                }
                IInteractableTool interactable = hitInteractable.collider.gameObject.GetComponent<IInteractableTool>();
                if (interactable == null) return;
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
