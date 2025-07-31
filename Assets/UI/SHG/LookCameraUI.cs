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
    private int index;

    void Awake()
    {
      gameObject.layer = LayerMask.NameToLayer("UI");
      index = 0;
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

    public void AddImage(Sprite image)
    {
      if (index > 2) return;
      images[index].sprite = image;
      images[index].gameObject.SetActive(true);
      index++;
    }

    public void SubImage()
    {
      index--;
      images[index].gameObject.SetActive(false);
    }

    public void SubAllImage()
    {
      foreach (Image image in images)
      {
        image.gameObject.SetActive(false);
      }
      index = 0;
    }
  }
}
