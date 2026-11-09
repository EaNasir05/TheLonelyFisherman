using UnityEngine;

public class DrawSecondLine : MonoBehaviour
{
    /*
    private Hook hookScript;
    private LineRenderer line;

    void Start()
    {
        hookScript = gameObject.GetComponent<Hook>();
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = (float)0.1;
        line.endWidth = (float)0.1;
        line.sortingOrder = -2;
    }

    void Update()
    {
        if (hookScript.IsInTheSea())
        {
            Vector3 contactPoint = hookScript.GetContactPointReturn();
            line.SetPosition(0, new Vector3(contactPoint.x, contactPoint.y, contactPoint.z));
            line.SetPosition(1, transform.position);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }
    */
}
