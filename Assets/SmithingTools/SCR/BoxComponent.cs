using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SCR
{
    public class BoxComponent : MonoBehaviourPun
    {
        [SerializeField] MaterialItemDataList data;
        [SerializeField] MaterialItemType type;
        private ItemData boxItemData;
        private OreType boxItemOre;
        private WoodType boxItemWood;

        [SerializeField] List<Material> itemMaterial;
        [SerializeField] MeshRenderer mesh;

        private GameObject itemObj;
        [SerializeField] PhotonView view;

        void Start()
        {
            SetItem();
        }

        private void SetItem()
        {
            if ((int)type < 3)
            {
                boxItemData = data.itemInfo[(int)ItemType.Ore].itemData;
                boxItemOre = (OreType)((int)type + 1);
                boxItemWood = WoodType.None;
                mesh.material = itemMaterial[0];

            }
            else if ((int)type < 5)
            {
                boxItemData = data.itemInfo[(int)ItemType.Wood].itemData;
                boxItemOre = OreType.None;
                boxItemWood = (WoodType)((int)type - 2);
                mesh.material = itemMaterial[1];
            }
            else
            {
                boxItemData = data.itemInfo[(int)ItemType.String].itemData;
                boxItemOre = OreType.None;
                boxItemWood = WoodType.None;
                mesh.material = itemMaterial[2];
            }
            Debug.Log(boxItemData);
            Debug.Log(boxItemOre);
            Debug.Log(boxItemWood);
            Debug.Log(mesh.material);
        }

        public GameObject CreateItem()
        {
            itemObj = PhotonNetwork.Instantiate("MatItem", transform.position, Quaternion.identity);
            //MaterialItem createItem = itemObj.GetComponent<MaterialItem>();
            //createItem.Data = boxItemData;
            //createItem.Ore = boxItemOre;
            //createItem.Wood = boxItemWood;

            int viewID = itemObj.GetComponent<PhotonView>().ViewID;
            view.RPC(nameof(RPC_SetItem), RpcTarget.AllBuffered, viewID);

            return itemObj;
        }

        [PunRPC]
        public void RPC_SetItem(int viewID)
        {
            PhotonView pv = PhotonView.Find(viewID);

            MaterialItem createItem = pv.gameObject.GetComponent<MaterialItem>();
            createItem.Data = boxItemData;
            createItem.Ore = boxItemOre;
            createItem.Wood = boxItemWood;
        }

    }

    public enum ItemType
    {
        Axe,
        AxeBlade,
        Bar,
        Blade,
        Bow,
        BowBase,
        Hammer,
        HammerHead,
        Handle,
        Ore,
        String,
        Sword,
        Trash,
        Wood
    }

    public enum MaterialItemType
    {
        SteelOre,
        CopperOre,
        GoldOre,
        OakWood,
        BirchWood,
        String
    }

}

