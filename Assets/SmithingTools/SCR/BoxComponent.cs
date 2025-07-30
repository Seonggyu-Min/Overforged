using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using SCR;

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
            GameObject itemObj = PhotonNetwork.Instantiate("MatItem", transform.position, Quaternion.identity);
            MaterialItem createItem = itemObj.GetComponent<MaterialItem>();
            createItem.Data = boxItemData;
            createItem.Ore = boxItemOre;
            createItem.Wood = boxItemWood;
            return itemObj;
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

