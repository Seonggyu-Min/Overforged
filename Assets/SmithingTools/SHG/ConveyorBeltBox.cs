using UnityEngine;
using UnityEngine.Splines;

namespace SHG
{
  [RequireComponent(typeof(SplineAnimate))]
  public class ConveyorBeltBox : MonoBehaviour
  {
    static int NEXT_ID = 0;
    public float OneLapDist;
    public int Id;
    SplineAnimate animator;

    void Awake()
    {
      this.animator = this.GetComponent<SplineAnimate>();
    }

    void OnEnable()
    {
      this.Id = NEXT_ID++;
    }

    public void StartMoveAlong(SplineContainer splineContainer)
    {
      this.animator.Container = splineContainer;
      this.animator.Play();
    }

    public void SetSpeed(float speed)
    {
      this.animator.MaxSpeed = speed;
      this.animator.Update();
    }

    public void Play()
    {
      this.animator.Play();
    }

    public void Pause()
    {
      this.animator.Pause();
    }

    public void SetOffset(float offset) 
    {
      this.animator.StartOffset = offset;
    }
  }
}
