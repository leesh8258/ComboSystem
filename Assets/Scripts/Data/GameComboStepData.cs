using System;
using UnityEngine;

[Serializable]
public class GameComboStepData
{
    [SerializeField] private string stepId;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField, Range(0f, 1f)] private float bufferOpenNormalizedTime = 0.3f;
    [SerializeField, Range(0f, 1f)] private float bufferCloseNormalizedTime = 0.8f;
    [SerializeField] private GameComboTransitionData[] transitions = Array.Empty<GameComboTransitionData>();

    public string StepId => stepId;
    public AnimationClip AnimationClip => animationClip;
    public float BufferOpenNormalizedTime => bufferOpenNormalizedTime;
    public float BufferCloseNormalizedTime => bufferCloseNormalizedTime;
    public GameComboTransitionData[] Transitions => transitions;

    public bool IsBufferWindowOpen(float normalizedTime)
    {
        return normalizedTime >= bufferOpenNormalizedTime && normalizedTime <= bufferCloseNormalizedTime;
    }

    public bool TryGetNextStepId(GameAttackInputType inputType, out string nextStepId)
    {
        nextStepId = string.Empty;

        if (transitions == null || transitions.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < transitions.Length; i++)
        {
            GameComboTransitionData transition = transitions[i];
            if (transition == null)
            {
                continue;
            }

            if (transition.InputType != inputType)
            {
                continue;
            }

            if (string.IsNullOrEmpty(transition.NextStepId))
            {
                continue;
            }

            nextStepId = transition.NextStepId;
            return true;
        }

        return false;
    }
}