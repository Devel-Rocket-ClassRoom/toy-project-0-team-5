// AND 노드. 자식을 순서대로 실행하다가 Failure 또는 Running이 나오면 즉시 반환한다.
// 모든 자식이 Success를 반환해야만 이 노드도 Success를 반환한다.
//
// 사용 예시 — 조건 + 액션 묶음:
//   SequenceNode
//     ├── ConditionNode(공격 범위 안에 있는가?)  ← 조건이 Failure면 여기서 중단
//     └── ActionNode(공격 실행)                  ← 조건 통과 시에만 실행
//
// FSM의 '전이 조건 + 상태 진입'을 하나의 묶음으로 표현한 것과 같다.
public class SequenceNode : CompositeNode
{
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Success) return result;
        }
        return NodeState.Success;
    }
}
