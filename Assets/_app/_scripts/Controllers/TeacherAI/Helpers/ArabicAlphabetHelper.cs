﻿using UnityEngine;
using System;
using System.Collections.Generic;
using ArabicSupport;

namespace EA4S
{
    public static class ArabicAlphabetHelper
    {
        static readonly List<string> LetterExceptions = new List<string>() { "0627", "062F", "0630", "0631", "0632", "0648", "0623" };

        /// <summary>
        /// Prepares the string for display (say from Arabic into TMPro Text
        /// </summary>
        /// <returns>The string for display.</returns>
        /// <param name="">.</param>
        public static string PrepareArabicStringForDisplay(string str, bool reversed = true)
        {
            if (reversed) {
                // needed to be set in a TMPro RTL text
                return GenericUtilities.ReverseText(ArabicFixer.Fix(str, true, true));
            }
            return ArabicFixer.Fix(str, true, true);
        }

        /// <summary>
        /// Return single letter string start from unicode hex code.
        /// </summary>
        /// <param name="hexCode">string Hexadecimal number</param>
        /// <returns>string char</returns>
        public static string GetLetterFromUnicode(string hexCode)
        {
            if (hexCode == "") {
                Debug.LogError("Letter requested with an empty hexacode (data is probably missing from the DataBase). Returning © for now.");
                hexCode = "00A9";
            }

            int unicode = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
            var character = (char)unicode;
            return character.ToString();
        }

        /// <summary>
        /// Get char hexa code.
        /// </summary>
        /// <param name="_char"></param>
        /// <param name="unicodePrefix"></param>
        /// <returns></returns>
        public static string GetHexUnicodeFromChar(char _char, bool unicodePrefix = false)
        {
            return string.Format("{1}{0:X4}", Convert.ToUInt16(_char), unicodePrefix ? "/U" : string.Empty);
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        /// <param name="word"></param>
        /// <param name="alphabet"></param>
        /// <param name="reverseOrder">Return in list position 0 most right letter in input string and last the most left.</param>
        /// <returns></returns>
        public static List<string> LetterDataList(string word, List<Db.LetterData> alphabet, bool reverseOrder = false)
        {
            var returnList = new List<string>();

            char[] chars = word.ToCharArray();
            if (reverseOrder) {
                Array.Reverse(chars);
            }

            for (int i = 0; i < chars.Length; i++) {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);
                Db.LetterData letterData = alphabet.Find(l => l.Isolated_Unicode == unicodeString);
                if (letterData != null)
                    returnList.Add(letterData.Id);
            }

            return returnList;
        }


        /// <summary>
        /// Return list of letter data for any letter of param word.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="alphabet"></param>
        /// <param name="reverseOrder">Return in list position 0 most right letter in input string and last the most left.</param>
        /// <returns></returns>
        public static List<LL_LetterData> LetterDataListFromWord(string word, List<LL_LetterData> alphabet, bool reverseOrder = false)
        {
            var returnList = new List<LL_LetterData>();

            char[] chars = word.ToCharArray();
            if (reverseOrder)
                Array.Reverse(chars);
            for (int i = 0; i < chars.Length; i++) {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);
                LL_LetterData letterData = alphabet.Find(l => l.Data.Isolated_Unicode == unicodeString);
                if (letterData != null)
                    returnList.Add(letterData);
            }
            return returnList;
        }

        /// <summary>
        /// Return last field.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="_vocabulary"></param>
        /// <returns></returns>
        public static string ParseWord(string word, List<LL_LetterData> _vocabulary)
        {
            string returnString = string.Empty;
            bool exceptionActive = false;
            List<LL_LetterData> letters = LetterDataListFromWord(word, _vocabulary);
            if (letters.Count == 1)
                return returnString = GetLetterFromUnicode(letters[0].Data.Isolated_Unicode);
            for (int i = 0; i < letters.Count; i++) {
                LL_LetterData let = letters[i];

                /// Exceptions
                if (exceptionActive) {
                    if (i == letters.Count - 1)
                        returnString += GetLetterFromUnicode(let.Data.Isolated_Unicode);
                    else
                        returnString += GetLetterFromUnicode(let.Data.Initial_Unicode);
                    exceptionActive = false;
                    continue;
                }
                if (LetterExceptions.Contains(let.Data.Isolated_Unicode))
                    exceptionActive = true;
                /// end Exceptions

                if (let != null) {
                    if (i == 0) {
                        returnString += GetLetterFromUnicode(let.Data.Initial_Unicode);
                        continue;
                    } else if (i == letters.Count - 1) {
                        returnString += GetLetterFromUnicode(let.Data.Final_Unicode);
                        continue;
                    } else {
                        returnString += GetLetterFromUnicode(let.Data.Medial_Unicode);
                        continue;
                    }
                } else {
                    returnString += string.Format("{0}{2}{1}", "<color=red>", "</color>", GetLetterFromUnicode(let.Data.Isolated_Unicode));
                }
            }
            return returnString;
        }
    }
}