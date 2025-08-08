using System.Collections.Generic;
using SCR;
using UnityEngine;
using UnityEngine.UI;

namespace SHG
{
  public class LookCameraUI : MonoBehaviour
  {
    Transform cameraToLook;
    Canvas canvas;
    [SerializeField] List<Image> images;
    [SerializeField] ImageList imageList;
    private int index;

    void Awake()
    {
      gameObject.layer = LayerMask.NameToLayer("UI");
      index = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
      if (this.cameraToLook == null)
      {
        this.cameraToLook = Camera.main.transform;
      }
      if (canvas == null) canvas = GetComponent<Canvas>();
      canvas.worldCamera = Camera.main.transform.Find("UICamera").gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
      transform.LookAt(transform.position + cameraToLook.transform.rotation * Vector3.forward,
                               cameraToLook.transform.rotation * Vector3.up);
    }

    public void AddImage(OreType ore, WoodType wood)
    {
      if (index > 2) return;
      images[index].sprite = SetImage(ore, wood);
      var parent = images[index].gameObject.transform.parent;
      if (!parent.gameObject.activeSelf)
      {
        parent.gameObject.SetActive(true);
      }
      images[index].gameObject.SetActive(true);
      index++;
    }

    private Sprite SetImage(OreType ore, WoodType wood)
    {
      if (ore == OreType.None && wood == WoodType.None)
      {
        return imageList.sprites[6];
      }
      else if (ore != OreType.None)
      {
        if (ore == OreType.Copper) return imageList.sprites[0];
        else if (ore == OreType.Steel) return imageList.sprites[1];
        else if (ore == OreType.Gold) return imageList.sprites[2];
      }
      else
      {
        if (wood == WoodType.Oak) return imageList.sprites[3];
        else if (wood == WoodType.Birch) return imageList.sprites[4];
      }
      return null;
    }

    public void SubImage()
    {
      Debug.Log($"sub image: {index}");
      index--;
      var parent = images[index].gameObject.transform.parent;
      if (parent.gameObject.activeSelf)
      {
        parent.gameObject.SetActive(false);
      }
      images[index].gameObject.SetActive(false);
    }

    public void SubAllImage()
    {
      Debug.Log($"sub all images");
      foreach (Image image in images)
      {
        var parent = image.gameObject.transform.parent;
        if (parent.gameObject.activeSelf)
        {
          parent.gameObject.SetActive(false);
        }
        image.gameObject.SetActive(false);
      }
      index = 0;
    }
  }
}
