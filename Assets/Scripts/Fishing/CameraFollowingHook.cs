using UnityEngine;

public class CameraFollowingHook : MonoBehaviour
{
    private Transform hookTransform;
    private Hook hookScript;
    private Vector3 startingPos;
    private bool following = false;

    private void Start()
    {
        startingPos = transform.position;
        hookScript = FindFirstObjectByType<Hook>();
        hookTransform = hookScript.transform;
    }

    private void LateUpdate()
    {
        if (hookScript.IsInTheSea())
        {
            float posY = hookTransform.position.y;
            if (!following && posY <= transform.position.y)
                following = true;
            else if (following && posY >= startingPos.y)
            {
                following = false;
                transform.position = startingPos;
            }

            if (following)
                transform.position = new Vector3(0, hookTransform.position.y, -10);
        }
    }
}
