# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Unity **6000.3.11f1** 프로젝트 — 절차적으로 생성되는 방들로 구성된 층(floor)을 탐험하는 탑다운 로그라이크(Isaac 스타일) 게임. URP, 신규 Input System, Unity Test Framework 사용. CLI 빌드/테스트 명령은 설정되어 있지 않으며, 모든 빌드·실행·테스트는 Unity Editor를 통해 수행한다 (Play Mode, `Window → General → Test Runner`).

소스 코드는 전부 [Assets/Scripts/](Assets/Scripts/) 아래에 있다. 씬은 [Assets/Scenes/](Assets/Scenes/)에 있으며, 메인 씬인 [InGameScene.unity](Assets/Scenes/InGameScene.unity)와 함께 `_Copy` / `_jh` 변형 씬들과 [MapTest.unity](Assets/Scenes/MapTest.unity)가 공존한다. 씬에 연결된 참조를 수정할 때는 어떤 씬이 정식 씬인지 먼저 확인할 것.

## 아키텍처

### 이벤트 버스가 중심축

[GameEvents.cs](Assets/Scripts/GameEvents.cs)는 거의 모든 시스템을 느슨하게 연결하는 정적 `Action` 델리게이트 모음이다. 각 시스템은 `OnEnable`에서 구독하고 `OnDisable`에서 해제한다. 시스템 간 통신을 새로 추가할 때는 **직접 참조를 추가하기보다 `GameEvents`를 확장하는 것을 우선**할 것 — 이 프로젝트의 정착된 패턴이다. 주요 채널:

- 플레이어 라이프사이클: `OnPlayerHpChanged`, `OnPlayerMaxHpChanged`, `OnPlayerHit`, `OnPlayerDie`, `OnPlayerUseBomb`
- 아이템: `OnActiveCharged`, `OnActiveUsed`
- 방 / 층 흐름: `OnRoomTransition(spawnPoint, nextRoom)`, `OnTransitionStart/End`, `OnRoomClear`, `OnEnemyDead(GameObject)`, `OnNormalRoomEnter`, `OnShopRoomEnter`, `OnBossRoomEnter(EnemyBase)`, `OnBossHpChanged`

### 층·방 생성 ([Assets/Scripts/Map/](Assets/Scripts/Map/))

- [MapGenerator.cs](Assets/Scripts/Map/MapGenerator.cs) — `Vector2Int` 격자 위에서 BFS로 동작하는 순수 C# 클래스. 시작점에서 가장 먼 노드를 `Boss`로, 인접 노드가 1개뿐인 막다른 방(dead-end)을 `Treasure`와 `Shop`으로 지정. `DoorFlags`가 미리 계산된 `RoomNode` 리스트를 반환한다.
- [FloorManager.cs](Assets/Scripts/Map/FloorManager.cs) — 런타임 소유자. `RoomData` ScriptableObject로부터 **가중치 기반 랜덤 선택**(`RoomData.Weight`)으로 방 프리팹을 인스턴스화하고, `_roomSpacing` 간격으로 월드에 배치하며, 각 `RoomController`에 격자 좌표·방 타입·도어 마스크와 함께 **이웃 방의 타입**까지 전달해 문 모양(Normal/Boss/Golden)을 결정한다.
- [RoomController.cs](Assets/Scripts/Map/RoomController.cs) — 방 단위 상태 머신: 적 리스트 관리, 입장 시 문 잠금, 리스트가 비면 `OnRoomClear` 발행. 문은 N/S/E/W 4개의 `DoorController` 직렬화 참조.
- [RoomTransitionManager.cs](Assets/Scripts/Map/RoomTransitionManager.cs) — `OnRoomTransition`을 듣고 카메라 슬라이드 수행. 슬라이드 동안 플레이어 이동/콜라이더를 비활성화하고, 목적지 스폰 포인트로 텔레포트한 뒤 `_triggerCooldown` 동안 재진입 방지.

방 흐름 디버깅 순서: 문 진입 → `FloorManager.OnPlayerEnterDoor` → `ActivateRoom` → `OnRoomTransition` 이벤트 → `RoomTransitionManager` 코루틴.

### 적 AI는 직접 구현한 Behavior Tree

[Assets/Scripts/BehaviorTree/](Assets/Scripts/BehaviorTree/)에 `BehaviorNode`, `Selector`, `Sequence`, `Action`, `Condition`, `NodeState`가 정의되어 있다. [EnemyBase.cs](Assets/Scripts/Enemy/EnemyBase.cs)는 추상 클래스이며, 각 적(Bat, Bee, Boss, Flower, Skeleton — [Assets/Scripts/Enemy/](Assets/Scripts/Enemy/) 하위)이 `BuildTree()`를 구현하고 매 `Update`마다 `Evaluate()`가 호출된다. 스탯(HP, 속도, 넉백, 탐지 범위, 순찰 주기)과 `_hitSound`/`_deathSound`는 `EnemyBase`에서 직렬화. 공격 동작은 형제 컴포넌트인 `EnemyAttackBase`에 위임된다(전략 패턴, [IAttackStrategy.cs](Assets/Scripts/Interfaces/IAttackStrategy.cs) 참고).

### 플레이어는 컴포넌트 파사드 ([Assets/Scripts/Player/](Assets/Scripts/Player/))

[Player.cs](Assets/Scripts/Player/Player.cs)는 형제 컴포넌트들을 캐싱만 한다: `PlayerHealth`, `PlayerAttack`, `PlayerMovement`, `PlayerAnimator`, `PlayerConsumableItem`. 데미지·회복·최대 HP 등 HP 관련 변경은 전부 `PlayerHealth`를 거쳐 `GameEvents.OnPlayerHpChanged` / `OnPlayerMaxHpChanged`로 전파된다 — **외부에서 HP를 직접 수정하지 말 것**. UI와 아이템은 이벤트만 듣는다.

### 아이템 ([Assets/Scripts/Item/](Assets/Scripts/Item/))

세 카테고리가 각자 베이스 클래스를 가진다: `ActiveItemBase`(충전식, `OnActiveCharged`/`OnActiveUsed` 발행), `PassiveItemBase`, `ConsumableItemBase`. 구체 아이템은 `카테고리.이름.cs` 네이밍을 따른다 (예: `ActiveItem.Bible.cs`, `PassiveItem.Meat.cs`).

### 총알 ([Assets/Scripts/Bullet/](Assets/Scripts/Bullet/))

현재 사용 중인 발사 파이프라인은 [BulletBase.cs](Assets/Scripts/Bullet/BulletBase.cs) + [BulletSpawner.cs](Assets/Scripts/Bullet/BulletSpawner.cs) + [BulletConfig.cs](Assets/Scripts/Bullet/BulletConfig.cs) 조합이다 (커밋 `9a02b88` — "EnemyBullet 스크립트 재작성 및 BulletBase 도입"). [BulletPool.cs](Assets/Scripts/Bullet/BulletPool.cs)와 구버전 [Bullet.cs](Assets/Scripts/Bullet/Bullet.cs)도 남아 있지만, [PlayerAttack.cs](Assets/Scripts/Player/PlayerAttack.cs)에서 `bulletPool?.Initialize / ?.GetBullet`처럼 null-conditional로만 호출되며 씬에서는 참조가 비어 있어 **실질적으로 미사용**이다. 풀링 기반으로 다시 가려면 인스펙터 와이어링부터 확인할 것.

### UI ([Assets/Scripts/UI/](Assets/Scripts/UI/))

[UIManager.cs](Assets/Scripts/UI/UIManager.cs)가 인게임/일시정지/게임오버 윈도우와 보스 바를 관리한다. 일시정지는 `Time.timeScale`로 처리하고, 입력은 직렬화된 `InputManager`의 `PausePressed`로 게이팅한다. 하트 패널은 플레이어 HP 이벤트를 듣고, 보스 UI는 `OnBossRoomEnter`에서 노출된다.

## 컨벤션

- **구독 패턴**: `OnEnable`의 `GameEvents.X += Handler`와 `OnDisable`의 `-= Handler`를 항상 쌍으로 둘 것. 이미 여러 파일이 이렇게 되어 있으니 따를 것 — 에디터의 플레이 루프에서 핸들러가 누수되는 것을 막는다.

## 협업
- **커밋 메시지는 한국어**가 기본 (`Feat:`, `브금/체력UI용 이벤트들 추가` 등). 커밋할 때 기존 스타일을 따를 것. `Feat:` 외에 `Refactor:`도 사용된다.
- **협업 흐름은 GitHub PR 기반**. 작업은 `feature/#<이슈번호>-<짧은-이름>` 브랜치에서 진행하고 (예: `feature/#22-CharacterMovement`, `feature/#82-bullet-sounds`), 완료되면 `main`에 PR을 열어 머지한다. 머지 커밋 메시지는 GitHub 기본 형식 (`Merge pull request #NN from ...`). 단, 최근 일부 커밋은 `main`에 직접 푸시된 것도 있다 — 새 기능은 브랜치 + PR 흐름을 우선할 것. 작성자 핸들이 사람마다 여러 개 (`JihooKim`/`kjh3357`/`김지후`, `Jaeseo Lee`/`n0wst4ndup`/`이재서`, `김지해`/`gamjaHoo`) 섞여 있으므로 `git log --author`로 거를 때 주의.
- 프로덕션 코드와 함께 테스트/샌드박스 파일이 공존한다: [MapTestDriver.cs](Assets/Scripts/Map/MapTestDriver.cs), [PropTestDriver.cs](Assets/Scripts/Prop/PropTestDriver.cs), [Assets/Scripts/Tests/HPTest.cs](Assets/Scripts/Tests/HPTest.cs). 죽은 코드로 단정하고 삭제하지 말 것 — 대응되는 `*Test*` 씬에서 사용한다.
- `RoomData`는 `Weight` 필드를 가진 ScriptableObject. 가중치 0인 항목은 후보가 그것뿐일 때를 제외하면 무시된다.