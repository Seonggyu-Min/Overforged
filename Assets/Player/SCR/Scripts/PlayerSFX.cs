using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCR
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player SFX", order = int.MaxValue)]
    public class PlayerSFX : ScriptableObject
    {
        [Header("기본 상태")]
        public AudioClip Idle;

        [Header("움직일 때")]
        public AudioClip Move;

        [Header("대쉬")]
        public AudioClip Dash;

        [Header("물건 던지기")]
        public AudioClip Throw;

        [Header("모드 변경")]
        public AudioClip ChangeState;

        [Header("물건 들 때")]
        public AudioClip Hold;

        [Header("물건 놓을 때")]
        public AudioClip Put;

        [Header("주괴 작업할 때")]
        public AudioClip Hammering;

        [Header("나무 작업할 때")]
        public AudioClip CutDown;

        [Header("자랑")]
        public AudioClip ShowOff;

        [Header("맞았을 때")]
        public List<AudioClip> Hit;

        public AudioClip RandonHit()
        {
            return Hit[Random.Range(0, Hit.Count - 1)];
        }
    }
}

