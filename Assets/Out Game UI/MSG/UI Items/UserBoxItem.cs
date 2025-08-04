using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class UserBoxItem : MonoBehaviour
    {
        private IFirebaseManager _firebaseManager;

        [SerializeField] private Image _statusImage;
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private GameObject _otherUserInfoPopPrefab;
        [SerializeField] private Transform _popUpParent;
        private string _uid;
        private GameObject _popUp;

        public void Init(string nickname, bool isGaming, string uid, IFirebaseManager firebaseManager, Transform parent)
        {
            _firebaseManager = firebaseManager;
            _popUpParent = parent;

            _nickNameText.text = nickname;
            // 게임 중이면 노랑, 로비나 방에 있으면 초록으로 표기
            _statusImage.color = isGaming ? Color.yellow : Color.green;

            _uid = uid;
        }

        // TODO: 일단 생성과 파괴로 만들었는데 추후 바꿔야될 듯
        // 또한 GetComponent하지 않는 구조로 변경 하면 좋을 듯
        public void OnClickThis()
        {
            _popUp = Instantiate(_otherUserInfoPopPrefab, _popUpParent);
            OtherUserInfoPopUp info = _popUp.GetComponent<OtherUserInfoPopUp>();
            info.SetText(_uid, _firebaseManager);
        }

        public void OnClickCloseButton()
        {
            Destroy(_popUp);
        }
    }
}
