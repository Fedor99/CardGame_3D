using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public delegate void OnUpdate();
    public class PlayerController : MonoBehaviour
    {
        public PlayerType playerType;
        public AbstractPalyer player;

        public OnUpdate onUpdate;

        // Start is called before the first frame update
        public void Launch() // Awake
        {
            PlayerVisuals playerVisuals = gameObject.GetComponent<PlayerVisuals>();
            #region player type
            switch (playerType)
            {
                case PlayerType.Human:
                    player = new HumanPlayer(playerVisuals);
                    break;
                case PlayerType.AI:
                    player = new AI(playerVisuals);
                    break;
            }
            player.Initialize();
            #endregion

            #region delegate functions
            onUpdate += player.OnUpdate;
            #endregion
        }

        // Update is called once per frame
        void Update()
        {
            //onUpdate.Invoke();
        }
    }

    public enum PlayerType
    {
        AI,
        Human
    }
}