using System.Text;

namespace RwSavEditor
{
    class Program
    {
        public static void Main(string[] args)
        {
            String txt = File.ReadAllText("C:\\Users\\Djimmy\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav");
            EditIntValue(txt, "Yellow&lt;svA&gt;SEED&lt;svB&gt;", "CYCLENUM", 50000);
            
            return;
            String characterChoiceSTR;
            char chosenValue;

            String statsToFind;

            Console.Write("Enter the character you want to edit:\n\n1 = Monk" +
                          "\n>");
            characterChoiceSTR = Console.ReadLine();
            char.TryParse(characterChoiceSTR, out chosenValue);
            
            if (chosenValue == '1')
            {
                characterChoiceSTR = "Yellow&lt;svA&gt;SEED&lt;svB&gt;";
            }
            
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
            
            //String txt = File.ReadAllText("C:\\Users\\Djimmy\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav");
            int valueReturned = GetIntValue(txt, characterChoiceSTR, statsToFind);
            Console.WriteLine("Value : " + valueReturned);
            
            
            
            
            
            Console.Read();
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
            File.WriteAllText("C:\\Users\\Djimmy\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav", txt);
            
            Console.WriteLine(txt);
        }
    }
}

