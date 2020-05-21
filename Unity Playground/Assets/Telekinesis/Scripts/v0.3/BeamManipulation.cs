using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Telekinesis
{
    public class BeamManipulation : MonoBehaviour
    {
        public bool IsRightHand = true;
        public Transform Head;
        public Transform ObjectToManipulate;
        public float BeamMinLength = 0.25f;
        public float BeamMaxLength = 15.0f;
        public AnimationCurve BeamLengthRatioCurve;

        private Vector3 beamDirection;
        private float beamLengthRatio = 1.0f;
        private float beamLength = 0.25f;
        private float handHeadMaxDistance = 0.6f;
        private float handHeadMinDistance = 0.23f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + beamDirection);
        }

        private void Awake()
        {
            SetObjectTransform();
        }

        private void Update()
        {
            //CalculateBeamLength();
            SetObjectTransform();

            if (Input.GetKeyDown(KeyCode.M))
            {
                handHeadMaxDistance = CalculateHandHeadDistance();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                handHeadMinDistance = CalculateHandHeadDistance();
            }
        }

        private float CalculateHandHeadDistance()
        {
            return (transform.position - new Vector3(Head.position.x, transform.position.y, Head.position.z)).magnitude;
        }

        private void CalculateBeamLength()
        {
            float headDistance = CalculateHandHeadDistance();
            float clampedHeadDistance = Mathf.Lerp(BeamMinLength, BeamMaxLength, headDistance);
            beamLength = beamLength * clampedHeadDistance;
        }

        private void SetObjectTransform()
        {
            // ObjectToManipulate.transform.rotation = transform.rotation;
            if (IsRightHand)
            {
                ObjectToManipulate.transform.position = transform.position + transform.right * (-1 * beamLength);
            }
            else
            {
                ObjectToManipulate.transform.position = transform.position + transform.right * beamLength;
            }
        }
    }
}