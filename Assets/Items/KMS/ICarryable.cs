using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KMS
{
    public interface ICarryable
    {
        public void Carry(Transform playerHand);
    }
}
