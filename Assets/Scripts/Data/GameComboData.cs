using System;
using UnityEngine;

[Serializable]
public class GameComboData
{
    [SerializeField] private GameComboTransitionData[] entryTransitions = Array.Empty<GameComboTransitionData>();
    [SerializeField] private GameComboStepData[] steps = Array.Empty<GameComboStepData>();

    public GameComboTransitionData[] EntryTransitions => entryTransitions;
    public GameComboStepData[] Steps => steps;
    public int StepCount => steps != null ? steps.Length : 0;

    public bool TryGetEntryStepId(GameAttackInputType inputType, out string stepId)
    {
        stepId = string.Empty;

        if (entryTransitions == null || entryTransitions.Length == 0) return false;

        for (int i = 0; i < entryTransitions.Length; i++)
        {
            GameComboTransitionData transition = entryTransitions[i];
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

            stepId = transition.NextStepId;
            return true;
        }

        return false;
    }

    public bool TryGetStep(string stepId, out GameComboStepData stepData)
    {
        stepData = null;

        if (string.IsNullOrEmpty(stepId))
        {
            return false;
        }

        if (steps == null || steps.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < steps.Length; i++)
        {
            GameComboStepData step = steps[i];
            if (step == null)
            {
                continue;
            }

            if (step.StepId != stepId)
            {
                continue;
            }

            stepData = step;
            return true;
        }

        return false;
    }
}