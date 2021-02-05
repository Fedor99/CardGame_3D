using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    public class PlayerManager : MonoBehaviour
    {
        private bool launched = false;

        public bool useGraphics;

        public TextMeshProUGUI victoryText;
        public GameObject victoryUI;

        public TextMeshProUGUI[] playerPointsText = new TextMeshProUGUI[2];
        PlayerController[] players = new PlayerController[2];

        int counter = 0;
        public void Launch() // start
        {

            launched = true;

            players = GameObject.FindObjectsOfType<PlayerController>();
            players[0].player.useGraphics = useGraphics;
            players[0].player.opponent = players[1].player;
            players[0].player.playerIndex = 0;

            players[1].player.useGraphics = useGraphics;
            players[1].player.opponent = players[0].player;
            players[1].player.playerIndex = 1;
            // Set player index to cards of player1
            for (int i = 0; i < players[1].player.playerVisuals.playerCards.Length; i++)
            {
                players[1].player.playerVisuals.playerCards[i].GetComponent<CardVisuals>().playerIndex = 1;
            }

            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            //while (players[0].player.playerCards.Length > 0 ||
            //            players[1].player.playerCards.Length > 0)
            while(players[0].player.numberOfPlayerCardsInHand > 0 || 
                    players[1].player.numberOfPlayerCardsInHand > 0)
            {
                //Debug.Log("players[0].player.numberOfPlayerCardsInHand" + players[0].player.numberOfPlayerCardsInHand);

                players[counter].player.PlayersTurn();

                yield return new WaitWhile(() => (players[0].player.playersTurn || players[1].player.playersTurn));

                counter++;
                if (counter >= players.Length)
                    counter = 0;
            }

            players[0].onUpdate.Invoke();
            players[1].onUpdate.Invoke();

            Debug.Log(players[0].player);
            Debug.Log(players[1].player);

            victoryUI.SetActive(true);
            victoryText.text = "Player #" + GetWinner(players).player.playerIndex + " wins!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerControllers"></param>
        /// <returns>returns null if points are the same for all players</returns>
        PlayerController GetWinner(PlayerController[] playerControllers)
        {
            // If points are the same
            bool theSame = true;
            for (int i = 0; i < playerControllers.Length; i++)
            {
                for (int inner = 0; inner < playerControllers.Length; inner++)
                {
                    if (inner != i)
                    {
                        if (playerControllers[i].player.CountPlayerPoints() != playerControllers[inner].player.CountPlayerPoints())
                            theSame = false;
                    }
                }
            }
            if (theSame)
                return null;

            PlayerController result = playerControllers[0];
            for (int i = 1; i < playerControllers.Length; i++)
            {
                if (result.player.CountPlayerPoints() < playerControllers[i].player.CountPlayerPoints())
                    result = playerControllers[i];
            }
            return result;
        }

        private void Update()
        {
            if (launched)
            {
                // Update points text
                for (int t = 0; t < playerPointsText.Length; t++)
                    playerPointsText[t].text = players[t].player.CountPlayerPoints() + "";
            }
        }
    }
}