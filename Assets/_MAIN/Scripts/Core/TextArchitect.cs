using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using System.Collections;


//membuat class yang bisa dipakai class lain
public class TextArchitect 
{
    //pendeklarasian variable
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;
    public string currentText => tmpro.text;
    public string targetText { get; private set;} = "";
    public string proText { get; private set;} = "";
    private int proTextLength = 0;
    public string fullTargetText => proText  + targetText;
    public enum BuildMethod {instant, typewriter, fade}
    public BuildMethod buildMethod = BuildMethod.typewriter;
    public Color textColor {get{return tmpro.color;} set {tmpro.color = value;}}
    public float speed {get{return baseSpeed * speedMultiplier;} set {speedMultiplier = value;}}
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int charactersPerCycle {get{return speed <= 2f? characterMultiplier : speed<=2.5f ? characterMultiplier*2 : characterMultiplier*3;}}
    public int characterMultiplier = 1;
    public bool hurryUp = false;

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    public Coroutine Build(string text)
    {
        proText = "";
        targetText = text;
        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    public Coroutine Append(string text)
    {
        proText = tmpro.text;
        targetText = text;
        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public void Stop()
    {
        if(!isBuilding)
            return;
        
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();
        switch(buildMethod)
        {
            case BuildMethod.typewriter:
            yield return Build_Typewrite();
                break;
            case BuildMethod.fade:
            yield return Build_Fade();
                break;
        }
        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    public void ForceComplete()
    {
        switch(buildMethod)
        {
            case BuildMethod.typewriter:
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            break;
            case BuildMethod.fade:
            tmpro.ForceMeshUpdate();
            break;
        }
    }
    private void Prepare()
    {
        switch(buildMethod)
        {
            case BuildMethod.instant:
            prepare_Instan();
            break;
            case BuildMethod.typewriter:
            prepare_Typeweriter();
            break;
            case BuildMethod.fade:
            prepare_Fade();
            break;
        }
    }

    
    private void prepare_Fade()
    {
        tmpro.text = proText;
        if(proText != "")
        {
            tmpro.ForceMeshUpdate();
            proTextLength = tmpro.textInfo.characterCount;
        }
        else
        {
            proTextLength = 0;
        }
        proTextLength = 0;
        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color colorVisible = new Color(textColor.r , textColor.g , textColor.b , 1);
        Color colorHidden = new Color(textColor.r , textColor.g , textColor.b , 0);
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i = 0; i<textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if(!charInfo.isVisible)
            {
                continue;
            }

            if( i< proTextLength)
            {
                for(int v = 0;v<4;v++)
                {
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
                }
            }else
            {
                for(int v = 0;v<4;v++)
                {
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
                }
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    private void prepare_Instan()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void prepare_Typeweriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = proText;

        if(proText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private IEnumerator Build_Typewrite()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    private IEnumerator Build_Fade()
    {
        int minRange  = proTextLength;
        int maxRange = minRange + 1;
        byte alphaThreshold = 15;
        TMP_TextInfo textInfo = tmpro.textInfo;
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while(true)
        {
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 10f;
            for(int i = minRange ; i<maxRange;i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if(!charInfo.isVisible)
                    {
                        continue;
                    }
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i],255,fadeSpeed);
                for(int v = 0;v<4;v++)
                {
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];
                }
                if(alphas[i]>=255)
                {
                    minRange++;
                }

            }
             tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                bool lastCharacterisVisible = !textInfo.characterInfo[maxRange-1].isVisible;
                if(alphas[maxRange-1]>alphaThreshold|| lastCharacterisVisible)
                {
                    if(maxRange< textInfo.characterCount)
                    {
                        maxRange++;
                    }else if(alphas[maxRange-1]>=255 || lastCharacterisVisible)
                    {
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
        }

    }

}
