using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    [System.Serializable]
//class untuk setting dialogue
public class DialogueContainer 
{
    //inisialiisasi
    public GameObject root;
    //pembuatan anak dari class name container
    public NameContainer nameContainer = new NameContainer();
    public TextMeshProUGUI dialogueText;

    //fungsi untuk setting warna dialogue
    public void SetDialogueColor (Color color) => dialogueText.color = color;
    public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
}

}
