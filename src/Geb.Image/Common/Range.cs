/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
	public class Range : IComparable<Range>
	{
		public int Min { get; set; }
		public int Max { get; set; }

		public Range()
		{ }

		public Range(int min, int max)
		{
			this.Min = min;
			this.Max = max;
		}

		public int Width { get { return Max - Min + 1; } }

		public Int32 Distance
		{
			get { return Max - Min; }
		}

		#region IComparable<Range> Members

		public int CompareTo(Range other)
		{
			return this.Distance.CompareTo(other.Distance);
		}

		#endregion
	}
}
