using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Text;

public class MessagesManager : MonoBehaviour
{
    public static MessagesManager Instance;

    public GameObject textGameobject;
    public Transform textBoxParent;

    public GameObject boxWidth;
    public GameObject boxHeigth;
    public GameObject widthMirror;
    public GameObject heigthMirror;
    public GameObject boxCenter;

    public GameObject bottomLeftCorner;
    public GameObject topRightCorner;
    public GameObject bottomRightCorner;

    public UnitOverworldMovement messageSource;
    public GameObject messageReceiver;

    public bool dialogueSelectionEnded = false;

    [HideInInspector] public int currentDialogueIndex = 0;

    private SpriteRenderer _boxWidthSR;
    private SpriteRenderer _boxWidthMirrorSR;
    private SpriteRenderer _boxHeigthSR;
    private SpriteRenderer _boxHeigthMirrorSR;
    private SpriteRenderer _boxCenterSR;

    private AudioSource audioSource;

    private RectTransform _textMeshProRectTransform;
    private TextMeshPro _textComponent;
    private Vector2 _textComponentInitialSize;

    private bool textFullyWritten = false;
    private bool singleText = false;

    Coroutine writeTextRoutine;

    private Vector3 _widthUnit = new Vector3(0.08f, 0.12f, 0);
    private Vector3 _heigthUnit = new Vector3(0.13f, 0.09f, 0);

    private ScreenBounds bounds = new ScreenBounds(1, 1);

    private const float TEXT_WIDTH_UNIT = 0.08f;
    private const float TEXT_HEIGTH_UNIT = 0.08f;
    private const float TEXTBOX_UNIT_SEPARATION = 1f;

    private const short MAX_MESSAGE_WIDTH = 28;
    private const short MAX_MESSAGE_HEIGTH = 28;
    public const string CONTINUE_TEXT_BUTTON = "Submit";

    public static short combatMessageWidth = 6;
    public static short combatMessageHeigth = 1;

    public NewMessageBox newMessageBox;
    private Vector2 originalMessageBoxPos;
    private Vector2 originalMessageBoxPivot;
    private Vector2 originalMessageBoxScale;

    private void OnEnable()
    {        
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Instance = null;

        Instance = FindObjectOfType<MessagesManager>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (newMessageBox)
            {
                originalMessageBoxPos = newMessageBox.GetComponent<RectTransform>().position;
                originalMessageBoxPivot = newMessageBox.GetComponent<RectTransform>().pivot;
                originalMessageBoxScale = newMessageBox.GetComponent<RectTransform>().localScale;

            }        
        }
        else
        {
            Destroy(this);
        }

        _boxWidthSR = boxWidth.GetComponent<SpriteRenderer>();
        _boxWidthMirrorSR = widthMirror.GetComponent<SpriteRenderer>();

        _boxHeigthSR = boxHeigth.GetComponent<SpriteRenderer>();
        _boxHeigthMirrorSR = heigthMirror.GetComponent<SpriteRenderer>();

        _boxCenterSR = boxCenter.GetComponent<SpriteRenderer>();

        _textMeshProRectTransform = textGameobject.GetComponent<RectTransform>();
        _textComponentInitialSize = _textMeshProRectTransform.sizeDelta;
        _textComponent = textGameobject.GetComponent<TextMeshPro>();

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (newMessageBox && !newMessageBox.gameObject.activeInHierarchy)
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    private void ClampPosition(ScreenBounds bounds)
    {
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(transform.position.x, -bounds.x, bounds.x),
            Mathf.Clamp(transform.position.y, -bounds.y, bounds.y),
            transform.position.z
            );

        transform.position = clampedPosition;
    }

    public void CurrentMessageSetActive(bool enabled)
    {
        textBoxParent?.gameObject.SetActive(enabled);
        textGameobject?.SetActive(enabled);
        newMessageBox?.gameObject.SetActive(enabled);
    }

    public void BuildMessageBox(string text, short width, short heigth, float time = -1, Action onDialogEnded = null)
    {        
        MakeMessageBox(width, heigth);

        textFullyWritten = false;

        StopWriteIfActive();

        textBoxParent.gameObject.SetActive(true);
        textGameobject.SetActive(true);
        newMessageBox.gameObject.SetActive(true);

        if (time > 0)
        {
            StartCoroutine(DisableMessageAfterTime(time, new string[] { text }, onDialogEnded));
        }
        else
        {
            StartCoroutine(DisableMessageAfterButtonPress(new string[] { text }, onDialogEnded));
        }
    }

    void StopWriteIfActive()
    {
        if (writeTextRoutine != null)
        {
            StopCoroutine(writeTextRoutine);
            writeTextRoutine = null;
        }
    }

    public void BuildMessageBox(string[] text, short width, short heigth, float time = -1, Action OnDialogEnded = null)
    {
        if (text.Length > 0)
        {
            //Debug.Log("MM here 1");
            textFullyWritten = false;
            MakeMessageBox(width, heigth);
            //Debug.Log("MM here 2");
            StopWriteIfActive();
            //Debug.Log("MM here 3");
            textBoxParent.gameObject.SetActive(true);
            textGameobject.SetActive(true);
            newMessageBox.gameObject.SetActive(true);
            //Debug.Log("MM here 4");
            if (time > 0)
            {
                //Debug.Log("MM here 4.1");
                StartCoroutine(DisableMessageAfterTime(time, text, OnDialogEnded));
            }
            else
            {
                //Debug.Log("MM here 4.2");
                StartCoroutine(DisableMessageAfterButtonPress(text, OnDialogEnded));
            }
        }
        else
        {
            Debug.LogError("Message source is empty!");
        }
    }

    public void BuildMessageBox(string[] text, short width, short heigth, float time = -1, Action OnDialogEnded = null, Action<int> BeforeSingleDialogueDeliver = null)
    {
        if (text.Length > 0)
        {
            //Debug.Log("MM here 1");
            textFullyWritten = false;
            MakeMessageBox(width, heigth);
            //Debug.Log("MM here 2");
            StopWriteIfActive();
            //Debug.Log("MM here 3");
            textBoxParent.gameObject.SetActive(true);
            textGameobject.SetActive(true);
            newMessageBox.gameObject.SetActive(true);
            //Debug.Log("MM here 4");
            if (time > 0)
            {
                //Debug.Log("MM here 4.1");
                StartCoroutine(DisableMessageAfterTime(time, text, OnDialogEnded));
            }
            else
            {
                //Debug.Log("MM here 4.2");
                StartCoroutine(DisableMessageAfterButtonPress(text, OnDialogEnded, BeforeSingleDialogueDeliver));
            }
        }
        else
        {
            Debug.LogError("Message source is empty!");
        }
    }

    public void BuildMessageBox(string[] text, short width, short heigth, float time = -1, Func<IEnumerator> OnDialogEnded = null)
    {
        if (text.Length > 0)
        {
            Action onDialogEndedAction = null;

            if (OnDialogEnded != null)
                onDialogEndedAction = ActionRoutine(OnDialogEnded);

            textFullyWritten = false;
            MakeMessageBox(width, heigth);

            StopWriteIfActive();

            // writeTextRoutine = StartCoroutine(WriteText(text[0]));

            textBoxParent.gameObject.SetActive(true);
            textGameobject.SetActive(true);
            newMessageBox.gameObject.SetActive(true);

            if (time > 0)
            {
                StartCoroutine(DisableMessageAfterTime(time, text, onDialogEndedAction));
            }
            else
            {
                StartCoroutine(DisableMessageAfterButtonPress(text, onDialogEndedAction));
            }
        }
        else
        {
            Debug.LogError("Message source is empty!");
        }
    }

    public void BuildMessageBox(InteractableLineInfo[] text, short width, short heigth, float time = -1, Action OnDialogEnded = null, GameObject sender = null, GameObject receiver = null)
    {
        if (text.Length > 0)
        {
            textFullyWritten = false;
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxOpen);
            MakeMessageBox(width, heigth);
            
            StopWriteIfActive();

            textBoxParent.gameObject.SetActive(true);
            textGameobject.SetActive(true);
            newMessageBox.gameObject.SetActive(true);
            if (time > 0)
            {
                StartCoroutine(DisableMessageAfterTime(time, text));
            }
            else
            {
                StartCoroutine(DisableMessageAfterButtonPress(text, OnDialogEnded, sender, receiver));
            }
        }
        else
        {
            Debug.LogError("Message source is empty!");
        }
    }

    private void MakeMessageBox(short width, short heigth)
    {
        //width related
        topRightCorner.transform.position = boxWidth.transform.position + new Vector3(((_widthUnit.x * width)) - _widthUnit.x + 0.08f, 0.04f);

        _boxWidthSR.size = new Vector2(_widthUnit.x * width, _widthUnit.y);
        _boxWidthMirrorSR.size = _boxWidthSR.size;
        _boxCenterSR.size = new Vector2(_boxWidthSR.size.x, _boxCenterSR.size.y);

        //heigth related
        bottomLeftCorner.transform.position = boxHeigth.transform.position + new Vector3(-0.04f, (_heigthUnit.y * -heigth));

        _boxHeigthSR.size = new Vector2(_heigthUnit.x, _heigthUnit.y * heigth);
        _boxHeigthMirrorSR.size = _boxHeigthSR.size;
        _boxCenterSR.size = new Vector2(_boxCenterSR.size.x, _boxHeigthSR.size.y + 0.01f);

        //Instantiate Clones
        Vector3 widthMirrorPos = new Vector3(boxWidth.transform.position.x, bottomLeftCorner.transform.position.y - _widthUnit.y);
        Vector3 heigthMirrorPos = new Vector3(topRightCorner.transform.position.x + _heigthUnit.x, boxHeigth.transform.position.y);

        widthMirror.transform.position = widthMirrorPos;
        heigthMirror.transform.position = heigthMirrorPos + (Vector3.left * 0.01f);

        bottomRightCorner.transform.position = new Vector3(topRightCorner.transform.position.x, bottomLeftCorner.transform.position.y);

        //Text
        _textMeshProRectTransform.sizeDelta = _textComponentInitialSize;

        _textMeshProRectTransform.sizeDelta = new Vector2(
            _textMeshProRectTransform.sizeDelta.x + ((TEXT_WIDTH_UNIT * width - 0.04f) - TEXT_WIDTH_UNIT),
            _textMeshProRectTransform.sizeDelta.y + ((TEXT_HEIGTH_UNIT * heigth) - TEXT_HEIGTH_UNIT));
        
        newMessageBox.UseString("");
        _textComponent.text = "";
    }

    private void ToggleNPCsBehaviors(bool enabled,NPCBehavior npc)
    {
        //NPCBehavior[] NPCs = FindObjectsOfType<NPCBehavior>();
        switch (enabled)
        {
            case true:
                //StartCoroutine(npc.Trigger(npc.nextPosition,npc.defaultPositions));
                npc.ResumeAfterConversation();
                break;
            case false:
                npc.GetComponent<UnitOverworldMovement>().DisableMovement();
                break;
        }   
    }
    
    IEnumerator DisableMessageAfterTime(float time, string[] text, Action onDialogEnded = null)
    {
        int counter = 0;

        while (counter < text.Length)
        {
            if (counter < text.Length) _textComponent.text = text[counter];
            counter++;
            yield return new WaitForSeconds(time);
        }

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxClose);

        textBoxParent.gameObject.SetActive(false);
        textGameobject.SetActive(false);
        newMessageBox.gameObject.SetActive(false);

        onDialogEnded?.Invoke();
    }

    IEnumerator DisableMessageAfterTime(float time, InteractableLineInfo[] text)
    {
        int counter = 0;

        while (counter < text.Length)
        {
            counter++;
            yield return new WaitForSeconds(time);
            if (counter < text.Length) _textComponent.text = text[counter].FinalMessage;
        }

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxOpen);

        textBoxParent.gameObject.SetActive(false);
        textGameobject.SetActive(false);
        newMessageBox.gameObject.SetActive(false);
    }

    bool skipReceived = false;

    IEnumerator DisableMessageAfterButtonPress(string[] text, Action onDialogEnded)
    {
        yield return new WaitForSeconds(0.05f);
        //Debug.Log("MM here 6");

        messageSource?.DisableMovement();
        //Debug.Log("MM here 7");
        int counter = 0;
        currentDialogueIndex = counter;
        //Debug.Log("MM here 8");
        writeTextRoutine = StartCoroutine(WriteText(text[counter]));

        while (counter < text.Length)
        {
            //Debug.Log("MM here 9");
            //Debug.Log($"Counter: {counter} - Text Length: {text.Length}");
            bool shouldProgress = false;

            if (textFullyWritten && RewiredInputHandler.Instance.player.GetButtonDown(CONTINUE_TEXT_BUTTON))
            {
                audioSource.Stop();
                shouldProgress = true;
            }

            if (shouldProgress)
            {
                skipReceived = false;
                counter++;
                currentDialogueIndex = counter;
                if (counter < text.Length)
                {
                    //if (!textFullyWritten)
                    textFullyWritten = false;

                    StopWriteIfActive();

                    yield return null;

                    yield return StartCoroutine(WriteText(text[counter]));
                    //_textComponent.text = text[counter];
                }
            }

            yield return null;
        }

        if (messageSource && messageReceiver != null)
            messageSource.Reading = true;

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxClose);
        onDialogEnded?.Invoke();

        newMessageBox.GetComponent<RectTransform>().pivot = originalMessageBoxPivot;
        newMessageBox.GetComponent<RectTransform>().position = originalMessageBoxPos;
        newMessageBox.GetComponent<RectTransform>().localScale = originalMessageBoxScale;

        textBoxParent.gameObject.SetActive(false);
        textGameobject.SetActive(false);
        newMessageBox.gameObject.SetActive(false);
        messageSource?.EnableMovement();
    }

    IEnumerator DisableMessageAfterButtonPress(string[] text, Action onDialogEnded, Action<int> BeforeSingleDialogueDeliver)
    {
        yield return new WaitForSeconds(0.05f);
        //Debug.Log("MM here 6");

        messageSource?.DisableMovement();
        //Debug.Log("MM here 7");
        int counter = 0;
        currentDialogueIndex = counter;
        //Debug.Log("MM here 8");
        writeTextRoutine = StartCoroutine(WriteText(text[counter]));
        BeforeSingleDialogueDeliver.Invoke(counter);
        while (counter < text.Length)
        {
            //Debug.Log("MM here 9");
            //Debug.Log($"Counter: {counter} - Text Length: {text.Length}");
            bool shouldProgress = false;

            if (textFullyWritten && RewiredInputHandler.Instance.player.GetButtonDown(CONTINUE_TEXT_BUTTON))
            {
                audioSource.Stop();
                shouldProgress = true;
            }
            if (shouldProgress)
            {
                if(counter < text.Length - 1)
                {
                    BeforeSingleDialogueDeliver.Invoke(counter + 1);
                }

                skipReceived = false;
                counter++;
                currentDialogueIndex = counter;
                if (counter < text.Length)
                {
                    //if (!textFullyWritten)
                    textFullyWritten = false;

                    StopWriteIfActive();

                    yield return null;

                    yield return StartCoroutine(WriteText(text[counter]));

                    //_textComponent.text = text[counter];
                }
            }

            yield return null;
        }

        if (messageSource && messageReceiver != null)
            messageSource.Reading = true;

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxClose);
        onDialogEnded?.Invoke();

        newMessageBox.GetComponent<RectTransform>().pivot = originalMessageBoxPivot;
        newMessageBox.GetComponent<RectTransform>().position = originalMessageBoxPos;
        newMessageBox.GetComponent<RectTransform>().localScale = originalMessageBoxScale;

        textBoxParent.gameObject.SetActive(false);
        textGameobject.SetActive(false);
        newMessageBox.gameObject.SetActive(false);
        messageSource?.EnableMovement();
    }

    IEnumerator DisableMessageAfterButtonPress(InteractableLineInfo[] text, Action OnDialogueEnded = null, GameObject sender = null, GameObject receiver = null)
    {
        yield return new WaitForSeconds(0.05f);

        messageSource.DisableMovement();
        receiver.GetComponent<NPCBehavior>().PauseBehavior();
        ToggleNPCsBehaviors(false,receiver.GetComponent<NPCBehavior>());

        int counter = 0;
        currentDialogueIndex = counter;

        writeTextRoutine = StartCoroutine(WriteText(text[counter].FinalMessage));

        while (counter < text.Length)
        {
            bool shouldProgress = false;

            receiver.GetComponent<UnitOverworldMovement>().isTalking = false;
            sender.GetComponent<UnitOverworldMovement>().isTalking = false;

            //_textComponent.text = text[counter].FinalMessage;

            switch (text[counter].source)
            {
                case InteractableMessageSource.Receiver:
                    receiver.GetComponent<UnitOverworldMovement>().isTalking = true;
                    AdjustTextBoxPosition(Camera.main.WorldToViewportPoint(receiver.transform.position + (Vector3.up * TEXTBOX_UNIT_SEPARATION)));
                    AdjustTextBoxSize(TextBoxSizes.Medium);
                    break;
                case InteractableMessageSource.Sender:
                    sender.GetComponent<UnitOverworldMovement>().isTalking = true;
                    AdjustTextBoxPosition(Camera.main.WorldToViewportPoint(sender.transform.position + (Vector3.up * TEXTBOX_UNIT_SEPARATION)));
                    AdjustTextBoxSize(TextBoxSizes.Medium);
                    break;
            }

            sender.GetComponent<UnitOverworldMovement>().SetTalkingAnims();
            receiver.GetComponent<UnitOverworldMovement>().SetTalkingAnims();

            if (textFullyWritten && RewiredInputHandler.Instance.player.GetButtonDown(CONTINUE_TEXT_BUTTON))
            {
                audioSource.Stop();
                shouldProgress = true;
            }

            if (shouldProgress)
            {
                counter++;
                currentDialogueIndex = counter;

                if (counter < text.Length)
                {
                    receiver.GetComponent<UnitOverworldMovement>().isTalking = false;
                    sender.GetComponent<UnitOverworldMovement>().isTalking = false;

                    switch (text[counter].source)
                    {
                        case InteractableMessageSource.Receiver:    
                            receiver.GetComponent<UnitOverworldMovement>().isTalking = true;
                            AdjustTextBoxPosition(Camera.main.WorldToViewportPoint(receiver.transform.position + (Vector3.up * TEXTBOX_UNIT_SEPARATION)));
                            AdjustTextBoxSize(TextBoxSizes.Medium);
                            break;
                        case InteractableMessageSource.Sender:
                            sender.GetComponent<UnitOverworldMovement>().isTalking = true;
                            AdjustTextBoxPosition(Camera.main.WorldToViewportPoint(sender.transform.position + (Vector3.up * TEXTBOX_UNIT_SEPARATION)));
                            AdjustTextBoxSize(TextBoxSizes.Medium);
                            break;
                    }

                    receiver?.GetComponent<UnitOverworldMovement>().SetTalkingAnims();
                    sender?.GetComponent<UnitOverworldMovement>().SetTalkingAnims();

                    textFullyWritten = false;

                    StopWriteIfActive();

                    yield return StartCoroutine(WriteText(text[counter].FinalMessage));
                }
                else
                {
                    receiver.GetComponent<UnitOverworldMovement>().isTalking = false;
                    sender.GetComponent<UnitOverworldMovement>().isTalking = false;
                    receiver?.GetComponent<UnitOverworldMovement>().SetTalkingAnims();
                    sender?.GetComponent<UnitOverworldMovement>().SetTalkingAnims();
                }
            }
            yield return null;
        }
        
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.textBoxClose);
        //messageReceiver?.GetComponent<NPCBehavior>()?.ResetBehavior();
        //receiver.GetComponent<NPCBehavior>().ResetBehavior();
        ToggleNPCsBehaviors(true,receiver.GetComponent<NPCBehavior>());

        messageSource.Reading = true;

        singleText = false;
        OnDialogueEnded?.Invoke();

        audioSource.Stop();

        newMessageBox.GetComponent<RectTransform>().pivot = originalMessageBoxPivot;
        newMessageBox.GetComponent<RectTransform>().position = originalMessageBoxPos;
        newMessageBox.GetComponent<RectTransform>().localScale = originalMessageBoxScale;

        textBoxParent.gameObject.SetActive(false);
        textGameobject.SetActive(false);
        newMessageBox.gameObject.SetActive(false);

        messageSource.EnableMovement();
    }

    IEnumerator WriteText(string text)
    {
        yield return new WaitForSeconds(0.0168f); //to not immediately consume the Submit button from a potential previous message box
        StringBuilder sb = new StringBuilder();

        StartCoroutine(CheckForSkip(text));

        int stringCounter = 0;

        int unresolvedRichText = 0;
        int resolvedRichText = 0;

        string currentRichTextCache = "";

        audioSource.Play();

        while ((stringCounter < text.Length) && !textFullyWritten) // CheckForSkip coroutine interrupts this by setting text fully written to true
        {
            if (textFullyWritten)
            {
                yield break;
            }

            char chara = text[stringCounter];
            bool useDelay = false;
            bool playSound = true;

            if (chara == '<')
            {
                currentRichTextCache = "<"; //new rich text buffer
                unresolvedRichText++;
            }
            else if (chara == '>')
            {
                currentRichTextCache += ">";
                sb.Append(currentRichTextCache); //add the rich text buffer to the string all at once.
                resolvedRichText++;
            }
            else if (unresolvedRichText == resolvedRichText)
            {
                useDelay = true;
                sb.Append(chara);
            }
            else //this is rich text contents
            {
                currentRichTextCache += chara;
                playSound = false;
            }

            string strToUse = sb.ToString();
            _textComponent.text = strToUse;
            newMessageBox.UseString(strToUse);

            stringCounter++;

            float delay = 0.0f;

            switch (chara)
            {
                case '.':
                case '?':
                case '!':
                case ':':
                    delay = 0.45f;
                    break;
                case ',':
                    delay = 0.35f;
                    break;
                default:
                    delay = 0.0167f;
                    break;
            }

            if (useDelay)
                yield return new WaitForSecondsRealtime(delay);
        }

        audioSource.Stop();

        textFullyWritten = true;
    }   

    IEnumerator CheckForSkip(string text)
    {
        while(!textFullyWritten)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown(CONTINUE_TEXT_BUTTON))
            {
                _textComponent.text = text;
                newMessageBox.UseString(text);
                textFullyWritten = true;
                skipReceived = true;
                yield break;
            }

            yield return null;
        }
    }

    public void AdjustTextBoxPosition(Vector2 viewportPos)
    {
        newMessageBox.GetComponent<RectTransform>().pivot = viewportPos;
        newMessageBox.GetComponent<RectTransform>().position = Camera.main.ViewportToScreenPoint(viewportPos);
    }

    public void AdjustTextBoxSize(Vector3 scale)
    {
        newMessageBox.transform.localScale = scale;
    }

    private Action ActionRoutine(Func<IEnumerator> coroutineFunc)
    {
        return () => { StartCoroutine(coroutineFunc()); };
    }

    private IEnumerator WaitForDialogueSelection()
    {
        while(!dialogueSelectionEnded)
        {
            yield return null;
        }

        dialogueSelectionEnded = false;
    }
}

public static class TextBoxSizes
{
    public static readonly Vector3 Small = new Vector3(2, 2, 1);
    public static readonly Vector3 Medium = new Vector3(5, 5, 1);
    public static readonly Vector3 Large = new Vector3(6, 6, 1);
}
