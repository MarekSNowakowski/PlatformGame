using Elympics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHandler : ElympicsMonoBehaviour, IInputHandler, IUpdatable, IInitializable
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private ResultManager resultManager;
    [SerializeField] private PlayerHandler opponentHandler;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;

    [SerializeField] private PlayerGUI playerGUI;
    [SerializeField] private PlayerGUI opponentGUI;
    
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
    
    public void Initialize()
    {
        if (Elympics.Player == PredictableFor)
        {
            InitializeGUI(playerGUI);
            opponentHandler.InitializeGUI(opponentGUI);
        }
        else if ((int)Elympics.Player == -2)
        {
            InitializeGUI(playerGUI);
            opponentHandler.InitializeGUI(opponentGUI);
        }
    }

    public void InitializeGUI(PlayerGUI gui)
    {
        playerInfo.InitializeGUI(gui);
    }
    
    private void Update()
    {
        if (gameRunning && Elympics.Player == PredictableFor)
        {
            inputHandler.UpdateInput();
        }
    }

    public void OnGameOver()
    {
        if (!gameRunning) return;
        if (resultManager)
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
        }

        Debug.Log($"Player: {gameObject.name} was disabled | ID [{Elympics.Player}] PredictalbeFor: {PredictableFor}");
        gameRunning = false;
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        playerController.UpdatePlayer(new GatheredInput());
        opponentHandler.GameOver();
        
        if (gameStateSynchronizer)
        {
            gameStateSynchronizer.FinishGame();
        }
        else
        {
            Debug.LogWarning("GameStateSynchronizer has been destroyed before game finished");
        }
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

    public struct Players
    {
        public GameObject Player;
        public GameObject Opponent;
    }

    public Players GetPlayers()
    {
        Players players = new Players();
        
        if (Elympics.Player == PredictableFor)
        {
            players.Player = gameObject;
            players.Opponent = opponentHandler.gameObject;
        }
        else
        {
            players.Player = opponentHandler.gameObject;
            players.Opponent = gameObject;
        }
        
        return players;
    }

    private void GameOver()
    {
        gameRunning = false;
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        playerController.UpdatePlayer(new GatheredInput());
    }
}
