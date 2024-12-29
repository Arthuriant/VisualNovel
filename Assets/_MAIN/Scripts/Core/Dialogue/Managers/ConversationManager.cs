using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Runtime.InteropServices;
using COMMANDS;

namespace DIALOGUE
{
public class ConversationManager 
{
    //inisiliasisasi
    private DialogueSystem dialogueSystem => DialogueSystem.instance;
    private Coroutine process = null;
    public bool isRunning => process != null;
    private bool userPrompt = false;
    private TextArchitect architect = null;

    //structnya untuk mendifinisikan architct dan dialogue system
    public ConversationManager(TextArchitect architect)
    {
        this.architect = architect;
        dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
    }

    //fungsi untuk mengganti nilai boolean userprompt
    private void OnUserPrompt_Next()
    {
        userPrompt = true;
    }

    //fungsi untuk memulai konverasi
    public Coroutine StartConversation(List<string> conversation)
    {
        //menghentikan konversasi yang ada terlebih dahulu
        StopConversation();
        //memakai dialogue system agar bisa startcoroutine di monobehaviour
        //menjalankan coroutine running conversation
        process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
        //mengembalikan nilai hasil dari coroutine dan dimasukan kedalam process
        return process;
    }

    //fungsi untuk menghentikan conversasi
    public void StopConversation()
    {
        //jika sudah berhenti maka keluar dari fungsi ini
        if(!isRunning)
        {
            return;
        }

        //menghentikan coroutine
        dialogueSystem.StopCoroutine(process);
        //membuat nilai dari process menjadi null
        process = null;
    }

    //Coroutine untuk menjalankan converasi
    IEnumerator RunningConversation(List<string> conversation)
    {
        //setiap list string akan dilakukan perulangan hingga habis
        for(int i = 0; i<conversation.Count; i++)
        {
            //jika kosong atau spasi maka akan dilanjutkan ke iterasi berikutnya
            if(string.IsNullOrWhiteSpace(conversation[i]))
            {
                continue;
            }

            //memasukan nilai line yang sudah di parse menggunakan dialogue parser
            DIALOGUE_LINE line = DialogueParser.Parse(conversation[i]);

            //mengecheck apakah  line memiliki dialogue ataupun command,jika ada maka akan menjalankan fungsi tertentu
            if(line.hasDialogue)
            {
                yield return Line_RunDialogue(line);
            }

            if(line.hasCommands)
            {
                yield return Line_RunCommands(line);
            }

            if(line.hasDialogue)
            {
                yield return WaitForUserInput();
            }
        }
    }

    
    //fungsi untuk menjalankan dialogue
    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {
        //jika line memiliki speaker maka
        if(line.hasSpeaker)
        {
            //menampilkan displayname dari speaker
            dialogueSystem.ShowSpeakerName(line.speakerData.displayname);
        }

        //dialogue dibuat
        yield return BuildLineSegments(line.dialogueData);
  


    }

    //fungsi untuk menjalankan commands
    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA.Command> commands = line.commandsData.commands;
        
        foreach(DL_COMMAND_DATA.Command command in commands)
        {
            //jika masih ada command yang berjalan
            if(command.waitForCompletion)
            {
                //menjalankan program setelah command sebelumnya selesai
                yield return CommandManager.instance.Execute(command.name, command.arguments);
            }
            else
            {
                //langsung menjalankan command
                CommandManager.instance.Execute(command.name, command.arguments);
            }
        }
        yield return null;
    }

    //fungsi untuk membuat dialogue
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
    {
        //untuk setiap segmentnya
        for(int i= 0;i<line.segments.Count;i++)
        {
            //mengambil setiap segment pada segments
            DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];
            //menjalankan fungsi untuk menunggu sinyal aktif, jika tidak ada sinyal yaudah
            yield return WaitForDialogueSegmentSignalToBeTriggered(segment);
            //menjalankan fungsi untuk membuat dialogue
            yield return BuildDialogue(segment.dialogue, segment.appendText);
        }
    }

    IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
    {
        switch(segment.startSignal)
        {
            //untuk signal c dan a maka yang dilakukan adalah menunggu input dari user
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
            yield return WaitForUserInput();
            break;

            //untuk wc dan wa maka akan menunggu sampai waktu yang diinputkan habis
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
            case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
            yield return new WaitForSeconds(segment.signalDelay);
            break;
            default:
            break;
        }
    }

    //fungsi untuk mebuat dialogue
    IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        //mengetahui apakah text dibuild baru atau hanya menamabahkan
        if(!append)
        {
            architect.Build(dialogue);
        }
        else
        {
            architect.Append(dialogue);
        }

        //ketika text di build
        while (architect.isBuilding)
        {
            //jika nilai promp true
            if(userPrompt)
            {
                //arsite statusnya bukan hurry up
                if(!architect.hurryUp)
                {
                    //mengubah status hurry up menjadi true
                    architect.hurryUp = true;
                }
                else
                {
                    //jika sudah hurry up maka langsung menjalankan fungsi force complete
                    architect.ForceComplete();
                }
                //mengubah nilai user promp menjadi false;

                userPrompt = false;
            }
            yield return null;
        }
    }

    //mengubah nilai dari userprompt menjadi faslse
    IEnumerator WaitForUserInput()
    {
        while(!userPrompt)
        {
            yield return null;
        }

        userPrompt = false;
    }

}

}

