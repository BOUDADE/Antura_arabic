﻿using EA4S.UI;
using EA4S.Utilities;
using UnityEngine;

namespace EA4S.PlayerBook
{
    public class TableRow : MonoBehaviour
    {
        public TextRender TxTitle;
        public TextRender TxSubtitle;
        public TextRender TxValue;

        public void Init(string _title, string _value, string _subtitle = "")
        {
            TxTitle.setText(_title);
            TxSubtitle.setText(_subtitle);
            TxValue.setText(_value);
        }
    }
}