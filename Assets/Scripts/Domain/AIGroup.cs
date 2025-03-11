using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Domain
{
    public class AIGroup : MonoBehaviour
    {
        protected GameObject gameManager;

        public List<AIIndividual> friendUnits;

        public List<AIIndividual> enemyUnits;
        public List<AIIndividual> targetUnits;

        public float speedCalm = 2.0f;
        public float speedAverage = 6.0f;
        public float speedAggressive = 10.0f;

        public enum EAgressivenessType { Calm, Average, Aggressive };

        protected string factionStr;

        void Start()
        {
            gameManager = GameObject.FindWithTag("GameManager");

            enemyUnits = new List<AIIndividual>();
            targetUnits = new List<AIIndividual>();

            factionStr = gameObject.tag.Remove(gameObject.tag.Length - 1);

            foreach (FactionRelation relation in gameManager.GetComponent<GameManager>().relationship)
            {
                if (relation.factionA.ToString() == factionStr)
                {
                    GameObject[] list = GameObject.FindGameObjectsWithTag(relation.factionB.ToString());
                    foreach(GameObject go in list)
					{
                        targetUnits.Add(go.GetComponent<AIIndividual>());
					}
                }
                else if (relation.factionB.ToString() == factionStr)
                {
                    GameObject[] list = GameObject.FindGameObjectsWithTag(relation.factionA.ToString());
                    foreach (GameObject go in list)
                    {
                        enemyUnits.Add(go.GetComponent<AIIndividual>());
                    }
                }
            }
        }

        public void Update()
		{
            UpdateAgressiveness();
		}

        protected void UpdateAgressiveness()
        {
        }

        public bool IsDefeated()
		{
            bool isAlive = false;
            foreach (AIIndividual unit in friendUnits)
            {
                if (!unit.isDefeated)
                {
                    isAlive = true;
                }
            }
            return !isAlive;
        }

        public void GameOver()
		{
            foreach (AIIndividual unit in friendUnits)
			{
                unit.GameOver();
			}
        }

        protected void SetAgressiveness(EAgressivenessType type)
		{
            float speed = 0.0f;
            switch (type)
			{
                case EAgressivenessType.Calm:
                    speed = speedCalm;
                    break;
                case EAgressivenessType.Aggressive:
                    speed = speedAggressive;
                    break;
                case EAgressivenessType.Average:
                    speed = speedAverage;
                    break;
			}
		}

        public void DebugDraw()
		{
            foreach(AIIndividual unit in friendUnits)
			{
                unit.DebugDraw();
			}
		}
    }
}