﻿using UnityEngine;

namespace AI
{
    public class Evade : AIMovement
    {
        public override SteeringOutput GetKinematic(AISteering agent)
        {
            var output = base.GetKinematic(agent);

            // TODO: calculate linear component
            Vector3 desiredVelocity = agent.transform.position - (TargetPosition(agent) + TargetVelocity(agent));
            desiredVelocity.y = 0;
            desiredVelocity = desiredVelocity.normalized * agent.maxSpeed;
            output.linear = desiredVelocity;

            if (debug) Debug.DrawRay(agent.transform.position, output.linear, Color.black);

            return output;
        }

        public override SteeringOutput GetSteering(AISteering agent)
        {
            var output = base.GetSteering(agent);

            // TODO: calculate linear component
            output.linear = GetKinematic(agent).linear - agent.Velocity;

            if (debug) Debug.DrawRay(agent.transform.position + agent.Velocity, output.linear, Color.grey);

            return output;
        }
    }
}
