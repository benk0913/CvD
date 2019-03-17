using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    #region Configurable

    [SerializeField]
    protected float FollowSpeed;

    [SerializeField]
    float AddedY = 1f;

    [SerializeField]
    protected GameObject followingObject;

    [SerializeField]
    protected CameraType CamType = CameraType.Normal;

    public bool CamFollowing = true;

    #endregion

    Vector3 dampRef;

    protected Vector3 initPos;
    protected Vector3 targetPos;

    public static Vector3 MousePosition;

    float InitCamSize;

    void Start()
    {
        initPos = transform.position;

        CORE.Instance.CurrentRoom.Settings.RegisterEntity(this, "Camera");
    }

    void FixedUpdate()
    {
        if (CamFollowing)
        {
            if (followingObject != null)
            {
                if (this.CamType == CameraType.Horizontal)
                {
                    targetPos = new Vector3(followingObject.transform.position.x, transform.position.y + AddedY, initPos.z);
                }
                else if (this.CamType == CameraType.Vertical)
                {
                    targetPos = new Vector3(transform.position.x, followingObject.transform.position.y + AddedY, initPos.z);
                }
                else if (this.CamType == CameraType.Static)
                {
                    targetPos = initPos;
                }
                else
                {
                    targetPos = new Vector3(followingObject.transform.position.x, followingObject.transform.position.y + AddedY, initPos.z);
                }

                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref dampRef, FollowSpeed);
            }
        }
    }

    public void SetFollowTarget(GameObject target)
    {
        followingObject = target;
    }

    public enum CameraType
    {
        Normal, Vertical, Horizontal, Static
    }

}
