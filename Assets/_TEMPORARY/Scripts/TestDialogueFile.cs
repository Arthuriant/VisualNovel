using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class TestDialogueFile : MonoBehaviour
{

    [SerializeField] private TextAsset file;
 void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        List<string> lines = FIleManager.ReadTextAsset(file);

        //foreach (string line in lines)
       // {
         //   if(string.IsNullOrWhiteSpace(line))
         //   {
         //       continue;
         //   }

         //   DIALOGUE_LINE dl = DialogueParser.Parse(line);

          //  for (int i=0; i<dl.commandsData.commands.Count; i++)
          //  {
          //      DL_COMMAND_DATA.Command command = dl.commandsData.commands[i];
           //     Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
           // }
       // }

        DialogueSystem.instance.Say(lines);
    }
}
