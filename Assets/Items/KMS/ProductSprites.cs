using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[CreateAssetMenu(menuName = "ProductSprites")]
public class ProductSprites : ScriptableObject
{
    [SerializeField] List<Sprite> list;
    [SerializeField] MaterialData matData;
    [SerializeField] ItemDataList itemList;
    public Dictionary<string, Sprite> Dict;

    void OnEnable()
    {
        Dict = new();
    }
    public void Init()
    {
        int index = 0;
        foreach (ProductItemData pi in itemList.productList)
        {
            foreach (string wood in matData.woodName.Values)
            {
                if (wood == "") continue;
                foreach (string ore in matData.oreName.Values)
                {
                    string temp = "";
                    if (ore == "")
                    {
                        if (pi.Name == "Bow")
                        {
                            temp = $"{ore}{wood}{pi.Name}";
                            Dict.Add(temp, list[index]);
                            index++;
                        }

                    }
                    else
                    {
                        if (pi.Name != "Bow")
                        {
                            temp = $"{ore}{wood}{pi.Name}";
                            Dict.Add(temp, list[index]);
                            index++;
                        }
                    }

                }
            }
        }
    }
}
