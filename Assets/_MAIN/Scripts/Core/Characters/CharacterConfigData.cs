using DIALOGUE;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [System.Serializable]

//class yang mengandung config dari character
public class CharacterConfigData 
{

    //inisialisasi
    public string name;
    public string alias;

    //inisialisasi tipe character dari class character
    public Character.CharacterType characterType;

    //inisialisasi lagi
    public Color nameColor;
    public Color dialogueColor;

    public TMP_FontAsset nameFont;
    public TMP_FontAsset dialogueFont;

    //membuat characterconfigdata yang baru namun tidak menimpa yang sebelumnya
    public CharacterConfigData Copy()
    {
        //membuat characterconfigdata baru didalam characterconfig data itu sendiri
        CharacterConfigData result = new CharacterConfigData();

        //mengisinya dengan data yang baru
        result.name = name;
        result.alias = alias;
        result.characterType = characterType;
        result.nameFont = nameFont;
        result.dialogueFont = dialogueFont;

        result.nameColor = new Color(nameColor.r,nameColor.g,nameColor.b,nameColor.a);
        result.dialogueColor = new Color(dialogueColor.r,dialogueColor.g,dialogueColor.b,dialogueColor.a);

        return result;
    }

    //fungsi untuk membuat defaultcolor dan default font dengan menggunakan fungsi yang sudah ada di dialogue sistem
    private static Color defualtColor => DialogueSystem.instance.config.defaultTextColor;
    private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;

    //fungsu character config data secara default yang nilainya bisa didapat dari class lain
    public static CharacterConfigData Default{
        get
        {
        CharacterConfigData result = new CharacterConfigData();
        result.name = "";
        result.alias = "";
        result.characterType = Character.CharacterType.Text;
        result.nameFont = defaultFont;
        result.dialogueFont = defaultFont;

        result.nameColor = new Color(defualtColor.r,defualtColor.g,defualtColor.b,255);
        result.dialogueColor = new Color(defualtColor.r,defualtColor.g,defualtColor.b,255);

        return result;
        }
    }
}
}