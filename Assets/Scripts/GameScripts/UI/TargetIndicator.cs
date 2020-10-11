using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    public Sprite targetIconOnScreen;
    public Sprite targetIconOffScreen;
    [Space]
    [Range(0, 100)]
    public float edgeBuffer;
    public Vector3 targetIconScale;
    [Space]
    public bool PointTarget = true;
    public bool ShowDebugLines;
    [ReadOnly] public Image _iconImage;
    //Indicates if the object is out of the screen

    private Transform _target;
    private Transform _player;
    private Camera mainCamera;
    private RectTransform _icon;
    private Canvas _mainCanvas;
    private Vector3 _cameraOffsetUp;
    private Vector3 _cameraOffsetRight;
    private Vector3 _cameraOffsetForward;
    private bool _outOfScreen;

    public Transform Target
    {
        get { return _target; }
        set { _target = value; }
    }

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat")
        {
            this.enabled = false;
            return;
        }

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        _mainCanvas = GetComponentInParent<Canvas>();
        InstantiateTargetIcon();
    }
    void Update()
    {
        if (ShowDebugLines)
            DrawDebugLines();

        UpdateTargetIconPosition();
    }
    private void InstantiateTargetIcon()
    {
        _icon = new GameObject().AddComponent<RectTransform>();
        _icon.transform.SetParent(this.transform);
        _icon.localScale = targetIconScale;
        _icon.name = name + " icon";
        _iconImage = _icon.gameObject.AddComponent<Image>();
        _iconImage.sprite = targetIconOnScreen;
        _iconImage.color = new Color(1, 1, 1, 0);

    }

    private void UpdateTargetIconPosition()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 targetPos = _target.position;
        targetPos = mainCamera.WorldToViewportPoint(targetPos);
        //Simple check if the target object is out of the screen or inside
        if (targetPos.x > 1 || targetPos.y > 1 || targetPos.x < 0 || targetPos.y < 0)
            _outOfScreen = true;
        else
            _outOfScreen = false;
        if (targetPos.z < 0)
        {
            targetPos.x = 1f - targetPos.x;
            targetPos.y = 1f - targetPos.y;
            targetPos.z = 0;
            targetPos = Vector3Maximize(targetPos);
        }
        targetPos = mainCamera.ViewportToScreenPoint(targetPos);
        targetPos.x = Mathf.Clamp(targetPos.x, edgeBuffer, Screen.width - edgeBuffer);
        targetPos.y = Mathf.Clamp(targetPos.y, edgeBuffer, Screen.height - edgeBuffer);
        _icon.transform.position = targetPos;
        //Operations if the object is out of the screen
        if (_outOfScreen)
        {
            //Show the target off screen icon
            _iconImage.sprite = targetIconOffScreen;
            _iconImage.color = Color.white;

            if (PointTarget)
            {
                var dir = Target.transform.position - mainCamera.ScreenToWorldPoint(_icon.transform.position);
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                _icon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);                
            }
        }
        else
        {
            //Reset rotation to zero and swap the sprite to the "on screen" one
            _icon.transform.eulerAngles = new Vector3(0, 0, 0);
            _iconImage.sprite = targetIconOnScreen;
            _iconImage.color = Color.white;
        }
    }

    public void UpdateTarget(Transform target = null)
    {
        Target = target;

        if (target == null)
        {
            _iconImage.color = new Color(1, 1, 1, 0);
        }
    }

    public void DrawDebugLines()
    {
        Vector3 directionFromCamera = _target.position - mainCamera.transform.position;
        Vector3 cameraForwad = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;
        cameraForwad *= Vector3.Dot(cameraForwad, directionFromCamera);
        cameraRight *= Vector3.Dot(cameraRight, directionFromCamera);
        cameraUp *= Vector3.Dot(cameraUp, directionFromCamera);
        Debug.DrawRay(mainCamera.transform.position, directionFromCamera, Color.magenta);
        Vector3 forwardPlaneCenter = mainCamera.transform.position + cameraForwad;
        Debug.DrawLine(mainCamera.transform.position, forwardPlaneCenter, Color.blue);
        Debug.DrawLine(forwardPlaneCenter, forwardPlaneCenter + cameraUp, Color.green);
        Debug.DrawLine(forwardPlaneCenter, forwardPlaneCenter + cameraRight, Color.red);
    }
    public Vector3 Vector3Maximize(Vector3 vector)
    {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }
}
