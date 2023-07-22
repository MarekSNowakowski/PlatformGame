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
    private Color shieldStaggerColor;
    [SerializeField]
    private int staggerCount;

    private Coroutine coroutine = null;

    private float time;

    public void Stagger(bool shield)
    {
        if (gameObject.activeInHierarchy && coroutine == null)
        {
            coroutine = StartCoroutine(StaggerCO(shield));
        }
    }

    private IEnumerator StaggerCO(bool shield)
    {
        time = 0f;
        
        while(time <= staggerTime)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.white, shield ? shieldStaggerColor : staggerColor, Mathf.Sin((time * staggerCount) / staggerTime));
            yield return null;
        }

        spriteRenderer.color = Color.white;
        coroutine = null;
    }
}
