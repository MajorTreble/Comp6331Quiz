using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using Domain;

public class TaskFlee : Node
{
    AIIndividual unit;

    public TaskFlee(AIIndividual aIIndividual)
    {
        unit = aIIndividual;
    }

    public override NodeState Evaluate()
    {
        unit.requestBehavior = AIIndividual.EBehaviorType.Flee;

        state = NodeState.RUNNING;
        return state;
    }

}
