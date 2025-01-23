using UnityEngine;

public static class Util
{
    public static AnimationClip GetAnimationClip(Animator animator, string name)
    {
        if (animator == null)
        {
            DebugTool.Error("animatorÎª¿Õ");
        }
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
}