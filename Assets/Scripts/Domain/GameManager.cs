using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Domain
{

    public class GameManager : MonoBehaviour
    {
        public List<FactionRelation> relationship;

        public GameObject groupPrefab;
        public GameObject unitPrefab;
        public List<Material> materials;

        public bool debug = false;

        public int numUnit = 5;
        public float rangeSpawn = 20.0f;

        protected List<AIGroup> groups;

        public void Awake()
		{
            groups = new List<AIGroup>();

            for (int i = 0; i < 5; ++i)
            {
                FactionType faction = (FactionType)(i % 5);

                Vector3 pos = Vector3.zero;
                GameObject instance = Instantiate(groupPrefab, pos, Quaternion.identity);
                instance.tag = faction.ToString() + "s";
                groups.Add(instance.GetComponent<AIGroup>());
            }

            for (int i=0; i<numUnit; ++i)
			{
                FactionType faction = (FactionType)(i % 5);
                int factionid = (int)faction;

                Vector3 pos = new Vector3(Random.Range(-rangeSpawn, rangeSpawn), 0.0f, Random.Range(-rangeSpawn, rangeSpawn));
                GameObject instance = Instantiate(unitPrefab, pos, Quaternion.identity);
                instance.tag = faction.ToString();

                instance.transform.Find("Cylinder").GetComponent<MeshRenderer>().material = materials[factionid];

                AIIndividual unit = instance.GetComponent<AIIndividual>();

                unit.group = groups[factionid];
                groups[factionid].friendUnits.Add(unit);
            }
		}

        public void Update()
		{
            if (debug)
			{
                foreach(AIGroup g in groups)
				{
                    g.DebugDraw();
				}
			}

            UpdateWinCondition();
		}

        protected void UpdateWinCondition()
		{
            foreach(AIGroup group in groups)
			{
                bool isAlive = false;
                foreach(AIIndividual unit in group.enemyUnits)
				{
                    if (!unit.isDefeated)
					{
                        isAlive = true;
					}
				}
                if (!isAlive)
				{
                    Debug.Log(group.tag + " Wins");
                }
            }
		}
    }
}