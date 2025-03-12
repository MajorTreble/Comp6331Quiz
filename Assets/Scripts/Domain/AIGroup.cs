using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Domain
{
    public class AIGroup : MonoBehaviour
    {
        protected GameManager gameManager;

        public List<AIIndividual> friendUnits;
        public List<AIIndividual> enemyUnits;
        public List<AIIndividual> targetUnits;

        public float calmSpeed = 2.0f;
        public float averageSpeed = 6.0f;
        public float aggressiveSpeed = 10.0f;
        public float maxSpeed = 10.0f;

        public AnimationCurve friendLowUnitCurve;
        public AnimationCurve friendHighUnitCurve;
        public AnimationCurve enemyLowUnitCurve;
        public AnimationCurve enemyHighUnitCurve;
        public AnimationCurve targetLowUnitCurve;
        public AnimationCurve targetHighUnitCurve;

        public float aggressiveness;

        public void Awake()
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            enemyUnits = new List<AIIndividual>();
            targetUnits = new List<AIIndividual>();
        }

        public void Start()
        {
            string factionStr = gameObject.tag.Remove(gameObject.tag.Length - 1);

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
            if (gameManager.isGameStarted && !(gameManager.isGamePaused || gameManager.isGameOver))
			{
                UpdateAgressiveness();
            }
        }

        public int GetRemaining()
		{
            return friendUnits.Where(unit => !unit.isDefeated).Count();
        }

        protected void UpdateAgressiveness()
        {
            int numFriendlyUnit = friendUnits.Where(unit => !unit.isDefeated).Count();
            int numEnemyUnit = enemyUnits.Where(unit => !unit.isDefeated).Count();
            int numTargetUnit = targetUnits.Where(unit => !unit.isDefeated).Count();

            // Step 1 Fuzzification

            float friendlyLowUnit = friendLowUnitCurve.Evaluate(numFriendlyUnit);
            float friendlyHightUnit = friendHighUnitCurve.Evaluate(numFriendlyUnit);
            float enemyLowUnit = enemyLowUnitCurve.Evaluate(numEnemyUnit);
            float enemyHightUnit = enemyHighUnitCurve.Evaluate(numEnemyUnit);
            float targetLowUnit = targetLowUnitCurve.Evaluate(numTargetUnit);
            float targetHightUnit = targetHighUnitCurve.Evaluate(numTargetUnit);

            // Step 2 Fuzzy Rule Base

            //If(High#EnemyUnits AND High#FriendlyUnits) then average speed
            float averageRule1 = Mathf.Min(enemyHightUnit, friendlyHightUnit);

            //If(High#EnemyUnits AND Low#FriendlyUnits) then aggressive
            float aggessiveRule1 = Mathf.Min(enemyHightUnit, friendlyLowUnit);

            //If(Low#EnemyUnits OR High#TargetUnits) then average speed
            float averageRule2 = Mathf.Max(enemyLowUnit, targetHightUnit);

            //If(Low#EnemyUnits OR Low#TargetUnits) then move calmly
            float calmRule1 = Mathf.Max(enemyLowUnit, targetLowUnit);

            //If(High#TargetUnits OR High#FriendlyUnits) AND NOT High#EnemyUnits then aggressive
            float aggessiveRule2 = Mathf.Min(Mathf.Max(targetHightUnit, friendlyHightUnit), 1.0f - enemyHightUnit);

            //If(Low#TargetUnits OR High#FriendlyUnits) AND NOT High#EnemyUnits then average speed
            float averageRule3 = Mathf.Min(Mathf.Max(targetLowUnit, friendlyHightUnit), 1.0f - enemyHightUnit);

            //If(Low#TargetUnits OR Low#FriendlyUnits) OR Low#EnemyUnits then move calmly
            float calmRule2 = Mathf.Max(Mathf.Max(targetLowUnit, friendlyLowUnit), enemyLowUnit);

            float aggressiveDegree = Mathf.Max(aggessiveRule1, aggessiveRule2);
            float averageDegree = Mathf.Max(averageRule1, averageRule2, averageRule3);
            float calmDegree = Mathf.Max(calmRule1, calmRule2);

            // Step 3 Defuzzification

            aggressiveness = (aggressiveDegree * aggressiveSpeed
                + averageDegree * averageSpeed 
                + calmDegree * calmSpeed) 
                / (aggressiveDegree + averageDegree + calmDegree);
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

        public void SetDeathRadius(Vector3 center, float radius)
        {
            foreach (AIIndividual unit in friendUnits)
            {
                unit.SetDeathRadius(center, radius);
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