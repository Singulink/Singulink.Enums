using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests
{
    [TestClass]
    public class EnumParsingTests
    {
        [TestMethod]
        public new void ToString()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.AreEqual("None", parser.ToString(NormalEnum.None));
            Assert.AreEqual("B", parser.ToString(NormalEnum.B));
            Assert.AreEqual("D", parser.ToString(NormalEnum.D));
        }

        [TestMethod]
        public void UndefinedValueToString()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.AreEqual("3", parser.ToString((NormalEnum)3));
            Assert.AreEqual("12", parser.ToString((NormalEnum)12));
        }

        [TestMethod]
        public void ToCustomNameString()
        {
            var parser = new EnumParser<NormalEnum>(m => m.Field.GetCustomAttribute<DisplayAttribute>()!.GetName());

            Assert.AreEqual("None (Display)", parser.ToString(NormalEnum.None));
            Assert.AreEqual("C (Display)", parser.ToString(NormalEnum.C));
        }

        [TestMethod]
        public void ParseWhitespace()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.ThrowsException<FormatException>(() => parser.Parse(string.Empty));
            Assert.ThrowsException<FormatException>(() => parser.Parse(" "));
            Assert.ThrowsException<FormatException>(() => parser.Parse("     "));
        }

        [TestMethod]
        public void Parse()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.AreEqual(NormalEnum.A, parser.Parse("A"));
            Assert.AreEqual(NormalEnum.C, parser.Parse("C"));
        }

        [TestMethod]
        public void ParseUndefined()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.AreEqual((NormalEnum)32, parser.Parse("32"));
            Assert.AreEqual((NormalEnum)3, parser.Parse("3"));
        }

        [TestMethod]
        public void ParseMissing()
        {
            var parser = EnumParser<NormalEnum>.Default;

            Assert.ThrowsException<FormatException>(() => parser.Parse("X"));
        }

        private enum NormalEnum : byte
        {
            [Display(Name = "None (Display)")]
            None = 0,
            [Display(Name = "A (Display)")]
            A = 1,
            [Display(Name = "B (Display)")]
            B = 2,
            [Display(Name = "C (Display)")]
            C = 4,
            [Display(Name = "D (Display)")]
            D = 8,
        }
    }
}
