using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        public GameObject startScreen;

        public PlayerController player0;
        public PlayerController player1;

        public PlayerManager playerManager;

        public CardTooltip cardTooltip0;
        public CardTooltip cardTooltip1;

        public GameObject[] objectsToBeTurnedOff;

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        public void Reload() { Application.LoadLevel(0); }

        public void AI_AI()
        {
            player0.playerType = PlayerType.AI;
            player0.Launch();

            player1.playerType = PlayerType.AI;
            player1.Launch();

            playerManager.Launch();

            cardTooltip0.Launch();
            cardTooltip1.Launch();

            startScreen.SetActive(false);
        }

        public void AI_AI_Fast()
        {
            TurnOffObjects();

            player0.playerType = PlayerType.AI;
            player0.Launch();

            player1.playerType = PlayerType.AI;
            player1.Launch();

            playerManager.useGraphics = false;
            playerManager.Launch();

            startScreen.SetActive(false);
        }

        public void AI_Human()
        {
            player0.playerType = PlayerType.Human;
            player0.Launch();

            player1.playerType = PlayerType.AI;
            player1.Launch();

            playerManager.Launch();

            cardTooltip0.Launch();
            cardTooltip1.Launch();

            startScreen.SetActive(false);
        }

        void TurnOffObjects()
        {
            for (int i = 0; i < objectsToBeTurnedOff.Length; i++)
            {
                objectsToBeTurnedOff[i].SetActive(false);
            }
        }
    }
}