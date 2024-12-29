using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
//class untuk memisahkan dialogue
public class DialogueParser 
{
    //pattern untuk diidentifikasi dengan regex nantinya
    private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";

    //Fungsi untuk  memecah dialogue dengan tipe dialogue line
    public static DIALOGUE_LINE Parse(string rawLine)
    {
        Debug.Log($"Pasting line = '{rawLine}'");
        //memecah rawline menjadi speaker,dialogue,dan commands menggunakan fungsi rip content
        (string speaker, string dialogue, string commands) = RipContent(rawLine);
        Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}'\nCommands = '{commands}'\n ");
        //speaker dialogue dan commands dijadikan dialogue line yang baru
        return new DIALOGUE_LINE(speaker,dialogue,commands);
    }

    private static (string,string,string) RipContent(string rawLine)
    {
        //inisiliasisasi
        string speaker = "",dialogue="",commands="";
        int dialogueStart = -1;
        int dialogueEnd = -1;
        bool isEscaped = false;

        //perulangan yang akan berhenti ketika panjang dari dialogue habis
        for(int i = 0; i<rawLine.Length; i++)
        {
            //mengetahui apakah dialogue memiliki escape atau tidak
            char current = rawLine[i];
            if(current == '\\')
            {   
                isEscaped = !isEscaped;
            }

            //mengetahui dimana posisi dialogue berawal dan berakhir
            else if(current == '"' && !isEscaped)
            {
                if(dialogueStart == -1)
                {
                    dialogueStart = i;
                }else if(dialogueEnd == -1)
                {
                    dialogueEnd = i;
                }
            }
            else
            {
                isEscaped = false;
            }
        }

        //identify command pattern using regex lalu menyimpannya pada mataches
        Regex commandRegex = new Regex(commandRegexPattern);
        MatchCollection matches = commandRegex.Matches(rawLine);
        int commandStart = -1;

        //untuk setiap match diberlakukan sebagai berikut
        foreach(Match match in matches)
        {
            //jika posisi match index itu lebih kecil daripada dialogue start atau lebih besar daripada dialogue end
            if (match.Index < dialogueStart || match.Index > dialogueEnd)
            {
                //start commandnya ada di index tersebut dan keluar dari foreach
                commandStart = match.Index;
                break;
            }

        }

        //jika command startnya bukan atau sama dengan 1, atau command startnya ada namun dialogue startnya tidak ada sedangkan dialogue endnya ada

        if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd ==1))
        {
            //mengembalikan nilai kosong untuk nama dan dialogue, dan menghapus spasi pada rawline untuk commmand
            return ("","", rawLine.Trim());
        }

        //jika dialoguenya ada dan command startnya ada atau command start posisinya lebih besar daripada dialogue end
        if(dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
        {
            //speaker diambil dari rawline dengan mengambil substringnya dari 0 sampai index dialogue start + menghapus spasi
            speaker = rawLine.Substring(0,dialogueStart).Trim();
            //dialogue diambil dari rawline dengan mengambil substringnya mulai dari dialogue start+1 sampai dialogue end-dialoguestart -1 + mengganti \\\ dengan \
            dialogue = rawLine.Substring(dialogueStart+1,dialogueEnd - dialogueStart-1).Replace("\\\"","\"");
            //jika command startnya ada maka
            if(commandStart != -1)
            {
                //command diambil dari substring commandstart sampai akhir + spasinya dihapus
                commands = rawLine.Substring(commandStart).Trim();
            }
            //jika commandstartnya ada dan dialogue start lebih besar daripada command start maka itu adalah commands
        }else if (commandStart != -1 && dialogueStart > commandStart)
        {
            commands = rawLine;
        }else
        {
            dialogue = rawLine;
        }
        //mengembalikan nilai speaker dialogue dan command
        return(speaker,dialogue,commands);
    }
}
}

