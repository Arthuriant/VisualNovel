using UnityEngine;
using CHARACTERS;
using TMPro;
namespace DIALOGUE
{

[CreateAssetMenu(fileName = "Dialogue Configuration Asset",menuName = "Dialogue System/Diallogue Configuration Asset")]
public class DialogueSystemConfigurationSO : ScriptableObject
{
    //meminta data characterconfig,color default, dan font default
    public CharacterConfigSO characterConfigurationAsset;

    public Color defaultTextColor = Color.white;
    public TMP_FontAsset defaultFont;
}

}

