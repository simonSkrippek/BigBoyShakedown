using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigBoyShakedown.UI.Manager
{
    public class PlayerTextManager : MonoBehaviour
    {
        [SerializeField] GameObject[] playerTexts;

        public void ActivatePlayerText(int playerIndex)
        {
            for (int i = 0; i < playerTexts.Length; i++)
            {
                playerTexts[i].SetActive(i == playerIndex);
            }
        }
    }
}
