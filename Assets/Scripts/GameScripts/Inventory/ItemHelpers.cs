public enum ItemType
{
    AllPurpose,
    CombatOnly,
    OverworldOnly,
    CraftingMat
}

public class ItemExecutionData
{
    private BaseBattleUnitHolder[] _targetUnits;

    public BaseBattleUnitHolder[] TargetUnits
    {
        get { return _targetUnits; }
    }

    public ItemExecutionData()
    {

    }

    public ItemExecutionData(params BaseBattleUnitHolder[] targetUnits)
    {
        this._targetUnits = targetUnits;
    }
}
