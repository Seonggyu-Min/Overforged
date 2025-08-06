using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class CameraController : MonoBehaviour
    {
        private Transform _transform;
        private Vector3 _camPos;

        [SerializeField] private float _xPos = 4;
        [SerializeField] private float _zPos = 0;

        private void Update()
        {
            if (_transform == null) return;

            Vector3 camPos = new Vector3(_transform.position.x + _xPos, transform.position.y, _transform.position.z + _zPos);
            transform.position = camPos;
        }

        public void SetPlayer(Transform transform)
        {
            _transform = transform;
        }
    }
}
