using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBoyShakedown.Game.Data
{
    [CreateAssetMenu(fileName = "New PlayerMetrics", menuName = "PlayerMetrics", order = 51)]
    public class VictoryData : ScriptableObject
    {
        public int playerIndex;
        public string characterName;
    }
}
