using System.Text;
using System.Text.RegularExpressions;

namespace RwSavEditor
{
    class Program
    {
        private static char chosenValue;
        private static string pattern = @"([A-Z]{2}_[A-Z][0-9]{2})";
        public static void Main(string[] args)
        {
            AskNewValueStr();
            
            return;
            
            string pattern = ".*?([A-Z]{2}_[A-Z][0-9]{2}).*";
            
            string input = "DENPOS&lt;svB&gt;SU_S01&lt;svA&gt;"; // Ta chaîne d'entrée
            
            Regex regex = new Regex(pattern);
            
            Match match = regex.Match(input);

            if (match.Success)
            {
                Console.WriteLine("Motif trouvé : " + match.Groups[1].Value);
            }
            else
            {
                Console.WriteLine("Aucun motif trouvé.");
            }
            
            return;
            
            
            
            
            String characterChoice = AskChar();
            String statsToFind = AskStat();
            //String txt = File.ReadAllText("C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav");
            
            
            //int valueReturned = GetIntValue(txt, characterChoice, statsToFind);
            //Console.WriteLine("Initial Value : " + valueReturned);
            String newValue = AskNewValueInt();
            
            //EditIntValue(txt, characterChoice, statsToFind, newValue);
            Console.WriteLine("Value changed to : " + newValue + " with success !");
            Console.Read();
        }

        private static String AskNewValueInt()
        {
            String newValue;

            Console.Write("\nEnter the new value : ");
            newValue = Console.ReadLine();
            if (!int.TryParse(newValue, out int newValueInt))
            {
                Console.WriteLine("Enter a number !");
                AskNewValueInt();
            }
            return newValue;
        }

        private static String AskNewValueStr()
        {
            String newValue;

            Console.Write("\nEnter the new value : ");
            newValue = Console.ReadLine();
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
                          "\n4 = Karma Level" +
                          "\n5 = Karma CAP" +
                          "\n>");
            statsToFind = Console.ReadLine();
            char.TryParse(statsToFind, out chosenValue);
            
            if (chosenValue == '1')
            {
                statsToFind = "CYCLENUM";
            }
            else if (chosenValue == '2')
            {
                statsToFind = "FOOD";
            }
            else if (chosenValue == '3')
            {
                statsToFind = "TOTFOOD";
            }
            else
            {
                Console.WriteLine("Wrong input !\n");
                AskStat();
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

        private static int GetIntValue(String txt, String character, String valueToFind)
        {
            int returnValue;
            int start = txt.IndexOf(character);
            // Find the first "CYCLENUM" after the start position and get the last char position
            int end = txt.IndexOf(valueToFind, start) + valueToFind.Length;
            // Find the first number after "CYCLENUM"
            StringBuilder numCycle = new StringBuilder();
            for (int i=end; i<txt.Length; i++)
            {
                if (char.IsDigit(txt[i]))
                {
                    for (int j=i; j<txt.Length; j++)
                    {
                        if (char.IsDigit(txt[j]))
                        {
                            numCycle.Append(txt[j]);
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
            int.TryParse(numCycleStr, out returnValue);
            return returnValue;
        }
        
        private static void EditStrValue(String txt, String character, String valueToFind, String newValue)
        {
            int start = txt.IndexOf(character);
            int end = txt.IndexOf(valueToFind, start) + valueToFind.Length;
            Regex regex = new Regex(pattern);
            Match match = regex.Match(txt, end);
            string replaced = txt.Substring(0, match.Index) + newValue + txt.Substring(match.Index + match.Length);
            
            File.WriteAllText("C:\\Users\\Domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav", replaced);
            
            
            
        }

        private static void EditIntValue(String txt, String character, String valueToFind, String newValue)
        {
            int start = txt.IndexOf(character);
            // Find the first "CYCLENUM" after the start position and get the last char position
            int end = txt.IndexOf(valueToFind, start) + valueToFind.Length;
            // Find the first number after "CYCLENUM"
            StringBuilder numCycle = new StringBuilder();
            for (int i=end; i<txt.Length; i++)
            {
                if (char.IsDigit(txt[i]))
                {
                    for (int j=i; j<txt.Length; j++)
                    {
                        if (char.IsDigit(txt[j]))
                        {
                            numCycle.Append(txt[j]);
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
            int index = txt.IndexOf(numCycleStr, end);
            txt = txt.Remove(index, numCycleStr.Length);
            txt = txt.Insert(index, newValue);
            File.WriteAllText("C:\\Users\\Domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav", txt);
        }
    }
}

