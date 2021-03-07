﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.Shared.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using static Nodexr.Shared.RegexParsers.GroupParser;

namespace Nodexr.Tests.RegexParserTests
{
    class IfElseParserTests
    {
        [TestCase(@"(1)a|b", "1", "a", "b")]
        [TestCase(@"(abc)a|b", "abc", "a", "b")]
        [TestCase(@"(abc)(a)|(b)", "abc", "(a)", "(b)")]
        public void VariousGroups_ReturnsLookaroundWithContentsAndType(string input, string expectedCondition, string expectedContents1, string expectedContents2)
        {
            var result = IfElseParser.ParseIfElse.ParseOrThrow(input);
            var lookaround = result as IfElseNode;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                //Assert.That(result, Is.TypeOf<LookaroundNode>());
                Assert.AreEqual(expectedCondition, lookaround.InputCondition.GetValue());
                Assert.AreEqual(expectedContents1, lookaround.InputThen.GetValue());
                Assert.AreEqual(expectedContents2, lookaround.InputElse.GetValue());
            });
        }
    }
}
