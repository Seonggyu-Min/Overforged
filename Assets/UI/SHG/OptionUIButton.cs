using UnityEngine;
using UnityEngine.UI;

namespace SHG
{
  [RequireComponent(typeof(Button))]
  public class OptionUIButton : MonoBehaviour
  {
    OptionUI optionUI; 
    Button settingButton;

    void Awake()
    {
      this.settingButton = this.GetComponent<Button>();
      this.settingButton.onClick.AddListener(this.OnClickSettingButton);
    }

    void Start()
    {
      this.optionUI = OptionUI.Instance;
      if (this.optionUI == null) {
        Debug.LogError($"{nameof(OptionUIButton)}: Fail to find {nameof(OptionUI)}");        
        Destroy(this.gameObject);
      }
    }

    void OnClickSettingButton()
    {
      if (!this.optionUI.IsShowing) {
        this.optionUI.Show();
      } 
    }
  }
}
