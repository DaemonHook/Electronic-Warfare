using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTile : GameTile
{
    private void Awake()
    {
        Animator ani = GetComponent<Animator>();
        AnimationEvent aniEvent = new AnimationEvent();
        aniEvent.functionName = "OnEffectComplete";
        AnimationClip animationClip = ani.runtimeAnimatorController.animationClips[0];
        float animationEndTime = animationClip.length;
        aniEvent.time = animationEndTime;
        animationClip.AddEvent(aniEvent);
    }

    void OnEffectComplete()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
