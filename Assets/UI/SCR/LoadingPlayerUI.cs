using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class LoadingPlayerUI : MonoBehaviour
    {
        [SerializeField] List<Sprite> states;
        [SerializeField] Image image;


        public void Connect()
        {
            image.sprite = states[0];
        }

        public void Disconnect()
        {
            image.sprite = states[1];
        }
    }
}

