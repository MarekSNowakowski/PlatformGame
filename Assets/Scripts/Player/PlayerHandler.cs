using Elympics;
using UnityEngine;

public class PlayerHandler : ElympicsMonoBehaviour, IInputHandler, IUpdatable
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInfo playerInfo;


    public void OnInputForClient(IInputWriter inputSerializer)
    {
        if (PredictableFor == Elympics.Player)
        {
            GatheredInput currentInput = inputHandler.GetInput();
            inputSerializer.Write(currentInput.movementInput.x);
            inputSerializer.Write(currentInput.movementInput.y);
            inputSerializer.Write(currentInput.jump);
            inputSerializer.Write(currentInput.slide);
            inputSerializer.Write(currentInput.attack);
        }
    }

    public void OnInputForBot(IInputWriter inputSerializer)
    {
        //throw new System.NotImplementedException();
    }
    
    private void Update()
    {
        if (Elympics.Player == PredictableFor)
        {
            inputHandler.UpdateInput();
        }
    }

    public void ElympicsUpdate()
    {
        GatheredInput currentInput = new GatheredInput();
        currentInput.movementInput = Vector2.zero;

        if (ElympicsBehaviour.TryGetInput(PredictableFor, out var inputReader))
        {
            inputReader.Read(out float x);
            inputReader.Read(out float y);
            inputReader.Read(out bool jump);
            inputReader.Read(out bool slide);
            inputReader.Read(out bool attack);

            currentInput.movementInput = new Vector2(x, y);
            currentInput.jump = jump;
            currentInput.slide = slide;
            currentInput.attack = attack;
        }
        
        playerController.UpdatePlayer(currentInput);
        if(Elympics.Player != PredictableFor)
            playerInfo.UpdateShield();
        // Debug.Log($"Elympics.Player: {Elympics.Player}, PredictableFor: {PredictableFor}, GameObject: {gameObject.name}");
        // Debug.Log($"MOVEMENT: {currentInput.movementInput}, JUMP: {currentInput.jump}, SLIDE: {currentInput.slide}");
    }
}
