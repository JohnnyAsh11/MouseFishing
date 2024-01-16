using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    [SerializeField]
    private FishManager fishManager;

    [SerializeField]
    private TextMesh timerText;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //setting the text to be the amount of time left
        timerText.text = "Time: " + Mathf.Round(fishManager.GameTimer);
    }
}
