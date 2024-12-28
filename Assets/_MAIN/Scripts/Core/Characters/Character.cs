using System.Collections.Generic;
using System.Collections;
using DIALOGUE;
using TMPro;
using UnityEngine;


namespace CHARACTERS
{
public abstract class Character 
{
    public const bool ENABLE_ON_START = true;
    private const float UNIGHLIGHTED_DARKEN_STRENGHT = 0.65f;
    public string name = "";
    public string displayname = "";
    public RectTransform root = null;
    public CharacterConfigData config;
    public Animator animator;
    public Color color {get; protected set;} = Color.white;
    protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
    protected Color highlightedColor => color;
    protected Color unhighlightedColor => new Color(color.r* UNIGHLIGHTED_DARKEN_STRENGHT, color.g* UNIGHLIGHTED_DARKEN_STRENGHT, color.b * UNIGHLIGHTED_DARKEN_STRENGHT, color.a);
    public bool highlighted {get; protected set;} = true;

    protected CharacterManager manager => CharacterManager.instance;
    public DialogueSystem dialogueSystem => DialogueSystem.instance;

    //coroutine here
    protected Coroutine co_changingColor;
    protected Coroutine co_revealing, co_hiding;
    protected Coroutine co_moving;
    protected Coroutine co_highLighting;
    public bool isRevealing => co_revealing != null;
    public bool isChangingColor => co_changingColor != null;
    public bool isMoving => co_moving != null;
    public bool isHiding => co_hiding != null;
    public bool isHighlighting => (highlighted && co_highLighting != null);
    public bool isUnHighLighting => (!highlighted && co_highLighting != null);
    public virtual bool isVisible {get; set;}

    public Character(string name, CharacterConfigData config, GameObject prefab)
    {
        this.name = name;
        displayname = name;
        this.config = config;

        if(prefab != null)
        {
            GameObject ob = Object.Instantiate(prefab, manager.characterpanel);
            ob.name = manager.FormatCharacrterPath(manager.characterPrefabName, name);
            ob.SetActive(true);
            root = ob.GetComponent<RectTransform>();
            animator = root.GetComponentInChildren<Animator>();
        }
    }

    public Coroutine Say(string dialogue) => Say(new List<string>{dialogue});
    public Coroutine Say(List<string> dialogue)
    {
        dialogueSystem.ShowSpeakerName(displayname);
        UpdateTextCustomiazationOnScreen();
        return dialogueSystem.Say(dialogue);
    }
    public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
    public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;
    public void SetNameColor(Color color) => config.nameColor = color;
    public void SetDialogueColor(Color color) => config.dialogueColor = color;

    public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);

    public void UpdateTextCustomiazationOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

    public virtual Coroutine Show()
    {
        if(isRevealing)
        {
            return co_revealing;
        }

        if(isHiding)
        {
            manager.StopCoroutine(co_hiding);
        }

        co_revealing = manager.StartCoroutine(ShowingOrHiding(true));
        return co_revealing;
    }

    public virtual Coroutine Hide()
    {
        if(isHiding)
        {
            return co_hiding;
        }

        if(isRevealing)
        {
            manager.StopCoroutine(co_revealing);
        }

        co_hiding = manager.StartCoroutine(ShowingOrHiding(false));
        return co_hiding;
    }

    public virtual IEnumerator ShowingOrHiding(bool Show)
    {
        Debug.Log("Show/hide cannot be called form here");
        yield return null;
    }

    public virtual void SetPosition(Vector2 position)
    {
        if(root == null)
        {
            return;
        }
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConverUITargetPositionToRelativeCharaterAnchorTargets(position);
        root.anchorMin = minAnchorTarget;
        root.anchorMax = maxAnchorTarget;
    }

    public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
    {
        if(root == null)
        {
            return null;
        }

        if(isMoving)
        {
            manager.StopCoroutine(co_moving);
        }
        co_moving = manager.StartCoroutine(MovingToPosition(position,speed,smooth));
        return co_moving;
    }

    private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
    {
        
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConverUITargetPositionToRelativeCharaterAnchorTargets(position);
        Vector2 padding = root.anchorMax - root.anchorMin;

        while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
        {
            root.anchorMin = smooth ? 
                Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                :Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

            root.anchorMax = root.anchorMin + padding;

            if(smooth && Vector2.Distance(root.anchorMin, minAnchorTarget)<= 0.001f)
            {
                root.anchorMin = minAnchorTarget;
                root.anchorMax = maxAnchorTarget;
                break;
            }
            yield return null;
        }
        Debug.Log("Done Moving");
        co_moving = null;
    }

    protected (Vector2, Vector2) ConverUITargetPositionToRelativeCharaterAnchorTargets(Vector2 position)
    {
        Vector2 padding = root.anchorMax - root.anchorMin;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
        Vector2 maxAnchorTarget = minAnchorTarget + padding;

        return(minAnchorTarget, maxAnchorTarget);
    }
    public virtual void SetColor(Color color)
    {
        this.color = color;
    }

    public Coroutine TransitionColor(Color color, float speed = 1f)
    {
        this.color = color;
        if(isChangingColor)
        {
            manager.StopCoroutine(co_changingColor);
        }

        co_changingColor = manager.StartCoroutine(ChangingColor(displayColor,speed));
        return co_changingColor;
    }

    public virtual IEnumerator ChangingColor(Color color, float speed)
    {
        Debug.Log("Color is not applicable");
        yield return null;

    }

    public Coroutine HighLight(float speed = 1f)
    {
        if (isHighlighting)
        {
            return co_highLighting;
        }

        if(isUnHighLighting)
        {
            manager.StopCoroutine(co_highLighting);
        }

        highlighted = true;
        co_highLighting = manager.StartCoroutine(HighLighting(highlighted,speed));
        return co_highLighting;
    }

    public Coroutine UnHighLight(float speed = 1f)
    {
        if (isUnHighLighting)
        {
            return co_highLighting;
        }

        if(isHighlighting)
        {
            manager.StopCoroutine(co_highLighting);
        }

        highlighted = false;
        co_highLighting = manager.StartCoroutine(HighLighting(highlighted,speed));
        return co_highLighting;   
    }



    public virtual IEnumerator HighLighting(bool HighLight, float speedMultiplier)
    {
        Debug.Log("highlighting is not avaible on this character type!");
        yield return null;
    }



    public enum CharacterType
    {
        Text,
        Sprite,
        SpriteSheet,
        Live2D,
        Model3D
    }
}

}