# Unity_3D_GameProject

##  핵심 코드 구조 (Click to view)
이 프로젝트의 주요 로직은 아래 스크립트에서 확인할 수 있습니다.

### 캐릭터
* [PlayerInputManager.cs](Assets/Scripts/Player/PlayerInputManager.cs)  - 유저 입력 처리
* [PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)      - 플레이어 캐릭터 이동 로직

### 플레이어 능력
* [PlayerAbility.cs](Assets/Scripts/Player/PlayerAbility.cs)        - 아이템 처리, 경험치 및 레벨 관리, 무기 레벨 관리
* [PlayerWeapon.cs](Assets/Scripts/Player/PlayerWeapon.cs)          - 무기 관리자 소유

### 무기 관리자
* [BulletWeaponManager.cs](Assets/Scripts/Weapon/Manager/BulletWeaponManager.cs)                - 투사체 무기 관리자
* [OrbitWeaponManager.cs](Assets/Scripts/Weapon/Manager/OrbitWeaponManager.cs)                  - 회전 무기 관리자
* [SwordWeaponManager.cs](Assets/Scripts/Weapon/Manager/SwordWeaponManager.cs)                  - 검격(근접 공격) 무기 관리자
* [ThunderStrikeWeaponManager.cs](Assets/Scripts/Weapon/Manager/ThunderStrikeWeaponManager.cs)  - 낙뢰(광역 범위) 무기 관리자

### 무기
* [BulletWeapon.cs](Assets/Scripts/Weapon/BulletWeapon.cs)                               - 투사체 무기
* [OrbitWeapon.cs](Assets/Scripts/Weapon/OrbitWeapon.cs)                                 - 회전 무기
* [SwordWeapon.cs](Assets/Scripts/Weapon/SwordWeapon.cs)                                 - 검격(근접 공격) 무기
* [ThunderStrikeWeapon.cs](Assets/Scripts/Weapon/ThunderStrikeWeapon.cs)                 - 낙뢰(광역 범위) 무기

### 몬스터
* [MonsterController.cs](Assets/Scripts/Monster/MonsterController.cs)   - 몬스터 움직임 관리 및 상태 헬퍼 함수
* [ChaseState.cs](Assets/Scripts/Monster/ChaseState.cs)                 - Chase 상태
* [ClimbState.cs](Assets/Scripts/Monster/ClimbState.cs)                 - Climb 상태
* [HitState.cs](Assets/Scripts/Monster/HitState.cs)                     - Hit 상태
* [JumpState.cs](Assets/Scripts/Monster/JumpState.cs)                   - Jump 상태
* [JumpingState.cs](Assets/Scripts/Monster/JumpingState.cs)             - Jumping 상태

### 데이터
* [CsvImporter.cs](Assets/Scripts/Editor/CsvImporter.cs)                - 리플렉션을 활용한 제네릭 CSV 임포터
* [AbilityData.csv](Assets/Scripts/GameData/AbilityData.csv)            - 능력 데이터 CSV
* [AbilityDataBaseSO.cs](Assets/Scripts/GameData/AbilityDataBaseSO.cs)  - 스크립터블 오브젝트를 활용한 데이터 관리

### 모듈
* [GameAbilityManager.cs](Assets/Scripts/Manager/GameAbilityManager.cs)             - Ability 시스템 모듈
* [WaveManager.cs](Assets/Scripts/Manager/WaveManager.cs)                           - 몬스터 Wave 시스템 모듈
* [MonsterSpawner.cs](Assets/Scripts/Manager/MonsterSpawner.cs)                     - 몬스터 생성 관리 모듈
* [MonsterInstacingManager.cs](Assets/Scripts/Manager/MonsterInstacingManager.cs)   - 몬스터 인스턴싱 렌더링 모듈
