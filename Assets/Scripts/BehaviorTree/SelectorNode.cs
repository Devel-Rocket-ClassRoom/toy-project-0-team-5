// OR 노드. 자식을 순서대로 실행하다가 Success 또는 Running이 나오면 즉시 반환한다.
// 모든 자식이 Failure를 반환해야만 이 노드도 Failure를 반환한다.
//
// 사용 예시 — 우선순위 행동 선택:
//   SelectorNode
//     ├── Sequence(죽음 조건 → 죽음 액션)   ← 최우선
//     ├── Sequence(공격 조건 → 공격 액션)
//     └── 순찰 액션                          ← 기본 행동 (fallback)
//
// FSM의 '상태 전이 우선순위'와 비슷하지만, 매 프레임 위에서부터 다시 평가한다는 점이 다르다.
public class SelectorNode : CompositeNode
{
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Failure) return result;
        }
        return NodeState.Failure;
    }
}
