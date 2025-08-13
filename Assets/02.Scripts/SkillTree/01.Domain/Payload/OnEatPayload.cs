using Fusion;

public class OnEatPayload : ISkillPayload
{
    /// <summary>음식을 먹는 주체</summary>
    public readonly NetworkObject Eater;

    /// <summary>음식의 기본 회복량 (아이템 데이터에 정의된 값)</summary>
    public readonly float BaseRestore;

    /// <summary>현재 배고픔 비율 (0~1)</summary>
    public readonly float HungerRatio;

    /// <summary>음식이 재료(수확물)인지 여부</summary>
    public readonly bool IsIngredient;

    /// <summary>효과 적용 배율 (기본 1.0f)</summary>
    public float Multiplier;

    /// <summary>추가 회복량</summary>
    public float ExtraRestore;

    /// <summary>아군 회복, 버프 등 추가 전달용 값</summary>
    public object ExtraData;

    public OnEatPayload(
        NetworkObject eater,
        float baseRestore,
        float hungerRatio,
        bool isIngredient
    )
    {
        Eater = eater;
        BaseRestore = baseRestore;
        HungerRatio = hungerRatio;
        IsIngredient = isIngredient;

        Multiplier = 1f;
        ExtraRestore = 0f;
        ExtraData = null;
    }

    public string SkillID { get; }
}