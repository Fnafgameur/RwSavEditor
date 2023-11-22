﻿using System.Text;
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
            Console.Write("Provide path to your \"sav\" file (example : C:/Users/Example/Desktop/sav) : ");
            filePath = Console.ReadLine();
            
            if (filePath == "d" || filePath == "debug")
            {
                filePath = "C:\\Users\\domicile\\RiderProjects\\RwSavEditor\\RwSavEditor\\sav";
            }
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found !");
                Main();
            }
            
            fileContent = File.ReadAllText(filePath);
            #endregion
            
            /*
             * TODO :
             * -Créer fichier save "original" pour pouvoir le restaurer en cas de problème
             * -Ajouter stats manquantes ~ (tester surv, deaths, abandon)
             * -Ajouter Survivor & Hunter 1.5/2 (tester pour hunter)
             * -Application sur l'esthétique (formulations des phrases, retour à la ligne, etc...)
             * -Revérifier le code et tester afin de trouver des bugs
             *
             * -Opti le code
             */
            
            string characterChoice = AskChar();
            String statsToFind = AskStat();
            String valueReturned;
            String newValue;

            if (!statsToFind.Contains("DEN"))
            {
                valueReturned = GetIntValue(characterChoice, statsToFind);
                Console.WriteLine("Initial Value : " + valueReturned);
                if (statsToFind == ";KARMA")
                {
                    Console.WriteLine("Karma CAP : " + GetIntValue(characterChoice, ";KARMACAP"));
                }
                newValue = AskNewValueInt(statsToFind);
                
                if (statsToFind == ";KARMA")
                {
                    String karmaCap = GetIntValue(characterChoice, ";KARMACAP");
                    if (int.Parse(newValue) > int.Parse(karmaCap))
                    {
                        Console.WriteLine("Karma level can't be higher than Karma CAP !");
                        AskNewValueInt(statsToFind);
                    }
                    if (int.Parse(newValue) > 9)
                    {
                        Console.WriteLine("Karma level can't be higher than 9 !");
                        AskNewValueInt(statsToFind);
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
                    AskStat();
                    break;
            }

            return statsToFind;
        }
        
        private static String AskChar()
        {
            String characterChoiceSTR;
            
            Console.Write("Enter the character's save you want to edit:\n" +
                          "\n1 = Monk" +
                          "\n2 = Survivor" +
                          "\n3 = Hunter" +
                          "\n>");
            characterChoiceSTR = Console.ReadLine();
            char.TryParse(characterChoiceSTR, out chosenValue);
            
            if (chosenValue == '1')
            {
                characterChoiceSTR = "Yellow&lt;svA&gt;SEED&lt;svB&gt;";
            }
            else if (chosenValue == '2')
            {
                characterChoiceSTR = "White&lt;svA&gt;SEED&lt;svB&gt;";
            }
            else if (chosenValue == '3')
            {
                characterChoiceSTR = "Red&lt;svA&gt;SEED&lt;svB&gt;";
            }
            else
            {
                Console.WriteLine("Wrong input !\n");
                AskChar();
            }
            
            if (fileContent.IndexOf(characterChoiceSTR) == -1)
            {
                Console.WriteLine("Character not found !\n");
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

