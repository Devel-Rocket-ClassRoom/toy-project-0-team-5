using System;

public class ActionNode : BehaviorNode
{
    private readonly Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate() => action();
}
