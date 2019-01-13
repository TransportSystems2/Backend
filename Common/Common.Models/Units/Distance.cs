using System;

namespace Common.Models.Units
{
    public struct Distance : IComparable, IComparable<Distance>, IEquatable<Distance>
    {
        private static readonly double MetersPerKilometer = 1000.0;
        private static readonly double CentimetersPerMeter = 100.0;
        private static readonly double CentimetersPerInch = 2.54;
        private static readonly double InchesPerFoot = 12.0;
        private static readonly double FeetPerYard = 3.0;
        private static readonly double FeetPerMeter = CentimetersPerMeter / (CentimetersPerInch * InchesPerFoot);
        private static readonly double InchesPerMeter = CentimetersPerMeter / CentimetersPerInch;

        public Distance(double totalMeters)
        {
            TotalMeters = totalMeters;
        }

        public double TotalKilometers
        {
            get
            {
                return TotalMeters / MetersPerKilometer;
            }
        }

        public double TotalMeters { get; }

        public double TotalCentimeters
        {
            get
            {
                return TotalMeters * CentimetersPerMeter;
            }
        }

        public double TotalYards
        {
            get
            {
                return TotalMeters * FeetPerMeter / FeetPerYard;
            }
        }

        public double TotalFeet
        {
            get
            {
                return TotalMeters * FeetPerMeter;
            }
        }

        public double TotalInches
        {
            get
            {
                return TotalMeters * InchesPerMeter;
            }
        }

        public static Distance FromKilometers(double value)
        {
            return new Distance(value * MetersPerKilometer);
        }

        public static Distance FromMeters(double value)
        {
            return new Distance(value);
        }

        public static Distance FromCentimeters(double value)
        {
            return new Distance(value / CentimetersPerMeter);
        }

        public static Distance FromYards(double value)
        {
            return new Distance(value * FeetPerYard / FeetPerMeter);
        }

        public static Distance FromFeet(double value)
        {
            return new Distance(value / FeetPerMeter);
        }

        public static Distance FromInches(double value)
        {
            return new Distance(value / InchesPerMeter);
        }

        public static bool operator ==(Distance d1, Distance d2)
        {
            if (ReferenceEquals(d1, d2))
            { 
                return true;
            }

            return d1.TotalMeters == d2.TotalMeters;
        }

        // this is second one '!='
        public static bool operator !=(Distance d1, Distance d2)
        {
            return !(d1 == d2);
        }

        public static Distance operator +(Distance a, Distance b)
        {
            return new Distance(a.TotalMeters + b.TotalMeters);
        }

        public static Distance operator -(Distance a, Distance b)
        {
            return new Distance(a.TotalMeters - b.TotalMeters);
        }

        public static Distance operator -(Distance a)
        {
            return new Distance(-a.TotalMeters);
        }

        public bool Equal(Distance other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Distance))
            {
                return false;
            }

            return Equals((Distance)obj);
        }

        public bool Equals(Distance other)
        {
            return this == other;
        }

        public static bool Equals(Distance d1, Distance d2)
        {
            return d1 == d2;
        }

        public int CompareTo(Distance other)
        {
            return TotalMeters.CompareTo(other.TotalMeters);
        }

        public override int GetHashCode()
        {
            return TotalMeters.GetHashCode();
        }

        public override string ToString()
        {
            return $"{TotalMeters}";
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Distance))
            {
                throw new InvalidCastException();
            }

            return CompareTo((Distance)obj);
        }
    }
}