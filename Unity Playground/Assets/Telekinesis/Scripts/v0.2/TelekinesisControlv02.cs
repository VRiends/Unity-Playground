using System;
using UnityEngine;

namespace Telekinesis
{
    public class TelekinesisControlv02 : MonoBehaviour
    {
        public Transform Controller;
        public float TranslationRatio = 10f;

        private Vector3 lastControllerPosition;
        private Vector3 controllerOffset = Vector3.zero;

        void Start()
        {
            SetPosition();
        }

        private void SetPosition()
        {
            transform.position += controllerOffset * TranslationRatio;
            transform.rotation = Controller.rotation;
            lastControllerPosition = Controller.position;
        }

        void Update()
        {
            CalculateControllerVelocity();
            SetPosition();
        }

        private void CalculateControllerVelocity()
        {
            controllerOffset = Controller.position - lastControllerPosition;
        }
    }
}