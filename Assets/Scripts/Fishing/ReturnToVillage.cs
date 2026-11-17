using UnityEngine;

public class ReturnToVillage : MonoBehaviour
{
    public void ExitFishingScene(string sceneName)
    {
        GameManager.instance.ChangeScene(sceneName);
    }
}
