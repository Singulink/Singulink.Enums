using System;
using System.Collections.Generic;
using System.Reflection;

#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace Singulink.Enums
{
    /// <summary>
    /// Represents an enumeration member.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    public readonly struct EnumMember<T>
    {
        private readonly FieldInfo? _fieldInfo;

        internal EnumMember(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        /// <summary>
        /// Gets the name of the enumeration member.
        /// </summary>
        public string Name => Field.Name;

        /// <summary>
        /// Gets the value of the enumeration member.
        /// </summary>
        public T Value => (T)Field.GetValue(null)!;

        /// <summary>
        /// Gets the field that represents the enumeration member.
        /// </summary>
        public FieldInfo Field => _fieldInfo ?? throw new InvalidOperationException("EnumMember was not properly initialized.");
    }
}