using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Domain;

namespace Controller
{
    public class MenuController : MonoBehaviour
    {

        private GameObject ui;
        private Button newGameButton;
        private Button pauseButton;
        private Button quitButton;

        public GameObject gameManagerGameObject;
        public GameManager gameManager;

        public List<string> textList;

        public void Start()
        {
            gameManager = gameManagerGameObject.GetComponent<GameManager>();

            ui = GameObject.Find("Canvas");

            if (ui)
            {
                GameObject newGameGO = ui.transform.Find("New Game").gameObject;
                if (newGameGO)
                {
                    newGameGO.SetActive(true);
                    newGameButton = newGameGO.GetComponent<Button>();
                }

                GameObject pauseGO = ui.transform.Find("Pause").gameObject;
                if (pauseGO)
                {
                    pauseGO.SetActive(false);
                    pauseButton = pauseGO.GetComponent<Button>();
                }

                GameObject quitGO = ui.transform.Find("Quit").gameObject;
                if (quitGO)
                {
                    quitGO.SetActive(false);
                    quitButton = quitGO.GetComponent<Button>();
                }

                foreach (string str in textList)
                {
                    GameObject go = ui.transform.Find(str).gameObject;
                    if (go)
                    {
                        go.SetActive(false);
                    }
                }

                GameObject winnerGO = ui.transform.Find("Winner").gameObject;
                if (winnerGO)
				{
                    winnerGO.SetActive(gameManager.isPlayerGame);
                    if (gameManager.isPlayerGame)
					{
                        winnerGO.GetComponent<Text>().text = "Place Units";
                    }
                }

                GameObject.Find("Capsule").GetComponent<CapsuleCollider>().enabled = true;
            }
        }

        public void StartGame()
        {
            newGameButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);

            foreach(string str in textList)
			{
                GameObject go = ui.transform.Find(str).gameObject;
                if (go)
				{
                    go.SetActive(true);
				}
            }

            GameObject winnerGO = ui.transform.Find("Winner").gameObject;
            if (winnerGO)
            {
                winnerGO.SetActive(false);
            }

            GameObject.Find("Capsule").GetComponent<CapsuleCollider>().enabled = false;

            gameManager.StartGame();
        }

        public void PauseGame()
        {
            gameManager.PauseGame();

            if (gameManager.isGamePaused)
			{
                pauseButton.transform.Find("Text").GetComponent<Text>().text = "Unpause";
            }
			else 
            {
                pauseButton.transform.Find("Text").GetComponent<Text>().text = "Pause";
            }
        }

        public void QuitGame()
        {
            newGameButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);

            pauseButton.transform.Find("Text").GetComponent<Text>().text = "Pause";

            foreach (string str in textList)
            {
                GameObject go = ui.transform.Find(str).gameObject;
                if (go)
                {
                    go.SetActive(false);
                }
            }


            GameObject winnerGO = ui.transform.Find("Winner").gameObject;
            if (winnerGO)
            {
                winnerGO.SetActive(true);
                winnerGO.GetComponent<Text>().text = "Place Units";
            }

            GameObject.Find("Capsule").GetComponent<CapsuleCollider>().enabled = true;

            gameManager.QuitGame();
        }

    }
}