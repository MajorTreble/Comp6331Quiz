using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AISteering : MonoBehaviour
    {
        //public Rigidbody rigidbody;

        public float initialMaxSpeed;
        public float maxSpeed;
        public float rotationSpeed = 1.0f;
        public float radius = 1.0f;
        public bool lockY = true;
        public bool debug = true;

        public enum EBehaviorType { Kinematic, Steering }
        public EBehaviorType behaviorType = EBehaviorType.Steering;

        public List<AIMovement> movements;

        public AISteering trackedTarget;
        public Vector3 targetPosition;
        public Vector3 TargetPosition
        {
            get => trackedTarget != null ? trackedTarget.transform.position : targetPosition;
        }
        public Vector3 TargetForward
        {
            get => trackedTarget != null ? trackedTarget.transform.forward : Vector3.forward;
        }
        public Vector3 TargetVelocity
        {
            get
            {
                Vector3 v = Vector3.zero;
                if (trackedTarget != null)
                {
                    v = trackedTarget.Velocity;
                }

                return v;
            }
        }

        public Vector3 Velocity { get; set; }

        public void TrackTarget(AISteering target)
        {
            trackedTarget = target;
        }

        public void UnTrackTarget()
        {
            trackedTarget = null;
        }

		private void Awake()
		{
            movements = new List<AIMovement>();

        }

		private void Update()
        {
            if (!enabled)
			{
                return;
			}

            if (debug)
            {
                Debug.DrawRay(transform.position, Velocity, Color.magenta);
            }

            if (behaviorType == EBehaviorType.Kinematic)
            {
                // TODO: average all kinematic behaviors attached to this object to obtain the final kinematic output and then apply it
                Vector3 kinematicAvg;
                Quaternion rotation;
                GetKinematicAvg(out kinematicAvg, out rotation);

                Velocity = Vector3.ClampMagnitude(kinematicAvg, maxSpeed);

                transform.rotation = rotation;
            }
            else
            {
                // TODO: combine all steering behaviors attached to this object to obtain the final steering output and then apply it
                Vector3 steeringForce;
                Quaternion rotation;
                GetSteeringSum(out steeringForce, out rotation);
                Velocity += steeringForce * Time.deltaTime;
                Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

                //transform.rotation = transform.rotation * rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rotation, rotationSpeed * Time.deltaTime);
            }

            //transform.position += Velocity * Time.deltaTime;
            GetComponent<Rigidbody>().MovePosition(transform.position + Velocity * Time.deltaTime);
        }

        private void GetKinematicAvg(out Vector3 kinematicAvg, out Quaternion rotation)
        {
            kinematicAvg = Vector3.zero;
            Vector3 eulerAvg = Vector3.zero;

            int count = 0;
            foreach (AIMovement movement in movements)
            {
                kinematicAvg += movement.GetKinematic(this).linear;
                eulerAvg += movement.GetKinematic(this).angular.eulerAngles;

                ++count;
            }

            if (count > 0)
            {
                kinematicAvg /= count;
                eulerAvg /= count;
                rotation = Quaternion.Euler(eulerAvg);
            }
            else
            {
                kinematicAvg = Velocity;
                rotation = transform.rotation;
            }
        }

        private void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
        {
            steeringForceSum = Vector3.zero;
            rotation = Quaternion.identity;

            foreach (AIMovement movement in movements)
            {
                steeringForceSum += movement.GetSteering(this).linear * movement.weight;
                rotation *= movement.GetSteering(this).angular;
            }
        }
    }
}