using UnityEngine;
using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using TMPro;
using Unity.VisualScripting;

public class TestingSection1 : MonoBehaviour
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
    Character_Sprite Adventurer = CreateCharacter("Adventurer as Generic") as Character_Sprite;
    Character_Text narator = CreateCharacter("Narrator") as Character_Text;
    kolbi.isVisible = false;
    Adventurer.isVisible = false;
    yield return narator.Say("You are a traveler who has arrived in the city seeking help."); 
    kolbi.SetPosition(new Vector2(0.5f, 0));

    yield return new WaitForSeconds(1f);
    kolbi.Show();
    yield return kolbi.Say("Ugh, my body feels weak, and my bones ache like broken rocks.");

    Adventurer.SetPosition(Vector2.zero);
    kolbi.UnHighLight();
    yield return Adventurer.Show();
    yield return Adventurer.Say("Are you blaming someone for this?");

    kolbi.HighLight();
    Adventurer.UnHighLight();
    Sprite Suprisebodykolbi = kolbi.GetSprite("B_Talking2");
    Sprite Suprisefacekolbi = kolbi.GetSprite("B_Blush");
    kolbi.TransitionSprite(Suprisebodykolbi, 0);
    kolbi.TransitionSprite(Suprisefacekolbi, 1);
    kolbi.Animate("Hop");
    yield return kolbi.Say("HEY!");
    yield return kolbi.MoveToPosition(new Vector2(1, 0));
    Sprite Normalbodykolbi = kolbi.GetSprite("A_Talking");
    Sprite Normalfacekolbi = kolbi.GetSprite("A_Normal");
    kolbi.TransitionSprite(Normalbodykolbi, 0);
    kolbi.TransitionSprite(Normalfacekolbi, 1);
    
    kolbi.Animate("Shiver", true);
    yield return kolbi.Say("Can you stop sneaking up on me like that?!");
    kolbi.Animate("Shiver", false);

    kolbi.UnHighLight();
    Adventurer.HighLight();       
    yield return Adventurer.Say("Haha, sorry about that.{c} So, do you need help?");

    kolbi.HighLight();
    Adventurer.UnHighLight();
    yield return kolbi.Say("YES, absolutely!{a} I need a machine to help me recover.");

    kolbi.UnHighLight();
    Adventurer.HighLight();       
    yield return Adventurer.Say("Alright, follow me.");

    kolbi.HighLight();
    Adventurer.UnHighLight();
    yield return kolbi.Say("Where?");

    kolbi.UnHighLight();
    Adventurer.HighLight();       
    yield return Adventurer.Say("Just follow me then.");

    kolbi.HighLight();
    Adventurer.UnHighLight();
    Sprite foolfacekolbi = kolbi.GetSprite("A_Fool");
    kolbi.TransitionSprite(foolfacekolbi, 1);
    yield return kolbi.Say("Alright whatever");

    Adventurer.Hide();
    kolbi.Hide();

    yield return kolbi.Say("K-KYAH!");

    yield return narator.Say("Something happened"); 

    
    yield return null;
}


    // Update is called once per frame
    void Update()
    {
        
    }
}
