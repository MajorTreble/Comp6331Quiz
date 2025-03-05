using UnityEngine;

namespace AI
{
    public class Arrive : AIMovement
    {
        public float slowRadius = 10;
        public float stopRadius = 2;

        private void DrawDebug(AISteering agent)
        {
            if (debug)
            {
                DebugUtil.DrawCircle(agent.TargetPosition, agent.transform.up, Color.yellow, stopRadius);
                DebugUtil.DrawCircle(agent.TargetPosition, agent.transform.up, Color.magenta, slowRadius);
            }
        }

        public override SteeringOutput GetKinematic(AISteering agent)
        {
            DrawDebug(agent);

            var output = base.GetKinematic(agent);

            // TODO: calculate linear component
            Vector3 desiredVelocity = agent.TargetPosition - agent.transform.position;
            float distance = desiredVelocity.magnitude;
            desiredVelocity.y = 0;
            desiredVelocity = desiredVelocity.normalized * agent.maxSpeed;

            if (distance <= stopRadius)
                desiredVelocity *= 0;
            else if (distance < slowRadius)
                desiredVelocity *= (distance / slowRadius);

            output.linear = desiredVelocity;

            if (debug) Debug.DrawRay(agent.transform.position, output.linear, Color.blue);

            return output;
        }

        public override SteeringOutput GetSteering(AISteering agent)
        {
            DrawDebug(agent);

            var output = base.GetSteering(agent);

            // TODO: calculate linear component
            output.linear = GetKinematic(agent).linear - agent.Velocity;

            //Vector3 acceleration = steeringForce;
            //Velocity += acceleration * Time.deltaTime;
            //Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            if (debug) Debug.DrawRay(agent.transform.position + agent.Velocity, output.linear, Color.cyan);

            return output;
        }
    }
}
