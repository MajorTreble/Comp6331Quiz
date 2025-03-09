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

        string factionStr;

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

        public void DebugDraw()
		{
            foreach(AIIndividual unit in friendUnits)
			{
                unit.DebugDraw();
			}
		}
    }
}