using UnityEngine;

namespace SHG
{
  public class LookCameraUI : MonoBehaviour
  {
    Transform cameraToLook;
    [SerializeField] Canvas canvas;
    void Awake()
    {

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
  }
}
