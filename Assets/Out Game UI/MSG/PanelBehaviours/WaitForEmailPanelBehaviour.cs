using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class WaitForEmailPanelBehaviour : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _infoText;

        private Coroutine _verificationCO;


        private void OnEnable() => StartVerification();


        public void OnClickBackButton() => _outGameUIManager.CloseTopPanel();


        private void StartVerification()
        {
            _firebaseManager.Auth.CurrentUser.SendEmailVerificationAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        _infoText.text = "인증 이메일 전송이 취소되었습니다. 다시 시도해주세요.";
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        _infoText.text = $"인증 이메일 전송 중 오류 발생: {task.Exception}";
                        return;
                    }
                    
                    _infoText.text = "인증 이메일이 전송되었습니다. 이메일을 확인해주세요.";
                    _verificationCO = StartCoroutine(EmailVerificationRoutine());
                });
        }

        private IEnumerator EmailVerificationRoutine()
        {
            FirebaseUser user = _firebaseManager.Auth.CurrentUser;
            WaitForSeconds delay = new WaitForSeconds(2f);

            while (true)
            {
                yield return delay;

                user.ReloadAsync();
                if (user.IsEmailVerified)
                {
                    Debug.Log("인증 완료");
                    _outGameUIManager.Hide("Wait For Email Panel", () =>
                    {
                        _outGameUIManager.Show("Set Nickname Panel");
                    });

                    StopCoroutine(_verificationCO);
                }
                else
                {
                    Debug.Log("인증 대기중");
                }
            }
        }
    }
}
