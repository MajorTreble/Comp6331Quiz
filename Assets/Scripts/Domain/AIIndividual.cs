using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Domain
{
    public class AIIndividual : MonoBehaviour
    {
        public AISteering steering;

        public enum EBehaviorType { Idle, Wander, Flee, Chase }
        public EBehaviorType behavior;
        public EBehaviorType requestBehavior;
        public EBehaviorType initialBehavior = EBehaviorType.Wander;

        public AIGroup group;

        public float rangePerception = 30.0f;
        public float rangeDefeat = 6.0f;

        public bool isDefeated = false;
        protected bool isGameOver = false;

        protected AIIndividual targetUnit;

        protected Color debugColor;

        void Start()
        {
            isDefeated = false;
            isGameOver = false;

            behavior = EBehaviorType.Idle;
            EnterIdle();

            requestBehavior = initialBehavior;
        }


        private void Update()
        {
            UpdatePerception();
            UpdateBehaviour();
        }

        private void UpdatePerception()
        {
            targetUnit = null;

            if (isGameOver)
			{
                return;
			}

            // Flee first
            float closeDistance = 99999.0f;
            AIIndividual closeUnit = null;
            foreach(AIIndividual unit in group.enemyUnits)
			{
                if (!unit)
				{
                    Debug.Log("Null");
				}
                if (unit.isDefeated)
				{
                    continue;
				}

                float distance = (transform.position - unit.transform.position).magnitude;
                if (false && distance <= rangePerception && distance < closeDistance)
				{
                    closeDistance = distance;
                    closeUnit = unit;
				}
            }

            if (closeUnit != null)
			{
                targetUnit = closeUnit;
                requestBehavior = EBehaviorType.Flee; 
			}
            else if (behavior == EBehaviorType.Flee)
			{
                targetUnit = null;
                requestBehavior = EBehaviorType.Wander;
			}

            // Pursue
            closeDistance = 99999.0f;
            closeUnit = null;
            foreach (AIIndividual unit in group.targetUnits)
            {
                if (unit.isDefeated)
                {
                    continue;
                }

                float distance = (transform.position - unit.transform.position).magnitude;
                if (distance <= rangePerception && distance < closeDistance)
                {
                    closeDistance = distance;
                    closeUnit = unit;
                }
            }

            if (closeUnit != null)
            {
                if (closeDistance < rangeDefeat) 
                {
                    closeUnit.Defeat();
                    return;
                }

                if (requestBehavior != EBehaviorType.Flee)
				{
                    targetUnit = closeUnit;
                    requestBehavior = EBehaviorType.Chase;
                }
            }
            else if (behavior == EBehaviorType.Chase)
            {
                targetUnit = null;
                requestBehavior = EBehaviorType.Wander;
            }
        }

        private void UpdateBehaviour()
        { 
            switch (behavior)
            {
                case EBehaviorType.Idle:
                    UpdateIdle();
                    break;
                case EBehaviorType.Wander:
                    UpdateWander();
                    break;
                case EBehaviorType.Flee:
                    UpdateFlee();
                    break;
                case EBehaviorType.Chase:
                    UpdateChase();
                    break;
            }

            if (requestBehavior != behavior)
            {
                switch (behavior)
                {
                    case EBehaviorType.Idle:
                        ExitIdle();
                        break;
                    case EBehaviorType.Wander:
                        ExitWander();
                        break;
                    case EBehaviorType.Flee:
                        ExitFlee();
                        break;
                    case EBehaviorType.Chase:
                        ExitChase();
                        break;
                }

                switch (requestBehavior)
                {
                    case EBehaviorType.Idle:
                        EnterIdle();
                        break;
                    case EBehaviorType.Wander:
                        EnterWander();
                        break;
                    case EBehaviorType.Flee:
                        EnterFlee();
                        break;
                    case EBehaviorType.Chase:
                        EnterChase();
                        break;
                }

                behavior = requestBehavior;
            }

        }

        public void SetSpeed(float speed)
        {
            steering.maxSpeed = speed;
        }

        public void GameOver()
		{
            isGameOver = true;

            requestBehavior = EBehaviorType.Idle;
		}

        public void Defeat()
		{
            isDefeated = true;

            transform.position += new Vector3(0.0f, -100.0f, 0.0f);

            requestBehavior = EBehaviorType.Idle;
		}

        // Idle //
        protected void EnterIdle()
        {
            debugColor = Color.grey;

            steering.Velocity = Vector3.zero;
        }
        protected void ExitIdle()
        {

        }
        virtual protected void UpdateIdle()
        {
        }

        // Wander //
        protected void EnterWander()
        {
            debugColor = Color.green;

            steering.movements.Add(new Wander());
            steering.movements.Add(new LookWhereYouAreGoing());
        }
        protected void ExitWander()
        {
            steering.movements.Clear();
            steering.Velocity = Vector3.zero;
        }
        virtual protected void UpdateWander()
        {
        }

        // Idle //
        protected void EnterFlee()
        {
            debugColor = Color.blue;

            if (targetUnit != null)
			{
                steering.trackedTarget = targetUnit.GetComponent<AISteering>();
            }

            //steering.movements.Add(new Flee());
            steering.movements.Add(new Evade());
        }
        protected void ExitFlee()
        {
            steering.movements.Clear();
            steering.Velocity = Vector3.zero;
        }
        virtual protected void UpdateFlee()
        {
        }

        // Idle //
        protected void EnterChase()
        {
            debugColor = Color.red;
        
            steering.movements.Add(new Pursue());
            steering.movements.Add(new LookWhereYouAreGoing());
        }
        protected void ExitChase()
        {
            steering.movements.Clear();
            steering.Velocity = Vector3.zero;
        }
        virtual protected void UpdateChase()
        {
        }


        public void DebugDraw()
        {
            
            Vector3 debugPos = transform.position;
            debugPos.y += 3.0f;
            DebugUtil.DrawWireSphere(debugPos, debugColor, 1.0f);

            if (targetUnit != null)
            {
                Vector3 portPos = targetUnit.transform.position;
                DebugUtil.DrawWireSphere(portPos, debugColor, 1.0f);
                Debug.DrawRay(debugPos, portPos - debugPos, debugColor);
            }
        }

    }
}