using System;

// 조건을 평가하는 leaf 노드. bool을 반환하는 함수를 받아 Success/Failure로 변환한다.
// SequenceNode의 첫 번째 자식으로 주로 사용되어, 조건이 거짓이면 이후 액션을 차단한다.
public class ConditionNode : BehaviorNode
{
    private readonly Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Evaluate() => condition() ? NodeState.Success : NodeState.Failure;
}
