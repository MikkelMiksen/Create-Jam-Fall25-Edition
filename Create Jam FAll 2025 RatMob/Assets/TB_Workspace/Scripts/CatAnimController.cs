using UnityEngine;

public class CatAnimController : MonoBehaviour
{
    public Animator animator;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        // Only count if we are currently in IdleMain state
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("IdleMain"))
        {
            timer += Time.deltaTime;

            if (timer >= 5f)
            {
                animator.SetTrigger("PlayIdleMove");
                timer = 0f;
            }
        }
        else
        {
            // Reset timer if walking or doing anything else
            timer = 0f;
        }
    }
}
