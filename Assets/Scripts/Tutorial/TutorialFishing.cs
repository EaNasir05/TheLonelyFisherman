using UnityEngine;

public class TutorialFishing : MonoBehaviour
{
    private Hook hook;

    private void Start()
    {
        hook = FindFirstObjectByType<Hook>();
        if (GameManager.instance.GetDay() == 0)
        {
            if (GameManager.instance.GetCurrentScene() != Scene.Fishing)
            {
                UIManager.instance.onFadeIn += Tutorial1;
                hook.onEnterTheSea += Tutorial2;
                hook.onStartReturning += Tutorial3;
                hook.onExitTheSea += Tutorial4;
            }
            else
            {
                UIManager.instance.onFadeIn += ExtraDialogue;
            }
        }
        else if (GameManager.instance.GetDay() == 2)
        {
            UIManager.instance.onFadeIn += Tutorial5;
            hook.onEnterTheSea += Tutorial6;
            hook.onStartReturning += Tutorial7;
            hook.onExitTheSea += Tutorial8;
        }
    }

    private void ExtraDialogue()
    {
        GameManager.instance.tutorialState = 3;
        DialoguesManager.instance.StartDialogue("PostTutorial");
    }

    private void Tutorial1()
    {
        GameManager.instance.tutorialState = 1;
        DialoguesManager.instance.StartDialogue("Tutorial1");
    }

    private void Tutorial2()
    {
        DialoguesManager.instance.StartDialogue("Tutorial2");
    }

    private void Tutorial3()
    {
        DialoguesManager.instance.StartDialogue("Tutorial3");
    }

    private void Tutorial4()
    {
        GameManager.instance.tutorialState = 2;
        DialoguesManager.instance.StartDialogue("Tutorial4");
    }

    private void Tutorial5()
    {
        GameManager.instance.tutorialState = 4;
        DialoguesManager.instance.StartDialogue("TutorialMagic1");
    }

    private void Tutorial6()
    {
        DialoguesManager.instance.StartDialogue("TutorialMagic2");
    }

    private void Tutorial7()
    {
        DialoguesManager.instance.StartDialogue("TutorialMagic3");
    }

    private void Tutorial8()
    {
        GameManager.instance.tutorialState = 5;
        DialoguesManager.instance.StartDialogue("TutorialMagic4");
    }
}
