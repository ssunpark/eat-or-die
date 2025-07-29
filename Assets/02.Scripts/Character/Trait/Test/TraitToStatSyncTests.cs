#if UNITY_EDITOR

using NUnit.Framework;

public class TraitToStatSyncTests
{
    [Test]
    public void Trait_AppliesStatModifier_Correctly()
    {
        // given
        var statRepo = new MockStatDataRepository();
        var traitRepo = new MockTraitDataRepository();
        var statManager = new StatManager(statRepo);
        var traitManager = new TraitManager(traitRepo, statManager);

        // when
        var traitData = MockTraitDataTable.GetMockData().Find(t => t.TraitType == ETraitType.Sprinting);
        traitManager.ForceSetLevel(ETraitType.Sprinting, 10, traitData);

        // then
        float expectedModifier = 0.001f * 10; // 10레벨 * per-level value
        float expected = 4f * (1f + expectedModifier); // 기본값 4f * (1 + modifier)
        float actual = statManager.GetStat(EStatType.MoveSpeed);

        Assert.AreEqual(expected, actual, 0.0001f);
    }
}
#endif