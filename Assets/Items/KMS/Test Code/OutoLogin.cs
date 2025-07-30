using System.Collections;
using System.Collections.Generic;
using MIN;
using TMPro;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;
    [SerializeField] TMP_InputField pwInputField;

    [SerializeField] LogInPanelBehaviour lpb;

    [SerializeField] string id;
    [SerializeField] string pw;
    [SerializeField] string id2;
    [SerializeField] string pw2;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            idInputField.text = id;
            pwInputField.text = pw;
            lpb.OnClickLoginButton();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            idInputField.text = id2;
            pwInputField.text = pw2;
            lpb.OnClickLoginButton();
        }
    }
}
