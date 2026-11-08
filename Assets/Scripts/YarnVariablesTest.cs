using UnityEngine;

public class YarnVariablesTest : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.PrintYarnVariable("$variable");
    }
}
