using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Domain
{
    public enum FactionType
	{
        Rock = 0,
        Paper,
        Scissors,
        Lizard,
        Spock
	}

    public enum FactionResultType
    {
        Tie = 0,
        Wins
    }

    [CreateAssetMenu(fileName = "FactionRelation", menuName = "ScriptableObjects/FactionRelation", order = 1)]
    public class FactionRelation : ScriptableObject
    {
        public FactionType factionA;
        public FactionType factionB;
        public FactionResultType result;

        public override string ToString()
		{
            return factionA.ToString() + " " + factionB.ToString() + " " + result.ToString();
		}
    }
}