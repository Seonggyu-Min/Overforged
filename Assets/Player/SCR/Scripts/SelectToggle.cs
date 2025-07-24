using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class SelectToggle : MonoBehaviour
    {
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
        }
    }
}

