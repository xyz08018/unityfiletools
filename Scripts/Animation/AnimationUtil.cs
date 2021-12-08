using UnityEngine;

public class AnimationUtil : MonoBehaviour
{
    Animation animation;
    Animator animator;
    AnimationState animaState;
    AnimatorStateInfo animaInfo;
    void Start()
    {
        animaInfo = animator.GetCurrentAnimatorStateInfo(0);
        var loop = animaInfo.loop;
        var length = animaInfo.length;
        var speed = animaInfo.speed;
        Debug.Log(string.Format("loop{0}length{1}speed{2}", loop, length, speed));
    }
    void Update()
    {

    }
}
