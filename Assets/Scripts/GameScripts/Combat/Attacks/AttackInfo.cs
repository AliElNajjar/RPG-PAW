[System.Serializable]
public class BaseAttackInfo
{
    public ushort attackID;
    public string attackName;
    public short damageValue;
    public DamageType damageType;
    public StatusAilmentType statusEffect;

    public BaseAttackInfo()
    {

    }

    public BaseAttackInfo(ushort attackID, string attackName, short damageValue, DamageType damageType, StatusAilmentType statusEffect)
    {
        this.attackID = attackID;
        this.attackName = attackName;
        this.damageValue = damageValue;
        this.damageType = damageType;
        this.statusEffect = statusEffect;
    }
}
