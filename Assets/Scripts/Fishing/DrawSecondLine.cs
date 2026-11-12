using UnityEngine;

public class DrawSecondLine : MonoBehaviour
{
    private Hook hookScript;
    private LineRenderer line;
    [SerializeField] private float lineStartWidth = 0.1f;
    [SerializeField] private float lineEndWidth = 0.1f;

    void Start()
    {
        hookScript = gameObject.GetComponent<Hook>();
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = lineStartWidth;
        line.endWidth = lineEndWidth;
        //line.sortingOrder = -2;
    }

    void Update()
    {
        if (hookScript.IsInTheSea())
        {
            Vector3 contactPoint = hookScript.GetContactPoint();
            line.SetPosition(0, new Vector3(contactPoint.x, contactPoint.y, contactPoint.z));
            line.SetPosition(1, transform.position);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }
}
