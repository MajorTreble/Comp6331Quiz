using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using Domain;

public class TaskWander : Node
{
    AIIndividual unit;

    public TaskWander(AIIndividual aIIndividual)
    {
        unit = aIIndividual;
    }

    public override NodeState Evaluate()
    {
        unit.requestBehavior = AIIndividual.EBehaviorType.Wander;

        state = NodeState.RUNNING;
        return state;
    }

}
