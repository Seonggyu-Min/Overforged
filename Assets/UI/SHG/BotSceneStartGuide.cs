using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;

namespace SHG
{
    public class BotSceneStartGuide : MonoBehaviour
    {
        [SerializeField] [Required]
        Button button;

        void Awake()
        {
            this.button.onClick.AddListener(this.OnClickStart);
        }

        void OnClickStart()
        {
            this.gameObject.SetActive(false);
            BotSceneManager.Instance.StartPlay();
        }
    }
}