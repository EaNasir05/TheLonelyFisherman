using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform hook;
    [SerializeField] private float lineStartWidth = 0.1f;
    [SerializeField] private float lineEndWidth = 0.1f;
    private LineRenderer line;
    private Hook hookScript;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        hookScript = hook.gameObject.GetComponent<Hook>();
        line.positionCount = 2;
        line.startWidth = lineStartWidth;
        line.endWidth = lineEndWidth;
        //line.sortingOrder = -2;
    }

    private void Update()
    {
        if (!hookScript.IsInTheSea())
        {
            line.SetPosition(0, startPoint.position);
            line.SetPosition(1, hook.position);
        }
        else
        {
            line.SetPosition(0, startPoint.position);
            line.SetPosition(1, hookScript.GetContactPoint());
        }
    }
}
