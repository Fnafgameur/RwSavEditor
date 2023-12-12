using System.Drawing;
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

    private static string[] characters = {
        "Yellow&lt;svA&gt;SEED&lt;svB&gt;",
        "White&lt;svA&gt;SEED&lt;svB&gt;",
        "Red&lt;svA&gt;SEED&lt;svB&gt;",
        "Gourmand&lt;svA&gt;SEED&lt;svB&gt;",
        "Artificer&lt;svA&gt;SEED&lt;svB&gt;",
        "Rivulet&lt;svA&gt;SEED&lt;svB&gt;",
        "Spear&lt;svA&gt;SEED&lt;svB&gt;",
        "Saint&lt;svA&gt;SEED&lt;svB&gt;"
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
            PrintMessage("\n/!\\ Modded scugs are not supported yet /!\\", "error", true);
            PrintMessage("/!\\ This project is still in development, even if the program create a backup, create one manually /!\\\n", "error", true);
            
            do
            {
                PrintMessage("Provide path to your \"sav\" file (e.g : \"C:/Users/Example/Desktop/sav\" OR \"./sav\" to select in the current directory) : ", "ask", false);
                filePath = Console.ReadLine();

                if (string.IsNullOrEmpty(filePath))
                {
                    PrintMessage("\nNo path provided !", "error", true);
                    continue;
                }
                if (filePath == "d" || filePath == "debug")
                {
                    filePath = "C:\\Users\\Djimmy\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav-t-pup";
                }
                else if (filePath[0] == '.')
                {
                    filePath = Directory.GetCurrentDirectory() + filePath.Substring(1);
                }
                
                if (!File.Exists(filePath))
                {
                    PrintMessage("\nFile not found !", "error", true);
                    continue;
                }
                
                if (Path.GetExtension(filePath) != ".sav" && Path.GetExtension(filePath) != "")
                {
                    PrintMessage("\nIncorrect file extension !", "error", true);
                    continue;
                }
                
                fileContent = File.ReadAllText(filePath).Substring(0, 32);
                
                if (!fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType"))
                {
                    PrintMessage("\nIncorrect file !", "error", true);
                }
            } while (!File.Exists(filePath) || !fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType") || Path.GetExtension(filePath) != ".sav" && Path.GetExtension(filePath) != "");
            
            Console.Write("\nOpened file : ");
            PrintMessage(filePath, "ask", true);
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
         * -Ajouter FORCEPUPS
         * ---Check if scug is in save
         * -Ajouter modded scugs ?
         * ---Ajout scugs name manuellement ?
         * ---Faire en sorte que le nombre de cycle de hunter ne soit pas négatif lors de l'incrémentation de cycles 🟢
         * -Revérifier le code et tester afin de trouver des bugs
         * ---Retester changer valeur string 🟢
         * ---Refaire système Cycle Hunter
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
            
            if (statsToFind != ";TOTTIME" && statsToFind != ";CyclesSinceSlugpup")
            {
                PrintMessage("\nCurrent Value : " + displayValue, "warning", true);
            }
            else if (statsToFind == ";TOTTIME")
            {
                PrintMessage("Current Value (in seconds) : " + displayValue, "warning", true);
            }

            if (statsToFind == ";KARMA")
            {
                PrintMessage("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"), "warning", true);
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
        PrintMessage("\nValue changed to : " + displayValue + " with success ! ", "success", true);
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
        var returnChars = new string[8];
        string[] charsName;
        
        foreach (var charsFound in characters)
        {
            if (fileContent.IndexOf(charsFound, StringComparison.Ordinal) == -1)
            {
                continue;
            }
            returnChars[index] = charsFound;
            index++;
        }
        
        charsName = new string[index];
        for (var i = 0; i < index; i++)
        {
            charsName[i] = returnChars[i].Substring(0, returnChars[i].IndexOf('&'));

            switch (charsName[i])
            {
                case "Yellow":
                    charsName[i] = "Monk";
                    break;
                case "White":
                    charsName[i] = "Survivor";
                    break;
                case "Red":
                    charsName[i] = "Hunter";
                    break;
                case "Spear":
                    charsName[i] = "Spearmaster";
                    break;
            }
            
            charsFoundDictionary.Add(i, charsName[i]);
        }

        if (charsFoundDictionary.Count != 0)
        {
            return;
        }
        PrintMessage("\nNo character found !\nPress Enter to restart the program...", "error", false);
        Console.ReadLine();
        Console.Clear();
        hasPath = false;
        Main();
    }
    
    private static string AskChar()
    {
        fileContent = File.ReadAllText(filePath);
        string characterChoiceStr;
        
        PrintMessage("\nEnter the character's save you want to edit:\n", "ask", true);

        foreach (var chars in charsFoundDictionary)
        {
            Console.WriteLine(chars.Key + " = " + chars.Value);
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
            PrintMessage("\nEnter a valid number !", "error", true);
            return AskChar();
        }
        
        if (num < 0 || num > charsFoundDictionary.Count - 1)
        {
            PrintMessage("\nEnter a number between 0 and " + (charsFoundDictionary.Count - 1) + " !", "error", true);
            return AskChar();
        }

        characterChoiceStr = characters[charsFoundDictionary.Keys.ElementAt(num)];

        if (fileContent.IndexOf(characterChoiceStr, StringComparison.Ordinal) != -1)
        {
            return characterChoiceStr;
        }
        PrintMessage("\nCharacter not found ! Have you saved in his campaign ?", "error", true);
        return AskChar();

    }
    
    private static string AskStat()
    {
        string statsToFind;
        
        PrintMessage("\nEnter the stat you want to edit:", "ask", false);
        
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
                      "\n10 = Force pup to spawn this cycle" +
                      "\nC = Cancel" +
                      "\n>");
        statsToFind = Console.ReadLine().ToUpper();

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
            case "10":
                statsToFind = ";CyclesSinceSlugpup";
                break;
            case "C":
                Console.Clear();
                Main();
                break;
            default:
                PrintMessage("\nWrong input !", "error", true);
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
        
        start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        
        if (start == -1)
        {
            PrintMessage("\nValue Not Found !", "error", true);
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
                PrintMessage("\nEnter the new value (in seconds) (enter C to cancel) : ", "ask", false);
                break;
            case ";REINFORCEDKARMA":
                PrintMessage("\nEnter the new value (0 or 1) (enter C to cancel) : ", "ask", false);
                break;
            case ";CyclesSinceSlugpup":
                PrintMessage("\nEnter the new value (0 or 1) (enter C to cancel) : ", "ask", false);
                break;
            default:
                PrintMessage("\nEnter the new value (enter C to cancel) : ", "ask", false);
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
            PrintMessage("\nEnter an integer number !", "error", true);
            return AskNewValueInt(character, stat);
        }
            
        if (num < 0 && stat != ";CYCLENUM")
        {
            PrintMessage("\nNumber must be positive !", "error", true);
            return AskNewValueInt(character, stat);
        }

        if ((stat == ";REINFORCEDKARMA" || stat == ";CyclesSinceSlugpup") && num != 0 && num != 1)
        {
            PrintMessage("\nNumber must be either 0 or 1 !", "error", true);
            return AskNewValueInt(character, stat);
        }
        
        displayValue = newValue;
        
        if (num == 0 && stat == ";CyclesSinceSlugpup")
        {
            newValue = "0";
        }
        else
        {
            newValue = "100";
        }

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
        
        PrintMessage("\nEnter the new value ", "ask", false);
        PrintMessage("/!\\ make sure that it is a valid DEN Room, use the interactive map to get the name of the den you want /!\\ ", "error", false);
        PrintMessage("(enter C to cancel) : ", "ask", false);
        
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
        PrintMessage("\nEnter a valid room (example : SU_S01) !", "error", true);
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
            PrintMessage("\nValue Not Found !", "error", true);
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
        
        Console.WriteLine("\nCreating backup save...");
        origFilePath = folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_backup" + Path.GetExtension(filePath);
        
        if (File.Exists(origFilePath))
        {
            string choice;
                
            Console.Write("Backup save already exists in ");
            PrintMessage(folderPath, "info", false);
            Console.Write("\n\nWould you like to overwrite it ?\n\ny/N : ");
            choice = Console.ReadLine();
            choice = choice.ToUpper();
            char.TryParse(choice, out chosenValue);
            if (chosenValue == 'Y')
            {
                // Overwrite the backup save
                File.WriteAllText(origFilePath, File.ReadAllText(filePath));
                PrintMessage("\nBackup save overwritten !", "success", true);
            }
            else
            {
                PrintMessage("\nBackup save not created !", "success", true);
            }
        }
        else
        {
            // Create a backup save
            File.Create(origFilePath).Dispose();
            File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            PrintMessage("\nBackup save created !", "success", true);
        }
    }
    
    private static void PrintMessage(string message, string messageType, bool line)
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
        if (!line)
        {
            Console.Write(message);
        }
        else
        {
            Console.WriteLine(message);
        }

        Console.ResetColor();
        Thread.Sleep(360);
    }
}