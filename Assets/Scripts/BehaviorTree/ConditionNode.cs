using System;

public class ConditionNode : BehaviorNode
{
    private readonly Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Evaluate() => condition() ? NodeState.Success : NodeState.Failure;
}
