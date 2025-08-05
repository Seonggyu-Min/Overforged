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

        void OnValidate()
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
            }
            else if ((int)type < 5)
            {
                boxItemData = data.itemInfo[(int)ItemType.Wood].itemData;
                boxItemOre = OreType.None;
                boxItemWood = (WoodType)((int)type - 2);
            }
            else
            {
                boxItemData = data.itemInfo[(int)ItemType.String].itemData;
                boxItemOre = OreType.None;
                boxItemWood = WoodType.None;
            }
            mesh.material = itemMaterial[(int)type];
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

