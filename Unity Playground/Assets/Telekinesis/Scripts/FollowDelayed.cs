using System;
using System.Collections.Generic;
using UnityEngine;

namespace Telekinesis
{
    public class FollowDelayed : MonoBehaviour
    {
        public Transform TransformToFollow;
        public int QueueCapacity = 45;

        private Queue<Vector3> positionQueue;

        private void Awake()
        {
            positionQueue = new Queue<Vector3>(QueueCapacity);
        }

        private void Update()
        {
            if (positionQueue.Count == QueueCapacity)
            {
                transform.position = positionQueue.Dequeue();
            }
            positionQueue.Enqueue(TransformToFollow.position);
        }
    }
}
