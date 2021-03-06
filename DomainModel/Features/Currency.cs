﻿using System;

namespace DomainModel.Features
{
    public class Currency : IEquatable<Currency>
    {
        public Currency(string name)
        {
            Name = name;
        }

        public Currency(string name, string longName) : this(name)
        {
            LongName = longName;
        }

        public string Name { get; }
        public string LongName { get; }

        public bool Equals(Currency other)
        {
            return other != null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}