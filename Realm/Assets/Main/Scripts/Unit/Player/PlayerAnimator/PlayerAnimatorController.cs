using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    [SerializeField] private RuntimeAnimatorController knightControllers;
    [SerializeField] private RuntimeAnimatorController archerControllers;


    private AnimatorOverrideController SetupOverrideController(Player target)
    {
        RuntimeAnimatorController Controller = target.PlayerAnimator.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(Controller);
        target.PlayerAnimator.runtimeAnimatorController = overrideController;
        return overrideController;
    }

    public void AnimatorChange(Player player)
    {
        switch (player.playerjob)
        {
            case Playerjob.knight:
                 player.PlayerAnimatorChange(knightControllers);
                break;
            case Playerjob.Archer:
                player.PlayerAnimatorChange(archerControllers);
                break;
            default:
                return;
        }
    }

    public void clipchange(Player player, AnimationClip animationClip)
    {
        AnimatorOverrideController controller = SetupOverrideController(player);
        controller["Attack"] = animationClip;
    }
}
