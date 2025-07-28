using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ICarryable
{
    public void Go(Transform otherTrs);

    public void Come(Transform otherTrs, Transform myTrs);

    public void Abandon();

}
