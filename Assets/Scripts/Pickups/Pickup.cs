using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private float oscillateFrequency = 1.2f;
    [SerializeField] private float oscillateAmplitude = 0.3f;

    private Vector3 initialPosition;

    protected void Start()
    {
        initialPosition = transform.position;
    }

    protected void Update()
    {
        OscillateVertically();
    }

    private void OscillateVertically()
    {
        transform.position = new Vector3(initialPosition.x,
            Mathf.Sin(Time.time * oscillateFrequency) * oscillateAmplitude + initialPosition.y,
            initialPosition.z);
    }
}
