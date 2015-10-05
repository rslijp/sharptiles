﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Templates
{
    public static class TagLibConstants
    {
        public static readonly string CLOSE_SLASH = "/";
        public static readonly string CLOSE_TAG = ">";

        public static readonly char QUOTE = '\'';
        public static readonly char DOUBLE_QUOTE = '"';

        public static readonly string FIELD_ASSIGNMENT = "=";
        public static readonly string GROUP_TAG_SEPERATOR = ":";
        public static readonly char[] LITERALS = new[] { QUOTE, DOUBLE_QUOTE };


        public static readonly string START_TAG = "<";
        public static readonly string TAG_FIELD_SEPERATOR_SPACE = " ";
        public static readonly string TAG_FIELD_SEPERATOR_TAB = "\t";
        public static readonly string TAG_FIELD_SEPERATOR_NEWLINE = "\n";
        public static readonly string TAG_FIELD_SEPERATOR_CARRIAGE_RETURN = "\r";

        public static readonly string[] SEPERATORS =
            new[] { START_TAG, CLOSE_SLASH, CLOSE_TAG, GROUP_TAG_SEPERATOR,
                TAG_FIELD_SEPERATOR_SPACE, TAG_FIELD_SEPERATOR_TAB, TAG_FIELD_SEPERATOR_NEWLINE, TAG_FIELD_SEPERATOR_CARRIAGE_RETURN,
                FIELD_ASSIGNMENT };

    }
}
