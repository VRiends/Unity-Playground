using UnityEngine;

namespace Telekinesis
{
    public class TelekinesisControlv01 : MonoBehaviour
    {
        public Transform Controller;

        private Vector3 DistanceToController;
        
        void Start()
        {
            DistanceToController = Controller.position - transform.position;
            SetPosition();
        }

        private void SetPosition()
        {
            transform.position = Controller.position + DistanceToController;
            transform.rotation = Controller.rotation;
        }

        void Update()
        {
            SetPosition();
        }
    }
}