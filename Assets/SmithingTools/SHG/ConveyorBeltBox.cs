using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace SHG
{
  [RequireComponent(typeof(SplineAnimate))]
  public class ConveyorBeltBox : MonoBehaviour
  {
    SplineAnimate animator;

    void Awake()
    {
      this.animator = this.GetComponent<SplineAnimate>();
    }

    public void StartMoveAlong(SplineContainer splineContainer)
    {
      this.animator.Container = splineContainer;
      this.animator.Play();
    }

    public void SetSpeed(float speed)
    {
      this.animator.MaxSpeed = speed;
    }

    public void SetOffset(float offset) 
    {
      this.animator.StartOffset = offset;
    }
  }
}
