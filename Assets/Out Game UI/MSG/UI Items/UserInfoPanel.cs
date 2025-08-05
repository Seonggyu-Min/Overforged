using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Cinemachine.DocumentationSortingAttribute;


namespace MIN
{
    public class UserInfoPanel : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private Slider _expSlider;

        private void OnEnable()
        {
            SetInfo();
        }

        private void SetInfo()
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

                    int level = exp / 100;
                    float currentLevelExp = exp % 100;

                    _nickNameText.text = nickname;
                    _levelText.text = $"{level + 1}";
                    _expSlider.value = currentLevelExp / 100f;
                }
                else
                {
                    Debug.LogWarning("사용자 정보 불러오기 실패");
                }
            });
        }
    }
}
