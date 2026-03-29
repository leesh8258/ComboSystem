using System;
using UnityEngine;

[Serializable]
public class PlayerComboRuntimeState
{
    [SerializeField] private bool isAttacking;
    [SerializeField] private string currentStepId;
    [SerializeField] private bool hasBufferedStep;
    [SerializeField] private string bufferedNextStepId;

    public bool IsAttacking => isAttacking;
    public string CurrentStepId => currentStepId;
    public bool HasBufferedStep => hasBufferedStep;
    public string BufferedNextStepId => bufferedNextStepId;

    public void BeginStep(GameComboStepData stepData)
    {
        if (stepData == null)
        {
            return;
        }

        isAttacking = true;
        currentStepId = stepData.StepId;
        ClearBufferedStep();
    }

    public void SetBufferedNextStep(string nextStepId)
    {
        if (string.IsNullOrEmpty(nextStepId))
        {
            return;
        }

        hasBufferedStep = true;
        bufferedNextStepId = nextStepId;
    }

    public void ClearBufferedStep()
    {
        hasBufferedStep = false;
        bufferedNextStepId = string.Empty;
    }

    public void Clear()
    {
        isAttacking = false;
        currentStepId = string.Empty;
        ClearBufferedStep();
    }
}