using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using AppUIElement = Unity.AppUI.UI;
using Zenject;

namespace SHG
{
  public class OptionUI : MonoBehaviour
  {
    public static OptionUI Instance => instance;
    static OptionUI instance;
    IAudioLibrary audioManger => SingletonAudio.Instance;

    UIDocument document;
    VisualElement root;
    AppUIElement.Panel panel;
    public bool IsShowing { get; private set; }
    AppUIElement.SliderFloat masterVolumeSlider;
    AppUIElement.SliderFloat bgmVolumeSlider;
    AppUIElement.SliderFloat sfxVolumeSlider;
    Nullable<KeyCode> keyCode;
    ListView bgmListView;
    List<string> bgmList;
    int[] selectedBgmIndex = new int[1];
    VisualElement container;

    public void Show()
    {
      this.container.style.display = DisplayStyle.Flex;
      this.container.contentContainer.BringToFront();
      this.container.BringToFront();
      this.root.BringToFront();
      this.root.style.display = DisplayStyle.Flex;
      this.IsShowing = true;
      this.UpdateUI();
    }

    public void Hide()
    {
      this.IsShowing = false;
      this.container.style.display = DisplayStyle.None;
      this.container.SendToBack();
      this.root.SendToBack();
      this.root.style.display = DisplayStyle.None;
    }

    public void SetKey(KeyCode keyCode) {
      this.keyCode = keyCode;
    }

    public void UnSetKey()
    {
      this.keyCode = null;
    }

    void Update()
    {
      if (this.keyCode != null && 
        Input.GetKeyDown(this.keyCode.Value)) {
        this.OnPressedKey();
      }
    }

    void OnPressedKey()
    {
      if (!this.IsShowing) {
        this.Show();
      }
      else {
        this.Hide();
      }
    }

    void UpdateUI()
    {
      this.masterVolumeSlider.SetValueWithoutNotify(
        this.audioManger.GetVolume(IAudioLibrary.VolumeType.Master));
      this.bgmVolumeSlider.SetValueWithoutNotify
        (this.audioManger.GetVolume(IAudioLibrary.VolumeType.Bgm));
      this.sfxVolumeSlider.SetValueWithoutNotify(
        this.audioManger.GetVolume(IAudioLibrary.VolumeType.Sfx));
      this.bgmList.Clear();
      foreach (var bgm in this.audioManger.GetBgmList()) {
        this.bgmList.Add(bgm); 
      }
      this.bgmListView.Rebuild();
      string currentBgm = this.audioManger.GetCurrentBgm();
      int bgmIndex = this.bgmList.FindIndex(bgm => bgm == currentBgm);
      if (bgmIndex != -1) {
        this.selectedBgmIndex[0] = bgmIndex;
        this.bgmListView.SetSelectionWithoutNotify(this.selectedBgmIndex) ;
      }
    }

    void OnVolumeSliderChanged(IAudioLibrary.VolumeType volumeType, float value)
    {
      this.audioManger.SetVolume(volumeType, value);
    }

    void Awake()
    {
      this.document = this.GetComponent<UIDocument>();
      this.root = this.document.rootVisualElement;
      this.root.AddToClassList("window");
      this.panel = new AppUIElement.Panel {
        theme = "dark"
      };
      this.panel.AddToClassList("window");
      this.root.Add(this.panel);
      this.container = new VisualElement {
        name = "option-window"
      };
      this.panel.Add(this.container);
      this.bgmList = new();
      this.CreateUI();
      this.UpdateUI();
      this.Hide();
      this.SetKey(KeyCode.Delete);
    }
    
    void OnEnable()
    {
      if (instance == null) {
        instance = this; 
      } 
      else {
        Destroy(this.gameObject);
        return;
      }
    }

    void OnDisable()
    {
      if (instance == this) {
        instance = null;
      }
    }

    void CreateUI()
    {
      var optionHeader = new AppUIElement.Heading {
        text = "Options",
             size = AppUIElement.HeadingSize.XL
      }; 
      optionHeader.AddToClassList(Constants.OPTION_HEADER);
      this.container.Add(optionHeader);
      var closeButtonContainer = new VisualElement {
         name = Constants.CLOSE_BUTTON 
      };
      closeButtonContainer.AddToClassList(Constants.GRAY_CIRCLE_BUTTON);
      var closeButton = new AppUIElement.Button{
        name = "option-close-button-icon"
      };
      closeButton.clicked += () => {
        if (this.IsShowing) {
          this.Hide();
        }
      };
      closeButtonContainer.Add(closeButton);
      this.container.Add(closeButtonContainer);
      this.container.Add(
        new AppUIElement.Spacer {
        spacing = AppUIElement.SpacerSpacing.S
        });
      var soundHeader = new AppUIElement.Heading {
        text = "Sound",
        size = AppUIElement.HeadingSize.M
      };
      this.container.Add(soundHeader);

      var masterVolumeRow = this.CreateRowSlider(
        labelText: "Master",
        slider: out this.masterVolumeSlider,
        labelIconName: "sound-icon"
        );
      this.container.Add(masterVolumeRow);

      this.masterVolumeSlider.RegisterValueChangedCallback<float>(
        evt => this.OnVolumeSliderChanged(IAudioLibrary.VolumeType.Master, evt.newValue));

      var bgmVolumeRow = this.CreateRowSlider(
        labelText: "BGM",
        slider: out this.bgmVolumeSlider,
        labelIconName: "bgm-icon"
        );
      this.container.Add(bgmVolumeRow); 
      this.bgmVolumeSlider.RegisterValueChangedCallback<float>(
        evt => this.OnVolumeSliderChanged(IAudioLibrary.VolumeType.Bgm, 
          evt.newValue));

      var sfxVolumeRow = this.CreateRowSlider(
        labelText: "SFX",
        slider: out this.sfxVolumeSlider
        );
      this.sfxVolumeSlider.RegisterValueChangedCallback<float>(
        evt => this.OnVolumeSliderChanged(IAudioLibrary.VolumeType.Sfx, 
          evt.newValue));
      this.container.Add(sfxVolumeRow);
      this.container.Add(
        new AppUIElement.Spacer {
        spacing = AppUIElement.SpacerSpacing.M
        });
      var bgmLabel = new AppUIElement.Heading {
        text = "Bgm",
        size = AppUIElement.HeadingSize.M
      };
      this.container.Add(bgmLabel);
      this.bgmListView = new ListView {
        name = Constants.BGM_LIST_CONTAINER,
        itemsSource = this.bgmList,
        makeItem = this.CreateBgmRow,
        bindItem = this.UpdateBgmRow,
        selectionType = SelectionType.Single,
        fixedItemHeight = 60
      };
      this.container.Add(
        new AppUIElement.Spacer {
        spacing = AppUIElement.SpacerSpacing.S
        });
      this.bgmListView.selectedIndicesChanged += this.OnBgmSelected; 
      this.container.Add(this.bgmListView);
    }

    void OnBgmSelected(IEnumerable indices)
    {
      var enumerator = indices.GetEnumerator();
      if (enumerator.MoveNext()) {
        int index = (int)enumerator.Current;
        this.audioManger.PlayBgm(this.bgmList[index]);
      }
    }

    void UpdateBgmRow(VisualElement row, int index)
    {
      var label = row.Q<AppUIElement.Text>(className: Constants.BGM_LIST_LABEL);
      label.text = this.bgmList[index];
    }

    VisualElement CreateBgmRow()
    {
      var container = new VisualElement(); 
      container.AddToClassList(Constants.BGM_LIST_ROW);
      var label = new AppUIElement.Text {
        size = AppUIElement.TextSize.L
      };
      label.AddToClassList(Constants.BGM_LIST_LABEL);
      container.Add(label);
      return (container);
    }

    VisualElement CreateRowSlider(
      in string labelText,
      out AppUIElement.SliderFloat slider, 
      in string labelIconName = null) 
    {
      var rowContainer = new VisualElement();
      rowContainer .AddToClassList(Constants.ROW_CLASS);    
      var icon = new VisualElement();
      if (labelIconName != null) {
        icon.name = labelIconName;
      }
      icon.AddToClassList(Constants.ROW_LABEL_ICON);
      rowContainer.Add(icon);
      var label = new AppUIElement.Text {
        text =  labelText,
             size = AppUIElement.TextSize.M
      };
      label.AddToClassList(Constants.ROW_LABEL);
      slider = new AppUIElement.SliderFloat {
        track = AppUIElement.TrackDisplayType.On,
      };
      slider.AddToClassList(Constants.ROW_SLIDER);
      slider.lowValue = 0f;
      slider.highValue = 1f;
      rowContainer.Add(label);
      rowContainer.Add(slider);
      return (rowContainer);
    }

    struct Constants
    {
      public const string OPTION_HEADER = "option-header";
      public const string GRAY_CIRCLE_BUTTON = "gray-circle-button";
      public const string CLOSE_BUTTON = "option-close-button";
      public const string ROW_CLASS = "option-row";
      public const string ROW_LABEL = "option-row-label";
      public const string ROW_LABEL_ICON = "option-row-label-icon";
      public const string ROW_SLIDER = "option-row-slider";
      public const string BGM_LIST_CONTAINER = "option-bgm-list-container";
      public const string BGM_LIST_ROW = "option-bgm-list-row";
      public const string BGM_LIST_LABEL = "option-bgm-list-label";
    }
  }

}
