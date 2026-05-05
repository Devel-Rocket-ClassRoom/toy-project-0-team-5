using System;

// 실제 행동을 실행하는 leaf 노드. NodeState를 반환하는 함수를 받아 그대로 전달한다.
// Running을 반환하면 다음 프레임에도 SelectorNode가 이 노드를 다시 선택하게 된다.
public class ActionNode : BehaviorNode
{
    private readonly Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate() => action();
}
