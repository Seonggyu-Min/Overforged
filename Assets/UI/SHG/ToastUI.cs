using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AppUIElement = Unity.AppUI.UI;

namespace SHG
{
  public class ToastUI : MonoBehaviour
  {
    public static ToastUI Instance => instance;
    static ToastUI instance;

    public bool IsShowing { get; private set; }
    UIDocument document;
    VisualElement root;
    AppUIElement.Panel panel;

    void OnEnable()
    {
      if (instance == null) {
        instance = this;
      }
      else {
        Destroy(this.gameObject);
      }
    }

    void Start()
    {
      this.document = this.GetComponent<UIDocument>();
      this.root = this.document.rootVisualElement;
      this.panel = this.root.Q<AppUIElement.Panel>();
      this.CreateUI();
    }

    void CreateUI()
    {

    }

    void OnDisable()
    {
      if (instance == this) {
        instance = null;
      }
    }
  }
}
