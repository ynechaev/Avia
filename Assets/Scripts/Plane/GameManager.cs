using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public Sprite rightSprite0;
    public Sprite rightSprite1;
    public Sprite leftSprite0;
    public Sprite leftSprite1;
    public Sprite straightSprite;

    public CarrierController carrier;
    public Waypoint landingPoint;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {

    }
}