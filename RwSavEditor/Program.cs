using System.Text;

namespace RwSavEditor
{
    class Program
    {
        static char chosenValue;
        public static void Main(string[] args)
        {
            String characterChoice = askChar();
            String statsToFind = askStat();
            String txt = File.ReadAllText("C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav");
            
            
            int valueReturned = GetIntValue(txt, characterChoice, statsToFind);
            Console.WriteLine("Initial Value : " + valueReturned);
            int newValue = askNewValue();
            
            EditIntValue(txt, characterChoice, statsToFind, newValue);
            Console.WriteLine("Value changed to : " + newValue + " with success !");
            Console.Read();
        }

        public static int askNewValue()
        {
            String newValue;
            int newValueInt;

            Console.Write("Enter the new value : ");
            newValue = Console.ReadLine();
            if (!int.TryParse(newValue, out newValueInt))
            {
                Console.WriteLine("Enter a number !\n");
                askNewValue();
            }
            return newValueInt;
        }
        
        public static String askStat()
        {
            String statsToFind;
           
            Console.Write("Enter the stat you want to edit:" +
                          "\n\n1 = Number Of Cycle passed" +
                          "\n2 = Number Of Food Eat" +
                          "\n3 = Food Cap" +
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
                askStat();
            }

            return statsToFind;
        }

        public static String askChar()
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
                askChar();
            }

            return characterChoiceSTR;
        }

        public static int GetIntValue(String txt, String character, String valueToFind)
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

        public static void EditIntValue(String txt, String character, String valueToFind, int newValue)
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
            txt = txt.Insert(index, newValue.ToString());
            File.WriteAllText("C:\\Users\\Domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav", txt);
        }
    }
}

