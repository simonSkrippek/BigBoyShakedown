using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.Data
{
    [CreateAssetMenu(fileName = "New VictoryData", menuName = "VictoryData", order = 52)]
    public class VictoryData : ScriptableObject
    {
        public int playerIndex;
        public float score;
        public string characterName;
    }
}
