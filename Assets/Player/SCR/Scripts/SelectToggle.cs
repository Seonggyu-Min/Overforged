using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class SelectToggle : MonoBehaviour
    {
        [Header("커스텀 프로퍼티 키")]
        public string propertyKey;

        [SerializeField] Color selectColor;
        [SerializeField] Color unSelectColor;
        [SerializeField] Toggle[] toggle;
        public int SelectIndex { get => selectIndex; }
        private int selectIndex;

        void Awake()
        {
            selectIndex = 0;
            ChangeType();
        }

        public void ChangeType()
        {
            for (int i = 0; i < toggle.Length; i++)
            {
                ColorBlock colorBlock = toggle[i].colors;
                if (toggle[i].isOn)
                {
                    selectIndex = i;
                    colorBlock.normalColor = selectColor;
                }
                else colorBlock.normalColor = unSelectColor;

                toggle[i].colors = colorBlock;
            }
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { propertyKey, selectIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);


        }
    }
}

