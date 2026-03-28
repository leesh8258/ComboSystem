using System;
using UnityEngine;

[Serializable]
public class GameComboStepData
{
    [SerializeField] private GameAttackInputType requiredInput = GameAttackInputType.LeftClick;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField, Range(0f, 1f)] private float bufferOpenNormalizedTime = 0.3f;
    [SerializeField, Range(0f, 1f)] private float bufferCloseNormalizedTime = 0.8f;

    public GameAttackInputType RequiredInput => requiredInput;
    public AnimationClip AnimationClip => animationClip;
    public float BufferOpenNormalizedTime => bufferOpenNormalizedTime;
    public float BufferCloseNormalizedTime => bufferCloseNormalizedTime;
}