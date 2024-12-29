using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using System.Collections;

namespace COMMANDS
{
public class CommandManager : MonoBehaviour
{
    //Deklarasi variable
    public static CommandManager instance {get; private set;}
    private CommandDatabase database;
    private static Coroutine process = null;
    public static bool isRunningProcess => process != null;

    //fungsi yang dijalankan pertama kali
    private void Awake()
    {

        if (instance == null)
        {
            //pembuatan database dan pengisian instance menjadi data dalam class saat ini
            instance = this;
            database = new CommandDatabase();

            //mendapat referensi terhadap code yang sedang dijalankan
            Assembly assembly = Assembly.GetExecutingAssembly();
            //mendapatkan data dari code yang berasal dari subclass CMD_DatabaseExtemsion dan mengonversinya menjadi array
            Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();


            //mendapatkan setiap method ketika bernama extend
            foreach(Type extension in extensionTypes)
            {
                MethodInfo extendMethod = extension.GetMethod("Extend");
                //Memanggil metode Extend secara statis (null digunakan karena ini metode statis).
                //Melewatkan database sebagai parameter ke metode Extend.
                extendMethod.Invoke(null, new object[] {database});
            }
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public Coroutine Execute(string commandName, params string[] args)
    {
        //mendapatkan command dari database
        Delegate command = database.GetCommand(commandName);
        if(command == null)
        {
            return null;
        }

        return StartProcess(commandName, command, args);

    }

    private Coroutine StartProcess(string commandName, Delegate command, string[] args)
    {
        //menghentikan procces yang sedang mberlangsung
        StopCurrentProcess();
        //menjalankan coroutine running procces
        process = StartCoroutine(RunningProcess(command, args));
        return process;
    }

    //fungsi untuk menghentikan process
    private void StopCurrentProcess()
    {
        if(process!=null)
        {
            StopCoroutine(process);
        }

        process =null;
    }

    //Coroutine untuk menajlankan procces
    private IEnumerator RunningProcess(Delegate command, string [] args)
    {
        //menjalankan coroutine laiting untuk memroses
        yield return WaitingForProcessToComplete(command, args);

        process = null;
    }

    private IEnumerator WaitingForProcessToComplete(Delegate command, string [] args)
    {
        if(command is Action)
        {
            //menjalankan fungsi yang ada dalam method dengan parameter menyesuaikan
            command.DynamicInvoke();
        }else if( command is Action<string>)
        {
            //ketika memiliki parameter
            command.DynamicInvoke(args[0]);
        }else if( command is Action<string[]>)
        {
            //ketika memiliki parameter dalam bentuk array
            command.DynamicInvoke((object)args);
        }
        else if (command is Func<IEnumerator>)
        {
            //menjalankan coroutine yang ada dalam method dengan parameter menyesuaikan
            yield return((Func<IEnumerator>)command)();
        }
        else if (command is Func<string,IEnumerator>)
        {
            //ketika memiliki parameter
            yield return((Func<string,IEnumerator>)command)(args[0]);
        }
        else if (command is Func<string[],IEnumerator>)
        {
            //ketika memiliki parameter dalam bentuk array
            yield return((Func<string[],IEnumerator>)command)(args);
        }
    }
}
}