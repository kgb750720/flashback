using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniStateInfoManager : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo animatorInfo;
    string stateName;
    float aniStateSpeed;
    int layer;
    float lastSpeed;
    bool isStart;
 
    
    
    public void SetAniSpeed(Animator anim, string stateName, int layer, float aniStateSpeed)
    {
        this.anim = anim;
        this.stateName = stateName;
        this.layer = layer;
        this.aniStateSpeed = aniStateSpeed;
        lastSpeed = anim.speed;
        isStart = true;
    }

    public void ResetAllSpeed()
    {
        if(anim!=null) anim.speed = 1;
        isStart = false;
    }
 
    void Update()
    {
        if (isStart)
        {
            animatorInfo = anim.GetCurrentAnimatorStateInfo(layer);
            if (animatorInfo.IsName(stateName))//注意这里指的不是动画的名字而是动画状态的名字
            {
                anim.speed = aniStateSpeed;
            }
            else
            {
                anim.speed = 1;
            }
        }
    }

    private void OnDestroy()
    {
        if (anim != null) anim.speed = 1;
    }
}
