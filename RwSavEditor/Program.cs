using System.Text;
using System.Text.RegularExpressions;

namespace RwSavEditor;

class Program
{
    private static string filePath;
    private static string folderPath;
    private static string fileContent;
        
    private static char chosenValue;
    private static string pattern = "([A-Z]{2}_[A-Z][0-9]{2})";
    private static bool hasPath;
    private static string displayValue;
        
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
                Console.Write("Provide path to your \"sav\" file (example : C:/Users/Example/Desktop/sav) : ");
                filePath = Console.ReadLine();


                if (filePath == "d" || filePath == "debug")
                {
                    filePath = "C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav_all";
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("\nFile not found !\n");
                }
            } while (!File.Exists(filePath));
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
         * -Ajouter modded scugs ?
         * ---Faire en sorte que le nombre de cycle de hunter ne soit pas négatif lors de l'incrémentation de cycles 🟢
         * -Revérifier le code et tester afin de trouver des bugs
         * ---Retester changer valeur string 🟠
         * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...) 🟠
         *
         * -Opti le code
         */
            
        characterChoice = AskChar();
        statsToFind = AskStat();

        if (!statsToFind.Contains("DEN"))
        {
            GetIntValue(characterChoice, statsToFind);
            if (statsToFind != ";TOTTIME")
            {
                Console.WriteLine("\nCurrent Value : " + displayValue);
            }
            else
            {
                Console.WriteLine("Current Value (in seconds) : " + displayValue);
            }

            if (statsToFind == ";KARMA")
            {
                Console.WriteLine("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"));
            }
            newValue = AskNewValueInt(characterChoice, statsToFind);

            if (statsToFind == ";KARMA")
            {
                String karmaCap = GetIntValue(characterChoice, ";KARMACAP");
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
                        Console.WriteLine("Karma level not changed !");
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
        Console.WriteLine("\nValue changed to : " + displayValue + " with success ! ");
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
            
        Console.Write("Enter the character's save you want to edit:\n" +
                      "\n0 = Monk" +
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
                Console.WriteLine("\nWrong input !\n");
                return AskChar();
        }
            
        if (fileContent.IndexOf(characterChoiceStr) == -1)
        {
            Console.WriteLine("\nCharacter not found ! Have you saved in his campaign ?\n");
            return AskChar();
        }

        return characterChoiceStr;
    }
    
    private static string AskStat()
    {
        String statsToFind;
           
        Console.Write("\nEnter the stat you want to edit:" +
                      "\n\n0 = Number Of Cycle passed" +
                      "\n1 = Number Of Deaths" +
                      "\n2 = Number Of Cycle survived" +
                      "\n3 = Number Of Cycle abandonned" +
                      "\n4 = Time passed" +
                      "\n5 = Number of food" +
                      "\n6 = Position of current den" +
                      "\n7 = Karma Level" +
                      "\n8 = Karma CAP" +
                      "\n9 = Reinforce Karma" +
                      "\n>");
        statsToFind = Console.ReadLine();
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
            default:
                Console.WriteLine("\nWrong input !");
                return AskStat();
        }

        return statsToFind;
    }
    
    private static string GetIntValue(string character, string valueToFind)
    {
        string fileContent = File.ReadAllText(filePath);
        string returnValue = "";
        int displayValueInt;
        int start;
        int end;
            
        start = fileContent.LastIndexOf(character);
        if (start == -1)
        {
            return "Value Not Found !";
        }

        end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;

        returnValue = FindInt(end);
            
        // For display value, remove the "-" if the character is Red
        displayValue = returnValue;
        if (character.Contains("Red") && valueToFind == ";CYCLENUM")
        {
            displayValue = returnValue.Substring(1);
            int.TryParse(displayValue, out displayValueInt);
            displayValue = (displayValueInt + 19).ToString();
        }

        return returnValue;
    }
    
    private static string GetStrValue(string character, string valueToFind)
    {
        fileContent = File.ReadAllText(filePath);
        int start = fileContent.LastIndexOf(character);
        int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
        Regex regex = new Regex(pattern);
        Match match = regex.Match(fileContent, end);
            
        return match.Groups[1].Value;
    }

    private static string AskNewValueInt(string character, string stat)
    {
        String newValue;

        if (stat != ";TOTTIME")
        {
            Console.Write("\nEnter the new value (enter C to cancel) : ");
        }
        else
        {
            Console.Write("\nEnter the new value (in seconds) (enter C to cancel) : ");
        }

        newValue = Console.ReadLine();
        newValue = newValue.ToUpper();
        char.TryParse(newValue, out chosenValue);
        if (chosenValue == 'C')
        {
            Console.Clear();
            Main();
        }
        //Check if the input is a number
        if (!int.TryParse(newValue, out int num))
        {
            Console.WriteLine("Enter an integer number !");
            return AskNewValueInt(character, stat);
        }
            
        if (num < 0 && stat != ";CYCLENUM")
        {
            Console.WriteLine("Number must be positive !");
            return AskNewValueInt(character, stat);
        }

        if (stat == ";REINFORCEDKARMA" && num != 0 && num != 1)
        {
            Console.WriteLine("Number must be either 0 or 1 !");
            return AskNewValueInt(character, stat);
        }
            
        if (character.Contains("Red") && stat == ";CYCLENUM")
        {
            // negate new value
            newValue = "-" + (num - 19);
        }
            
        return newValue;
    }

    private static string AskNewValueStr()
    {
        String newValue;
            
        Console.Write("\nEnter the new value /!\\make sure that it is a valid RW Room /!\\ (enter C to cancel) : ");
        newValue = Console.ReadLine();
        newValue = newValue.ToUpper();
        char.TryParse(newValue, out chosenValue);
        if (chosenValue == 'C')
        {
            Console.Clear();
            Main();
        }
        Regex regex = new Regex(pattern);
        Match match = regex.Match(newValue);
        if (!match.Success)
        {
            Console.WriteLine("Enter a valid room (example : SU_S01) !");
            return AskNewValueStr();
        }
        return newValue;
    }
    
    private static string EditIntValue(string character, string valueToFind, string newValue)
    {
        string fileContent = File.ReadAllText(filePath);
        int newValueInt = int.Parse(newValue);
        string numberStr;
        string replaced;
        int start;
        int end;
        int index;
            
        start = fileContent.LastIndexOf(character);
        if (start == -1)
        {
            return "Value Not Found !";
        }

        end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            
        numberStr = FindInt(end);
            
        index = fileContent.IndexOf(numberStr, end);
        replaced = fileContent.Substring(0, index) + newValue + fileContent.Substring(index + numberStr.Length);
            
        // Write the modified content back to the file
        File.WriteAllText(filePath, replaced);
        if (character.Contains("Red"))
        {
            displayValue = newValue.Substring(1);
            int.TryParse(displayValue, out newValueInt);
            displayValue = (newValueInt + 19).ToString();
        }
        else
        {
            displayValue = newValue;
        }
        return numberStr;
    }
    
    private static void EditStrValue(String character, String valueToFind, String newValue)
    {
        fileContent = File.ReadAllText(filePath);
        int start = fileContent.LastIndexOf(character);
        int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
        Regex regex = new Regex(pattern);
        Match match = regex.Match(fileContent, end);
        string replaced = fileContent.Substring(0, match.Index) + newValue + fileContent.Substring(match.Index + match.Length);
        displayValue = newValue;
            
        File.WriteAllText(filePath, replaced);
    }

    private static string FindInt(int startIndex)
    {
        string returnValue = "";
            
        StringBuilder number = new StringBuilder();
        for (int i = startIndex; i < fileContent.Length; i++)
        {
            if (char.IsDigit(fileContent[i]) || fileContent[i] == '-')
            {
                for (int j = i; j < fileContent.Length; j++)
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
        }
        return returnValue;
    }
    
    private static void CreateBackupSave()
    {
        string origFilePath;
            
        Console.WriteLine("\nCreating backup save...");
        // Create a new file with the same name as the original + "_orig"
        origFilePath = folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_backup" + Path.GetExtension(filePath);
        // Check if the file already exists
        if (File.Exists(origFilePath))
        {
            string choice;
                
            Console.WriteLine("Backup save already exists in " + folderPath + " !");
            Console.Write("Would you like to overwrite it ?\n\ny/N : ");
            choice = Console.ReadLine();
            choice = choice.ToUpper();
            char.TryParse(choice, out chosenValue);
            if (chosenValue == 'Y')
            {
                // Copy the content of the original file to the new one
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
            // Create the file
            File.Create(origFilePath).Dispose();
            // Copy the content of the original file to the new one
            File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            Console.WriteLine("Backup save created !\n");
        }
    }
}