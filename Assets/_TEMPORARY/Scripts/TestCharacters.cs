using UnityEngine;
using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using TMPro;

public class TestCharacters : MonoBehaviour
{
    public TMP_FontAsset tempFont;
    private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        //Character Florence = CharacterManager.instance.CreateCharacter("Kolbi");
        //Character Andika = CharacterManager.instance.CreateCharacter("Andika");
        //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
        StartCoroutine(Test());
    }

    IEnumerator Test()
{
    Character_Sprite kolbi = CreateCharacter("Kolbi") as Character_Sprite;
    Character_Sprite adventurer = CreateCharacter("adventurer as Generic") as Character_Sprite;
    adventurer.SetPosition(Vector2.zero);
    kolbi.SetPosition(Vector2.zero);
    yield return new WaitForSeconds(1f);
    kolbi.SetPriority(1);
    yield return new WaitForSeconds(2f);
    CharacterManager.instance.SortCharacters(new string[] {"adventurer"});
    yield return new WaitForSeconds(2f);
    CharacterManager.instance.SortCharacters();
    yield return new WaitForSeconds(2f);
    CharacterManager.instance.SortCharacters(new string[] {"Kolbi","adventurer"});
    yield return null;
}


    // Update is called once per frame
    void Update()
    {
        
    }
}
