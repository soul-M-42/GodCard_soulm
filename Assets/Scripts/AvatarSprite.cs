using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSprite : MonoBehaviour
{
    public PlayerState player;
    public int playerNum;
    public GameStateMachine gameStateMachine;
    public Text HealthText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void renderHealthText()
    {
        HealthText.text = $"HP {player.Health}";
    }
}
