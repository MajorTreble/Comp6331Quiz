using UnityEngine;

namespace AI
{
    public abstract class AIMovement
    {
        public bool debug;
        public float weight = 1.0f;

        public AISteering overrideTarget = null;

        public virtual SteeringOutput GetKinematic(AISteering agent)
        {
            return new SteeringOutput { angular = agent.transform.rotation };
        }

        public virtual SteeringOutput GetSteering(AISteering agent)
        {
            return new SteeringOutput { angular = Quaternion.identity };
        }

        public Vector3 TargetPosition(AISteering agent)
		{
            if (overrideTarget != null)
			{
                return overrideTarget.transform.position;

            }

            return agent.TargetPosition;
		}

        public Vector3 TargetVelocity(AISteering agent)
        {
            if (overrideTarget != null)
            {
                return overrideTarget.Velocity;

            }

            return agent.TargetVelocity;
        }
    }
}
