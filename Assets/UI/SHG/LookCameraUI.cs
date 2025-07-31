using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SHG
{
  public class LookCameraUI : MonoBehaviour
  {
    Transform cameraToLook;
    Canvas canvas;
    [SerializeField] List<Image> images;

    void Awake()
    {
      gameObject.layer = LayerMask.NameToLayer("UI");
      if (this.cameraToLook == null)
      {
        this.cameraToLook = Camera.main.transform;
      }
      if (canvas == null) canvas = GetComponent<Canvas>();
      canvas.worldCamera = Camera.main.transform.Find("UICamera").gameObject.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
      if (this.cameraToLook == null)
      {
        this.cameraToLook = Camera.main.transform;
      }
    }

    // Update is called once per frame
    void Update()
    {
      transform.LookAt(transform.position + cameraToLook.transform.rotation * Vector3.forward,
                               cameraToLook.transform.rotation * Vector3.up);
    }

    public void SetImage(Sprite image, int index = 0)
    {
      images[index].sprite = image;
      images[index].gameObject.SetActive(true);
    }

    public void OffImage(int index = 0)
    {
      images[index].gameObject.SetActive(false);
    }
  }
}
