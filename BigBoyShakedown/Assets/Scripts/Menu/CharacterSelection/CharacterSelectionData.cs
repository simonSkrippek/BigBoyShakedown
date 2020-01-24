using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace BigBoyShakedown.UI.Data
{
    public class CharacterSelectionData : ScriptableObject
    {
        [SerializeField]
        public JoinData[] playerJoinData
        {
            get;
            private set;
        }
        

        public void AddData(int playerIndex_, CallbackContext context_, string characterName_)
        {
            if (playerJoinData == null) playerJoinData = new JoinData[4];

            if (playerIndex_ >= playerJoinData.Length || playerIndex_ < 0) throw new System.IndexOutOfRangeException("playerIndex not valid : " + playerIndex_);
            playerJoinData[playerIndex_] = new JoinData(context_, characterName_);
        }
        public void ResetData()
        {
            playerJoinData = new JoinData[4];
        }
    }

    [System.Serializable]
    public class JoinData
    {
        [SerializeField]
        public string characterName;
        [SerializeField]
        public readonly CallbackContext context;

        public JoinData(CallbackContext context_, string characterName_)
        {
            context = context_;
            characterName = characterName_;
        }

    }
}
