using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGui : MonoBehaviour
{

    [Header("Coins")]
    public int points = 0;
    // Start is called before the first frame update

    void OnGUI() //Coin Gui
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score : " + points);
    }

}
