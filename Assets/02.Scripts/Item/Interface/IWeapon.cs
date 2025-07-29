public interface IWeapon
{
    public EWeaponType Type { get; }
    public float Damage { get; }
    public float AttackSpeed { get; }
    public float Range { get; }
}