using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Domain
{

    public class GameManager : MonoBehaviour
    {
        public List<FactionRelation> relationship;

        public GameObject groupPrefab;
        public GameObject unitPrefab;
        public List<Material> materials;
        public List<GameObject> obstaclePrefabs;

        public GameObject platform;

        public bool debug = false;

        public int numUnit = 5;
        public float rangeSpawn = 65.0f;
        public float rangeObstacleSpawn = 50.0f;

        public float platformRadius = 75.0f;

        public float reduceDistance = 10.0f;
        public float reduceDelay = 10.0f;
        public int reduceNumber = 3;

        public float reduceTimer = 0.0f;
        public int reduceCount = 0;

        public List<string> textList;

        protected Vector3 zeroY = new Vector3(1, 0, 1);

        protected List<AIGroup> groups;

        public bool isGameStarted = false;
        public bool isGamePaused = false;
        public bool isGameOver = false;

        public bool isPlayerGame = false;

        private GameObject ui;
        private Text winnerText;

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

            SpawnObstacles();
        }

        public void Start()
		{
            ui = GameObject.Find("Canvas");

            GameObject winnerGO = ui.transform.Find("Winner").gameObject;
            if (winnerGO)
            {
                winnerGO.SetActive(true);
                winnerText = winnerGO.GetComponent<Text>();
            }
        }

        public void Update()
		{
            if (!isGameStarted || isGamePaused || isGameOver)
			{
                if (!isGameStarted && groups[0].friendUnits.Count < 10 && Input.GetMouseButtonDown(2))
                {
                    Vector3 pos = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(pos);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        GameObject instance = Instantiate(unitPrefab, hit.point, Quaternion.identity);
                        instance.tag = "Rock";

                        instance.transform.Find("Cylinder").GetComponent<MeshRenderer>().material = materials[0];

                        AIIndividual unit = instance.GetComponent<AIIndividual>();

                        unit.group = groups[0];
                        groups[0].friendUnits.Add(unit);
                    }
                }
                return;
			}

            if (debug)
			{
                foreach(AIGroup g in groups)
				{
                    g.DebugDraw();
				}
			}

            for (int i = 0; i < 5; ++i)
            {
                FactionType faction = (FactionType)(i % 5);
                int factionid = (int)faction;

                GameObject go = ui.transform.Find(faction.ToString() + " Count").gameObject;
                if (go)
                {
                    go.GetComponent<Text>().text = groups[factionid].GetRemaining().ToString();
                }
            }

            UpdateWinCondition();
            UpdateReduceCircle();
        }

        protected void UpdateWinCondition()
		{
            if (isGameOver)
			{
                return;
			}

            foreach(AIGroup group in groups)
			{
                if (group.IsDefeated())
				{
                    GameOver(group);

                    GameObject winnerGO = ui.transform.Find("Winner").gameObject;
                    if (winnerGO)
                    {
                        winnerGO.SetActive(true);
                        winnerGO.GetComponent<Text>().text = group.tag.Remove(group.tag.Length - 1) + " Loses";
                    }

                    return;
                }
            }
		}

        protected void UpdateReduceCircle()
        {
            if (reduceCount >= reduceNumber)
			{
                return;
			}

            reduceTimer -= Time.deltaTime;

            if (reduceTimer <= 0.0f)
			{
                reduceTimer = reduceDelay;
                reduceCount++;

                SetGameRadius();
            }
        }

        public void OnMouseDown()
        {
            // Code here is called when the GameObject is clicked on.
        }

        void SpawnObstacles()
		{
            for (int i = 0; i < 10; ++i)
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count - 1)];
                Vector3 pos = new Vector3(Random.Range(-rangeObstacleSpawn, rangeObstacleSpawn), 0.0f, Random.Range(-rangeObstacleSpawn, rangeObstacleSpawn));

                GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
            }
        }

        void SpawnUnits()
		{

            for (int i = 0; i < numUnit; ++i)
            {
                FactionType faction = (FactionType)(i % 5);
                int factionid = (int)faction;

                if (isPlayerGame && factionid == 0)
				{
                    continue;
				}

                Vector3 pos = new Vector3(Random.Range(-rangeSpawn, rangeSpawn), 0.0f, Random.Range(-rangeSpawn, rangeSpawn));
                GameObject instance = Instantiate(unitPrefab, pos, Quaternion.identity);
                instance.tag = faction.ToString();

                instance.transform.Find("Cylinder").GetComponent<MeshRenderer>().material = materials[factionid];

                AIIndividual unit = instance.GetComponent<AIIndividual>();

                unit.group = groups[factionid];
                groups[factionid].friendUnits.Add(unit);
            }
        }

        protected void SetGameRadius()
		{
            float radius = platformRadius - reduceCount * reduceDistance;

            platform.transform.localScale = new Vector3(radius * 2.0f, 1.0f, radius * 2.0f);
            foreach (AIGroup group in groups)
            {
                group.SetDeathRadius(Vector3.Scale(platform.transform.position, zeroY), radius);
            }
        }

        public void StartGame()
        {
            isGameStarted = true;
            isGameOver = false;

            reduceTimer = reduceDelay;
            reduceCount = 0;

            SetGameRadius();
            SpawnUnits();

            foreach(AIIndividual unit in groups[0].friendUnits)
			{
                unit.transform.position = new Vector3(unit.transform.position.x, 0.0f, unit.transform.position.z);
			}

            foreach (AIGroup g in groups)
            {
                g.Start();
            }
        }

        public void PauseGame()
        {
            isGamePaused = !isGamePaused;
        }

        public void QuitGame()
        {
            isGameStarted = false;
            isGameOver = false;

            reduceTimer = reduceDelay;
            reduceCount = 0;

            SetGameRadius();
            for (int i = 0; i < numUnit; ++i)
            {
                FactionType faction = (FactionType)(i % 5);
                int factionid = (int)faction;

                foreach (AIIndividual unit in groups[factionid].friendUnits)
				{
                    Destroy(unit.gameObject);
				}

                groups[factionid].friendUnits.Clear();
                groups[factionid].enemyUnits.Clear();
                groups[factionid].targetUnits.Clear();
            }

            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach(GameObject go in obstacles)
			{
                Destroy(go);
			}

            SpawnObstacles();
        }

        protected void GameOver(AIGroup lostGroup)
		{
            isGameOver = true;

            Debug.Log(lostGroup.tag + " Loses");

            foreach (AIGroup group in groups)
            {
                group.GameOver();
            }
        }
    }
}