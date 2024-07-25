using UnityEngine;
using Zenject;

namespace Systems
{
    public class WorldUI : MonoBehaviour
    {
        [SerializeField] Camera worldUICamera;

        public Camera Camera => worldUICamera;

        private Camera mainCamera;
        private Transform mainCameraTransform;

        [Inject]
        private void Construct(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            this.mainCameraTransform = mainCamera.transform;
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                return;
            }

            if (mainCameraTransform.position != worldUICamera.transform.position)
            {
                worldUICamera.transform.position = mainCameraTransform.position;
            }

            if (mainCameraTransform.rotation != worldUICamera.transform.rotation)
            {
                worldUICamera.transform.rotation = mainCameraTransform.rotation;
            }
        }
    }
}