using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public sealed class ConstantMotion : MonoBehaviour
    {
        public Vector3 velocity;
        public Vector3 rotation;

        public Vector3 acceleration;
        public Vector3 torque;

        Vector3 initialPosition;
        Vector3 initialRotation;

        Vector3 currentVelocity;
        Vector3 currentRotation;

        private void OnEnable()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles;

            currentVelocity = velocity;
            currentRotation = rotation;
        }

        private void OnDisable()
        {
            transform.SetLocalPositionAndRotation(initialPosition, Quaternion.Euler(initialRotation));
        }

        private void Update()
        {
            currentVelocity += acceleration * Time.deltaTime;
            currentRotation += torque * Time.deltaTime;

            transform.SetLocalPositionAndRotation(
                transform.localPosition += currentVelocity * Time.deltaTime,
                Quaternion.Euler(transform.localEulerAngles + currentRotation * Time.deltaTime)
            );
        }
    }
}
