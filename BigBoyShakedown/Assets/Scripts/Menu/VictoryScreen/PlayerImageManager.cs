using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigBoyShakedown.UI.Manager
{
    public class PlayerImageManager : MonoBehaviour
    {
        [SerializeField] GameObject[] playerImage;

        public void ActivatePlayerImage(string characterName, int playerIndex)
        {
            int characterIndex = -1;
            switch (characterName)
            {
                case "mark":
                    characterIndex = 0;
                    break;
                case "grease":
                    characterIndex = 1;
                    break;
                case "ace":
                    characterIndex = 2;
                    break;
                case "specci":
                    characterIndex = 3;
                    break;
            }

            for (int i = 0; i < playerImage.Length; i++)
            {
                playerImage[i].SetActive(i == characterIndex);
                if (i == characterIndex) playerImage[i].GetComponent<PlayerTextManager>().ActivatePlayerText(playerIndex);
            }
        }
    }
}