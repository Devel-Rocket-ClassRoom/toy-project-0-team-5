// BT의 모든 노드가 상속받는 기반 클래스.
// 매 프레임 EnemyBase.Update()에서 루트 노드의 Evaluate()를 호출하면
// 트리 전체가 위→아래 순서로 평가된다.
public abstract class BehaviorNode
{
    public abstract NodeState Evaluate();
}
