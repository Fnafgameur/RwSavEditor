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
        "Photomaniac&lt;svA&gt;SEED&lt;svB&gt;",
        "Pearlcat&lt;svA&gt;SEED&lt;svB&gt;",
        "WingCat&lt;svA&gt;SEED&lt;svB&gt;",
    };
    
    public static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        
        string characterChoice;
        string statsToFind;
        string valueReturned;
        string newValue;
        string selectedChoice;
        
        charsFoundDictionary.Clear();
        
        if (!hasPath)
        {
            Console.WriteLine("  _____            _____             ______    _ _ _             ");
            Console.WriteLine(" |  __ \\          / ____|           |  ____|  | (_) |            ");
            Console.WriteLine(" | |__) |_      _| (___   __ ___   _| |__   __| |_| |_ ___  _ __ ");
            Console.WriteLine(" |  _  /\\ \\ /\\ / /\\___ \\ / _` \\ \\ / /  __| / _` | | __/ _ \\| '__|");
            Console.WriteLine(" | | \\ \\ \\ V  V / ____) | (_| |\\ V /| |___| (_| | | || (_) | |   ");
            Console.WriteLine(" |_|  \\_\\ \\_/\\_/ |_____/ \\__,_| \\_/ |______\\__,_|_|\\__\\___/|_|   ");
            Console.WriteLine("                                                                 ");
            Console.Write("                                                                 ");
            PrintMessage("\n/!\\ Modded scugs are not fully supported yet /!\\", "error", true);
            PrintMessage("/!\\ This project is still in development, even if the program create a backup itself, create one manually /!\\", "error", true);
            PrintMessage("/!\\ If you encounter a bug, please report it on the discord server : https://discord.gg/ejNwfEqsTn /!\\\n", "error", true);
            
            do
            {
                PrintMessage("Provide path to your \"sav\" file e.g : \"C:/Users/Example/Desktop/sav\" OR \"./sav\" to select in the current directory of the editor OR \"find\" to search in the default save location", "ask", true);
                Console.Write(">");
                filePath = Console.ReadLine();

                if (string.IsNullOrEmpty(filePath))
                {
                    PrintMessage("\nNo path provided !", "error", true);
                    continue;
                }
                
                if (filePath == "find")
                {
                    //Get locallow folder 
                    var folderFromUserProfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow");
                    filePath = folderFromUserProfile + "\\Videocult\\Rain World\\sav";
                }
                
                if (filePath[0] == '.')
                {
                    filePath = Directory.GetCurrentDirectory() + filePath.Substring(1);
                }
                
                filePath = filePath.Replace("\"", "");
                
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

                fileContent = File.ReadAllText(filePath);
                
                if (string.IsNullOrEmpty(fileContent))
                {
                    PrintMessage("\nFile is empty !", "error", true);
                    continue;
                }
                
                fileContent = fileContent.Substring(0, 32);
                
                
                if (!fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType"))
                {
                    PrintMessage("\nIncorrect file !", "error", true);
                }
                
            } while (!File.Exists(filePath) || !fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType") || Path.GetExtension(filePath) != ".sav" && Path.GetExtension(filePath) != "");
            
            Console.Write("\nOpened file : ");
            filePath = filePath.Replace("/", "\\");
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
         * ---Check if scug is in save 🟢
         * -Ajouter modded scugs ? 🟢
         * ---Ajout scugs name manuellement ?
         * ---Faire en sorte que le nombre de cycle de hunter ne soit pas négatif lors de l'incrémentation de cycles 🟢
         * -Revérifier le code et tester afin de trouver des bugs 🟠
         * ---Retester changer valeur string 🟢
         * ---Refaire système Cycle Hunter 🟢
         * * ---Check pourquoi on ne peut pas forcer le spawn de pup si on en a déjà un 🟠
         * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...) 🔵
         * ---Ajouter des couleurs 🟢
         * ---Refaire message intro 🟢
         * ---Faire message de fin 🟢
         *
         * -Opti le code
         * -Ajouter des commentaires
         *
         * 
         * 🟢 = Fini
         * 🔵 = Fini mais sujet à modification
         * 🟠 = En cours
         */
        
        FindChars();
        
        characterChoice = AskChar();
        statsToFind = AskStat();

        if (!statsToFind.Contains("DEN"))
        {
            GetIntValue(characterChoice, statsToFind, false);
            
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
                PrintMessage("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP", true), "warning", true);
            }
            newValue = AskNewValueInt(characterChoice, statsToFind);

            if (statsToFind == ";KARMA")
            {
                var karmaCap = GetIntValue(characterChoice, ";KARMACAP", true);

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
                        EditIntValue(characterChoice, ";KARMACAP", newValue);
                    }
                    else
                    {
                        PrintMessage("\nKarma level not changed !", "error", true);
                        Main();
                    }
                }
            }
            var returnedMsg = EditIntValue(characterChoice, statsToFind, newValue);
            
            if (returnedMsg == "")
            {
                Main();
            }
        }
        else
        {
            valueReturned = GetStrValue(characterChoice, statsToFind);
            PrintMessage("\nCurrent Value : " + valueReturned, "warning", true);
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
            EndProgram();
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
        string charRenamed;
        
        PrintMessage("\nEnter the character's campaign you want to edit:\n", "ask", true);

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
                case "Spear":
                    charRenamed = "Spearmaster";
                    break;
                default:
                    charRenamed = charsFoundDictionary[chars.Key];
                    break;
            }
            
            Console.WriteLine(chars.Key + " = " + charRenamed);
        }
        Console.WriteLine("\nR = Restart the program");
        Console.Write("Q = Exit the program\n");
        Console.Write("\n>");
        
        characterChoiceStr = Console.ReadLine();
        characterChoiceStr = characterChoiceStr.ToUpper().Trim();
        char.TryParse(characterChoiceStr, out chosenValue);
        
        if (chosenValue == 'R')
        {
            Console.Clear();
            hasPath = false;
            Main();
        }

        if (chosenValue == 'Q')
        {
            EndProgram();
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

        characterChoiceStr = charsFoundDictionary[num];

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
        
        Console.Write("\n\n0 = Number of cycle passed" +
                      "\n1 = Number of deaths" +
                      "\n2 = Number of cycle survived" +
                      "\n3 = Number of cycle abandonned" +
                      "\n4 = Time passed" +
                      "\n5 = Total number of foods eaten" +
                      "\n6 = Number of food" +
                      "\n7 = Position of current den" +
                      "\n8 = Karma level" +
                      "\n9 = Karma CAP" +
                      "\n10 = Reinforce karma" +
                      "\n11 = Force pup to spawn this cycle (doesn't work if you already have a spup)" +
                      "\n\nC = Cancel\n" +
                      "\n>");
        statsToFind = Console.ReadLine().ToUpper().Trim();
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
                statsToFind = ";TOTFOOD";
                break;
            case "6":
                statsToFind = ";FOOD";
                break;
            case "7":
                statsToFind = ";DENPOS";
                break;
            case "8":
                statsToFind = ";KARMA";
                break;
            case "9":
                statsToFind = ";KARMACAP";
                break;
            case "10":
                statsToFind = ";REINFORCEDKARMA";
                break;
            case "11":
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
    
    private static string GetIntValue(string character, string valueToFind, bool isAuto)
    {
        var fileContent = File.ReadAllText(filePath);
        string returnValue;
        int start;
        int end;
        character += "&lt;svA&gt;SEED&lt;svB&gt;";
        
        start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        
        if (start == -1)
        {
            PrintMessage("\nValue Not Found !", "error", true);
            return "";
        }

        end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal) + valueToFind.Length;
        
        returnValue = FindInt(end);

        if (!isAuto)
        {
            displayValue = returnValue;
        }
        
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
        int num;
        var isNum = false;
        
        const int maxKarma = 9;
        
        switch (stat)
        {
            case ";TOTTIME":
                PrintMessage("\nEnter the new value (in seconds) (enter C to cancel) : ", "ask", false);
                isNum = true;
                break;
            case ";REINFORCEDKARMA":
            case ";CyclesSinceSlugpup":
                PrintMessage("\nEnter the new value (Yes or No) (enter C to cancel) : ", "ask", false);
                break;
            default:
                PrintMessage("\nEnter the new value (enter C to cancel) : ", "ask", false);
                isNum = true;
                break;
        }
        
        newValue = Console.ReadLine().ToUpper().Trim();
        char.TryParse(newValue, out chosenValue);
        if (chosenValue == 'C')
        {
            Console.Clear();
            Main();
        }

        if (isNum)
        {
            if (!int.TryParse(newValue, out num))
            {
                PrintMessage("\nEnter an integer number !", "error", true);
                return AskNewValueInt(character, stat);
            }
            
            if (num < 0 && stat != ";CYCLENUM")
            {
                PrintMessage("\nNumber must be positive !", "error", true);
                return AskNewValueInt(character, stat);
            }
            
            if (num > maxKarma && (stat == ";KARMA" || stat == ";KARMACAP"))
            {
                PrintMessage("\nKarma level can't be higher than 9 !", "error", true);
                return AskNewValueInt(character, stat);
            }
        }

        newValue = newValue.ToUpper();

        if ((stat == ";REINFORCEDKARMA" || stat == ";CyclesSinceSlugpup") && newValue != "YES" && newValue != "NO" && newValue != "Y" && newValue != "N")
        {
            PrintMessage("\nValue must be either Yes or No !", "error", true);
            return AskNewValueInt(character, stat);
        }

        switch (newValue)
        {
            case "YES":
            case "Y":
                displayValue = "Yes";
                break;
            case "NO":
            case "N":
                displayValue = "No";
                break;
            default:
                displayValue = newValue;
                break;
        }
        
        
        if ((newValue == "N" || newValue == "NO") && (stat == ";CyclesSinceSlugpup" || stat == ";REINFORCEDKARMA"))
        {
            newValue = "0";
        }
        
        else if ((newValue == "Y" || newValue == "YES") && (stat == ";CyclesSinceSlugpup" || stat == ";REINFORCEDKARMA"))
        {
            if (stat == ";REINFORCEDKARMA")
            {
                newValue = "1";
            }
            else
            {
                newValue = "100";
            }
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
        
        PrintMessage("\n/!\\ make sure that you enter a valid DEN Room, use an interactive map to get the name of the den you want /!\\ ", "error", true);
        PrintMessage("Enter the new value ", "ask", false);
        PrintMessage("(enter C to cancel) : ", "ask", false);
        
        newValue = Console.ReadLine().ToUpper().Trim();
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
        int stopIndex;
        int index;
        character += "&lt;svA&gt;SEED&lt;svB&gt;";
        
        start = fileContent.LastIndexOf(character, StringComparison.Ordinal);
        
        if (start == -1)
        {
            PrintMessage("\nValue Not Found !", "error", true);
            return "";
        }
        
        stopIndex = fileContent.IndexOf(";progDivA&gt;", start, StringComparison.Ordinal);
        end = fileContent.IndexOf(valueToFind, start, StringComparison.Ordinal);
        
        if (end == -1 || end > stopIndex)
        {
            PrintMessage("\nValue Not Found !", "error", true);
            return "";
        }
        
        end += valueToFind.Length;

        numberStr = FindInt(end);
            
        index = fileContent.IndexOf(numberStr, end, StringComparison.Ordinal);
        replaced = fileContent.Substring(0, index) + newValue + fileContent.Substring(index + numberStr.Length);
        
        File.WriteAllText(filePath, replaced);
        return numberStr;
    }
    
    private static void EditStrValue(string character, string valueToFind, string newValue)
    {
        character += "&lt;svA&gt;SEED&lt;svB&gt;";
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
                PrintMessage("\nBackup save not overwritten !", "error", true);
            }
        }
        else
        {
            // Create a backup save
            File.Create(origFilePath).Dispose();
            File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            PrintMessage("\nBackup save created in " + folderPath, "success", true);
            Thread.Sleep(360);
        }
    }
    
    private static void PrintMessage(string message, string messageType, bool line)
    { 
        switch (messageType)
        {
            case "error":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case "success":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case "warning":
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            case "info":
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case "ask":
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }
        
        if (!line)
        {
            Console.Write(message);
        }
        else
        {
            Console.WriteLine(message);
        }

        Console.ResetColor();
        if (messageType != "error" && messageType != "success")
        {
            return;
        }
        Thread.Sleep(360);
    }
    
    private static void EndProgram()
    {
        PrintMessage("\nThank you for using RwSavEditor !\nPress Enter to exit...", "info", false);
        Console.ReadLine();
        Environment.Exit(0);
    }
    static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Exception exception = (Exception)e.ExceptionObject;
        
        string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog.txt");
        
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"Date/Hour: {DateTime.Now}");
            writer.WriteLine($"Error: {exception.Message}");
            writer.WriteLine($"StackTrace: {exception.StackTrace}");
            writer.WriteLine(new string('-', 40));
        }
        
        PrintMessage("\nAn error occured !\nLogs have been saved in " + logFilePath + "\nPress Enter to exit...", "error", false);
        Console.ReadLine();
    }
}