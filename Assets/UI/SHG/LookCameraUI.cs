using UnityEngine;

namespace SHG
{
  public class LookCameraUI : MonoBehaviour
  {
    [SerializeField]
    Transform cameraToLook;

    void Awake()
    {

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
      this.transform.rotation = this.cameraToLook.rotation;
    }
  }
}
