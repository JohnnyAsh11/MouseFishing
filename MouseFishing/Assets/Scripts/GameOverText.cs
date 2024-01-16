using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField]
    private FishManager fishManager;

    [SerializeField]
    private TextMesh gameOverText;

    [SerializeField]
    private RodCollisions player;

    [SerializeField]
    private TextMesh scoreText;

    [SerializeField]
    private TextMesh timerText;

    // Start is called before the first frame update
    void Start()
    {
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (fishManager.GameTimer <= 0)
        {
            //setting the gameover text
            gameOverText.text = "GAMEOVER\nfinal score: " + player.Score;

            //removing uneccessary game objects
            scoreText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            player.gameObject.SetActive(false);

            //stopping the game
            Time.timeScale = 0;
        }
    }
}
