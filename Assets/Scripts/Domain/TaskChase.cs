using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using Domain;

public class TaskChase : Node
{
    AIIndividual unit;

    public TaskChase(AIIndividual aIIndividual)
    {
        unit = aIIndividual;
    }

    public override NodeState Evaluate()
    {
        unit.requestBehavior = AIIndividual.EBehaviorType.Chase;

        state = NodeState.RUNNING;
        return state;
    }

}
