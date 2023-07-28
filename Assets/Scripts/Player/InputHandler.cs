using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    private GatheredInput gatheredInput;

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void UpdateInput()
    {
        if (gameStateSynchronizer.GameState == GameState.Gameplay)
        {
            gatheredInput.movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            gatheredInput.jump = Input.GetButtonDown("Jump") || gatheredInput.jump;
            gatheredInput.slide = Input.GetButtonDown("slide") || gatheredInput.slide;
            gatheredInput.attack = Input.GetButtonDown("attack") || gatheredInput.attack;
        }
    }

    public GatheredInput GetInput()
    {
        GatheredInput returnedInput = gatheredInput;
        gatheredInput.movementInput = Vector2.zero;
        gatheredInput.jump = false;
        gatheredInput.slide = false;
        gatheredInput.attack = false;
        return returnedInput;
    }
}

public struct GatheredInput
{
    public Vector2 movementInput;
    public bool jump;
    public bool slide;
    public bool attack;
}
