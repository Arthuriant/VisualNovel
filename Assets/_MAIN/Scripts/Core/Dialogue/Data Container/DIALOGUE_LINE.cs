using UnityEngine;

namespace DIALOGUE
{
    public class DIALOGUE_LINE 
{
    //pembuatan tipe data khusus
    public DL_SPEAKER_DATA speakerData;
    public DL_DIALOGUE_DATA dialogueData;
    public DL_COMMAND_DATA commandsData;

    //boolean sebagai pengaman
    public bool hasSpeaker => speakerData != null;
    public bool hasDialogue => dialogueData != null;
    public bool hasCommands => commandsData != null;
    

    //pembuatan dialogue_line berdasarkan 3 tipe data yang telah dibuat sebelumnya
    public DIALOGUE_LINE(string speaker, string dialogue, string commands)
    {
        this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SPEAKER_DATA (speaker));
        this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null :new DL_DIALOGUE_DATA (dialogue));
        this.commandsData =(string.IsNullOrWhiteSpace(commands) ? null : new DL_COMMAND_DATA (commands));
    }
}
}
