using Elympics;
using UnityEngine;

public class PlayerHandler : ElympicsMonoBehaviour, IInputHandler, IUpdatable
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private ResultManager resultManager;
    [SerializeField] private PlayerHandler oponentHandler;
    
    private bool gameRunning = true;

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
        if (gameRunning && Elympics.Player == PredictableFor)
        {
            inputHandler.UpdateInput();
        }
    }

    private void OnDisable()
    {
        if (PredictableFor == Elympics.Player)
        {
            resultManager.GameOver(false);
        }
        else if (Elympics.IsServer)
        {
            resultManager.GameOver();
        }
        else
        {
            resultManager.GameOver(true);
        }
        
        Debug.Log($"Player: {gameObject.name} was disabled | ID [{Elympics.Player}] PredictalbeFor: {PredictableFor}");
        gameRunning = false;
        oponentHandler.GameOver();
    }

    public void ElympicsUpdate()
    {
        if (gameRunning)
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
            if (Elympics.Player != PredictableFor)
            {
                playerInfo.UpdateShield();
            }
        }
    }

    private void GameOver()
    {
        gameRunning = false;
    }
}
