using UnityEngine;

namespace AI
{
    public class FaceAway : AIMovement
    {
        public override SteeringOutput GetKinematic(AISteering agent)
        {
            var output = base.GetKinematic(agent);

            // TODO: calculate angular component
            Vector3 direction = agent.transform.position - agent.TargetPosition;

            if (direction.normalized == agent.transform.forward || Mathf.Approximately(direction.magnitude, 0))
            {
                output.angular = agent.transform.rotation;
            }
            else
            {
                output.angular = Quaternion.LookRotation(direction);
            }

            return output;
        }

        public override SteeringOutput GetSteering(AISteering agent)
        {
            var output = base.GetSteering(agent);

            // TODO: calculate angular component
            if (agent.lockY)
            {
                // get the rotation around the y-axis
                Vector3 from = Vector3.ProjectOnPlane(agent.transform.forward, Vector3.up);
                Vector3 to = GetKinematic(agent).angular * Vector3.forward;
                float angleY = Vector3.SignedAngle(from, to, Vector3.up);
                output.angular = Quaternion.AngleAxis(angleY, Vector3.up);
            }
            else
                output.angular = Quaternion.FromToRotation(agent.transform.forward, GetKinematic(agent).angular * Vector3.forward);

            return output;
        }
    }
}
