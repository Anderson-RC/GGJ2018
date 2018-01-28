using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour {
    public Slider healthBarSlider;
    public Text message;
    public Text objectiveCounter;

    private float _stopwatch = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _stopwatch -= Time.deltaTime;
        if (_stopwatch < 0.0f)
        {
            message.enabled = false;
        } else
        {
            message.enabled = true;
        }
	}
    public void SendMessage(string messageText, float timeToShow = 3.0f)
    {
        _stopwatch = timeToShow;
        message.text = messageText;
    }
    public void SetObjectiveCounter(int count)
    {
        objectiveCounter.text = count.ToString();
    }
    public void SetHealthSlider(float healthFraction)
    {
        if (healthFraction > 1.0f) { healthBarSlider.value = 1.0f; }
        if (healthFraction < 0.0f) { healthBarSlider.value = 1.0f; }
        else { healthBarSlider.value = healthFraction; }
    }

  
}
