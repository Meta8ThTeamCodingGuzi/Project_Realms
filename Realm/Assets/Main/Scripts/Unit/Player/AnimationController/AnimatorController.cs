using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Unit owner;

    [SerializeField] private List<RuntimeAnimatorController> AnimaContoller;


    private AnimatorOverrideController currentController;

    public void IsInitialized(Unit owner)
    {
       this.owner = owner;
    }

    private AnimatorOverrideController SetupOverrideController()
    {
        RuntimeAnimatorController Controller = owner.Animator.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(Controller);
        owner.Animator.runtimeAnimatorController = overrideController;
        return overrideController;
    }


    public void AnimatorChange(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.None:
                owner.ChangeAnimController(AnimaContoller[0]);
                break;
            case ItemType.Sword:
                owner.ChangeAnimController(AnimaContoller[1]);
                break;
            case ItemType.Bow:
                owner.ChangeAnimController(AnimaContoller[2]);
                break;
            default:
                return;
        }
    }


    public void Clipchange(AnimationClip animationClip)
    {
        if (currentController == null ||
            currentController.runtimeAnimatorController != owner.Animator.runtimeAnimatorController)
        {
            currentController = SetupOverrideController();
        }

        if (animationClip != null)
        {
            currentController["Attack"] = animationClip;
        }
    }
}
