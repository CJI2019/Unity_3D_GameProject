# Unity_3D_GameProject

##  핵심 코드 구조 (Click to view)
이 프로젝트의 주요 로직은 아래 스크립트에서 확인할 수 있습니다.

### 1. 캐릭터
* [PlayerController.cs](Assets/Scripts/Player/PlayerController.cs) - 플레이어 캐릭터 입력 처리

### 2. 데이터
* [AbilityDataBaseSO.cs](Assets/Scripts/GameData/AbilityDataBaseSO.cs) - 스크립터블 오브젝트를 활용한 데이터 관리
* [CsvImporter.cs](Assets/Scripts/GameData/CsvImporter.cs) - 리플렉션을 활용한 제네릭 CSV 임포터

### 3. 모듈
* [GameAbilityManager.cs](Assets/Scripts/GameData/GameAbilityManager.cs) - 싱글턴 / Ability 시스템 모듈