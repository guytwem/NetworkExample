using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlecars.Player
{
    [System.Serializable]
    public struct Axel
    {
        public WheelCollider left;
        public WheelCollider right;
        public bool isMotor;
    }

    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private List<Axel> axels = new List<Axel>();
        [SerializeField] private float maxMotorTorque; // maximum torque the motor can apply to wheel
        [SerializeField] private float acceleration;
        [SerializeField] private float maxSteeringAngle; // maximum steer angle the wheel can have

        [SerializeField] private bool isEnabled = false;

        public void Enable() => isEnabled = true;

        private void ApplyLocalPositionToVisuals(WheelCollider _wheel)
        {
            Transform child = _wheel.transform.GetChild(0);

            _wheel.GetWorldPose(out Vector3 position, out Quaternion rotation);

            child.position = position;
            child.rotation = rotation;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(!isEnabled)
                return;

            float motor = maxMotorTorque * Input.GetAxis("Vertical") * acceleration;
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

            foreach(Axel axel in axels)
            {
                if(axel.isMotor)
                {
                    axel.left.motorTorque = motor;
                    axel.right.motorTorque = motor;
                }
                else
                {
                    axel.left.steerAngle = steering;
                    axel.right.steerAngle = steering;
                }

                ApplyLocalPositionToVisuals(axel.left);
                ApplyLocalPositionToVisuals(axel.right);
            }
        }
    }   
}