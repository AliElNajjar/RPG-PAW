using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private bool canBePressed = false;
    [SerializeField] private KeyCode hitKeyCode;
    [SerializeField] private ParticleSystem effects;

    void Update()
    {
        //make transparent if player has stumbled
        if (GameManager.gameManager.stumbled && GetComponent<SpriteRenderer>().color.a == 1f)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
        }
        else if (!GameManager.gameManager.stumbled && GetComponent<SpriteRenderer>().color.a == 0.5f)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
        }
        if (canBePressed && !GameManager.gameManager.stumbled)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                if (hitKeyCode == KeyCode.UpArrow)
                {
                    GameManager.gameManager.HitKey();
                    PlayTriggerEffects();
                    Destroy(gameObject);
                }
                else
                {
                    Miss();
                }
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                if (hitKeyCode == KeyCode.DownArrow)
                {
                    GameManager.gameManager.HitKey();
                    PlayTriggerEffects();
                    Destroy(gameObject);
                }
                else
                {
                    Miss();
                }
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
            {
                if (hitKeyCode == KeyCode.LeftArrow)
                {
                    GameManager.gameManager.HitKey();
                    PlayTriggerEffects();
                    Destroy(gameObject);
                }
                else
                {
                    Miss();
                }
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
            {
                if (hitKeyCode == KeyCode.RightArrow)
                {
                    GameManager.gameManager.HitKey();
                    PlayTriggerEffects();
                    Destroy(gameObject);
                }
                else
                {
                    Miss();
                }
            }
        }
    }

    private void Miss()
    {
        SFXHandler.Instance.PlaySoundFX(GameManager.gameManager.Miss);
        StopAllCoroutines();
        StartCoroutine(WrongKey());
        Destroy(gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>());
        GameManager.gameManager.MissKey();
        GameManager.gameManager.stumbled = true; ;
        GameManager.gameManager.muchachoAnimator.SetBool("stumbled", true);
        canBePressed = false;
    }

    private IEnumerator ColorLerp(Color target, float speed)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        while (true)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, target, speed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator WrongKey()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        float cooldown = 0.005f;
        while (true)
        {
            if (transform.localScale.y < 2f)
            {
                transform.localScale += new Vector3(0.2f, 0.2f, 0);
            }
            else
            {
                transform.localScale -= new Vector3(0.3f, 0.3f, 0);
            }

            yield return new WaitForSeconds(cooldown);
        }
    }
    private IEnumerator Highlight()
    {
        SpriteGlow.SpriteGlowEffect glow = gameObject.AddComponent<SpriteGlow.SpriteGlowEffect>();
        glow.OutlineWidth = 2;
        float cooldown = 0.005f;
        float addDefault = 0.05f;
        float addFinal = addDefault;
        while (true)
        {
            if (transform.localScale.x > 1.5f)
            {
                addFinal = -addDefault;
            }
            else if (transform.localScale.x < 0.5f)
            {
                addFinal = addDefault;
            }
            transform.localScale += new Vector3(addFinal, addFinal, 0);
            yield return new WaitForSeconds(cooldown);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            canBePressed = true;
            StartCoroutine(Highlight());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            canBePressed = false;
        }
    }


    private void PlayTriggerEffects()
    {
        effects.transform.parent = null;
        effects.Play();
    }
}
