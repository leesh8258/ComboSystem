# 콤보 시스템
<img width="49%" height="auto" alt="콤보시스템" src="https://github.com/user-attachments/assets/9f002f58-bb93-456d-8974-01df57218bf8" />
<img width="49%" height="auto" alt="콤보시스템2" src="https://github.com/user-attachments/assets/f078c1dc-9dcd-453b-8855-6255e7e8143c" />

- 무기 별 Animation 행동 프리셋 및 행동별 데미지 배율 기능을 구현한 개인 프로젝트 입니다

- 무료 에셋만 사용한 점을 밝힙니다

## 1. 기능 소개
- 소개
    - "**무기별 콤보 프리셋을 만들어보자**" 로 시작한 토이 프로젝트 입니다
    - 각 무기는 ScriptableObject 데이터에 정보가 담겨져 있으며, 무기마다 사용 가능한 콤보가 다르도록 구현 했습니다

- 개발 일자: 2026.03.30 ~ 2026.04.02
- 개발 환경: C#, Unity 3D

## 2. 플레이 영상
https://youtu.be/lFkzOvs89Y0?si=V2MaZ0E-qvfDESRE

## 3. 핵심 기능 소개

### 1. 무기 별 SO 데이터
<img width="29.8%" height="auto" alt="image" src="https://github.com/user-attachments/assets/7b992648-4894-43ce-9b03-24c6af4d0b81" />
<img width="30%" height="auto" alt="image" src="https://github.com/user-attachments/assets/87d517a2-2e30-4a74-a814-3739e4ed993c" />

- 각 무기 SO에 담긴 데이터
  - Weapon Prefab
  - Transform Offset
  - Base Damage
  - Combo Data

### 2. 콤보 분기 시스템
<details open>
  <summary>콤보 버퍼 함수</summary>
  
```csharp
//PlayerActionController.cs
private void TryBufferNextStep(GameAttackInputType inputType)
{
    if (runtimeState.HasBufferedStep) return;

    GameComboData comboData = GetCurrentComboData();
    if (comboData == null) return;
    if (!comboData.TryGetStep(runtimeState.CurrentStepId, out GameComboStepData currentStepData) || !animationController.IsPlayingAttackState()) return;

    float normalizedTime = animationController.GetAttackNormalizedTime();
    if (!currentStepData.IsBufferWindowOpen(normalizedTime) || !currentStepData.TryGetNextStepId(inputType, out string nextStepId)) return;

    runtimeState.SetBufferedNextStep(nextStepId);
}
```
</details>

- 고정 타수 방식이 아닌 단계 간 전이 구조로 설계하여, 무기별로 다양한 콤보 루트와 시작 패턴을 ScriptableObject에서 유연하게 확장할 수 있도록 제작

- 좌클릭 / 우클릭 입력을 GameAttackInputType으로 받아 SO 내 데이터와 비교하여 콤보 분기 흐름을 구현

- 현재 공격중일 때 다음 입력을 즉시 실행하지 않고, 입력 버퍼에 입력을 저장했다가 의도한 타이밍에 다음 행동이 실행되도록 구성

- SO 내 행동 데이터를 **그래프 형식** 으로 구성하여 콤보로 1타를 진행했더라도 2타/3타에서 다시 1타로 돌아갈 수 있는 유연한 구조

### 3. 단일 Attack 상태 + 런타임 애니메이션 교체
<details open>
  <summary>공격 애니메이션 함수 일부</summary>
  
```csharp
//PlayerAnimationController.cs
public void PlayAttackClip(AnimationClip clip)
{
    if (!CanPlayAttackClip(clip)) return;

    ApplyAttackClipOverride(clip);
    animator.Play(attackStateName, 0, 0f);
}

private bool CanPlayAttackClip(AnimationClip clip)
{
    if (animator == null || attackPlaceholderClip == null || clip == null) return false;

    CreateRuntimeOverrideController();
    return runtimeOverrideController != null;
}

private void CreateRuntimeOverrideController()
{
    if (animator == null || runtimeOverrideController != null) return;

    RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
    if (baseController == null) return;

    runtimeOverrideController = new AnimatorOverrideController(baseController);
    animator.runtimeAnimatorController = runtimeOverrideController;
}

```
</details>

- Animator에 공격 상태를 여러 개 두는 대신, Attack 상태 하나만 두고 공격 시점마다 **AnimatorOverrideController**로 현재 단계의 애니메이션 클립을 덮어씌우는 방식으로 구현

- 애니메이터 상태 수를 늘리지 않고도 무기마다 완전히 다른 공격 모션을 재생할 수 있도록 구성

- 무기 장착/해제 시에는 기본 placeholder 클립으로 초기화해 상태 꼬임을 방지함.

## 4. 트러블 슈팅
### 1. Animator Controller 관리
- 문제 상황
  - 초기에는 무기별 공격 타수를 애니메이터 노드 구조로 직접 나누어 관리하려고 했음. 예를 들어 3타 무기를 위해서는 3개의 공격 노드, 4타 무기를 위해서는 4개의 공격 노드를 따로 만들어 두고, 각 노드에 무기별 애니메이션을 오버라이드하는 방식으로 접근했으나 금방 한계를 느꼈습니다.
  - 즉 **콤보 구조를 데이터가 아니라 애니메이터 상태 수에 의존하고 있다는 점**이 가장 큰 문제임을 인식했습니다.

- 해결 방식
  - “무기마다 몇 타까지 가능한가”를 애니메이터가 아니라 코드와 데이터가 결정하도록 변경
  - **Attack 상태를 단 1개만 유지**하도록 단순화

- 결과
  - 무기별 입력 조합과 콤보 분기를 **데이터로 관리**할 수 있어 확장성이 높아짐
  - 새로운 무기를 추가할 때 애니메이터를 계속 수정하지 않아도 되어 **유지보수가 쉬워짐**
  - 3타 무기, 4타 무기처럼 **타수가 다른 무기도 같은 구조 안에서 처리** 가능해짐

### 2. 무기 다단히트
- 문제 상황
  - 단순 무기 콜라이더 충돌 OnTriggerEnter / OnTriggerStay 의 지속적인 호출로 한 번의 공격에서 같은 적에게 여러번 데미지가 들어가는 문제 발생
  - 또한 히트박스가 켜져 있어 공격 애니메이션이 아닌데도 콜라이더가 닿아 있으면 데미지가 들어가는 문제 또한 발생

- 해결 방식
  - 공격 흐름에 따라 **Collider를 활성/비활성하는 전투 판정 시스템**을 생각
  - 또한 공격마다 **attackSequenceId** 와 **HashSet**을 활용하여 **"맞은 적 목록"** 관리 하는 시스템 설계

- 결과
  - 공격 타이밍 외에는 데미지가 들어가지 않게 구현
  - 한 번의 공격에서 같은 적이 여러 번 피격되는 문제를 방지

## 5. 사용 에셋
- Mixamo (YBot, Maria W/Prop J J Ong) : https://www.mixamo.com/#/
- RPG_Animations_Pack_FREE: https://assetstore.unity.com/packages/3d/animations/rpg-animations-pack-free-288783
- Low Poly Swords - RPG Weapons: https://assetstore.unity.com/packages/3d/props/weapons/free-low-poly-swords-rpg-weapons-198166
- Prototype Map: https://assetstore.unity.com/packages/3d/environments/prototype-map-315588
