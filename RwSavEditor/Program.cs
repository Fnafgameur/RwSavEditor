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
        
    public static void Main()
    {
        string characterChoice;
        string statsToFind;
        string valueReturned;
        string newValue;
        string selectedChoice;
            
        const int maxKarma = 9;
            
        if (!hasPath)
        {
            Console.WriteLine("Welcome to the Rain World Save Editor !\n" +
                              "This program allows you to edit your save file in order to change stats of your scugs\n" +
                              "/!\\ Modded scugs are not supported yet /!\\\n");
                
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Provide path to your \"sav\" file (e.g : \"C:/Users/Example/Desktop/sav\" OR \"./sav\" to select in the current directory) : ");
                Console.ResetColor();
                filePath = Console.ReadLine();

                if (string.IsNullOrEmpty(filePath))
                {
                    PrintError("\nNo path provided !\n");
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
                    PrintError("\nFile not found !\n");
                    continue;
                }
                
                fileContent = File.ReadAllText(filePath).Substring(0, 32);
                
                if (!fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType"))
                {
                    PrintError("\nIncorrect file !\n");
                }
            } while (!File.Exists(filePath) || !fileContent.StartsWith("<ArrayOfKeyValueOfanyTypeanyType"));
            
            Console.Write("\nOpened file : ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(filePath + "\n");
            Console.ResetColor();
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
         * ---Check if scug is in save
         * -Ajouter modded scugs ?
         * ---Ajout scugs name manuellement ?
         * ---Faire en sorte que le nombre de cycle de hunter ne soit pas négatif lors de l'incrémentation de cycles 🟢
         * -Revérifier le code et tester afin de trouver des bugs
         * ---Retester changer valeur string 🟢
         * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...) 🟠
         *
         * -Opti le code
         */
            
        characterChoice = AskChar();
        statsToFind = AskStat();

        if (!statsToFind.Contains("DEN"))
        {
            GetIntValue(characterChoice, statsToFind);
            
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (statsToFind != ";TOTTIME")
            {
                Console.WriteLine("\nCurrent Value : " + displayValue);
            }
            else
            {
                Console.WriteLine("Current Value (in seconds) : " + displayValue);
            }
            Console.ResetColor();

            if (statsToFind == ";KARMA")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"));
                Console.ResetColor();
            }
            newValue = AskNewValueInt(characterChoice, statsToFind);

            if (statsToFind == ";KARMA")
            {
                string karmaCap = GetIntValue(characterChoice, ";KARMACAP");
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
                    if (chosenValue == 'Y' || chosenValue == '\0' || chosenValue == ' ')
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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nValue changed to : " + displayValue + " with success ! ");
        Console.ResetColor();
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
    
    private static string AskChar()
    {
        fileContent = File.ReadAllText(filePath);
        string characterChoiceStr;

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("Enter the character's save you want to edit:\n");
        Console.ResetColor();
        
        Console.Write("\n0 = Monk" +
                          "\n1 = Survivor" +
                          "\n2 = Hunter" +
                          "\n3 = Gourmand" +
                          "\n4 = Artificer" +
                          "\n5 = Rivulet" +
                          "\n6 = Spearmaster" +
                          "\n7 = Saint" +
                          "\nR = Restart the program" +
                          "\n>");
    
        characterChoiceStr = Console.ReadLine();
        characterChoiceStr = characterChoiceStr.ToUpper();
        char.TryParse(characterChoiceStr, out chosenValue);
            
        switch (chosenValue)
        {
            case '0':
                characterChoiceStr = "Yellow&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '1':
                characterChoiceStr = "White&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '2':
                characterChoiceStr = "Red&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '3':
                characterChoiceStr = "Gourmand&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '4':
                characterChoiceStr = "Artificer&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '5':
                characterChoiceStr = "Rivulet&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '6':
                characterChoiceStr = "Spear&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case '7':
                characterChoiceStr = "Saint&lt;svA&gt;SEED&lt;svB&gt;";
                break;
            case 'R':
                hasPath = false;
                Console.Clear();
                Main();
                break;
            default:
                PrintError("\nWrong input !\n");
                return AskChar();
        }

        if (fileContent.IndexOf(characterChoiceStr, StringComparison.Ordinal) != -1)
        {
            return characterChoiceStr;
        }
        PrintError("\nCharacter not found ! Have you saved in his campaign ?\n");
        return AskChar();

    }
    
    private static string AskStat()
    {
        string statsToFind;
        
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("\nEnter the stat you want to edit:");
        Console.ResetColor();
           
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
                PrintError("\nWrong input !");
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
            PrintError("Value Not Found !");
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

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        switch (stat)
        {
            case ";TOTTIME":
                Console.Write("\nEnter the new value (in seconds) (enter C to cancel) : ");
                break;
            case ";REINFORCEDKARMA":
                Console.Write("\nEnter the new value (0 or 1) (enter C to cancel) : ");
                break;
            default:
                Console.Write("\nEnter the new value (enter C to cancel) : ");
                break;
        }
        Console.ResetColor();
        
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
            PrintError("Enter an integer number !");
            return AskNewValueInt(character, stat);
        }
            
        if (num < 0 && stat != ";CYCLENUM")
        {
            PrintError("Number must be positive !");
            return AskNewValueInt(character, stat);
        }

        if (stat == ";REINFORCEDKARMA" && num != 0 && num != 1)
        {
            PrintError("Number must be either 0 or 1 !");
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
        
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("\nEnter the new value ");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("\n/!\\ make sure that it is a valid DEN Room, use the interactive map to get the name of the den you want /!\\ ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("(enter C to cancel) : ");
        Console.ResetColor();
        
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
        PrintError("\nEnter a valid room (example : SU_S01) !");
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
            PrintError("Value Not Found !");
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
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(folderPath);
            Console.ResetColor();
            Console.Write("Would you like to overwrite it ?\n\ny/N : ");
            choice = Console.ReadLine();
            choice = choice.ToUpper();
            char.TryParse(choice, out chosenValue);
            Console.ForegroundColor = ConsoleColor.Green;
            if (chosenValue == 'Y')
            {
                // Overwrite the backup save
                File.WriteAllText(origFilePath, File.ReadAllText(filePath));
                Console.WriteLine("\nBackup save overwritten !\n");
            }
            else
            {
                Console.WriteLine("\nBackup save not created !\n");
            }
        }
        else
        {
            // Create a backup save
            File.Create(origFilePath).Dispose();
            File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            Console.WriteLine("Backup save created !\n");
        }
        Console.ResetColor();
    }

    private static void PrintError(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
        Console.ResetColor();
        Thread.Sleep(360);
    }
}