using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis
{
    public class ResetControlledObjectPosition : MonoBehaviour
    {
        public Transform ResetPoint;

        void Start()
        {
            Reset();
        }

        private void Reset()
        {
            transform.position = ResetPoint.position;
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