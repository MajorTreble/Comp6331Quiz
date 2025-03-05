using UnityEngine;

namespace AI
{
    public class Flee : AIMovement
    {
        public override SteeringOutput GetKinematic(AISteering agent)
        {
            var output = base.GetKinematic(agent);

            // TODO: calculate linear component
            Vector3 desiredVelocity = agent.transform.position - agent.TargetPosition;
            desiredVelocity.y = 0;
            desiredVelocity = desiredVelocity.normalized * agent.maxSpeed;
            output.linear = desiredVelocity;

            return output;
        }

        public override SteeringOutput GetSteering(AISteering agent)
        {
            var output = base.GetSteering(agent);

            // TODO: calculate linear component
            output.linear = GetKinematic(agent).linear - agent.Velocity;

            return output;
        }
    }
}
