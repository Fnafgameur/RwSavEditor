using System.Text;
using System.Text.RegularExpressions;

namespace RwSavEditor
{
    
    class Program
    {
        private static string filePath;
        private static string folderPath;
        private static string fileContent;
        
        private static char chosenValue;
        private static string pattern = "([A-Z]{2}_[A-Z][0-9]{2})";
        private static bool hasPath;
        
        public static void Main()
        {
            string characterChoice;
            string statsToFind;
            string valueReturned;
            string newValue;
            string selectedChoice;
            
            if (!hasPath)
            {
                Console.Write("Provide path to your \"sav\" file (example : C:/Users/Example/Desktop/sav) : ");
                filePath = Console.ReadLine();
                

                if (filePath == "d" || filePath == "debug")
                {
                    filePath = "C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav_hunter";
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found !");
                    Main();
                }
                folderPath = Path.GetDirectoryName(filePath);
                CreateOrigSave();
                hasPath = true;
            }

            /*
             * TODO :
             * -Créer fichier save "original" pour pouvoir le restaurer en cas de problème 🟠
             * ---Faire vérif si orig file existe, demander si on l'écrase ou non
             * -Ajouter stats manquantes 🟢
             * -Ajouter Survivor & Hunter 🟢
             * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...)
             * -Revérifier le code et tester afin de trouver des bugs
             *
             * -Opti le code
             */
            
            characterChoice = AskChar();
            statsToFind = AskStat();

            if (!statsToFind.Contains("DEN"))
            {
                valueReturned = GetIntValue(characterChoice, statsToFind);
                Console.WriteLine("Initial Value : " + valueReturned);
                if (statsToFind == ";KARMA")
                {
                    Console.WriteLine("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"));
                }
                newValue = AskNewValueInt(characterChoice, statsToFind);

                if (statsToFind == ";KARMA")
                {
                    String karmaCap = GetIntValue(characterChoice, ";KARMACAP");
                    if (int.Parse(newValue) > 9)
                    {
                        Console.WriteLine("Karma level can't be higher than 9 !");
                        newValue = AskNewValueInt(characterChoice, statsToFind);
                    }

                    if (int.Parse(newValue) > int.Parse(karmaCap))
                    {
                        String choice;

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
            Console.WriteLine("Value changed to : " + newValue + " with success ! ");
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
            Main();
        }

        private static String AskNewValueInt(string character, string stat)
        {
            String newValue;

            Console.Write("\nEnter the new value (enter C to cancel) : ");
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
                Console.WriteLine("Enter a number !");
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
            return newValue;
        }

        private static String AskNewValueStr()
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
                AskNewValueStr();
            }
            return newValue;
        }
        
        private static String AskStat()
        {
            String statsToFind;
           
            Console.Write("Enter the stat you want to edit:" +
                          "\n\n0 = Number Of Cycle passed" +
                          "\n1 = Number Of Deaths" +
                          "\n2 = Number Of Cycle survived" +
                          "\n3 = Number Of Cycle abandonned" +
                          "\n4 = Number of food" +
                          "\n5 = Position of current den" +
                          "\n6 = Karma Level" +
                          "\n7 = Karma CAP" +
                          "\n8 = Reinforce Karma" +
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
                    statsToFind = "FOOD";
                    break;
                case "5":
                    statsToFind = ";DENPOS";
                    break;
                case "6":
                    statsToFind = ";KARMA";
                    break;
                case "7":
                    statsToFind = ";KARMACAP";
                    break;
                case "8":
                    statsToFind = ";REINFORCEDKARMA";
                    break;
                default:
                    Console.WriteLine("Wrong input !\n");
                    return AskStat();
            }

            return statsToFind;
        }
        
        private static String AskChar()
        {
            fileContent = File.ReadAllText(filePath);
            String characterChoiceStr;
            
            Console.Write("Enter the character's save you want to edit:\n" +
                          "\n1 = Monk" +
                          "\n2 = Survivor" +
                          "\n3 = Hunter" +
                          "\nR = Restart the program" +
                          "\n>");
            characterChoiceStr = Console.ReadLine();
            characterChoiceStr = characterChoiceStr.ToUpper();
            char.TryParse(characterChoiceStr, out chosenValue);
            switch (chosenValue)
            {
                case '1':
                    characterChoiceStr = "Yellow&lt;svA&gt;SEED&lt;svB&gt;";
                    break;
                case '2':
                    characterChoiceStr = "White&lt;svA&gt;SEED&lt;svB&gt;";
                    break;
                case '3':
                    characterChoiceStr = "Red&lt;svA&gt;SEED&lt;svB&gt;";
                    break;
                case 'R':
                    hasPath = false;
                    Console.Clear();
                    Main();
                    break;
                default:
                    Console.WriteLine("Wrong input !\n");
                    return AskChar();
            }
            
            if (fileContent.IndexOf(characterChoiceStr) == -1)
            {
                Console.WriteLine("Character not found !\n");
                AskChar();
            }

            return characterChoiceStr;
        }
        
        private static String GetStrValue(String character, String valueToFind)
        {
            fileContent = File.ReadAllText(filePath);
            int start = fileContent.IndexOf(character);
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(fileContent, end);
            
            return match.Groups[1].Value;
        }
        
        private static string GetIntValue(string character, string valueToFind)
        {
            string fileContent = File.ReadAllText(filePath);
            string returnValue = "";
            int start;
            int end;
            
            start = fileContent.LastIndexOf(character);
            if (start != -1)
            {
                end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
                StringBuilder numCycle = new StringBuilder();

                for (int i = end; i < fileContent.Length; i++)
                {
                    if (char.IsDigit(fileContent[i]))
                    {
                        for (int j = i; j < fileContent.Length; j++)
                        {
                            if (char.IsDigit(fileContent[j])) 
                            {
                                numCycle.Append(fileContent[j]);
                            }
                            else
                            {
                                break;
                            }
                        }
                        returnValue = numCycle.ToString();
                        break;
                    }
                }
            }

            Console.WriteLine("value : " + returnValue);

            return returnValue;
        }
        
        private static void EditStrValue(String character, String valueToFind, String newValue)
        {
            fileContent = File.ReadAllText(filePath);
            int start = fileContent.IndexOf(character);
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(fileContent, end);
            string replaced = fileContent.Substring(0, match.Index) + newValue + fileContent.Substring(match.Index + match.Length);
            
            File.WriteAllText(filePath, replaced);
        }

        private static string EditIntValue(string character, string valueToFind, string newValue)
        {
            string fileContent = File.ReadAllText(filePath);
    
            int start = fileContent.LastIndexOf(character);
            if (start != -1)
            {
                int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
                StringBuilder number = new StringBuilder();
        
                for (int i = end; i < fileContent.Length; i++)
                {
                    if (char.IsDigit(fileContent[i]))
                    {
                        for (int j = i; j < fileContent.Length; j++)
                        {
                            if (char.IsDigit(fileContent[j]))
                            {
                                number.Append(fileContent[j]);
                            }
                            else
                            {
                                break;
                            }
                        }
                        string numCycleStr = number.ToString();
                        int index = fileContent.IndexOf(numCycleStr, end);
                        string replaced = fileContent.Substring(0, index) + newValue + fileContent.Substring(index + numCycleStr.Length);
                
                        // Write the modified content back to the file
                        File.WriteAllText(filePath, replaced);
                        return numCycleStr;
                    }
                }
            }
            return valueToFind;
        }

        private static void CreateOrigSave()
        {
            Console.WriteLine("Creating original save...");
            // Create a new file with the same name as the original + "_orig"
            string origFilePath = folderPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + "_orig" + Path.GetExtension(filePath);
            // Check if the file already exists
            if (!File.Exists(origFilePath))
            {
                // Create the file
                File.Create(origFilePath).Dispose();
                // Copy the content of the original file to the new one
                File.WriteAllText(origFilePath, File.ReadAllText(filePath));
            }

            Console.WriteLine("Original save created !");
        }
    }
}