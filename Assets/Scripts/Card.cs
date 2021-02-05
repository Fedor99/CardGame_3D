using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Card class defines card abilities and functions.
    /// Designer can create new cards in inspector.
    /// </summary>
    [System.Serializable]
    public struct Card
    {
        public Card(int cardIndex, int playerIndex)
        {
            this.cardIndex = cardIndex;
            this.playerIndex = playerIndex;

            CardName = "Name";
            Description = "Description";

            cardMaterial = new Material(Shader.Find("Specular"));

            cardPoints = 1;

            abilitiesApplied = false;
            cardAbilities = new CardAbility[1];

            destroyAfterApplying = false;
        }

        public int playerIndex;
        public int cardIndex;

        public string CardName;
        public string Description;

        public Material cardMaterial;

        public int cardPoints;

        bool abilitiesApplied;
        public CardAbility[] cardAbilities;

        public bool destroyAfterApplying;

#region Apply Passive to Ally/Opponent
        /// <summary>
        /// 
        /// </summary>
        /// <param player OR opponent table part="playerTablePart"></param>
        /// <param name="thisCardPosition"></param>
        /// <returns>returns an array of int - indexes of cards this ability applied to</returns>
        public int[] ApplyPassiveToAlly(CardTablePart playerTablePart, CardPosition thisCardPosition)
        {
            // Indexes of cards this ability applied to
            int[] result = new int[0];

            for (int a = 0; a < cardAbilities.Length; a++)
            {
                CardAbility ability = cardAbilities[a];

                // toAlly
                for (int i = 0; i < ability.toAlly.Length; i++)
                {
                    switch (ability.toAlly[i])
                    {
                        case ApplyToAlly.ALL:
                            for (int row = 0; row < playerTablePart.cardArray.Length; row++)
                            {
                                for (int col = 0; col < playerTablePart.cardArray[row].Length; col++)
                                {
                                    //Debug.Log("Passive ability applieed");
                                    playerTablePart.cardArray[row][col].ApplyAbility(ability);

                                    result = HelperFunctions.Add(result, playerTablePart.cardArray[row][col].cardIndex);
                                }
                            }
                            break;
                        case ApplyToAlly.ITSELF:
                            this.ApplyAbility(ability);
                            result = HelperFunctions.Add<int>(result, this.cardIndex);
                            break;
                        case ApplyToAlly.LEFT:
                            try
                            {
                                playerTablePart.cardArray[thisCardPosition.row][thisCardPosition.column - 1].ApplyAbility(ability);
                                result = HelperFunctions.Add<int>(result, 
                                    playerTablePart.cardArray[thisCardPosition.row][thisCardPosition.column - 1].cardIndex);
                            }
                            catch (IndexOutOfRangeException) { }
                            break;
                        case ApplyToAlly.RIGHT:
                            try
                            {
                                playerTablePart.cardArray[thisCardPosition.row][thisCardPosition.column + 1].ApplyAbility(ability);
                                result = HelperFunctions.Add<int>(result, 
                                    playerTablePart.cardArray[thisCardPosition.row][thisCardPosition.column + 1].cardIndex);
                            }
                            catch (IndexOutOfRangeException) { }
                            break;
                        case ApplyToAlly.ROW:
                            for (int col = 0; col < playerTablePart.cardArray[thisCardPosition.row].Length; col++)
                            {
                                playerTablePart.cardArray[thisCardPosition.row][col].ApplyAbility(ability);
                                result = HelperFunctions.Add<int>(result, playerTablePart.cardArray[thisCardPosition.row][col].cardIndex);
                            }
                            break;
                        case ApplyToAlly.RANDOM:
                            {
                                int row = playerTablePart.GetRandomRowWithCards();
                                if (row != -1)
                                {
                                    int col = HelperFunctions.RandomExcept(
                                                                            0, playerTablePart.cardArray[row].Length,
                                                                            thisCardPosition.column);
                                    if (col != 0)
                                    {
                                        playerTablePart.cardArray[row][col].ApplyAbility(ability);
                                        result = HelperFunctions.Add<int>(result, playerTablePart.cardArray[row][col].cardIndex);
                                    }
                                }
                            }
                            break;
                        case ApplyToAlly.NONE:
                            break;
                        default:
                            break;
                    }

                    Debug.Log("     ApplyPassiveAbilityToAlly");
                }
            }

            return result;
        }

        public int[] ApplyPassiveToOpponent(CardTablePart opponentTablePart, CardPosition thisCardPosition)
        {
            // Indexes of cards this ability applied to
            int[] result = new int[0];

            for (int a = 0; a < cardAbilities.Length; a++)
            {
                CardAbility ability = cardAbilities[a];

                // toOpponent
                for (int i = 0; i < ability.toOpponent.Length; i++)
                {
                    switch (ability.toOpponent[i])
                    {
                        case ApplyToOpponent.ALL:
                            for (int row = 0; row < opponentTablePart.cardArray.Length; row++)
                            {
                                for (int col = 0; col < opponentTablePart.cardArray[row].Length; col++)
                                {
                                    //Debug.Log("Passive ability applieed");
                                    opponentTablePart.cardArray[row][col].ApplyAbility(ability);

                                    result = HelperFunctions.Add<int>(result, opponentTablePart.cardArray[row][col].cardIndex);
                                }
                            }
                            break;
                        case ApplyToOpponent.ROW:
                            for (int col = 0; col < opponentTablePart.cardArray[thisCardPosition.row].Length; col++)
                            {
                                //TODO: !!!!!!!!!!!!!!!!!!!!!!
                                //opponentTablePart.cardArray[thisCardPosition.row][col].ApplyAbility(ability);
                            }
                            break;
                        case ApplyToOpponent.RANDOM:
                            {
                                //Debug.Log("Random applied");
                                int row = opponentTablePart.GetRandomRowWithCards();
                                if (row == -1)
                                    break;
                                int col = UnityEngine.Random.Range(
                                                                    0, opponentTablePart.cardArray[row].Length);
                                opponentTablePart.cardArray[row][col].ApplyAbility(ability);
                                result = HelperFunctions.Add<int>(result, opponentTablePart.cardArray[row][col].cardIndex);
                            }
                            break;
                        case ApplyToOpponent.NONE:
                            break;

                        default:
                            break;
                    }

                    Debug.Log("     ApplyPassiveAbilityToOpponent");
                }
            }
            return result;
        }
        #endregion

#region Apply Active to Ally/Opponent
        public int[] ApplyActiveToAlly(CardTablePart playerTablepart,
                                 CardPosition playerSpecifiedCardPosition = null)
        {
            int[] result = new int[0];

            for (int a = 0; a < cardAbilities.Length; a++)
            {
                if (playerSpecifiedCardPosition != null)
                {
                    for (int ta = 0; ta < cardAbilities[a].toAlly.Length; ta++)
                        if (cardAbilities[a].toAlly[ta] == ApplyToAlly.SPECIFIED)
                        {
                            //Debug.Log("playerSpecifiedCardPosition.row = " + playerSpecifiedCardPosition.row);
                            //Debug.Log("playerSpecifiedCardPosition.column = " + playerSpecifiedCardPosition.column);
                            playerTablepart.cardArray
                                [playerSpecifiedCardPosition.row]
                                [playerSpecifiedCardPosition.column]
                                .ApplyAbility(cardAbilities[a]);

                            result = HelperFunctions.Add<int>(result, 
                                playerTablepart.cardArray[playerSpecifiedCardPosition.row]
                                                            [playerSpecifiedCardPosition.column].cardIndex);
                        }
                }
            }
            return result;
        }

        public int[] ApplyActiveToOpponent(CardTablePart opponentTablePart, CardPosition opponentSpecifiedCardPosition = null)
        {
            int[] result = new int[0];

            for (int a = 0; a < cardAbilities.Length; a++)
            {
                if (opponentSpecifiedCardPosition != null)
                {
                    for (int ta = 0; ta < cardAbilities[a].toOpponent.Length; ta++)
                        if (cardAbilities[a].toOpponent[ta] == ApplyToOpponent.SPECIFIED)
                            opponentTablePart.cardArray
                                [opponentSpecifiedCardPosition.row]
                                [opponentSpecifiedCardPosition.column]
                                .ApplyAbility(cardAbilities[a]);

                    result = HelperFunctions.Add<int>(result,
                                opponentTablePart.cardArray[opponentSpecifiedCardPosition.row]
                                                            [opponentSpecifiedCardPosition.column].cardIndex);
                }
            }

            return result;
        }
#endregion

        /// <summary>
        /// Applies CardAbility to this card instance.
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool ApplyAbility(CardAbility ability)
        {
            cardPoints += ability.increaseCardPointsBy;
            if (cardPoints < 0)
                cardPoints = 0;

            Debug.Log("     Ability '" + ability + "' applied to (player #" + playerIndex + ") card with index " + cardIndex);

            return true;
        }

        public bool HasActiveAbilityToAlly()
        {
            foreach (CardAbility ability in cardAbilities)
            {
                foreach (ApplyToAlly toAlly in ability.toAlly)
                {
                    if (toAlly == ApplyToAlly.SPECIFIED)
                        return true;
                }
            }
            return false;
        }
        public bool HasActiveAbilityToOpponent()
        {
            foreach (CardAbility ability in cardAbilities)
            {
                foreach (ApplyToOpponent toOpponent in ability.toOpponent)
                {
                    if (toOpponent == ApplyToOpponent.SPECIFIED)
                        return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string result = "";
            result += "\n\n Name: " + CardName;
            result += "\n Index: " + cardIndex;
            result += "\n  CardPoints: " + cardPoints;
            return result;
        }
    }

    /// <summary>
    /// Represents card abilities.
    /// </summary>
    [System.Serializable]
    public class CardAbility
    {
        public CardAbility() { }

        public string name = "AbilityName";

        //public AbilityType abilityType;

        /// <summary>
        /// Card abilities
        /// </summary>
        public int increaseCardPointsBy;

        /// <summary>
        /// To wich cards on the CardTable you want to apply this card`s abilities
        /// </summary>
        public ApplyToAlly[] toAlly;
        public ApplyToOpponent[] toOpponent;

        public override string ToString()
        {
            return name;
        }
    }

    public enum ApplyToAlly
    {
        NONE,
        ALL,
        ROW,
        RIGHT,
        LEFT,
        RANDOM,
        ITSELF,

        SPECIFIED
    }
    
    public enum ApplyToOpponent
    {
        NONE,
        ALL,
        ROW,
        RANDOM,

        SPECIFIED
    }
}