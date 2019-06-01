using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAlphaGroup : MonoBehaviour {

    public float Alpha = 1f;

    [SerializeField]
    Color OriginalColor = Color.clear;

    [SerializeField]
    bool CollectInRuntime = false;

    private void Awake()
    {
        if (OriginalColor == Color.clear)
        {
            OriginalColor = Color.white;
        }

        if(CollectInRuntime)
        {
            CollectRenderers();
        }
    }

    private void Reset()
    {
        CollectRenderers();
        OriginalColor = Color.white;
        Alpha = 1f;
    }

    [SerializeField]
    List<SpriteRendererInstance> CollectedRenderers = new List<SpriteRendererInstance>();
    void CollectRenderers()
    {
        CollectedRenderers.Clear();
        CollectSpriteRenderers(transform);
    }

    void CollectSpriteRenderers(Transform givenTransform)
    {
        SpriteRenderer sRenderer = givenTransform.GetComponent<SpriteRenderer>();

        if(sRenderer != null)
        {
            CollectedRenderers.Add(new SpriteRendererInstance(sRenderer));
        }

        for (int i = 0; i < givenTransform.childCount; i++)
        {
            CollectSpriteRenderers(givenTransform.GetChild(i));
        }
    }


    public void SetAlpha(float fValue)
    {
        Alpha = fValue;

        RefreshChildrenAlpha(transform);
    }

    public void FadeIn(float Speed = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine(Speed));
    }

    public void FadeOut(float Speed = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(Speed));
    }

    private IEnumerator FadeOutRoutine(float speed)
    {
        Alpha = 1f;
        while (Alpha > 0f)
        {
            SetAlpha(Alpha - (speed * Time.deltaTime));
            yield return 0;
        }
    }

    private IEnumerator FadeInRoutine(float speed)
    {
        Alpha = 0f;
        while(Alpha < 1f)
        {
            SetAlpha(Alpha + (speed * Time.deltaTime));
            yield return 0;
        }
    }

    protected void RefreshChildrenAlpha(Transform currentChild)
    {
        SpriteRenderer renderer = currentChild.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
        }

        for(int i=0;i<currentChild.transform.childCount;i++)
        {
            RefreshChildrenAlpha(currentChild.transform.GetChild(i));
        }
    }


    protected void SetChildrenColor(Transform currentChild, Color clr)
    {
        SpriteRenderer renderer = currentChild.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = clr;
        }

        for (int i = 0; i < currentChild.transform.childCount; i++)
        {
            SetChildrenColor(currentChild.transform.GetChild(i), clr);
        }
    }

    Coroutine BlinkRoutineInstance;
    public void BlinkColor(Color clr)
    {
        if(BlinkRoutineInstance != null)
        {
            StopCoroutine(BlinkRoutineInstance);
        }

        BlinkRoutineInstance = StartCoroutine(BlinkDamageRoutine(clr));
    }

    public void BlinkColor()
    {
        if (BlinkRoutineInstance != null)
        {
            StopCoroutine(BlinkRoutineInstance);
        }

        BlinkRoutineInstance = StartCoroutine(BlinkDamageRoutine(Color.black));
    }

    private IEnumerator BlinkDamageRoutine(Color clr)
    {
        SetChildrenColor(transform, clr);

        yield return new WaitForSeconds(0.05f);

        SetChildrenColor(transform, OriginalColor);

        BlinkRoutineInstance = null;
    }

    public void SetSpritesMaterial(Material givenMaterial)
    {
        for(int i=0;i<CollectedRenderers.Count;i++)
        {
            CollectedRenderers[i].SRenderer.material = givenMaterial; 
        }
    }

    public void ResetSpritesMaterial()
    {
        for (int i = 0; i < CollectedRenderers.Count; i++)
        {
            CollectedRenderers[i].SRenderer.material = CollectedRenderers[i].initMaterial;
        }
    }

}

public class SpriteRendererInstance
{
    public SpriteRenderer SRenderer;
    public Material initMaterial;

    public SpriteRendererInstance(SpriteRenderer renderer)
    {
        this.SRenderer = renderer;
        this.initMaterial = renderer.material;

    }
}

