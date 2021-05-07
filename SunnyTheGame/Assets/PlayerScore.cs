using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerScore : ExtendedBehavior
{
    public float MaxPlayerHealth = 100f;
    public float StartPlayerHealth = 10f;

    private bool m_gameover = false;
    public event EventHandler GameOverEvent;
    public event EventHandler<float> HealthChangeEvent;

    private void Start()
    {
        onHealthChangeEvent();
    }

    private void onGameOverEvent ()
    {
        if (GameOverEvent != null)
            GameOverEvent.Invoke(this, EventArgs.Empty);
    }
    private void onHealthChangeEvent()
    {
        if (HealthChangeEvent != null)
            HealthChangeEvent.Invoke(this, PlayerHealth);
    }

    private int playerscore = 0;
    public int Score
    {
        get { return playerscore; }
    }

    public float PlayerHealth
    {
        get { return StartPlayerHealth; }
    }

    public void UpdatePlayerHealth(float value)
    {
        if ((StartPlayerHealth + value) > MaxPlayerHealth) value = MaxPlayerHealth - StartPlayerHealth;
        StartPlayerHealth += value;
        if (StartPlayerHealth < 0) StartPlayerHealth = 0;
        onHealthChangeEvent();
        Debug.Log("UpdatePlayerHealth  damage: " + value + " --playerhealth: " + PlayerHealth.ToString());
        if (StartPlayerHealth <= 0)
        {
            StartPlayerHealth = 0;
            m_gameover = true;
            onGameOverEvent();
        }
    }

    public void UpdateScore (int value)
    {
        playerscore += value;
        Debug.Log("playerscore: " + playerscore.ToString());
    }

}
