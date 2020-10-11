using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyHypeEffect : MonoBehaviour
{
    private BattleManager _battleManager;
    private ActivePlayerButtons _activePlayerButtons;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
    }

    private void OnEnable()
    {
        ApplyEffects();
    }

    private void OnDisable()
    {
        UnapplyEffects();
    }

    private void ApplyEffects()
    {
        if (GetComponent<DamageUpData>())
        {
            DamageUpData damageUpData = GetComponent<DamageUpData>();

            damageUpData.originalDamageUp = 1;

            UnitEffects.RaiseDamageByPercent(GetComponent<DamageUpData>().damageUpPercent, _battleManager.playableUnits.activePartyMembers);
        }

        if (GetComponent<RaiseGoldAfterBattleData>())
        {
            RaiseGoldAfterBattleData raiseGoldAfterBattleData = GetComponent<RaiseGoldAfterBattleData>();

            raiseGoldAfterBattleData.originalGoldPercentUp = new float[_battleManager.enemyUnits.activeEnemies.Count];

            for (int i = 0; i < _battleManager.enemyUnits.activeEnemies.Count; i++)
            {
                raiseGoldAfterBattleData.originalGoldPercentUp[i] = _battleManager.enemyUnits.activeEnemies[i].enemyUnitData.goldGiven;
            }

            BattleEffects.SetTotalGoldFromBattle(raiseGoldAfterBattleData.goldUpPercent, _battleManager.enemyUnits.activeEnemies.ToArray());
        }

        if (GetComponent<RegenerateSPData>())
        {
            for (int i = 0; i < _battleManager.playableUnits.activePartyMembers.Length; i++)
            {
                _battleManager.playableUnits.activePartyMembers[i].OnSPRegenerationActivate += ApplySPRegeneration;
            }
        }

        if (GetComponent<SpecialTechniqueDisableData>())
        {
            _activePlayerButtons.gimmicksActive = false;
        }

        if (GetComponent<RaiseExperienceAfterBattleData>())
        {
            RaiseExperienceAfterBattleData raiseEXPAfterBattleData = GetComponent<RaiseExperienceAfterBattleData>();

            raiseEXPAfterBattleData.originalEXP = new int[_battleManager.enemyUnits.activeEnemies.Count];

            for (int i = 0; i < _battleManager.enemyUnits.activeEnemies.Count; i++)
            {
                raiseEXPAfterBattleData.originalEXP[i] = _battleManager.enemyUnits.activeEnemies[i].enemyUnitData.expGiven;
            }

            BattleEffects.SetTotalEXPFromBattle(raiseEXPAfterBattleData.expUpPercent, _battleManager.enemyUnits.activeEnemies.ToArray());
        }

        if (GetComponent<ReduceDamageTakenData>())
        {
            ReduceDamageTakenData reduceDamageTakenData = GetComponent<ReduceDamageTakenData>();

            reduceDamageTakenData.originalDamageReduction = 1;

            UnitEffects.SetDamageReduction(GetComponent<ReduceDamageTakenData>().damageReductionPercentOverride, _battleManager.playableUnits.activePartyMembers);
        }

        if (GetComponent<RaiseManagerActivationRate>())
        {
            _battleManager.playableUnits.ActiveManager.initialActivationPercentage = _battleManager.playableUnits.ActiveManager.activationPercentage;
            BattleEffects.RaiseManagerActivationRate(GetComponent<RaiseManagerActivationRate>().activationUp, _battleManager.playableUnits.ActiveManager);
        }

        if (GetComponent<ButtonPromptsDisableData>())
        {
            if (_activePlayerButtons.buttonPromptsActive)
                _battleManager.playableUnits.activePartyMembers[0].ShowPopupText("Button prompts disabled!");

            _activePlayerButtons.buttonPromptsActive = false;
        }

        if (GetComponent<ManagerAbilitiesDisableData>())
        {
            _battleManager.playableUnits.managerAbilitiesActive = false;
        }

        if (GetComponent<InstantKOData>())
        {
            // This effect is done in BaseBattleUnitHolder.
            //for (int i = 0; i < _battleManager.playableUnits.activePartyMembers.Length; i++)
            //{
            //    _battleManager.playableUnits.activePartyMembers[i].TakeDamage(999999);
            //}
        }

        if (GetComponent<ActivateHypeArrows>())
        {
            GetComponent<ActivateHypeArrows>().hypeArrowsObject.SetActive(true);
        }

        if (GetComponent<PlaySFXData>())
        {
            SFXHandler.Instance.PlaySoundFX(GetComponent<PlaySFXData>().soundToPlay);
        }


        gameObject.OnComponent<CrossFadeSpriteData>(crossFadeSprite =>
        {
            StartCoroutine(crossFadeSprite.DoCrossFade());
        });
    }

    private void UnapplyEffects()
    {
        if (GetComponent<DamageUpData>())
        {
            DamageUpData damageUpData = GetComponent<DamageUpData>();

            foreach (var unit in _battleManager.playableUnits.activePartyMembers)
            {
                unit.extraDamageDealtPercent = damageUpData.originalDamageUp;
            }
        }

        if (GetComponent<RaiseGoldAfterBattleData>())
        {
            RaiseGoldAfterBattleData raiseGoldAfterBattleData = GetComponent<RaiseGoldAfterBattleData>();

            for (int i = 0; i < _battleManager.enemyUnits.activeEnemies.Count; i++)
            {
                _battleManager.enemyUnits.activeEnemies[i].enemyUnitData.goldGiven = raiseGoldAfterBattleData.originalGoldPercentUp[i];
            }
        }

        if (GetComponent<RegenerateSPData>())
        {
            for (int i = 0; i < _battleManager.playableUnits.activePartyMembers.Length; i++)
            {
                _battleManager.playableUnits.activePartyMembers[i].OnSPRegenerationActivate -= ApplySPRegeneration;
            }
        }

        if (GetComponent<SpecialTechniqueDisableData>())
        {
            _activePlayerButtons.gimmicksActive = true;
        }

        if (GetComponent<RaiseExperienceAfterBattleData>())
        {
            RaiseExperienceAfterBattleData raiseEXPAfterBattleData = GetComponent<RaiseExperienceAfterBattleData>();

            for (int i = 0; i < _battleManager.enemyUnits.activeEnemies.Count; i++)
            {
                _battleManager.enemyUnits.activeEnemies[i].enemyUnitData.expGiven = raiseEXPAfterBattleData.originalEXP[i];
            }
        }

        if (GetComponent<ReduceDamageTakenData>())
        {
            ReduceDamageTakenData reduceDamageTakenData = GetComponent<ReduceDamageTakenData>();

            foreach (var unit in _battleManager.playableUnits.activePartyMembers)
            {
                unit.extraDamageDealtPercent = reduceDamageTakenData.originalDamageReduction;
            }
        }

        if (GetComponent<RaiseManagerActivationRate>())
        {
            _battleManager.playableUnits.ActiveManager.activationPercentage = _battleManager.playableUnits.ActiveManager.initialActivationPercentage;
        }

        if (GetComponent<ButtonPromptsDisableData>())
        {
            _activePlayerButtons.buttonPromptsActive = true;
        }

        if (GetComponent<ManagerAbilitiesDisableData>())
        {
            _battleManager.playableUnits.managerAbilitiesActive = true;
        }

        if (GetComponent<ActivateHypeArrows>())
        {
            GetComponent<ActivateHypeArrows>().hypeArrowsObject.SetActive(false);
        }

        gameObject.OnComponent<CrossFadeSpriteData>(crossFadeSprite =>
        {
            crossFadeSprite.StopAllCoroutines();
        });
    }

    private void ApplySPRegeneration(float regenerationValue, BaseBattleUnitHolder targetUnit)
    {
        BattleEffects.RegenerateSP(regenerationValue, targetUnit);
    }
}
