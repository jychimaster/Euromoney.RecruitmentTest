using System;
using System.Diagnostics;
using System.Resources;
using System.Collections;
using System.Globalization;


namespace ContentConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string content = Resources.Content.Material;

            try
            {
                Console.Clear();
                Console.WriteLine("\n\nSelect User Type\n---------------------");
                Console.WriteLine("<1> User");
                Console.WriteLine("<2> Administrator");
                Console.WriteLine("<3> Reader");
                Console.WriteLine("<4> Content Curator\n");
                Console.Write("=> ");
                ConsoleKeyInfo inputkey = Console.ReadKey();

                char menuUserTypeInput = Convert.ToChar(inputkey.Key);

                switch (menuUserTypeInput)
                {
                    case '1':
                        runUser(content);
                        break;
                    case '2':
                        runAdministrator();
                        break;
                    case '3':
                        runReader(content);
                        break;
                    case '4':
                        runCurator();
                        break;
                    default:
                        Console.WriteLine("\nPress ANY key to exit.");
                        Console.ReadKey();
                        break;

                }

                

            }
            catch (Exception Exception)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(Exception.Message);
                    Console.Write(Exception.Message);
                }
            }
        }



        #region Utility Functions
        static string radactWord(string word)
        {
            int bannedWordLength = word.Length;
            string firstCharBannedWord = word.Substring(0, 1);
            string lastCharBannedWord = word.Substring(bannedWordLength - 1, 1);
            string radactedword = "";

            string finalradactedword = "";

            for (int i = 0; i < bannedWordLength - 2; i++)
            {
                radactedword = radactedword + "#";
            }

            finalradactedword = firstCharBannedWord + radactedword + lastCharBannedWord;

            return finalradactedword;
        }
        #endregion


        #region Menu Functions

        static void runCurator()
        {
            bool FilterEnabled = true;

            using (var settingkeys = new ResXResourceReader(@"../../Resources/Setting.resx"))
            {
                foreach (DictionaryEntry filterkey in settingkeys)
                {
                    if (filterkey.Key.ToString() == "FilterSetting")
                    {
                        if (filterkey.Value.ToString() == "Disable")
                            FilterEnabled = false;
                        else
                            FilterEnabled = true;
                    }
                }


                Console.WriteLine("\n\nCurator Menu\n--------------------");

                if (FilterEnabled)
                    Console.WriteLine("<1> Disable Filter");
                else
                    Console.WriteLine("<1> Enable Filter");

                Console.WriteLine("<2> Show Content and Report\n\n");

                ConsoleKeyInfo inputkey = Console.ReadKey();

                char AdminInput = Convert.ToChar(inputkey.Key);

                if (AdminInput == '1')
                {                
                    //Load the settings into a dictionary
                    var resourceEntries = new Hashtable();
                    foreach (DictionaryEntry filterkey in settingkeys)
                    {
                        if (!FilterEnabled)
                            resourceEntries.Add("FilterSetting", "");
                        else
                            resourceEntries.Add("FilterSetting", "Disable");
                    }


                    //Recreate the resource file with a fresh setting from the dictionary
                    ResXResourceWriter resourceWriter = new ResXResourceWriter(@"../../Resources/Setting.resx");
                    foreach (String key in resourceEntries.Keys)
                    {
                        resourceWriter.AddResource(key, resourceEntries[key]);
                    }
                    resourceWriter.Generate();
                    resourceWriter.Close();


                    if (FilterEnabled)
                        Console.WriteLine("FILTER DISABLED");
                    else
                        Console.WriteLine("FILTER ENABLED");
                   
                    runReader(Resources.Content.Material);

                    Console.WriteLine("Press ANY key to exit.");
                    Console.ReadKey();
                 
                }
                else if (AdminInput == '2')
                {
                    runUser(Resources.Content.Material);
                }
                else
                {
                    Console.WriteLine("Press ANY key to exit.");
                    Console.ReadKey();
                }
            }
        }		

        static void runReader(string content)
        {
            using (var settingkeys = new ResXResourceReader(@"../../Resources/Setting.resx"))
            {
                bool FilterEnabled = true;

                foreach (DictionaryEntry filterkey in settingkeys)
                {
                    if (filterkey.Key.ToString() == "FilterSetting")
                    {
                        if (filterkey.Value.ToString() == "Disable")
                            FilterEnabled = false;
                        else
                            FilterEnabled = true;
                    }
                }


                if (FilterEnabled)
                {
                    IDictionaryEnumerator bannedWordsDictionary = Resources.Filter.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true).GetEnumerator();
                    while (bannedWordsDictionary.MoveNext())
                    {
                        if (content.Contains(bannedWordsDictionary.Key.ToString()))
                        {
                            content = content.Replace(bannedWordsDictionary.Key.ToString(), radactWord(bannedWordsDictionary.Key.ToString()));
                            continue;
                        }
                    }
                }

                Console.WriteLine("\n\nScanned the text:\n\n");
                Console.WriteLine(content);

                Console.WriteLine("\n\nPress ANY key to exit.");
                Console.ReadKey();
            }
        }


        static void runUser(string content)
        {
            int badWordCount = 0;

            IDictionaryEnumerator bannedWordsDictionary = Resources.Filter.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true).GetEnumerator();
            while (bannedWordsDictionary.MoveNext())
            {
                if (content.Contains(bannedWordsDictionary.Key.ToString()))
                {
                    badWordCount++;
                }
            }


            Console.WriteLine("\n\nScanned the text:\n\n");
            Console.WriteLine(content);
            Console.WriteLine("\nTotal Number of negative words: " + badWordCount);

            Console.WriteLine("\n\nPress ANY key to exit.");
            Console.ReadKey();
        }

        static void runAdministrator()
        {
            using (var bannedWords = new ResXResourceReader(@"../../Resources/Filter.resx"))
            {
                Console.WriteLine("\n\nCurrent Banned Words\n-------------------");

                foreach (DictionaryEntry bannedWord in bannedWords)
                {
                    Console.WriteLine(bannedWord.Key.ToString());
                }

                Console.WriteLine("\n\nAdmin Menu\n--------------------");
                Console.WriteLine("<1> Remove Word");
                Console.WriteLine("<2> Add Word");
                Console.WriteLine("<3> Show Content and Report\n\n");

                ConsoleKeyInfo inputkey = Console.ReadKey();

                char AdminInput = Convert.ToChar(inputkey.Key);

                if (AdminInput == '1')
                {
                    short countMatchFound = 0;

                    Console.WriteLine("\n");
                    foreach (DictionaryEntry bannedWord in bannedWords)
                    {
                        Console.WriteLine(bannedWord.Key.ToString());
                    }

                    Console.Write("\n\nChoose the word to remove: ");
                    string chosenWord = Console.ReadLine();

                    if (chosenWord != "")
                    {
                        Console.WriteLine("\nRemoving: " + chosenWord);

                        //Load the words without the removed word into a dictionary
                        var resourceEntries = new Hashtable();
                        foreach (DictionaryEntry bannedWord in bannedWords)
                        {
                            if (bannedWord.Key.ToString().ToLower() != chosenWord.ToLower())
                                resourceEntries.Add(bannedWord.Key.ToString(), bannedWord.Value.ToString());
                            else
                                countMatchFound++;
                        }

                        if (countMatchFound > 0)
                        {
                            //Recreate the resource file with a fresh list from the dictionary
                            ResXResourceWriter resourceWriter = new ResXResourceWriter(@"../../Resources/Filter.resx");
                            foreach (String key in resourceEntries.Keys)
                            {
                                resourceWriter.AddResource(key, resourceEntries[key]);
                            }
                            resourceWriter.Generate();
                            resourceWriter.Close();
                        }

                        Console.WriteLine("Press ANY key to exit.");
                        Console.ReadKey();
                    }
                    else
                    {
                        runAdministrator();
                    }
                }
                else if (AdminInput == '2')
                {
                    Console.WriteLine("\n");
                    foreach (DictionaryEntry bannedWord in bannedWords)
                    {
                        Console.WriteLine(bannedWord.Key.ToString());
                    }

                    Console.Write("\n\nEnter a new word to filter: ");
                    string chosenWord = Console.ReadLine();

                    if (chosenWord != "")
                    {

                        Console.WriteLine("\nAdding: " + chosenWord);


                        //Load the words from the filter resource file a dictionary
                        var resourceEntries = new Hashtable();
                        foreach (DictionaryEntry bannedWord in bannedWords)
                        {
                            resourceEntries.Add(bannedWord.Key.ToString(), bannedWord.Value.ToString());
                        }

                        //Add the new word into the dictionary
                        resourceEntries.Add(chosenWord, "");

                        //Recreate the resource file with a fresh list from the dictionary
                        ResXResourceWriter resourceWriter = new ResXResourceWriter(@"../../Resources/Filter.resx");
                        foreach (String key in resourceEntries.Keys)
                        {
                            resourceWriter.AddResource(key, resourceEntries[key]);
                        }
                        resourceWriter.Generate();
                        resourceWriter.Close();

                        Console.WriteLine("Press ANY key to exit.");
                        Console.ReadKey();
                    }
                    else
                    {
                        runAdministrator();
                    }

                }
                else if (AdminInput == '3')
                {
                    runUser(Resources.Content.Material);
                }
                else
                {
                    Console.WriteLine("Press ANY key to exit.");
                    Console.ReadKey();
                }
            }
        }
#endregion

    }

}
