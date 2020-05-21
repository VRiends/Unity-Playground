using UnityEngine;
using VRPlayground.Extensions;

namespace Telekinesis
{
    public class BeamManipulation : MonoBehaviour
    {
        public bool IsRightHand = true;
        public Transform Head;
        public Transform ObjectToManipulate;
        public AnimationCurve BeamLengthRatioCurve;

        private float beamLength = 0.25f;
        private float handHeadMaxDistance = 0.6f;
        private float handHeadMinDistance = 0.35f;

        private void Awake()
        {
            SetObjectTransform();
        }

        private void Update()
        {
            CalculateBeamLength();
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
            
            if (headDistance < handHeadMinDistance) headDistance = handHeadMinDistance;
            if (headDistance > handHeadMaxDistance) headDistance = handHeadMaxDistance;

            var mappedDistance = headDistance.Map(handHeadMinDistance, handHeadMaxDistance, 0, 1.0f);
            beamLength = BeamLengthRatioCurve.Evaluate(mappedDistance);
        }

        private void SetObjectTransform()
        {
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