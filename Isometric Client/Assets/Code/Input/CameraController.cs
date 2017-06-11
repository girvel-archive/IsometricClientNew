using UnityEngine;

namespace Assets.Code.Input
{
    public class CameraController : MonoBehaviour
    {
        public float ScrollingFactor = 0.005f;
        public float ScalingFactor = 1.2f;

        public float ScrollingSpeedMax = 3f;

        public float ScaleMax = 12;
        public float ScaleMin = 2;

        public float CameraMovingSpeed = 3f;

        public static Vector3 TargetPosition { get; set; }

        protected Vector3 PreviousMousePosition;
        protected Camera ThisCamera;

        protected void Start()
        {
            TargetPosition = transform.position;

            ThisCamera = GetComponent<Camera>();

        }

        protected virtual void Update()
        {
            CameraControlUpdate();
            CameraMoving();
        }

        private void CameraMoving()
        {
            if (TargetPosition == transform.position)
                return;

            var path = TargetPosition - transform.position;
            if (path.magnitude <= CameraMovingSpeed * Time.deltaTime * ThisCamera.orthographicSize)
            {
                transform.position = TargetPosition;
            }
            else
            {
                transform.position += path.normalized * CameraMovingSpeed * Time.deltaTime * ThisCamera.orthographicSize;
            }
        }

        private void CameraControlUpdate()
        {
            if (UnityEngine.Input.GetMouseButton(2))
            {
                var path = PreviousMousePosition - UnityEngine.Input.mousePosition;

                transform.position +=
                    path.magnitude * ScrollingFactor * ThisCamera.orthographicSize > ScrollingSpeedMax
                        ? ScrollingSpeedMax * path.normalized
                        : path * ScrollingFactor * ThisCamera.orthographicSize;
                TargetPosition = transform.position;
            }

            if (UnityEngine.Input.mouseScrollDelta.y != 0)
            {
                ThisCamera.orthographicSize = Mathf.Clamp(
                    ThisCamera.orthographicSize *
                    (UnityEngine.Input.mouseScrollDelta.y < 0 ? ScalingFactor : 1 / ScalingFactor),
                    ScaleMin, ScaleMax);
            }

            PreviousMousePosition = UnityEngine.Input.mousePosition;
        }
    }
}