using System.Collections;
using UnityEngine;

public class StaggerHandler : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float staggerTime;
    [SerializeField]
    private Color staggerColor;
    [SerializeField]
    private int staggerCount;

    private Coroutine coroutine = null;

    private float time;

    public void Stagger()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(StaggerCO());
        }
    }

    private IEnumerator StaggerCO()
    {
        time = 0f;
        
        while(time <= staggerTime)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.white, staggerColor, Mathf.Sin((time * staggerCount) / staggerTime));
            yield return null;
        }

        spriteRenderer.color = Color.white;
        coroutine = null;
    }
}
