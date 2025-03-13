using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AI;

namespace Domain
{
    public class AIIndividual : MonoBehaviour
    {
        protected GameManager gameManager;

        public AISteering steering;

        public enum EBehaviorType { Idle, Wander, Flee, Chase }
        public EBehaviorType behavior;
        public EBehaviorType requestBehavior;
        public EBehaviorType initialBehavior = EBehaviorType.Wander;

        public AIGroup group;

        public float rangePerception = 5.0f;
        public float rangePerceptionFlee = 7.5f;
        public float rangeDefeat = 6.0f;

        public bool isDefeated = false;

        public float closeEnemyDistance = 99999.0f;
        public AIIndividual closeEnemyUnit = null;

        public float closeTargetDistance = 99999.0f;
        public AIIndividual closeTargetUnit = null;

        public float closeFriendDistance = 99999.0f;
        public AIIndividual closeFriendUnit = null;

        public AIIndividual targetUnit = null;

        public float boostTimer = 0;
        public float depletedTimer = 0;

        public Vector3 deathCenter = Vector3.zero;
        public float deathRadius = 75.0f;

        protected CollisionAvoidance avoidance;
        protected Wander wander;
        protected Seek seek;

        protected Color debugColor;

        protected GameObject[] obstacles;

        protected UnitBT tree;

        void Start()
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            isDefeated = false;

            behavior = EBehaviorType.Idle;
            EnterIdle();

            requestBehavior = initialBehavior;

            obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        }


        private void Update()
        {
            if (!gameManager.isGameStarted || gameManager.isGamePaused || gameManager.isGameOver)
			{
                steering.enabled = false;
                return;
			}

            if (isDefeated)
            {
                targetUnit = null;
                steering.trackedTarget = null;
                return;
            }

            steering.enabled = true;

            UpdatePerception();
            UpdateDecision();
            UpdateBehaviour();
            UpdateRadius();
        }

        private void UpdatePerception()
        {
            if (targetUnit && targetUnit.isDefeated)
			{
                targetUnit = null;
			}

            // Enemy
            closeEnemyDistance = 999999.0f;
            closeEnemyUnit = null;
            foreach (AIIndividual unit in group.enemyUnits)
			{
                if (unit.isDefeated)
				{
                    continue;
				}

                float distance = (transform.position - unit.transform.position).magnitude;
                if (distance <= rangePerception && distance < closeEnemyDistance)
				{
                    closeEnemyDistance = distance;
                    closeEnemyUnit = unit;
				}
            }

            // Target
            closeTargetDistance = 99999.0f;
            closeTargetUnit = null;
            foreach (AIIndividual unit in group.targetUnits)
            {
                if (unit.isDefeated)
                {
                    continue;
                }

                float distance = (transform.position - unit.transform.position).magnitude;
                if (distance <= rangePerception && distance < closeTargetDistance)
                {
                    closeTargetDistance = distance;
                    closeTargetUnit = unit;
                }
            }

            // Target
            closeFriendDistance = 99999.0f;
            closeFriendUnit = null;
            foreach (AIIndividual unit in group.targetUnits)
            {
                if (unit.isDefeated)
                {
                    continue;
                }

                float distance = (transform.position - unit.transform.position).magnitude;
                if (distance <= rangePerception && distance < closeFriendDistance)
                {
                    closeFriendDistance = distance;
                    closeFriendUnit = unit;
                }
            }
        }

        private void UpdateDecision()
        {

            if (targetUnit && targetUnit.isDefeated)
			{
                targetUnit = null;
                requestBehavior = EBehaviorType.Wander;
            }

            // If there are no enemies around the unit within 5m, the speed of the unit will be set to aggressiveness, which GroupAI assigns.
            if (closeEnemyDistance >= 5.0f)
			{
                steering.maxSpeed = Mathf.Min(group.aggressiveness, group.maxSpeed);
            }

            // The unit will find the closest target.
            // The unit will chase the closest target to catch it.
            if (closeTargetDistance < 7.5f && behavior != EBehaviorType.Chase)
            {
                targetUnit = closeTargetUnit;
                requestBehavior = EBehaviorType.Chase;
            }

            // If there is an enemy in close proximity(less than 5 m), the speed of the unit will be set to the current aggressiveness speed + 2 for 2 seconds.This is called the boost speed.
            if (closeEnemyDistance < 5.0f && behavior != EBehaviorType.Flee)
			{
                boostTimer = 2.0f;

                // The unit will start fleeing.
                targetUnit = closeEnemyUnit;
                requestBehavior = EBehaviorType.Flee;
            }

            // New: If their is a nearby friend with a target
            if (closeFriendDistance < 10.0f && behavior == EBehaviorType.Wander 
                && requestBehavior != EBehaviorType.Flee && requestBehavior != EBehaviorType.Chase
                && closeFriendUnit.targetUnit != null)
            {
                // The unit will start fleeing.
                targetUnit = closeFriendUnit.targetUnit;
                requestBehavior = EBehaviorType.Chase;
            }

            // When 2 seconds ends, the speed of the unit returns to its aggressiveness speed.
            // The unit will continue fleeing if the enemy is still less than 7.5 m away.
            if (behavior == EBehaviorType.Flee)
			{
                targetUnit = closeEnemyUnit;

                float boostSpeed = 0.0f;
                if (boostTimer > 0.0f)
				{
                    boostTimer -= Time.deltaTime;
                    boostSpeed = 2.0f;
                }
				else 
                {
                    boostTimer = 0.0f;
                    depletedTimer = 2.0f;
                }

                // The unit can get this speed increase(boost) again after 2 seconds.
                if (depletedTimer > 0.0f)
                {
                    depletedTimer -= Time.deltaTime;
                }
                else
                {
                    boostTimer = 2.0f;
                    depletedTimer = 0.0f;
                }

                steering.maxSpeed = Mathf.Min(group.aggressiveness + boostSpeed, group.maxSpeed);

                // If the enemy is more than 7.5 m away, the unit stops fleeing and returns to chasing the closest enemy.
                if (closeEnemyDistance >= 7.5f)
				{
                    boostTimer = 0.0f;
                    depletedTimer = 0.0f;
                    targetUnit = null;
                    requestBehavior = EBehaviorType.Wander;
                }
            }

            // Similarly, the enemy stops chasing the unit if the distance between the enemy unit is more than 7.5 m.In this case, the enemy must find the other closest target.
            // If you want, you can also add strategies or tactics to the individual AI to create a more exciting game.
            // The unit will avoid colliding with obstacles(see below).
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

        private void UpdateRadius()
        {
            Vector3 pos2D = transform.position;
            float distance = (transform.position - deathCenter).magnitude;
            if (distance > deathRadius)
			{
                Defeat();
			}
        }

        public void SetSpeed(float speed)
        {
            steering.maxSpeed = speed;
        }

        public void GameOver()
		{
            requestBehavior = EBehaviorType.Idle;
            UpdateBehaviour();
		}

        public void Defeat()
		{
            isDefeated = true;

            transform.position += new Vector3(0.0f, -100.0f, 0.0f);

            requestBehavior = EBehaviorType.Idle;
            UpdateBehaviour();
		}

        public void SetDeathRadius(Vector3 center, float radius)
		{
            deathCenter = center;
            deathRadius = radius;
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

            wander = new Wander();
            steering.movements.Add(wander);
            steering.movements.Add(new LookWhereYouAreGoing());

            seek = new Seek();
            seek.weight = 0.0f;
            steering.movements.Add(seek);

            CollisionAvoidance avoidance = new CollisionAvoidance();
            avoidance.weight = 2.0f;
            foreach (GameObject o in obstacles)
            {
                avoidance.avoidList.Add(o.GetComponent<AISteering>());
            }
            steering.movements.Add(avoidance);

        }
        protected void ExitWander()
        {
            steering.movements.Clear();
            steering.Velocity = Vector3.zero;
        }
        virtual protected void UpdateWander()
        {
            float distance = (transform.position - deathCenter).magnitude;
            float clampDistance = Mathf.Clamp(distance, deathRadius - 10.0f, deathRadius);
            float distanceRatio = (clampDistance - (deathRadius - 10.0f)) / 10.0f;
            seek.weight = distanceRatio * 2.0f;

            if (seek.weight > 0.5f)
            {
                wander.lastWanderDirection = (deathCenter - transform.position).normalized;
            }
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

            seek = new Seek();
            seek.weight = 0.0f;
            steering.movements.Add(seek);
        }
        protected void ExitFlee()
        {
            steering.movements.Clear();
            steering.Velocity = Vector3.zero;
        }
        virtual protected void UpdateFlee()
        {
            float distance = (transform.position - deathCenter).magnitude;
            float clampDistance = Mathf.Clamp(distance, deathRadius - 10.0f, deathRadius);
            float distanceRatio = (clampDistance - (deathRadius - 5.0f)) / 10.0f;
            seek.weight = distanceRatio * 2.0f;
        }

        // Idle //
        protected void EnterChase()
        {
            debugColor = Color.red;

            if (targetUnit != null)
            {
                steering.trackedTarget = targetUnit.GetComponent<AISteering>();
            }

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

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Obstacle")
			{
                Defeat();
            }

            AIIndividual unit = collision.gameObject.GetComponent<AIIndividual>();
            if (unit && group.targetUnits.Contains(unit))
            {
                unit.Defeat();
                if (behavior == EBehaviorType.Chase)
                {
                    requestBehavior = EBehaviorType.Wander;
				}
            }
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