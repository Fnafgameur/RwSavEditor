using System.Text;
using System.Text.RegularExpressions;

namespace RwSavEditor
{
    class Program
    {
        private static string filePath;
        private static string fileContent;
        
        private static char chosenValue;
        private static string pattern = "([A-Z]{2}_[A-Z][0-9]{2})";
        
        public static void Main()
        {
            #region GetPath
            Console.Write("Provide path to your \"sav\" file (example : C:\\Users\\Example\\Desktop\\sav) : ");
            filePath = Console.ReadLine();
            
            if (filePath == "d" || filePath == "debug")
            {
                filePath = "C:\\Users\\Djimmy\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav";
            }
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found !");
                Main();
            }
            
            fileContent = File.ReadAllText(filePath);
            #endregion
            
            string characterChoice = AskChar();
            String statsToFind = AskStat();
            String valueReturned;
            String newValue;

            if (!statsToFind.Contains("DEN"))
            {
                valueReturned = GetIntValue(characterChoice, statsToFind);
                Console.WriteLine("Initial Value : " + valueReturned);
                newValue = AskNewValueInt(statsToFind);
                EditIntValue(characterChoice, statsToFind, newValue);
            }
            else
            {
                valueReturned = GetStrValue(characterChoice, statsToFind);
                Console.WriteLine("Initial Value : " + valueReturned);
                newValue = AskNewValueStr();
                EditStrValue(characterChoice, statsToFind, newValue);
            }
            Console.WriteLine("Value changed to : " + newValue + " with success !");
            Console.Read();
        }

        private static String AskNewValueInt(string stat)
        {
            String newValue;

            Console.Write("\nEnter the new value : ");
            newValue = Console.ReadLine();
            //Check if the input is a number
            if (!int.TryParse(newValue, out int num))
            {
                Console.WriteLine("Enter a number !");
                AskNewValueInt(stat);
            }
            
            if (num < 0)
            {
                Console.WriteLine("Number must be positive !");
                AskNewValueInt(stat);
            }

            if (stat == ";REINFORCEDKARMA")
            {
                if (num != 0 && num != 1)
                {
                    Console.WriteLine("Number must be either 0 or 1 !");
                    AskNewValueInt(stat);
                }
            }
            return newValue;
        }

        private static String AskNewValueStr()
        {
            String newValue;
            
            Console.Write("\nEnter the new value (/!\\make sure that it is a valid RW Room /!\\) : ");
            newValue = Console.ReadLine();
            newValue = newValue.ToUpper();
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
                          "\n\n1 = Number Of Cycle passed" +
                          "\n2 = Number Of Food Eat" +
                          "\n3 = Food Cap" +
                          "\n4 = Position of current den" +
                          "\n5 = Karma Level" +
                          "\n6 = Karma CAP" +
                          "\n7 = Reinforce Karma" +
                          "\n>");
            statsToFind = Console.ReadLine();
            char.TryParse(statsToFind, out chosenValue);

            switch (statsToFind) 
            {
                case "1":
                    statsToFind = ";CYCLENUM";
                    break;
                case "2":
                    statsToFind = "FOOD";
                    break;
                case "3":
                    statsToFind = ";TOTFOOD";
                    break;
                case "4":
                    statsToFind = ";DENPOS";
                    break;
                case "5":
                    statsToFind = ";KARMA";
                    break;
                case "6":
                    statsToFind = ";KARMACAP";
                    break;
                case "7":
                    statsToFind = ";REINFORCEDKARMA";
                    break;
                default:
                    Console.WriteLine("Wrong input !\n");
                    AskStat();
                    break;
            }

            return statsToFind;
        }
        
        private static String AskChar()
        {
            String characterChoiceSTR;
            
            Console.Write("Enter the character's save you want to edit:\n\n1 = Monk" +
                          "\n>");
            characterChoiceSTR = Console.ReadLine();
            char.TryParse(characterChoiceSTR, out chosenValue);
            
            if (chosenValue == '1')
            {
                characterChoiceSTR = "Yellow&lt;svA&gt;SEED&lt;svB&gt;";
            }
            else
            {
                Console.WriteLine("Wrong input !\n");
                AskChar();
            }

            return characterChoiceSTR;
        }
        
        private static String GetStrValue(String character, String valueToFind)
        {
            int start = fileContent.IndexOf(character);
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(fileContent, end);
            
            return match.Groups[1].Value;
        }

        private static String GetIntValue(String character, String valueToFind)
        {
            String returnValue;
            int start = fileContent.IndexOf(character);
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            // Find the first number after "CYCLENUM"
            StringBuilder numCycle = new StringBuilder();
            for (int i=end; i<fileContent.Length; i++)
            {
                if (char.IsDigit(fileContent[i]))
                {
                    for (int j=i; j<fileContent.Length; j++)
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
                    break;
                }
            }
            returnValue = numCycle.ToString();
            return returnValue;
        }
        
        private static void EditStrValue(String character, String valueToFind, String newValue)
        {
            int start = fileContent.IndexOf(character);
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(fileContent, end);
            string replaced = fileContent.Substring(0, match.Index) + newValue + fileContent.Substring(match.Index + match.Length);
            
            File.WriteAllText(filePath, replaced);
        }

        private static void EditIntValue(String character, String valueToFind, String newValue)
        {
            int start = fileContent.IndexOf(character);
            // Find the first "CYCLENUM" after the start position and get the last char position
            int end = fileContent.IndexOf(valueToFind, start) + valueToFind.Length;
            // Find the first number after "CYCLENUM"
            StringBuilder numCycle = new StringBuilder();
            for (int i=end; i<fileContent.Length; i++)
            {
                if (char.IsDigit(fileContent[i]))
                {
                    for (int j=i; j<fileContent.Length; j++)
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
                    break;
                }
            }
            String numCycleStr = numCycle.ToString();
            int index = fileContent.IndexOf(numCycleStr, end);
            string replaced = fileContent.Substring(0, index) + newValue + fileContent.Substring(index + numCycleStr.Length);
            // Get file by path
            File.WriteAllText(filePath, replaced);
        }
    }
}

