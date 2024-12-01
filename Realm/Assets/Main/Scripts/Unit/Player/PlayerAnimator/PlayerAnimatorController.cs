using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator plainsAnimator;
    private AnimatorOverrideController controller;

    [SerializeField] private RuntimeAnimatorController knightControllers;
    [SerializeField] private RuntimeAnimatorController archerControllers;

    [SerializeField] private AnimationClip Testclip;
    [SerializeField] private AnimationClip Testclip2;


    public void Initialize()
    {
        plainsAnimator = GameManager.Instance.player.PlayerAnimator;
        SetupOverrideController();
    }

    private void SetupOverrideController()
    {
        controller = new AnimatorOverrideController(plainsAnimator.runtimeAnimatorController);
        plainsAnimator.runtimeAnimatorController = controller;
    }

    //enum으로 받아서 스위치로 구현해야지
    public void PlayerAnimatorChange()
    {
        GameManager.Instance.player.PlayerAnimatorChange(knightControllers);


        GameManager.Instance.player.PlayerAnimatorChange(archerControllers);

        SetupOverrideController();
    }

    public void aniclip1()
    {
        controller["Attack"] = Testclip;
    }
    public void aniclip2()
    {
        controller["Attack"] = Testclip2;
    }
}
