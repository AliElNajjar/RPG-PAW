using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Fighters/PlayerBattleUnit", fileName = "PlayerBattleUnitInfo"), System.Serializable]
public class PlayerBattleUnit : BaseBattleUnit
{
    public PlayableUnit playableUnit;    
    public UnitEquipment equipment;
    public GameObject unitPrefab;
    public Sprite equipmentSprite;

    public override BaseUnit UnitData
    {
        get { return playableUnit; }
    }
}
