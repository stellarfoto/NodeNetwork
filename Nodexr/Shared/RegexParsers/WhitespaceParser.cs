﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using Nodexr.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Nodexr.Shared.RegexParsers.ParsersShared;

namespace Nodexr.Shared.RegexParsers
{
    public static class WhitespaceParser
    {
        public static Parser<char, WhitespaceNode> ParseWhitespaceAfterEscape =>
            UpperOrLower('s')
            .Select(isUpper => CreateWhitespaceNode(invert: isUpper));

        private static WhitespaceNode CreateWhitespaceNode(bool invert)
        {
            var node = new WhitespaceNode();
            node.InputInvert.Checked = invert;
            return node;
        }
    }
}
