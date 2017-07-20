using System;
namespace FourCC
{
	/**
	* A FourCC descriptor
	*/
	public struct FourCC : IEquatable<FourCC>
	{
		/**
		* Empty fourCC
		*/
		public static readonly FourCC Empty = new FourCC(0);
		/**
		* FourCC int value
		* @private
		*/
		private uint value;

		/**
		* Initializes a new instance of the FourCC struct.
		* @param fourCC The fourCC value as a string.
		*/
		public FourCC(string fourCC)
		{
			if (fourCC.Length != 4)
				throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid length for FourCC(\"{0}\". Must be be 4 characters long ", fourCC), "fourCC");
			this.value = ((uint)fourCC[3]) << 24 | ((uint)fourCC[2]) << 16 | ((uint)fourCC[1]) << 8 | ((uint)fourCC[0]);
		}

		/**
		* Initializes a new instance of the FourCC struct.
		* @param byte1 The first byte
		* @param byte2 The second byte
		* @param byte3 The third byte
		* @param byte4 The fourth byte
		*/
		public FourCC(char byte1, char byte2, char byte3, char byte4)
		{
			this.value = ((uint)byte4) << 24 | ((uint)byte3) << 16 | ((uint)byte2) << 8 | ((uint)byte1);
		}
		
		/**
		* Initializes a new instance of the FourCC struct.
		* @param fourCC The fourCC value as an int.
		*/
		public FourCC(uint fourCC)
		{
			this.value = fourCC;
		}
		
		/**
		* Initializes a new instance of the FourCC struct.
		* @param fourCC The fourCC value as an int.
		*/
		public FourCC(int fourCC)
		{
			this.value = unchecked((uint)fourCC);
		}
		
		/**
		* Performs an implicit conversion from FourCC to Int32
		* @param d The d
		* @return Int32 The result of the conversion
		*/
		public static implicit operator uint(FourCC d)
		{
			return d.value;
		}
		
		/**
		* Performs an implicit conversion from FourCC to Int32
		* @param d The d
		* @return Int32 The result of the conversion
		*/
		public static implicit operator int(FourCC d)
		{
			return unchecked((int)d.value);
		}
		
		/**
		* Performs an implicit conversion from Int32 to FourCC
		* @param d The d
		* @return FourCC The result of the conversion
		*/
		public static implicit operator FourCC(uint d)
		{
			return new FourCC(d);
		}

		/**
		* Performs an implicit conversion from Int32 to FourCC
		* @param d The d
		* @return FourCC The result of the conversion
		*/
		public static implicit operator FourCC(int d)
		{
			return new FourCC(d);
		}
		
		/**
		* Performs an implicit conversion from FourCC to string.
		* @param d The d
		* @return string The result of the conversion
		*/
		public static implicit operator string(FourCC d)
		{
			return d.ToString();
		}
		
		/**
		* Performs an implicit conversion from string to FourCC.
		* @param d The d
		* @return FourCC The result of the conversion
		*/
		public static implicit operator FourCC(string d)
		{
			return new FourCC(d);
		}

		/**
		* Performs a conversion from FourCC to string.
		* @return string The result of the conversion
		*/
		public override string ToString()
		{
			return string.Format("{0}", new string(new[]
			                                       {
				(char) (value & 0xFF),
				(char) ((value >> 8) & 0xFF),
				(char) ((value >> 16) & 0xFF),
				(char) ((value >> 24) & 0xFF),
			}));
		}

		/**
		* Performs a comparison to another FourCC.
		* @param other The other FourCC
		* @return bool The result of the comparison
		*/
		public bool Equals(FourCC other)
		{
			return value == other.value;
		}

		/**
		* Performs a comparison to another Object.
		* @param obj The other object
		* @return bool The result of the comparison
		*/
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is FourCC && Equals((FourCC) obj);
		}

		/**
		* Gets the hashcode of the FourCC
		* @return int The resulting hashcode;
		*/
		public override int GetHashCode()
		{
			return (int) value;
		}
		
		public static bool operator ==(FourCC left, FourCC right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(FourCC left, FourCC right)
		{
			return !left.Equals(right);
		}
	}
}