using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;


namespace MIN
{
    public class UserInfoPopUp : MonoBehaviour
    {
        [Inject] IFirebaseManager _firebaseManager;

        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _matchCountText;
        [SerializeField] private TMP_Text _winCountText;
        [SerializeField] private TMP_Text _loseCountText;
        [SerializeField] private TMP_Text _winRateText;


        private void OnEnable()
        {
            SetText();
        }

        private void SetText()
        {
            string uid = _firebaseManager.Auth.CurrentUser.UserId;
            DatabaseReference userRef = _firebaseManager.Database.GetReference("users").Child(uid);

            userRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogWarning("유저 정보 불러오기가 취소됨");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogWarning($"유저 정보 불러오기에 실패함. 이유 : {task.Exception}");
                        return;
                    }

                    var snapshot = task.Result;

                    string nickname = snapshot.Child("nickname").Value?.ToString() ?? "Unknown";
                    int exp = int.TryParse(snapshot.Child("exp").Value?.ToString(), out int e) ? e : 0;

                    int winCount = int.TryParse(snapshot.Child("stats").Child("win").Value?.ToString(), out int w) ? w : 0;
                    int loseCount = int.TryParse(snapshot.Child("stats").Child("lose").Value?.ToString(), out int l) ? l : 0;
                    int drawCount = int.TryParse(snapshot.Child("stats").Child("draw").Value?.ToString(), out int d) ? d : 0;

                    int matchCount = winCount + loseCount + drawCount;

                    int level = exp / 100;
                    float winRate = (winCount + loseCount) == 0 ? 0 : (float)winCount / (winCount + loseCount);

                    _nickNameText.text = nickname;
                    _levelText.text = $"Lv. {level + 1}";
                    _matchCountText.text = matchCount.ToString();
                    _winCountText.text = winCount.ToString();
                    _loseCountText.text = loseCount.ToString();
                    _winRateText.text = $"{winRate * 100:F2}%";
                }
                else
                {
                    Debug.LogWarning("사용자 정보 불러오기 실패");
                }
            });
        }
    }
}
