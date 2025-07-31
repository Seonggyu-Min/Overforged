using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIBehaviour : MonoBehaviour
{
    [SerializeField] private Button _okButton;
    [SerializeField] private TMP_Text _TitleText;
    [SerializeField] private TMP_Text _infoText;

    void Awake()
    {
        _okButton.onClick.AddListener(ClosePopup);
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void ShowError(string text)
    {
        _TitleText.text = "Error";
        _infoText.text = text;
        _okButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void ShowInfo(string text)
    {
        _TitleText.text = "Message";
        _infoText.text = text;
        _okButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void ShowInfoOneSecond(string text)
    {
        _TitleText.text = "Message";
        _infoText.text = text;
        _okButton.gameObject.SetActive(false);
        StartCoroutine(CloseOneSecond());
    }

    private IEnumerator CloseOneSecond()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
