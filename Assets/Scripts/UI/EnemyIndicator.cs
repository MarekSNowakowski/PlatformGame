using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyIndicator : MonoBehaviour
{
    [SerializeField]
    private PlayerHandler playerHandler;
    
    [SerializeField]
    private Image image;

    [SerializeField]
    private Vector2 offset = new Vector2(60, 80);

    private Transform playerTransform;
    private Transform opponentTransform;
    
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        
        PlayerHandler.Players players = playerHandler.GetPlayers();
        playerTransform = players.Player.transform;
        opponentTransform = players.Opponent.transform;
    }

    void Update()
    {
        Vector3 vpPos = camera.WorldToViewportPoint(opponentTransform.position);
        if (vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f)
        {
            if (image.enabled)
            {
                image.enabled = false;
            }
        }
        else
        {
            if (!image.enabled)
            {
                image.enabled = true;
            }

            Vector2 direction = (opponentTransform.position - playerTransform.position).normalized;
            var angle = Vector2.Angle(Vector2.up, direction);
            angle=Vector2.Dot(Vector2.left, direction) >0.0 ? angle : -angle;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            float xPosition = (float)Screen.width / 2 + ((float)Screen.width / 2 * direction.x);
            float yPosition = (float)Screen.height / 2 + ((float)Screen.height / 2 * direction.y);
            float xOffset = Mathf.Pow(direction.x, 3) * -offset.x;
            float yOffset = Mathf.Pow(direction.y, 3) * -offset.y;
            transform.position = new Vector2(xPosition + xOffset, yPosition + yOffset);
        }
    }
}
