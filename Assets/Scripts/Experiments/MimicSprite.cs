using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicSprite : MonoBehaviour {
    [SerializeField] SpriteRenderer mTargetRenderer;
    [SerializeField] SpriteRenderer mRenderer;
    

    private void Update()
    {
        if(mRenderer.sprite != mTargetRenderer.sprite)
        {
            mRenderer.sprite = mTargetRenderer.sprite;

            if ((mTargetRenderer.transform.localScale.x < 0f && mRenderer.transform.localScale.x > 0f)
                ||
                (mTargetRenderer.transform.localScale.x > 0f && mRenderer.transform.localScale.x < 0f))
            {
                mRenderer.transform.localScale = new Vector3(mRenderer.transform.localScale.x * -1f, mRenderer.transform.localScale.y, mRenderer.transform.localScale.z);
            }
        }
    }
}
