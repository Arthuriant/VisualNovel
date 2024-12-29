using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using CHARACTERS;

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
{
    //data yang diterima dari scriptable object
    [SerializeField] private DialogueSystemConfigurationSO _config;
    public DialogueSystemConfigurationSO config => _config;
    //pendeklarasian dan penginisialisasian variable
    public DialogueContainer dialogueContainer = new DialogueContainer();
    private ConversationManager conversationManager;
    private TextArchitect architect;

    public static DialogueSystem instance {get; private set;}

    public delegate void DialogueSystemEvent();
    public event DialogueSystemEvent onUserPrompt_Next;
    public bool isRunningConversation => conversationManager.isRunning;

    //fungsi yang perama kali dijalankan : memastikan skrip muncul 2x
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Initialize();
        }else{
            DestroyImmediate(gameObject);
        }
    }

    //inisiliasisasi textarchitext dan convercation manager
    bool _initialized = false;
    private void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        architect = new TextArchitect(dialogueContainer.dialogueText);
        conversationManager = new ConversationManager(architect);
    }

    public void OnUserPrompt_Next()
    {
        onUserPrompt_Next?.Invoke();
    }

    //menampilkan data dari speaker 
    public void ApplySpeakerDataToDialogueContainer(string speakerName)
    {   //mendapatkan karakter berdasarkan namanya
        Character character = CharacterManager.instance.GetCharacter(speakerName);
        //memilih nilai yang akan diisi ke cofig
        CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);
        //menerapkan config dari character ke container visualnya
        ApplySpeakerDataToDialogueContainer(config);
        
    }

    public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
    {
        //menerapkan stiap config pada setiap fungsi yang berbeda
        //untuk dialogue container keseluruhan
        dialogueContainer.SetDialogueColor(config.dialogueColor);
        dialogueContainer.SetDialogueFont(config.dialogueFont);
        //untuk nama saja
        dialogueContainer.nameContainer.SetNameColor(config.nameColor);
        dialogueContainer.nameContainer.SetNameFont(config.nameFont);
    }

    //fungsi untuk menampilkan nama speaker
    public void ShowSpeakerName(string speakerName = "") 
    {
        //jika bukan narator maka akan di hide
        if(speakerName.ToLower() != "narrator")
        {
            dialogueContainer.nameContainer.show(speakerName);
        }else
        {
            HideSpeakerName();
        }
    }

    //fungsi ini akan menjalakan fungsi hide dalam namecontaienr
    public void HideSpeakerName()=> dialogueContainer.nameContainer.Hide();

    //fungsi untuk memformat dialogue
    public Coroutine Say(string speaker, string dialogue)
    {
        //membuat speaker dan dialogue yang masuk menjadi berformat seperti adnika "hallo"
        List<string> conversation =new List<string>() {$"{speaker}\"{dialogue}\""};
        //mengembalikan nilai dari fungsi say yang lain
        return Say(conversation);
    }

    //fungsi untuk memasukan dialogue
    public Coroutine Say(List<string> conversation)
    {
        //memasukan dialogue yang sudah difromat pada conversation manager
        return conversationManager.StartConversation(conversation);
    }
}    
}

