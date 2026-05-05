// BT(Behavior Tree)의 각 노드가 Evaluate() 후 반환하는 상태값.
// FSM과 달리 BT는 매 프레임 트리 전체를 다시 평가하며, 이 값으로 흐름을 제어한다.
public enum NodeState
{
    Success, // 노드 실행 성공 — 부모 노드가 다음 단계로 진행
    Failure, // 노드 실행 실패 — 부모 노드가 대안을 탐색
    Running  // 노드가 아직 실행 중 — 다음 프레임에도 이 노드를 유지
}
