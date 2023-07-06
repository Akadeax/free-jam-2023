using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI trinketCounter;

    int currentScore;
    public int CurrentScore => currentScore;

    public static GameUIHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        trinketCounter.text = "Trinket Haul: 0";
    }

    public void IncreaseScore()
    {
        currentScore++;
        trinketCounter.text = "Trinket Haul: " + currentScore;

        if (currentScore == 5)
        {
            FindObjectOfType<PlayerSteal>().angleSizeRange = new(40, 90);
        }
        else if (currentScore == 10)
        {
            var guards = FindObjectsOfType<NPCData>().Where(x => x.Type == NPCData.NPCType.Guard);
            foreach (var guard in guards)
            {
                var data = guard.GetComponent<PixelAnimator>().GetState("ChasePlayer").data as NPCChasePlayerStateData;
                data.playerChaseSpeed *= 1.25f;
            }
        }
        else if (currentScore == 20)
        {
            FindObjectOfType<PlayerSteal>().angleSizeRange = new(20, 35);
            var nonGuards = FindObjectsOfType<NPCData>().Where(x => x.Type != NPCData.NPCType.Guard);
            foreach (var nonGuard in nonGuards)
            {
                nonGuard.suspiciousBarIncreaseFactor += 0.25f;
            }
        }
    }
}
