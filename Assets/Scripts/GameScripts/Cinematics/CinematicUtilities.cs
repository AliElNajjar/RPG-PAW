using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicUtilities : MonoBehaviour
{
    private static CinematicUtilities Instance;

    [Header("Shades")]
    public Transform upperShade;
    public Transform lowerShade;
    private Vector3 upperShadeHideLocalPos, lowerShadeHideLocalPos, upperShadeShowPos, lowerShadeShowPos;

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        upperShadeShowPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f));
        lowerShadeShowPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f));
        Debug.Log(upperShadeShowPos + " fdkjfhd " + lowerShadeShowPos);

        upperShadeShowPos = transform.InverseTransformPoint(upperShadeShowPos);
        lowerShadeShowPos = transform.InverseTransformPoint(lowerShadeShowPos);

        upperShadeShowPos.z = 1f;
        lowerShadeShowPos.z = 1f;

        upperShadeHideLocalPos = upperShadeShowPos;
        upperShadeHideLocalPos.y += 0.65f;

        lowerShadeHideLocalPos = lowerShadeShowPos;
        lowerShadeHideLocalPos.y -= 0.65f;

        upperShade.position = upperShadeHideLocalPos;
        lowerShade.position = lowerShadeHideLocalPos;

        Debug.Log(upperShadeShowPos + " 4575846587 " + lowerShadeShowPos);
    }

    #region Shades
    public static void ShowShades(float time = 1f)
    {
        if(Instance != null)
        {
            Instance.m_ShowShades(time);
        }
    }

    private void m_ShowShades(float time)
    {
        upperShade.localPosition = upperShadeHideLocalPos;
        lowerShade.localPosition = lowerShadeHideLocalPos;
        upperShade.gameObject.SetActive(true);
        lowerShade.gameObject.SetActive(true);

        Debug.Log(upperShade.localPosition + " and " + upperShadeShowPos);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(upperShade.gameObject, upperShadeShowPos, time, Space.Self));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(lowerShade.gameObject, lowerShadeShowPos, time, Space.Self));
    }

    public static void HideShades(float time = 1f)
    {
        if (Instance != null)
        {
            Instance.m_HideShades(time);
        }
    }

    private void m_HideShades(float time)
    {
        upperShade.localPosition = upperShadeShowPos;
        lowerShade.localPosition = lowerShadeShowPos;
        upperShade.gameObject.SetActive(true);
        lowerShade.gameObject.SetActive(true);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(upperShade.gameObject, upperShadeHideLocalPos, time, Space.Self));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(lowerShade.gameObject, lowerShadeHideLocalPos, time, Space.Self));
        StartCoroutine(DisableShadesAfterTimeRoutine(time));
    }

    private IEnumerator DisableShadesAfterTimeRoutine(float disableTime)
    {
        yield return new WaitForSeconds(disableTime);
        upperShade.gameObject.SetActive(false);
        lowerShade.gameObject.SetActive(false);
    }

    #endregion Shades
}
