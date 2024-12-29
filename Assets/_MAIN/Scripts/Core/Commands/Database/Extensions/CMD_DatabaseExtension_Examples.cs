using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using COMMANDS;

//testing ketika ingin membuat command
namespace TESTING
{

//dibuat menjadi anakna dari database extension agar bisa di detect oleh command manager
public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtension
{
    //variable yang mengandung fungsi fungsi didalamnya
    //menggunakan nama extend agar semua method dialamnya bisa diakses oleh command manager
    new public static void Extend(CommandDatabase database)
    {
        //add Action` with no parameters
        database.AddCommand("print", new Action(PrintDefaultMassage));
        database.AddCommand("print_1p", new Action<string>(PrintUsermessage));
        database.AddCommand("print_mp", new Action<string[]>(PrintLines));
        //Make lambda with no parameter
        database.AddCommand("lambda", new Action(() => { Debug.Log("Printing a default message to console from lambda command."); }));
        database.AddCommand("lambda_1p", new Action<string>((arg)=> {Debug.Log($"Log user lambda message = '{arg}'");}));
        database.AddCommand("lambda_mp", new Action<string[]>((args)=>{Debug.Log(string.Join(", ", args));}));
        //add coroutine with no parameter
        database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
        database.AddCommand("process_1p", new Func<string, IEnumerator>(LineProcess));
        database.AddCommand("process_mp", new Func<string[],IEnumerator>(MultilineProcess));

        //Special Example
        database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));
    }

    //perilaku dari setiap fungsinya ada dibawah sini
    private static void PrintDefaultMassage()
    {
        Debug.Log("Printing a default message to console.");
    }

    private static void PrintUsermessage (string message)
    {
        Debug.Log($"'User MEssage: '{message}'");
    }

    private static void PrintLines(string[] lines)
    {
        int i = 1;
        foreach (string line in lines)
        {
            Debug.Log($"{i++}. '{line}'");
        }
    }

    private static IEnumerator SimpleProcess()
    {
        for(int i = 1; i<=5;i++)
        {
            Debug.Log($"Process Running...[{i}]");
            yield return new WaitForSeconds(1);
        }
    }

    private static IEnumerator LineProcess(string data)
    {
        if(int.TryParse(data, out int num))
        {
            for(int i = 1; i<=num;i++)
        {
            Debug.Log($"Process Running...[{i}]");
            yield return new WaitForSeconds(1);
        }
        }
    }

    private static IEnumerator MultilineProcess(string[] data)
    {
        foreach(string line in data)
        {
            Debug.Log($"Procces Message : '{line}");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private static IEnumerator MoveCharacter(string direction)
    {
        bool left = direction.ToLower() == "left";

        Transform character = GameObject.Find("Image").transform;
        float moveSpeed = 15;

        float targetx = left ? -8 : 8;

        float currentX = character.position.x;

        while (Mathf.Abs(targetx-currentX) > 0.1f)
        {
            currentX = Mathf.MoveTowards(currentX, targetx, moveSpeed * Time.deltaTime);
            character.position = new Vector3(currentX, character.position.y, character.position.z);
            yield return null;
        }
    }
}
}