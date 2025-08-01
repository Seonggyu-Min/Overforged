using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopupUIBehaviour : MonoBehaviour
{
    [Inject] private MIN.IOutGameUIManager _outGameUIManager;

    [SerializeField] private Button _okButton;
    [SerializeField] private TMP_Text _TitleText;
    [SerializeField] private TMP_Text _infoText;

    void Awake()
    {
        _okButton.onClick.AddListener(ClosePopup);
    }

    void OnDestroy()
    {
        _okButton.onClick.RemoveListener(ClosePopup);
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
        //_outGameUIManager.CloseTopPanel();
    }

    public void ShowError(string text)
    {
        _TitleText.text = "Error";
        _infoText.text = text;
        _okButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
        //_outGameUIManager.Show("Popup Panel");
    }

    public void ShowInfo(string text)
    {
        _TitleText.text = "Message";
        _infoText.text = text;
        _okButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
        //_outGameUIManager.Show("Popup Panel");
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
        //_outGameUIManager.Show("Popup Panel");
    }
}
