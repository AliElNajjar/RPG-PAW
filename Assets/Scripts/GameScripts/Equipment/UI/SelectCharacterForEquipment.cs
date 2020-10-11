using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectCharacterForEquipment : MonoBehaviour
{
    public Transform pauseRoot;

    public PauseMenuItemSelection pauseMenuItemSelection;
    public TargettableManager _targettableManager;
    public CharacterInfoEquipment characterInfo;

    public ActivatePauseMenuTransition transitionEffect;

    public UnityEvent OnLoadCharacterInfo;

    private void Start()
    {
        transitionEffect = FindObjectOfType<ActivatePauseMenuTransition>();
    }

    public void StartSelectingCharacter()
    {
        StartCoroutine(SelectCharacter());
    }

    public IEnumerator SelectCharacter()
    {
        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            LoadCharacterInfo,
            GoBackToMenu,
            pauseRoot.gameObject.GetComponentsInChildren<BaseBattleUnitHolder>()
            ));
    }

    public void LoadCharacterInfo()
    {
        transitionEffect.ActivateEffect();

        characterInfo.unit = _targettableManager.SingleTargetCurrent.GetComponent<PlayerBattleUnitHolder>();

        OnLoadCharacterInfo?.Invoke();
    }
    
    public void GoBackToMenu()
    {
        pauseMenuItemSelection.Reading = true;
    }
}
