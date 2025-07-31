using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public enum DeveloperType
    {
        None,
        MSG1, MSG2, MSG3,
        SCR1, SCR2, SCR3,
        KMS1, KMS2, KMS3,
        SHG1, SHG2, SHG3,
        JJY1, JJY2, JJY3,
    }

    public class AutoConnector : MonoBehaviour
    {
        [SerializeField] private LogInPanelBehaviour _logInPanelBehaviour;
        [SerializeField] private LobbyPanelBehaviour _lobbyPanelBehaviour;
        [SerializeField] private CreateRoomPopUpPanelBehaviour _createRoomPopUpPanelBehaviour;
        [SerializeField] private TMP_InputField _roomNameText;

        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;

        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Button _makeTestRoomButton;
        [SerializeField] private Sprite _trueImage;
        [SerializeField] private Sprite _falseImage;
        private bool _willMakeRoom = false;
        private Coroutine _makeRoomCO;

        [Header("Put Your Account SO For Auto Log in")]
        [Header("MSG")]
        [SerializeField] private AccountSO _msgAccount;
        [Header("SCR")]
        [SerializeField] private AccountSO _scrAccount;
        [Header("KMS")]
        [SerializeField] private AccountSO _kmsAccount;
        [Header("SHG")]
        [SerializeField] private AccountSO _shgAccount;
        [Header("JJY")]
        [SerializeField] private AccountSO _jjyAccount;

        private Dictionary<DeveloperType, (string email, string password)> _developerCredentials;

        private void Start()
        {
            _developerCredentials = new Dictionary<DeveloperType, (string, string)>
            {
                { DeveloperType.MSG1, (_msgAccount.Email1, _msgAccount.Password1) },
                { DeveloperType.MSG2, (_msgAccount.Email2, _msgAccount.Password2) },
                { DeveloperType.MSG3, (_msgAccount.Email3, _msgAccount.Password3) },

                { DeveloperType.SCR1, (_scrAccount.Email1, _scrAccount.Password1) },
                { DeveloperType.SCR2, (_scrAccount.Email2, _scrAccount.Password2) },
                { DeveloperType.SCR3, (_scrAccount.Email3, _scrAccount.Password3) },

                { DeveloperType.KMS1, (_kmsAccount.Email1, _kmsAccount.Password1) },
                { DeveloperType.KMS2, (_kmsAccount.Email2, _kmsAccount.Password2) },
                { DeveloperType.KMS3, (_kmsAccount.Email3, _kmsAccount.Password3) },

                { DeveloperType.SHG1, (_shgAccount.Email1, _shgAccount.Password1) },
                { DeveloperType.SHG2, (_shgAccount.Email2, _shgAccount.Password2) },
                { DeveloperType.SHG3, (_shgAccount.Email3, _shgAccount.Password3) },

                { DeveloperType.JJY1, (_jjyAccount.Email1, _jjyAccount.Password1) },
                { DeveloperType.JJY2, (_jjyAccount.Email2, _jjyAccount.Password2) },
                { DeveloperType.JJY3, (_jjyAccount.Email3, _jjyAccount.Password3) },
            };

            _dropdown.ClearOptions();

            List<string> options = new();
            foreach (DeveloperType type in System.Enum.GetValues(typeof(DeveloperType)))
            {
                options.Add(type.ToString());
            }

            _dropdown.AddOptions(options);
            _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            OnDropdownValueChanged(_dropdown.value);
        }

        private void OnEnable()
        {
            _makeTestRoomButton.image.sprite = _willMakeRoom ? _trueImage : _falseImage;
        }

        private void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }


        public void OnClickMakeRoomButton()
        {
            _willMakeRoom = !_willMakeRoom;
            _makeTestRoomButton.image.sprite = _willMakeRoom ? _trueImage : _falseImage;

            if (_willMakeRoom)
            {
                if (_makeRoomCO != null)
                {
                    StopCoroutine(_makeRoomCO);
                }
                _makeRoomCO = StartCoroutine(TryMakeRoomRoutine());
            }
            else
            {
                if (_makeRoomCO != null)
                {
                    StopCoroutine(_makeRoomCO);
                    _makeRoomCO = null;
                }
            }
        }


        private void OnDropdownValueChanged(int index)
        {
            DeveloperType selected = (DeveloperType)index;

            if (_developerCredentials.TryGetValue(selected, out var credentials))
            {
                _emailField.text = credentials.email;
                _passwordField.text = credentials.password;
            }
            else
            {
                _emailField.text = string.Empty;
                _passwordField.text = string.Empty;
            }

            if (selected != DeveloperType.None)
            {
                _logInPanelBehaviour.OnClickLoginButton();
            }
        }

        private IEnumerator TryMakeRoomRoutine()
        {
            yield return new WaitUntil(() => _lobbyPanelBehaviour.gameObject.activeSelf);
            _lobbyPanelBehaviour.OnClickCreateRoomButton();

            yield return new WaitUntil(() => _createRoomPopUpPanelBehaviour.gameObject.activeSelf);
            _roomNameText.text = "Developer Test Room";
            _createRoomPopUpPanelBehaviour.OnClickConfirmButton();
        }
    }
}
