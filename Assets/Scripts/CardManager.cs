using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Stores all types of cards cards and makes it possible for a designer to edit them
    /// </summary>
    [System.Serializable]
    public class CardManager : MonoBehaviour
    {
        public int initialNumberOfCards = 10;
        public Card[] allTypesOfCards;
    }
}