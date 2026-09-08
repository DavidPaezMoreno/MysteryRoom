using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : MonoBehaviour
{
    public string MotionName = "Idle";
    public Vector2 SpeedRandomRange = new Vector2(1f, 1.5f);
    Animator anim;
    float randomOffset;
    float randomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 1f);
        randomSpeed = Random.Range(SpeedRandomRange.x, SpeedRandomRange.y);

        anim.Play(MotionName, 0, randomOffset);
        anim.speed = randomSpeed;
    }
}
