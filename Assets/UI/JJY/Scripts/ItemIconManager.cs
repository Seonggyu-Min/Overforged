using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    public class ItemIconManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private GameObject iconPrefab;
        private Dictionary<Transform, RectTransform> iconMap = new();

        public void RegisterItemIcon(Transform worldTarget, Sprite iconSprite) // 아이콘 표시
        {
            if (iconMap.ContainsKey(worldTarget)) return; // 중복방지

            RectTransform icon = Instantiate(iconPrefab, transform).GetComponent<RectTransform>();

            Image image = icon.GetComponentInChildren<Image>();
            image.sprite = iconSprite;

            iconMap.Add(worldTarget, icon);
        }

        public void UnregisterItemIcon(Transform worldTarget) // 아이콘 삭제
        {
            if (iconMap.TryGetValue(worldTarget, out var icon))
            {
                Destroy(icon.gameObject);
                iconMap.Remove(worldTarget);
            }
        }
        void LateUpdate()
        {
            IconFollowing();
        }

        void IconFollowing()
        {
            foreach (var kvp in iconMap)
            {
                Vector3 screenPos = mainCam.WorldToScreenPoint(kvp.Key.position + new Vector3 (0, 5f, 5f));
                kvp.Value.position = screenPos;
            }
        }
        public void IconBurning()
        {
            // 아이템이 붉어지는 함수. 용광로에서 여러번 호출?
            // 0.5초마다 () 1 / 아이템에 있는 제련시간 ) 만큼 붉어짐(투명 -> 불투명)
        }

        // Test전용
        [Header("테스트 전용")]
        public Transform itemA;
        public Transform itemB;
        public Transform itemC;
        public Sprite iconA;
        public Sprite iconB;
        public Sprite iconC;

        void Update()
        {
            // 등록
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RegisterItemIcon(itemA, iconA);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RegisterItemIcon(itemB, iconB);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RegisterItemIcon(itemC, iconC);
            }

            // 삭제
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UnregisterItemIcon(itemA);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                UnregisterItemIcon(itemB);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                UnregisterItemIcon(itemC);
            }
        }
    }
}

