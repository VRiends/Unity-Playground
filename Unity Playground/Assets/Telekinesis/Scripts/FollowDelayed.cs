using System;
using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis
{
    public class FollowDelayed : MonoBehaviour
    {
        public Transform TransformToFollow;
        public float Speed = 2f;
        public float RecalculateJourneyTime = 0.1f;

        private Vector3 targetPosition;
        private float startTime;
        private float journeyLength;

        private void Start()
        {
            PrepareJourney();
            InvokeRepeating("PrepareJourney", 0, RecalculateJourneyTime);
        }

        private void PrepareJourney()
        {
            startTime = Time.time;
            targetPosition = TransformToFollow.position;
            journeyLength = Vector3.Distance(transform.position, targetPosition);
        }
        
        private void Update()
        {
            if (TransformToFollow.position != transform.position)
            {
                DoJourney();
            }
        }

        private void DoJourney()
        {
            float distCovered = (Time.time - startTime) * Speed;

            float fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, targetPosition, fractionOfJourney);
        }
    }
}
