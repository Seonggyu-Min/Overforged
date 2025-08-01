using System;
using UnityEngine;
using UnityEngine.UI;

namespace SHG
{
  public class GauageImageUI : MonoBehaviour
  {
    const float ANIMATE_LERP_STEP = 10f;
    const float ANIMATE_LERP_THRESHOLD = 0.01f;

    [SerializeField] Slider gauageImage;
    [SerializeField] Sprite workSprite;
    [SerializeField] Image workIcon;

    public void SetWorkImage()
    {
      workIcon.sprite = workSprite;
    }

    public ObservableValue<(float, float)> WatchingFloatValue
    {
      get => this.watchingFloatValue;
      set {
        if (this.watchingFloatValue != null) {
          this.watchingFloatValue.OnChanged -= this.OnValueChanged;
        }
        this.watchingFloatValue = value;
        if (value != null) {
          value.OnChanged += this.OnValueChanged;
          this.OnValueChanged(value.Value);
        }
      }
    }
    public ObservableValue<(int, int)> WatchingIntValue
    {
      get => this.watchingIntValue;
      set {
        if (this.watchingIntValue != null) {
          this.watchingIntValue.OnChanged -= this.OnValueChanged;
        }
        this.watchingIntValue = value;
        if (value != null) {
          value.OnChanged += this.OnValueChanged;
          this.OnValueChanged(value.Value);
        }
      }
    }

    ObservableValue<(float, float)> watchingFloatValue;
    ObservableValue<(int, int)> watchingIntValue;
    bool isAnimating;

    float destValue;

    // Start is called before the first frame update
    void OnEnable()
    {
      SetWorkImage();
      if (this.watchingFloatValue != null) {
        var (current, max) = this.watchingFloatValue.Value;
        this.gauageImage.value = Math.Clamp(current /max, 0, 1);
      }
      else if (this.watchingIntValue != null) {
        var (current, max) = this.watchingIntValue.Value;
        this.gauageImage.value = Math.Clamp(current /max, 0, 1);
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (this.isAnimating) {
        if (Math.Abs(this.gauageImage.value - this.destValue)
          < ANIMATE_LERP_THRESHOLD)
        {
          this.gauageImage.value = this.destValue;
          this.isAnimating = false;
        }
        else {
          this.gauageImage.value = Mathf.Lerp(
            this.gauageImage.value,
            this.destValue,
            ANIMATE_LERP_STEP * Time.deltaTime
            );
        }
      }
    }

    void OnValueChanged((int current, int max) value)
    {
      this.OnValueChanged(((float)value.current, (float)value.max));
    }

    void OnValueChanged((float current, float max) value)
    {
      if (this.gauageImage.value != this.destValue ||
        value.current <= 0f) {
        this.gauageImage.value = this.destValue;
      }
      this.destValue = Math.Clamp(value.current / value.max, 0, 1);
      this.isAnimating = true;
    }
  }
}
