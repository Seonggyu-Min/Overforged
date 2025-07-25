using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MIN
{
    public enum SlideDirection
    {
        Horizontal,
        Vertical
    }

    [Serializable]
    public class UIGroup
    {
        [Header("애니메이션 대상 설정")]
        public List<RectTransform> Targets;

        [Header("방향 설정")]
        public SlideDirection SlideDirection = SlideDirection.Horizontal;

        [Header("슬라이드 설정")]
        public float OriginOffset = 0f;
        public float MoveInOffset = -300f;
        public float MoveOutOffset = -300f;

        [Header("애니메이션 시간 설정")]
        public float MoveInDuration = 0.5f;
        public float FadeInDuration = 0.5f;
        public float MoveOutDuration = 0.5f;
        public float FadeOutDuration = 0.5f;
        public float Interval = 0.05f;

        [Header("Ease 설정")]
        public Ease MoveInEase = Ease.OutBack;
        public Ease MoveOutEase = Ease.OutCirc;
    }

    public class UIAnimator : MonoBehaviour
    {
        [Header("애니메이션 그룹 등록")]
        [SerializeField] private List<UIGroup> _uiGroups;


        public IEnumerator AnimateIn(Action onComplete = null)
        {
            foreach (var group in _uiGroups)
            {
                foreach (var rect in group.Targets)
                {
                    rect.gameObject.SetActive(true);
                    rect.anchoredPosition = GetOffsetPosition(rect, group.SlideDirection, group.MoveInOffset);
                    rect.DOAnchorPos(GetOffsetPosition(rect, group.SlideDirection, group.OriginOffset), group.MoveInDuration)
                        .SetEase(group.MoveInEase);

                    yield return new WaitForSecondsRealtime(group.Interval);
                }
            }

            onComplete?.Invoke();
        }

        public IEnumerator AnimateOut(Action onComplete = null)
        {
            foreach (var group in _uiGroups)
            {
                foreach (var rect in group.Targets)
                {
                    rect.DOAnchorPos(GetOffsetPosition(rect, group.SlideDirection, group.MoveOutOffset), group.MoveOutDuration)
                        .SetEase(group.MoveOutEase);

                    yield return new WaitForSecondsRealtime(group.Interval);
                }
            }

            foreach (var group in _uiGroups)
            {
                foreach (var rect in group.Targets)
                {
                    rect.gameObject.SetActive(false);
                }
            }

            onComplete?.Invoke();
        }

        private Vector2 GetOffsetPosition(RectTransform rect, SlideDirection direction, float offset)
        {
            return direction == SlideDirection.Horizontal
                ? new Vector2(offset, rect.anchoredPosition.y)
                : new Vector2(rect.anchoredPosition.x, offset);
        }
    }
}

