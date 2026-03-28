using System;
using UnityEngine;

[Serializable]
public class GameComboData
{
    [SerializeField] private GameComboStepData[] steps = Array.Empty<GameComboStepData>();

    public GameComboStepData[] Steps => steps;
    public int StepCount => steps != null ? steps.Length : 0;

    public GameComboStepData GetStep(int index)
    {
        return steps[index];
    }
}