using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoringScript : MonoBehaviour
{
    public int score = 0;
    public TMP_Text scoreText;

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }
}
