using System.Collections.Generic;

using BehaviorTree;
using Domain;

public class UnitBT : Tree
{
    public AIIndividual unit;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            //new Sequence(new List<Node>
            //{
            //}),
            //new TaskWander(unit)
        });

        return root;
    }
}
