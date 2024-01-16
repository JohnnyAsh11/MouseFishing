using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField]
    private TextMesh scoreText;

    [SerializeField]
    private RodCollisions scoreSource;

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "score: " + scoreSource.Score;
    }
}
