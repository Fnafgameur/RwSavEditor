
using System.Text;
using System.Text.RegularExpressions;

namespace RwSavEditor;

class Program
{
    private static string filePath = "";
    private static string folderPath = "";
    private static string fileContent = "";
        
    private static char chosenValue = ' ';
    private static string pattern = "([A-Z]{2}_[A-Z][0-9]{2})";
    private static bool hasPath;
    private static string displayValue;
    private static int displayValueInt;
    private static Dictionary<int, string> charsFoundDictionary = new();

    private static string[] vanillaCharacters = {
        "Yellow&lt;svA&gt;SEED&lt;svB&gt;",
        "White&lt;svA&gt;SEED&lt;svB&gt;",
        "Red&lt;svA&gt;SEED&lt;svB&gt;",
        "Gourmand&lt;svA&gt;SEED&lt;svB&gt;",
        "Artificer&lt;svA&gt;SEED&lt;svB&gt;",
        "Rivulet&lt;svA&gt;SEED&lt;svB&gt;",
        "Spear&lt;svA&gt;SEED&lt;svB&gt;",
        "Saint&lt;svA&gt;SEED&lt;svB&gt;",
        "Inv&lt;svA&gt;SEED&lt;svB&gt;"
    };

    private static string[] moddedCharacters = {
        "Vinki&lt;svA&gt;SEED&lt;svB&gt;",
        "darkness&lt;svA&gt;SEED&lt;svB&gt;",
        "SlugSpore&lt;svA&gt;SEED&lt;svB&gt;",
        "thedronemaster&lt;svA&gt;SEED&lt;svB&gt;",
        "Hubert&lt;svA&gt;SEED&lt;svB&gt;",
        "Photomaniac&lt;svA&gt;SEED&lt;svB&gt;"
    };
        
    public static void Main()
    {
        string characterChoice;
        string statsToFind;
        string valueReturned;
        string newValue;
        string selectedChoice;
            
        const int maxKarma = 9;
        
        charsFoundDictionary.Clear();
            
        if (!hasPath)
        {
            Console.Write("Welcome to the Rain World Save Editor !\n" +
                              "This program allows you to edit your save file in order to change stats of your scugs\n");
            PrintMessage("/!\\ Modded scugs are not supported yet /!\\", "error");
            PrintMessage("/!\\ This project is still in development, even if the program create a backup, create one manually /!\\", "error");
                
            do
            {
                PrintMessage("Provide path to your \"sav\" file (e.g : \"C:/Users/Example/Desktop/sav\" OR \"./sav\" to select in the current directory) : ", "ask");
                filePath = Console.ReadLine();

                if (string.IsNullOrEmpty(filePath))
                {
                    PrintMessage("No path provided !", "error");
                    continue;
                }
                if (filePath == "d" || filePath == "debug")
                {
                    filePath = "C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav_all";
                }
                else if (filePath[0] == '.')
                {
                    filePath = Directory.GetCurrentDirectory() + filePath.Substring(1);
                }
                
                if (!File.Exists(filePath))
                {
                    PrintMessage("File not found !", "error");
                    continue;
                }
                
                if (Path.GetExtension(filePath) != ".sav" && Path.GetExtension(filePath) != "")
                {
                    PrintMessage("Incorrect file extension !", "error");
                    continue;
                }
                
                fileContent = File.ReadAllText(filePath).Substring(0, 32);
                
                if (!fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType"))
                {
                    PrintMessage("Incorrect file !", "error");
                }
            } while (!File.Exists(filePath) || !fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType") || Path.GetExtension(filePath) != ".sav" && Path.GetExtension(filePath) != "");
            
            Console.Write("\nOpened file : ");
            PrintMessage(filePath, "ask");
            folderPath = Path.GetDirectoryName(filePath);
            CreateBackupSave();
            hasPath = true;
        }

        /*
         * TODO :
         * -Créer fichier save "original" pour pouvoir le restaurer en cas de problème 🟢
         * ---Faire vérif si orig file existe, demander si on l'écrase ou non 🟢
         * -Ajouter stats manquantes 🟢
         * -Ajouter Survivor & Hunter 🟢
         * -Ajouter DLC scugs (à tester) 🟢
         * ---Check if scug is in save 🟢
         * -Ajouter modded scugs ? 🟠
         * ---Ajout scugs name manuellement ? 🟠
         * ---Faire en sorte que le nombre de cycle de hunter ne soit pas négatif lors de l'incrémentation de cycles 🟢
         * -Revérifier le code et tester afin de trouver des bugs
         * ---Retester changer valeur string 🟢
         * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...) 🟠
         *
         * -Opti le code
         * -Ajouter des commentaires
         */
        
        FindChars();
        
        characterChoice = AskChar();
        statsToFind = AskStat();

        if (!statsToFind.Contains("DEN"))
        {
            GetIntValue(characterChoice, statsToFind);
            
            if (statsToFind != ";TOTTIME")
            {
                PrintMessage("\nCurrent Value : " + displayValue, "warning");
            }
            else
            {
                PrintMessage("Current Value (in seconds) : " + displayValue, "warning");
            }

            if (statsToFind == ";KARMA")
            {
                PrintMessage("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"), "warning");
            }
            newValue = AskNewValueInt(characterChoice, statsToFind);

            if (statsToFind == ";KARMA")
            {
                var karmaCap = GetIntValue(characterChoice, ";KARMACAP");
                if (int.Parse(newValue) > maxKarma)
                {
                    Console.WriteLine("Karma level can't be higher than 9 !");
                    newValue = AskNewValueInt(characterChoice, statsToFind);
                }

                if (int.Parse(newValue) > int.Parse(karmaCap))
                {
                    string choice;

                    Console.WriteLine("\nKarma level can't be higher than Karma CAP !" +
                                      "\nWould you like to set the Karma CAP to the same value as the Karma level ?\n");
                    Console.Write("Y/n : ");
                    choice = Console.ReadLine();
                    choice = choice.ToUpper();
                    char.TryParse(choice, out chosenValue);
                    if (chosenValue is 'Y' or '\0' or ' ')
                    {
                        Console.WriteLine(EditIntValue(characterChoice, ";KARMACAP", newValue));
                    }
                    else
                    {
                        Console.WriteLine("\nKarma level not changed !\n");
                        Main();
                    }
                }
            }
            EditIntValue(characterChoice, statsToFind, newValue);
        }
        else
        {
            valueReturned = GetStrValue(characterChoice, statsToFind);
            Console.WriteLine("Initial Value : " + valueReturned);
            newValue = AskNewValueStr();
            EditStrValue(characterChoice, statsToFind, newValue);
        }
        PrintMessage("\nValue changed to : " + displayValue + " with success ! ", "success");
        Console.WriteLine("\nWould you like to edit another stat ?\n");
        Console.Write("Y/n : ");
        selectedChoice = Console.ReadLine();
        selectedChoice = selectedChoice.ToUpper();
        char.TryParse(selectedChoice, out chosenValue);
        if (chosenValue == 'Y' || chosenValue == '\0' || chosenValue == ' ')
        {
            Console.Clear();
            Main();
        }
        else
        {
            Console.WriteLine("Goodbye !");
            Environment.Exit(0);
        }
    }

    private static void FindChars()
    {
        var fileContent = File.ReadAllText(filePath);
        var index = 0;
        var returnChars = new string[vanillaCharacters.Length + moddedCharacters.Length];
        string[] charsName;
        var i = 0;
        
        foreach (var charsFound in vanillaCharacters)
        {
            if (!fileContent.Contains(charsFound))
            {
                continue;
            }
            returnChars[index] = charsFound;
            index++;
        }
        
        foreach (var charsFound in moddedCharacters)
        {
            if (!fileContent.Contains(charsFound))
            {
                continue;
            }
            returnChars[index] = charsFound;
            index++;
        }
        
        charsName = new string[index];
        for (i = 0; i < index; i++)
        {
            charsName[i] = returnChars[i].Substring(0, returnChars[i].IndexOf('&'));
            
            charsFoundDictionary.Add(i, charsName[i]);
        }
        
        if (charsFoundDictionary.Count != 0)
        {
            return;
        }
        PrintMessage("No character found !\nPress Enter to restart the program...", "error");
        Console.ReadLine();
        Console.Clear();
        hasPath = false;
        Main();
    }
    
    private static string AskChar()
    {
        fileContent = File.ReadAllText(filePath);
        string characterChoiceStr;
        string charRenamed;
        
        PrintMessage("Enter the character's save you want to edit:", "ask");

        foreach (var chars in charsFoundDictionary)
        {
            switch (chars.Value)
            {
                case "Yellow":
                    charRenamed = "Monk";
                    break;
                case "White":
                    charRenamed = "Survivor";
                    break;
                case "Red":
                    charRenamed = "Hunter";
                    break;
                default:
                    charRenamed = charsFoundDictionary[chars.Key];
                    break;
            }
            
            Console.WriteLine(chars.Key + " = " + charRenamed);
        }
        Console.Write("R = Restart the program\n>");
        
        characterChoiceStr = Console.ReadLine();
        characterChoiceStr = characterChoiceStr.ToUpper();
        char.TryParse(characterChoiceStr, out chosenValue);
        
        if (chosenValue == 'R')
        {
            Console.Clear();
            hasPath = false;
            Main();
        }
        
        if (!int.TryParse(characterChoiceStr, out var num))
        {
            PrintMessage("Enter a valid number !", "error");
            return AskChar();
        }
        
        if (num < 0 || num > charsFoundDictionary.Count - 1)
        {
            PrintMessage("Enter a number between 0 and " + (charsFoundDictionary.Count - 1) + " !", "error");
            return AskChar();
        }

        characterChoiceStr = charsFoundDictionary[num];

        if (fileContent.IndexOf(characterChoiceStr, StringComparison.Ordinal) != -1)
        {
            return characterChoiceStr;
        }
        PrintMessage("Character not found ! Have you saved in his campaign ?", "error");
        return AskChar();

    }
    
    private static string AskStat()
    {
        string statsToFind;
        
        PrintMessage("\nEnter the stat you want to edit:", "ask");
        
        Console.Write("\n\n0 = Number Of Cycle passed" +
                      "\n1 = Number Of Deaths" +
                      "\n2 = Number Of Cycle survived" +
                      "\n3 = Number Of Cycle abandonned" +
                      "\n4 = Time passed" +
                      "\n5 = Number of food" +
                      "\n6 = Position of current den" +
                      "\n7 = Karma Level" +
                      "\n8 = Karma CAP" +
                      "\n9 = Reinforce Karma" +
                      "\nC = Cancel" +
                      "\n>");
        statsToFind = Console.ReadLine();
        statsToFind = statsToFind.ToUpper();
        char.TryParse(statsToFind, out chosenValue);

        switch (statsToFind) 
        {
            case "0":
                statsToFind = ";CYCLENUM";
                break;
            case "1":
                statsToFind = ";DEATHS";
                break;
            case "2":
                statsToFind = ";SURVIVES";
                break;
            case "3":
                statsToFind = ";QUITS";
                break;
            case "4":
                statsToFind = ";TOTTIME";
                break;
            case "5":
                statsToFind = ";FOOD";
                break;
            case "6":
                statsToFind = ";DENPOS";
                break;
            case "7":
                statsToFind = ";KARMA";
                break;
            case "8":
                statsToFind = ";KARMACAP";
                break;
            case "9":
                statsToFind = ";REINFORCEDKARMA";
                break;
            case "C":
                Console.Clear();
                Main();
                break;
            default:
                PrintMessage("Wrong input !", "error");
                return AskStat();
        }

        return statsToFind;
    }
    
    private static string GetIntValue(string character, string valueToFind)
    {
        var fileContent = File.ReadAllText(filePath);
        string returnValue;
        int start;
        int end;

        Console.WriteLine(character);
        
        start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        if (start == -1)
        {
            PrintMessage("Value Not Found !", "error");
            return "";
        }

        end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal) + valueToFind.Length;
        
        returnValue = FindInt(end);
        
        displayValue = returnValue;
        if (!character.Contains("Red") || valueToFind != ";CYCLENUM")
        {
            return returnValue;
        }
        
        // Reverse the value of the cycle number to display it correctly
        int.TryParse(displayValue, out displayValueInt);
        displayValue = (19 - displayValueInt).ToString();

        return returnValue;
    }
    
    private static string GetStrValue(string character, string valueToFind)
    {
        fileContent = File.ReadAllText(filePath);
        var start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        var end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal) + valueToFind.Length;
        var regex = new Regex(pattern);
        var match = regex.Match(fileContent, end);
            
        return match.Groups[1].Value;
    }

    private static string AskNewValueInt(string character, string stat)
    {
        string newValue;
        int newValueInt;
        
        switch (stat)
        {
            case ";TOTTIME":
                PrintMessage("\nEnter the new value (in seconds) (enter C to cancel) : ", "ask");
                break;
            case ";REINFORCEDKARMA":
                PrintMessage("\nEnter the new value (0 or 1) (enter C to cancel) : ", "ask");
                break;
            default:
                PrintMessage("\nEnter the new value (enter C to cancel) : ", "ask");
                break;
        }
        
        newValue = Console.ReadLine();
        newValue = newValue.ToUpper();
        char.TryParse(newValue, out chosenValue);
        if (chosenValue == 'C')
        {
            Console.Clear();
            Main();
        }
        if (!int.TryParse(newValue, out var num))
        {
            PrintMessage("Enter an integer number !", "error");
            return AskNewValueInt(character, stat);
        }
            
        if (num < 0 && stat != ";CYCLENUM")
        {
            PrintMessage("Number must be positive !", "error");
            return AskNewValueInt(character, stat);
        }

        if (stat == ";REINFORCEDKARMA" && num != 0 && num != 1)
        {
            PrintMessage("Number must be either 0 or 1 !", "error");
            return AskNewValueInt(character, stat);
        }
        
        displayValue = newValue;

        if (!character.Contains("Red") || stat != ";CYCLENUM")
        {
            return newValue;
        }
        
        int.TryParse(newValue, out newValueInt);
        newValue = (19 - newValueInt).ToString();

        return newValue;
    }

    private static string AskNewValueStr()
    {
        string newValue;
        
        PrintMessage("\nEnter the new value ", "ask");
        PrintMessage("\n/!\\ make sure that it is a valid DEN Room, use the interactive map to get the name of the den you want /!\\ ", "error");
        PrintMessage("(enter C to cancel) : ", "ask");
        
        newValue = Console.ReadLine();
        newValue = newValue.ToUpper();
        char.TryParse(newValue, out chosenValue);
        if (chosenValue == 'C')
        {
            Console.Clear();
            Main();
        }
        var regex = new Regex(pattern);
        var match = regex.Match(newValue);
        if (match.Success)
        {
            return newValue;
        }
        PrintMessage("Enter a valid room (example : SU_S01) !", "error");
        return AskNewValueStr();
    }
    
    private static string EditIntValue(string character, string valueToFind, string newValue)
    {
        var fileContent = File.ReadAllText(filePath);
        string numberStr;
        string replaced;
        int start;
        int end;
        int index;
        
        start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        if (start == -1)
        {
            PrintMessage("Value Not Found !", "error");
            return "";
        }

        end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal) + valueToFind.Length;
            
        numberStr = FindInt(end);
            
        index = fileContent.IndexOf(numberStr, end, StringComparison.Ordinal);
        replaced = fileContent.Substring(0, index) + newValue + fileContent.Substring(index + numberStr.Length);
        
        File.WriteAllText(filePath, replaced);
        return numberStr;
    }
    
    private static void EditStrValue(string character, string valueToFind, string newValue)
    {
        fileContent = File.ReadAllText(filePath);
        var start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        var end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal) + valueToFind.Length;
        var regex = new Regex(pattern);
        var match = regex.Match(fileContent, end);
        var replaced = fileContent.Substring(0, match.Index) + newValue + fileContent.Substring(match.Index + match.Length);
        displayValue = newValue;
            
        File.WriteAllText(filePath, replaced);
    }

    private static string FindInt(int startIndex)
    {
        var returnValue = "";
        
        var number = new StringBuilder();
        for (int i = startIndex; i < fileContent.Length; i++)
        {
            if (!char.IsDigit(fileContent[i]) && fileContent[i] != '-')
            {
                continue;
            }
            for (var j = i; j < fileContent.Length; j++)
            {
                if (char.IsDigit(fileContent[j]) || fileContent[j] == '-') 
                {
                    number.Append(fileContent[j]);
                }
                else
                {
                    break;
                }
            }
            returnValue = number.ToString();
            break;
        }
        return returnValue;
    }
    
    private static void CreateBackupSave()
    {
        string origFilePath;
        
        Console.WriteLine("Creating backup save...");
        origFilePath = folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_backup" + Path.GetExtension(filePath);
        
        if (File.Exists(origFilePath))
        {
            string choice;
                
            Console.Write("Backup save already exists in ");
            PrintMessage(folderPath, "info");
            Console.Write("Would you like to overwrite it ?\n\ny/N : ");
            choice = Console.ReadLine();
            choice = choice.ToUpper();
            char.TryParse(choice, out chosenValue);
            if (chosenValue == 'Y')
            {
                // Overwrite the backup save
                File.WriteAllText(origFilePath, File.ReadAllText(filePath));
                PrintMessage("Backup save overwritten !", "success");
            }
            else
            {
                PrintMessage("Backup save not created !", "success");
            }
        }
        else
        {
            // Create a backup save
            File.Create(origFilePath).Dispose();
            File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            PrintMessage("Backup save created !", "success");
        }
    }
    
    private static void PrintMessage(string message, string messageType)
    {
        Console.ForegroundColor = messageType switch
        {
            "error" => ConsoleColor.Red,
            "success" => ConsoleColor.Green,
            "warning" => ConsoleColor.DarkYellow,
            "info" => ConsoleColor.Cyan,
            "ask" => ConsoleColor.DarkCyan,
            _ => ConsoleColor.White
        };
        Console.WriteLine("\n" + message + "\n");
        Console.ResetColor();
        Thread.Sleep(360);
    }
}