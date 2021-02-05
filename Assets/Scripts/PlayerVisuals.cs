using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// plays animation and makes and makes a delay so that game logic is paused while animation is playing.
    /// </summary>
    public class PlayerVisuals : MonoBehaviour
    {
        public float cardSpeed = 0.3f;

        public GameObject phantomCardPrefab;
        public GameObject cardChooserPrefab;
        public GameObject arrowPrefab;
        public GameObject spherePrefab;
        public GameObject spherePrefab1;
        public GameObject cardPrefab;

        public Vector3 cardOffset;
        public GameObject[] playerCards;
        GameObject[][] playerCardsRow = new GameObject[2][];

        public GameObject playerHand;
        //public GameObject graveyard;

        /// <summary>
        /// Melee and range.
        /// </summary>
        public GameObject[] cardRowObject = new GameObject[2];

        private float waitFor = 0.01f;

        /// <summary>
        /// Set at AbstractPalyer.initialize().
        /// </summary>
        public AbstractPalyer player;

        private void Awake()
        {
            for(int r = 0; r < playerCardsRow.Length; r++)
            {
                playerCardsRow[r] = new GameObject[0];
            }
        }

        GameObject FindCardObjectByIndex(int cardIndex)
        {
            for (int row = 0; row < playerCardsRow.Length; row++)
            {
                for (int col = 0; col < playerCardsRow[row].Length; col++)
                {
                    if (playerCardsRow[row][col].GetComponent<CardVisuals>().cardIndex == cardIndex)
                        return playerCardsRow[row][col];
                }
            }
            return null;
        }

        /// <summary>
        /// Called from AbstructPlayer.Initialize()
        /// </summary>
        /// <param name="card"></param>
        public void AddCardToHand(Card card)
        {
            GameObject newCard = Instantiate(cardPrefab);
            CardVisuals cardVisuals = newCard.GetComponent<CardVisuals>();
            newCard.GetComponent<CardVisuals>().Initialize(card, player.playerIndex);
            newCard.transform.SetParent(playerHand.transform);
            newCard.transform.localPosition = new Vector3(cardOffset.x * card.cardIndex, 0, 0);
            playerCards = HelperFunctions.Add<GameObject>(playerCards, newCard);

            //TODO: Update player card points text
            UpdateCardPointsText(cardVisuals, card);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param card position in card matrix="cardPosition"></param>
        /// <returns>returns real card position inside CardRow transform</returns>
        Vector3 GetRealCardPosition(CardPosition cardPosition)
        {
            float x = cardPosition.column * cardOffset.x;
            float z = cardRowObject[cardPosition.row].transform.position.z;
            return new Vector3(x, cardOffset.y, z);
        }
        public Vector3 GetCardRowPositionX(int rowIndex)
        {
            return new Vector3((player.cardTablePart.cardArray[rowIndex].Length - 3) * cardOffset.x / 2, 0, 0);
        }

        public GameObject GetCardObjectByCardIndex(int cardIndex)
        {
            for (int i = 0; i < playerCards.Length; i++)
                if (playerCards[i].GetComponent<CardVisuals>().cardIndex == cardIndex)
                    return playerCards[i];

            return null;
        }

        #region UpdateCardPointsText
        /// <summary>
        /// Updates TextMeshPro.text on card gameObject
        /// </summary>
        /// <param name="cardIndex"></param>
        public void UpdateCardPointsText(int cardIndex)
        {
            // Update card points text
            CardPosition appliedTo_CardPosition = 
                player.cardTablePart.GetCardByIndex(cardIndex);
            FindCardObjectByIndex(cardIndex).GetComponent<CardVisuals>().
                SetPointsText(
                player.cardTablePart.cardArray
                [appliedTo_CardPosition.row]
                [appliedTo_CardPosition.column].cardPoints + "");
        }
        public void UpdateCardPointsText(CardVisuals cardVisuals, Card card)
        {
            // Update card points text
            cardVisuals.
                SetPointsText(
                card.cardPoints + "");
        }
        #endregion

        #region PlayCard animation
        public IEnumerator PlayCard(int cardIndex, CardPosition cardPosition)
        {
            #region Move card to table
            if(player.useGraphics)
            { 
                GameObject thisCard = GetCardObjectByCardIndex(cardIndex);

                thisCard.transform.SetParent(cardRowObject[cardPosition.row].transform);

                Vector3 cardRowPosition = GetCardRowPositionX(cardPosition.row);

                // Add card to the row
                playerCardsRow[cardPosition.row] =
                    HelperFunctions.AddAfter<GameObject>(playerCardsRow[cardPosition.row], thisCard, cardPosition.column);

                //TODO: Update card points text
                UpdateCardPointsText(cardIndex);

                Vector3 realPosition = GetRealCardPosition(cardPosition) - cardRowPosition;

                GameObject[] cardsOnTheRight = new GameObject[0];
                for (int i = cardPosition.column + 2; i < playerCardsRow[cardPosition.row].Length; i++)
                {
                    cardsOnTheRight =
                        HelperFunctions.AddAfter<GameObject>(cardsOnTheRight,
                        playerCardsRow[cardPosition.row][i],
                        cardsOnTheRight.Length - 1);
                }
                GameObject[] cardsOnTheLeft = new GameObject[0];
                for (int i = cardPosition.column; i >= 0; i--)
                {
                    cardsOnTheLeft =
                        HelperFunctions.AddAfter<GameObject>(cardsOnTheLeft,
                        playerCardsRow[cardPosition.row][i],
                        cardsOnTheLeft.Length - 1);
                }

                int coef = 1;

                while (realPosition != thisCard.transform.position)
                {
                    #region Move this card
                    thisCard.transform.position =
                        Vector3.Lerp(thisCard.transform.position, realPosition, cardSpeed);
                    #endregion

                    #region Move cards on the right to the right
                    for (int c = 0; c < cardsOnTheRight.Length; c++)
                    {
                        cardsOnTheRight[c].transform.position =
                                    Vector3.Lerp(cardsOnTheRight[c].transform.position,
                                        new Vector3(realPosition.x + (cardOffset.x * (c + coef)), realPosition.y, realPosition.z),
                                        cardSpeed);
                    }
                    #endregion

                    #region Move cards on the left to the left
                    for (int c = 0; c < cardsOnTheLeft.Length; c++)
                    {
                        cardsOnTheLeft[c].transform.position =
                                    Vector3.Lerp(cardsOnTheLeft[c].transform.position,
                                        new Vector3(realPosition.x - (cardOffset.x * (c + coef)), realPosition.y, realPosition.z),
                                        cardSpeed);
                    }
                    #endregion

                    // Pause and resume next frame
                    yield return null;
                }
                thisCard.GetComponent<CardVisuals>().onTable = true;
                #endregion
                //Debug.Log("play card end");
            }
            player.ApplyPassive(cardPosition);
        }
        #endregion
        #region ApplyPassive animation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cardPosition"></param>
        /// <param indexes of ally cards this ability applied to="appliedToAllyCards"></param>
        /// <param indexes of opponent cards this ability applied to="appliedToOpponentCards"></param>
        /// <returns></returns>
        public IEnumerator ApplyPassive(CardPosition cardPosition, 
                                                int[] appliedToAllyCards, int[] appliedToOpponentCards)
        {
            if (player.useGraphics)
            {
                #region ToAlly
                for (int i = 0; i < appliedToAllyCards.Length; i++)
                {
                    //TODO: Update card points text
                    UpdateCardPointsText(appliedToAllyCards[i]);

                    GameObject created1 = Instantiate(spherePrefab);
                    created1.transform.SetParent(FindCardObjectByIndex(appliedToAllyCards[i]).transform);
                    created1.transform.localPosition = new Vector3(0, 0, 0);

                    IVisualEffect visualEffect =
                        created1.GetComponent<IVisualEffect>();

                    // Wait for animation to proceed
                    while (visualEffect.ProceedAnimation())
                    {
                        yield return null;
                    }

                    Destroy(created1);
                }
                #endregion

                #region ToOpponent
                for (int i = 0; i < appliedToOpponentCards.Length; i++)
                {
                    //TODO: Update card points text
                    player.opponent.playerVisuals.UpdateCardPointsText(appliedToOpponentCards[i]);

                    GameObject cardObject = player.
                        opponent.
                        playerVisuals.
                        FindCardObjectByIndex(appliedToOpponentCards[i]);

                    GameObject created1 = Instantiate(spherePrefab);
                    created1.transform.SetParent(cardObject.transform);
                    created1.transform.localPosition = new Vector3(0, 0, 0);

                    IVisualEffect visualEffect =
                        created1.GetComponent<IVisualEffect>();

                    // Wait for animation to proceed
                    while (visualEffect.ProceedAnimation())
                    {
                        yield return null;
                    }

                    Destroy(created1);
                }
                #endregion
            }
            player.ApplyActive(cardPosition);
        }
        #endregion
        #region ApplyActive animation
        public IEnumerator ApplyActive(CardPosition cardPosition,
                                                int[] appliedToAllyCards, int[] appliedToOpponentCards)
        {
            if (player.useGraphics)
            {
                #region ToAlly
                for (int i = 0; i < appliedToAllyCards.Length; i++)
                {
                    //TODO: Update card points text
                    UpdateCardPointsText(appliedToAllyCards[i]);

                    GameObject created1 = Instantiate(spherePrefab1);
                    created1.transform.SetParent(FindCardObjectByIndex(appliedToAllyCards[i]).transform);
                    created1.transform.localPosition = new Vector3(0, 0, 0);

                    IVisualEffect visualEffect =
                        created1.GetComponent<IVisualEffect>();

                    // Wait for animation to proceed
                    while (visualEffect.ProceedAnimation())
                    {
                        yield return null;
                    }

                    Destroy(created1);
                }
                #endregion

                #region ToOpponent
                for (int i = 0; i < appliedToOpponentCards.Length; i++)
                {
                    //TODO: Update card points text
                    player.opponent.playerVisuals.UpdateCardPointsText(appliedToOpponentCards[i]);

                    GameObject cardObject = player.
                        opponent.
                        playerVisuals.
                        FindCardObjectByIndex(appliedToOpponentCards[i]);

                    GameObject created1 = Instantiate(spherePrefab1);
                    created1.transform.SetParent(cardObject.transform);
                    created1.transform.localPosition = new Vector3(0, 0, 0);

                    IVisualEffect visualEffect =
                        created1.GetComponent<IVisualEffect>();

                    // Wait for animation to proceed
                    while (visualEffect.ProceedAnimation())
                    {
                        yield return null;
                    }

                    Destroy(created1);
                }
                #endregion
            }

            player.EndTurn();
        }
        #endregion

        // ////////////////////////////////////////////

        public IEnumerator ChooseCard()
        {

            GameObject cardChooserObject = Instantiate(cardChooserPrefab);
            CardChooser cardChooser = cardChooserObject.GetComponent<CardChooser>();
            cardChooser.isVisible = false;
            cardChooser.playerIndex = player.playerIndex;
            cardChooser.onTable = false;

            while (cardChooser.chosenCardObject == null)
            {
                yield return null;
            }

            GameObject cardChooserObject1 = Instantiate(cardChooserPrefab);
            CardChooser cardChooser1 = cardChooserObject1.GetComponent<CardChooser>();
            cardChooser1.playerIndex = player.playerIndex;
            cardChooser1.onTable = true;
            cardChooser1.cardIndexException = -2;
            cardChooser1.SetChosenCardObject(cardChooser.chosenCardObject);

            while (cardChooser1.applyToCardObject == null)
            {
                yield return null;
            }

            int chosenCardIndex = cardChooser.chosenCardObject.GetComponent<CardVisuals>().cardIndex;
            player.chosenCard = player.FindCardByIndex(chosenCardIndex);
            CardPosition cardPos = player.cardTablePart.CardPositionByIndex(
                                                cardChooser1.applyToCardObject.GetComponent<CardVisuals>().cardIndex);
            if(cardPos == null)
            {
                cardPos = cardChooser1.applyToCardObject.GetComponent<RowVisuals>().rowPosition;
            }

            Destroy(cardChooserObject);
            Destroy(cardChooserObject1);

            player.numberOfPlayerCardsInHand--;

            player.PlayCard(cardPos);
        }



        /// <summary>
        /// Called from HumanPlayer.
        /// </summary>
        /// <param name="cardPosition"></param>
        /// <param name="chosenCard"></param>
        /// <returns></returns>
        public IEnumerator ApplyActive(CardPosition cardPosition, Card chosenCard)
        {
            int[] appliedToAllyCards = new int[0];
            int[] appliedToOpponentCards = new int[0];

            if (chosenCard.HasActiveAbilityToAlly())
            {
                GameObject cardChooserObject = Instantiate(cardChooserPrefab);
                CardChooser cardChooser = cardChooserObject.GetComponent<CardChooser>();
                cardChooser.playerIndex = player.playerIndex;
                cardChooser.onTable = true;
                cardChooser.SetChosenCardObject(FindCardObjectByIndex(chosenCard.cardIndex));

                CardPosition chosenCardPosition = null;
                while (cardChooser.applyToCardObject == null)
                {
                    yield return null;
                }
                chosenCardPosition = player.cardTablePart.CardPositionByIndex(
                    cardChooser.applyToCardObject.GetComponent<CardVisuals>().cardIndex);
                appliedToAllyCards = chosenCard.ApplyActiveToAlly(player.cardTablePart, chosenCardPosition);

                //TODO: Update card points text
                UpdateCardPointsText(chosenCard.cardIndex);

                Destroy(cardChooserObject);
            }

            if (chosenCard.HasActiveAbilityToOpponent() && player.opponent.cardTablePart.GetRandomRowWithCards() != -1)
            {
                GameObject cardChooserObject = Instantiate(cardChooserPrefab);
                CardChooser cardChooser = cardChooserObject.GetComponent<CardChooser>();
                cardChooser.SetChosenCardObject(FindCardObjectByIndex(chosenCard.cardIndex));
                cardChooser.playerIndex = player.opponent.playerIndex;
                cardChooser.onTable = true;

                CardPosition chosenCardPosition = null;
                while (cardChooser.applyToCardObject == null)
                {
                    yield return null;
                }
                
                chosenCardPosition = player.opponent.cardTablePart.CardPositionByIndex(
                                            cardChooser.applyToCardObject.GetComponent<CardVisuals>().cardIndex);
                appliedToOpponentCards = chosenCard.ApplyActiveToOpponent(player.opponent.cardTablePart,
                        chosenCardPosition);

                //TODO: Update card points text
                player.opponent.playerVisuals.UpdateCardPointsText(
                    cardChooser.applyToCardObject.GetComponent<CardVisuals>().cardIndex);

                Destroy(cardChooserObject);
            }

            //   *** Active abilities animation
            StartCoroutine(ApplyActive(
                cardPosition,
                appliedToAllyCards,
                appliedToOpponentCards));

            player.OnUpdate();

            yield return null;
        }
    }
}