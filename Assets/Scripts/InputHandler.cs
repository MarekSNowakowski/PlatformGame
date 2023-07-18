using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private GatheredInput gatheredInput;
    
    public void UpdateInput()
    {
        gatheredInput.movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        gatheredInput.jump = Input.GetButtonDown("Jump");
        gatheredInput.slide = Input.GetButtonDown("slide");
    }

    public GatheredInput GetInput()
    {
        return gatheredInput;
    }
}

public struct GatheredInput
{
    public Vector3 movementInput;
    public bool jump;
    public bool slide;
}
