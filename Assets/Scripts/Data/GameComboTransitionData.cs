using System;
using UnityEngine;

[Serializable]
public class GameComboTransitionData
{
    [SerializeField] private GameAttackInputType inputType = GameAttackInputType.None;
    [SerializeField] private string nextStepId;

    public GameAttackInputType InputType => inputType;
    public string NextStepId => nextStepId;
}
