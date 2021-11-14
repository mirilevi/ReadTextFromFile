using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.RegularExpressions;

namespace ReadTextFromFile
{
    public class Program
    {
        public static void manipulationDataFromFile(string path)
        {

            int sumLines = 0, sumWords = 0, sumUniqueWords = 0, countPopularWord = 0, currentSequenceNotK = 0, maxK = -999;
            string popularWord = "";
            //dictionary to save the number of instances of each word in the file
            IDictionary<string, int> mapWords = new Dictionary<string, int>();
            using (var mappedFile = MemoryMappedFile.CreateFromFile(path))
            {
                using (Stream mmStream = mappedFile.CreateViewStream())
                {
                    using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            sumLines++;
                            string[] lineWords = line.Split(new char[] { ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                            sumWords += lineWords.Length;
                            foreach (var word in lineWords)
                            {
                                var wordWithoutPunctuations= Regex.Replace(word, @"[^\w\s]", "").ToLower();
                                //check if this word exists in the dictionary
                                //If exists, increase the amount by 1
                                if (mapWords.ContainsKey(wordWithoutPunctuations))
                                {
                                    mapWords[wordWithoutPunctuations]++;
                                }
                                //if this word does not already exist, add a single instance
                                else
                                {
                                    mapWords.Add(wordWithoutPunctuations, 1);
                                }
                                //Checking if this word contain K
                                if (wordWithoutPunctuations.Contains('k') || wordWithoutPunctuations.Contains('K'))
                                {
                                    if (currentSequenceNotK > maxK)
                                    {
                                        maxK = currentSequenceNotK;
                                    }
                                    currentSequenceNotK = 0;
                                }
                                else
                                {
                                    currentSequenceNotK++;
                                }
                            }

                            if (currentSequenceNotK > maxK)
                            {
                                maxK = currentSequenceNotK;
                            }
                        }
                    }
                }
            }
            IDictionary<string, int> mapColors = new Dictionary<string, int>();
            List<string> colors = new List<string> {"red" ,"blue","green","yellow","purple","pink","orange",
            "brown","black","white","gray","gold","silver","navy blue","sky blue","lime green","teal","indigo","beige"};
            List<string> commonWords = new List<string> { "by", "have", "there", "like", "some", "as", "at", "or", "so", "my", "her", "his", "with", "all", "best", "a", "in", "for", "on", "were", "before", "to", "had", "and", "it", "of", "", " ", "am", "are","don't", "that", "the", "It", "was" };

            foreach (var word in mapWords.Keys)
            {
                //If the word appears once it is unique
                if (mapWords[word] == 1)
                {
                    sumUniqueWords++;
                }
                //If the number of times this word appears is greater than the number of times the previous popular word appeared and this word not common, it is popular
                if (mapWords[word] > countPopularWord && !commonWords.Contains(word))
                {
                    countPopularWord = mapWords[word];
                    popularWord = word;
                }
                //If this word is one of the colors
                if (colors.Contains(word))
                {
                    mapColors.Add(word, mapWords[word]);
                }
            }
            string allText = "";
            int maxSentence = 0, sumLengthSenteces = 0, countSentences = 0;
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new StreamReader(path))
                {
                    allText = sr.ReadToEnd();
                    //Dividing the text into an array according to sentences
                    string[] sentences = allText.Split(new char[] { '.', '?', '!' });
                    foreach (var sentence in sentences)
                    {
                        //Raising the amount of sentences
                        countSentences++;
                        //The length of this sentence-
                        int numWordsInSentence = sentence.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        sumLengthSenteces += numWordsInSentence;
                        if (numWordsInSentence > maxSentence)
                        {
                            maxSentence = numWordsInSentence;
                        }
                    }
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("the number lines in the file: {0}", sumLines);
            Console.WriteLine("the number words in the file: {0}", sumWords);
            Console.WriteLine("the number uniqueWords in the file: {0}", sumUniqueWords);
            Console.WriteLine("the very popular Word in the file: {0}", popularWord);
            Console.WriteLine("the very long sequence not contain k in the file: {0}", maxK);
            Console.WriteLine("maximum sentence length in file: {0}", maxSentence);
            Console.WriteLine("average sentence length in the file: {0}", sumLengthSenteces / countSentences);
            foreach (var color in mapColors.Keys)
            {
                Console.WriteLine("{0} appear: {1}",color,mapColors[color] );
            }
        }
        static void Main(string[] args)
        {
            manipulationDataFromFile(@"F:\dickens.txt");
            Console.ReadLine();
        }
    }
}
