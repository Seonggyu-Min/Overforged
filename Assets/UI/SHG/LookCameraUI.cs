using UnityEngine;

namespace SHG
{
  public class LookCameraUI : MonoBehaviour
  {
    [SerializeField]
    Transform cameraToLook;

    void Awake()
    {
      if (this.cameraToLook == null) {
        this.cameraToLook = Camera.main.transform;
      }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      this.transform.rotation = this.cameraToLook.rotation;
    }
  }
}
