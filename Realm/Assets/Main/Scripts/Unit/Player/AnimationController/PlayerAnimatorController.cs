using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController none;
    [SerializeField] private RuntimeAnimatorController knightControllers;
    [SerializeField] private RuntimeAnimatorController archerControllers;

    private AnimatorOverrideController currentController;

    private AnimatorOverrideController SetupOverrideController()
    {
        RuntimeAnimatorController Controller = GameManager.Instance.player.PlayerAnimator.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(Controller);
        GameManager.Instance.player.PlayerAnimator.runtimeAnimatorController = overrideController;
        return overrideController;
    }


    public void AnimatorChange(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.None:
                GameManager.Instance.player.PlayerAnimatorChange(none);
                break;
            case ItemType.Sword:
                GameManager.Instance.player.PlayerAnimatorChange(knightControllers);
                break;
            case ItemType.Bow:
                GameManager.Instance.player.PlayerAnimatorChange(archerControllers);
                break;
            default:
                return;
        }
    }


    public void Clipchange(AnimationClip animationClip)
    {
        if (currentController == null ||
            currentController.runtimeAnimatorController != GameManager.Instance.player.PlayerAnimator.runtimeAnimatorController)
        {
            currentController = SetupOverrideController();
        }

        if (animationClip != null)
        {
            currentController["Attack"] = animationClip;
        }
    }
}
