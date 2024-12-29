using System;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
//class dengan fungsi database manager untuk commands
public class CommandDatabase 
{
    private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

    public bool hasCommands(string commandName) => database.ContainsKey(commandName);

    //ketika ingin menambahkan command ke tipe delagate command tertentu
    public void AddCommand(string commandName, Delegate command)
    {
        if(!database.ContainsKey(commandName))
        {
            database.Add(commandName, command);
        }
        else
        {
            Debug.LogError($"Command alredy exists in the database '{commandName}'");
        }
    }

    //mendapatkan command menyesuaikan dengan namanya
    public Delegate GetCommand(string commandName)
    {
        if(!database.ContainsKey(commandName))
        {
            Debug.LogError($"Command'{commandName}' does not exist in database");
            return null;
        }

        return database[commandName];
    }
}
}