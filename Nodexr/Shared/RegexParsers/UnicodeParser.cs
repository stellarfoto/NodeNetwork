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
    public static class UnicodeParser
    {
        public static Parser<char, UnicodeNode> ParseUnicode =>
            OneOf(
                HexX,
                HexU,
                UnicodeClass,
                UnicodeClassInverted);

        private static Parser<char, UnicodeNode> HexX =>
            Char('x')
                .Then(HexString(2))
                .Select(hex => CreateWithHexCode(hex));

        private static Parser<char, UnicodeNode> HexU =>
            Char('u')
                .Then(HexString(4))
                .Select(hex => CreateWithHexCode(hex));

        private static Parser<char, UnicodeNode> UnicodeClass =>
            Char('p')
            .Then(UnicodeClassContents);

        private static Parser<char, UnicodeNode> UnicodeClassInverted =>
            Char('P')
            .Then(UnicodeClassContents)
            .Select(node =>
            {
                node.InputInvert.Checked = true;
                return node;
            });

        private static Parser<char, UnicodeNode> UnicodeClassContents =>
            Token(c => c != '}')
            .AtLeastOnceString()
            .Between(Char('{'), Char('}'))
            .Select(str => CreateWithCategory(str));

        private static Parser<char, string> HexString(int length) =>
            LetterOrDigit
            .RepeatString(length);

        private static UnicodeNode CreateWithCategory(string unicodeCategory)
        {
            var node = new UnicodeNode();
            node.InputMode.Value = UnicodeNode.Modes.Category;
            node.InputCategory.Value = unicodeCategory;
            return node;
        }

        private static UnicodeNode CreateWithHexCode(string hex)
        {
            var node = new UnicodeNode();
            node.InputMode.Value = UnicodeNode.Modes.Hex;
            node.InputHexCode.Value = hex;
            return node;
        }
    }
}
