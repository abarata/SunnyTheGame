using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int Score = 0;
    private string lastCherryDestroyed = "";

    public void onWinPrize(GameObject gameobject)
    {
        //Debug.Log("2 - gameobject.name: " + gameobject.name + "  -- gameobject.tag: " + gameobject.tag);
        var prizeojb = gameobject.GetComponent<PrizeCherry>();
        if (lastCherryDestroyed != gameobject.name)
        {
            prizeojb.showPrizeAnimation();
            UpdateScore(prizeojb.Prize);
            lastCherryDestroyed = gameobject.name;
        }

    }

    public void UpdateScore (int prize)
    {
        Score += prize;
    }

}
