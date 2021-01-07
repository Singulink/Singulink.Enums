using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Singulink.Enums.Tests
{
    [TestClass]
    public class FlagsParsingTests
    {
        [TestMethod]
        public new void ToString()
        {
            var parser = EnumParser<FlagsEnum>.Default;

            Assert.AreEqual("All", parser.ToString(FlagsEnum.All, false));
            Assert.AreEqual("A, B, C, D, All", parser.ToString(FlagsEnum.All, true));
            Assert.AreEqual("A, D", parser.ToString(FlagsEnum.A | FlagsEnum.D));
        }

        [TestMethod]
        public void UndefinedValueToString()
        {
            var parser = EnumParser<FlagsEnum>.Default;

            Assert.AreEqual("16", parser.ToString((FlagsEnum)16, false));
            Assert.AreEqual("D, 16", parser.ToString(FlagsEnum.D | (FlagsEnum)16, true));
        }

        [TestMethod]
        public void ToCustomNameString()
        {
            var parser = new EnumParser<FlagsEnum>(m => m.Field.GetCustomAttribute<DisplayAttribute>()!.GetName()!);

            Assert.AreEqual("A (Display), D (Display)", parser.ToString(FlagsEnum.A | FlagsEnum.D));
        }

        [TestMethod]
        public void Parse()
        {
            var parser = new EnumParser<FlagsEnum>(separator: " | ");

            Assert.AreEqual(FlagsEnum.All, parser.Parse("All"));
            Assert.AreEqual(FlagsEnum.All, parser.Parse("A | B | C | D | All"));
            Assert.AreEqual(FlagsEnum.A | FlagsEnum.D, parser.Parse("A|D"));
        }

        [TestMethod]
        public void ParseWhitespace()
        {
            var parser = EnumParser<FlagsEnum>.Default;

            Assert.AreEqual(FlagsEnum.None, parser.Parse(string.Empty));
            Assert.AreEqual(FlagsEnum.None, parser.Parse(" "));
            Assert.AreEqual(FlagsEnum.None, parser.Parse("    "));
        }

        [TestMethod]
        public void ParseCustomName()
        {
            var parser = new EnumParser<FlagsEnum>(m => m.Field.GetCustomAttribute<DisplayAttribute>()!.GetName()!);

            Assert.AreEqual(FlagsEnum.All, parser.Parse("All (Display)"));
            Assert.AreEqual(FlagsEnum.A | FlagsEnum.B, parser.Parse("A (Display)  , B (Display)"));
            Assert.AreEqual(FlagsEnum.A | FlagsEnum.D | (FlagsEnum)32, parser.Parse("A (Display),D (Display),  32"));
        }

        [TestMethod]
        public void InvalidSeparator()
        {
            Assert.ThrowsException<ArgumentException>(() => new EnumParser<FlagsEnum>(separator: string.Empty));
            Assert.ThrowsException<ArgumentException>(() => new EnumParser<FlagsEnum>(separator: "  "));
            Assert.ThrowsException<ArgumentException>(() => new EnumParser<FlagsEnum>(separator: "  ,"));
            Assert.ThrowsException<ArgumentException>(() => new EnumParser<FlagsEnum>(separator: ",  "));
            Assert.ThrowsException<ArgumentException>(() => new EnumParser<FlagsEnum>(separator: "   "));
        }

        [TestMethod]
        public void ParseInvalidSeparator()
        {
            var parser = EnumParser<FlagsEnum>.Default;

            Assert.ThrowsException<FormatException>(() => parser.Parse("A,,B"));
            Assert.ThrowsException<FormatException>(() => parser.Parse(" , "));
            Assert.ThrowsException<FormatException>(() => parser.Parse(","));
            Assert.ThrowsException<FormatException>(() => parser.Parse("A,"));
            Assert.ThrowsException<FormatException>(() => parser.Parse(",B"));
        }

        [Flags]
        private enum FlagsEnum : ushort
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
            [Display(Name = "All (Display)")]
            All = A | B | C | D,
        }
    }
}
