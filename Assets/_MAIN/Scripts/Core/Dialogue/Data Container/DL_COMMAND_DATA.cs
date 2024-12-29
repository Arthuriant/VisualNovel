using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{

//class untuk tipe command 
public class DL_COMMAND_DATA
{
    //tempat menyimpan ellemen command secara dinamis
    public List<Command> commands;

    //pendeteksi string nya menggunakan keyword tertentu
    private const char COMMANDSPLITTER_ID = ',';
    private const char ARGUMENTSCONTAINER_ID = '(';
    private const string WAITCOMMAND_ID = "[wait]";

    //isi dari command 
    public struct Command
    {
    public string name;
    public string [] arguments;
    public bool waitForCompletion;
    }

    //mengisi data commands dengan listcommand yg sudah dibuat pada fungsi lain dan ini adalah constructor
    public DL_COMMAND_DATA(string rawCommands)
    {
        commands = RipCommands(rawCommands);
    }

    //fungsi untuk mengisi listcommand
    private List<Command> RipCommands(string rawCommands)
    {
        //data dipecah dengan indikator commandsplitter
        string[] data = rawCommands.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
        //membuat listcommand baru
        List<Command> result = new List<Command>();
        foreach(string cmd in data)
        {
            Command command = new Command();
            //memisahkan antara nama dan argument pada command
            int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
            command.name = cmd.Substring(0,index).Trim();

            //jika namanya wautcommand maka
            if (command.name.ToLower().StartsWith(WAITCOMMAND_ID))
            {
                command.name = command.name.Substring(WAITCOMMAND_ID.Length);
                command.waitForCompletion = true;
            }
            else
            {
                command.waitForCompletion = false;
            }

            //mendapatkan argument menggunakan fungsi getargs dari substring command
            command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
            result.Add(command);
        }

        return result;
    }

    //fungsi untuk mendapatkan argument
    private string[] GetArgs(string args)
    {
        List<string> argList =  new List<string>();
        //string yang dapat diubah ubah
        StringBuilder currentArg = new StringBuilder();
        bool inQuotes = false;

        for (int i=0; i < args.Length; i++)
        {
            //mengecheck apakah argument memiliki kutip atau tidak
            if(args[i] == '"')
            {
                inQuotes = !inQuotes;
                //langsung ke iterasi berikutnya
                continue;
            }

            //jika tidak memiliki kutip dan args adalah spasi  maka

            if (!inQuotes && args[i] == ' ')
            {
                //yang dimasukan ke arglist adalah current argnya/spasinya di skip
                argList.Add(currentArg.ToString());
                //current arg di clear
                currentArg.Clear();
                //masuk ke iterasi berikutnya
                continue;
            }
            //memasukan args dalam iterasi kedalam current arg
            currentArg.Append(args[i]);
        }

        //jika current arg masih ada sisa setelah iterasi artinya belum masuk ke arglist
        if(currentArg.Length > 0)
        {
            //data dari current arg dimasukan ke arglist
            argList.Add(currentArg.ToString());
        }
        //arglist dimasukan ke array
        return argList.ToArray();
    }
}
}