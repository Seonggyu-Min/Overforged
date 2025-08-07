using System;
using System.Collections;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public abstract class DoorLocker: MonoBehaviour
  {
    public Action OnUnlock { get; set; }
    public bool IsLocked { get; protected set; }
  }

  public class DoorController : MonoBehaviour, IInteractableTool
  {
    public bool IsClosed { get; private set; }
    public bool IsLocked => this.isLocked;
    [SerializeField] [Required]
    Transform doorHinge;
    [SerializeField]
    DoorLocker locker;
    [SerializeField] 
    Vector3 openedAngle;
    [SerializeField] 
    Vector3 closedAngle;
    [SerializeField] [Range (1f, 30f)]
    float rotateSpeed;
    [SerializeField]
    bool isClosed;
    bool isLocked;
    Coroutine rotateRoutine;
    Quaternion opendedRotation;
    Quaternion closedRotation;
    Quaternion destRotation;

    void Awake()
    {
      this.opendedRotation = Quaternion.Euler(
        this.openedAngle.x,
        this.openedAngle.y,
        this.openedAngle.z);
      this.closedRotation = Quaternion.Euler(
        this.closedAngle.x,
        this.closedAngle.y, 
        this.closedAngle.z);
      this.IsClosed = this.isClosed;
      if (this.locker != null) {
        this.isLocked = this.locker.IsLocked;
      } 
      else {
        this.isLocked = false;
      }
      if (this.IsClosed) {
        this.doorHinge.localRotation = this.closedRotation;
      }
      else {
        this.doorHinge.localRotation = this.opendedRotation;
      }
    }

    void Start()
    {
      if (this.locker != null) {
        this.locker.OnUnlock += this.OnUnlock;
      }
    }

    public void OnUnlock()
    {
      this.isLocked = false; 
      if (this.IsClosed) {
        this.Open();
      }
    }

    [Button ("Open")]
    void Open()
    {
      if (this.isLocked) {
        return ;
      }
      if (this.rotateRoutine != null) {
        this.StopCoroutine(this.rotateRoutine);
        this.rotateRoutine = null;
        this.doorHinge.localRotation = this.destRotation;
      }
      this.destRotation = this.opendedRotation;
      this.rotateRoutine = this.StartCoroutine(this.CreateRotateRoutine());
      this.IsClosed = false;
    }

    [Button ("Close")]
    void Close()
    {
      if (this.rotateRoutine != null) {
        this.StopCoroutine(this.rotateRoutine);
        this.rotateRoutine = null;
        this.doorHinge.localRotation = this.destRotation;
      }
      this.destRotation = this.closedRotation;
      this.rotateRoutine = this.StartCoroutine(this.CreateRotateRoutine());
      this.IsClosed = true; 
    }

    IEnumerator CreateRotateRoutine()
    {
      while (Quaternion.Angle(
          this.doorHinge.localRotation ,
          this.destRotation 
          ) > float.Epsilon) {
        this.doorHinge.localRotation = Quaternion.Lerp(
          this.doorHinge.localRotation ,
          this.destRotation,
          this.rotateSpeed * Time.deltaTime
          );
        yield return (null);
      } 
      this.doorHinge.localRotation = this.destRotation;
    }

    public bool CanTransferItem(ToolTransferArgs args)
    {
        return (false);
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
            return (new ToolTransferResult());
    }

    public bool CanWork()
    {
        return (this.IsClosed);
    }

    public ToolWorkResult Work()
    {
       if (this.IsClosed) {
        this.Open();
      }
      else {
        this.Close();
      }
       return (new ToolWorkResult());
    }
  }
}
