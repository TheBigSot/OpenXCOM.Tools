using System;
using System.IO;
using System.Collections.Generic;

namespace DSShared
{
	public class Variable
	{
		private static int count = 0;

		private string varName;
		private string varValue;
		private List<string> list;

		public Variable(string prefix, string post)
		{
			varName = "${var" + (count++) + "}";
			varValue = post;
			list = new List<string>();
			list.Add(prefix);
		}

		public string Name
		{
			get { return varName; }
		}

		public string Value
		{
			get { return varValue; }
			set { varValue = value; }
		}

		public Variable(string baseVar, string prefix, string post)
		{
			varName = "${var" + baseVar + (count++) + "}";
			varValue = post;
			list = new List<string>();
			list.Add(prefix);
		}

		public void Inc(string prefix)
		{
			list.Add(prefix);
		}

		public void Write(StreamWriter sw)
		{
			Write(sw, "");
		}

		public void Write(StreamWriter sw, string pref)
		{
			if (list.Count > 1)
			{
				sw.WriteLine(pref + varName + VarCollection.Separator + varValue);
				foreach (string pre in list)
					sw.WriteLine(pref + pre + varName);
			}
			else
				sw.WriteLine(pref + (string)list[0] + varValue);
		}
	}
}
