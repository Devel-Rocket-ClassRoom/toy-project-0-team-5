using System.Collections.Generic;

// 자식 노드를 여러 개 가질 수 있는 복합 노드의 기반 클래스.
// 자식을 어떤 순서/조건으로 실행할지는 SelectorNode / SequenceNode가 각각 정의한다.
// AddChild()가 CompositeNode를 반환하므로 메서드 체이닝으로 트리를 구성할 수 있다.
public abstract class CompositeNode : BehaviorNode
{
    protected readonly List<BehaviorNode> children = new();

    public CompositeNode AddChild(BehaviorNode child)
    {
        children.Add(child);
        return this;
    }
}
