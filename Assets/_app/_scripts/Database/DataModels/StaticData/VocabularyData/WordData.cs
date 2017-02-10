﻿using System;
using EA4S.MinigamesAPI;

namespace EA4S.Database
{
    /// <summary>
    /// Data defining a Word.
    /// This is one of the fundamental dictionary (i.e. learning content) elements.
    /// <seealso cref="PhraseData"/>
    /// <seealso cref="LetterData"/>
    /// </summary>
    [Serializable]
    public class WordData : IVocabularyData, IConvertibleToLivingLetterData
    {
        public string Id;
        public bool Active;
        public WordDataKind Kind;
        public WordDataCategory Category;
        public WordDataForm Form;
        public WordDataArticle Article;
        public string LinkedWord;
        public string Arabic;
        public string Value;
        public string[] Letters;
        //public LetterSymbol[] Symbols; //TODO
        public string Drawing;
        public float Intrinsic;

        public int NumberOfLetters { get { return Letters.Length; } }

        public string GetId()
        {
            return Id;
        }

        public float GetIntrinsicDifficulty()
        {
            return Intrinsic;
        }

        public override string ToString()
        {
            return Id + ": " + Arabic;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_WordData(GetId(), this);
        }

        public bool HasDrawing()
        {
            return Drawing != "";
        }

    }

    /*[Serializable]
    public struct LetterSymbol
    {
        public string LetterId;
        public string SymbolId;
    }*/
}