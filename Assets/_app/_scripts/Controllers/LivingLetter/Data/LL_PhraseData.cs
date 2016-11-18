﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class LL_PhraseData : ILivingLetterData
    {

        public Db.PhraseData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Phrase; }
        }

        public string Key {
            get { return key; }
            set { key = value; }
        }

        private string key;

        public LL_PhraseData(string _keyRow, Db.PhraseData _data)
        {
            Key = _keyRow;
            Data = _data;
        }

        /// <summary>
        /// @note Not ready yet!
        /// Living Letter Phrase Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get {
                new System.Exception("TextForLivingLetter for LL_PhraseData not ready yet");
                return string.Empty;
            }
        }

        /// <summary>
        /// @note Not ready yet!
        /// Gets the drawing character for living letter.
        /// </summary>
        /// <value>
        /// The drawing character for living letter.
        /// </value>
        public string DrawingCharForLivingLetter {
            ///
            get {
                new System.Exception("DrawingCharForLivingLetter for LL_PhraseData not ready yet");
                return string.Empty;
                }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return null; }
        }

    }
}