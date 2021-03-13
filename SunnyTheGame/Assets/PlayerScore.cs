using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public float MaxPlayerHealth = 100f;

    private int playerscore = 0;
    public int Score
    {
        get { return playerscore; }
    }

    private float playerhealth = 100f;
    public float PlayerHealth
    {
        get { return playerhealth; }
    }

    public void UpdatePlayerHealth(float value)
    {
        if ((playerhealth + value) > MaxPlayerHealth) value = MaxPlayerHealth - playerhealth;
        if ((playerhealth + value) <= 0) value = 0;
        playerhealth += value;
        Debug.Log("playerhealth: " + PlayerHealth.ToString());
    }

    public void UpdateScore (int value)
    {
        playerscore += value;
        Debug.Log("playerscore: " + playerscore.ToString());
    }

}
