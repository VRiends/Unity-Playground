using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis
{
    public class ResetControlledObjectPosition : MonoBehaviour
    {
        public Transform ResetPoint;
        public Vector3 ResetPositionOffset = new Vector3(0, 0, 2f);

        void Start()
        {
            Reset();
        }

        private void Reset()
        {
            transform.position = ResetPoint.position + ResetPositionOffset;
            transform.rotation = ResetPoint.rotation;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        }
    }
}