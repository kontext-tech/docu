using System;
using System.Collections.Generic;

namespace Kontext.Utils
{
    public class EnumHelper : IEnumHelper
    {
        public object GetEnumFromArray(Type enumType, int[] values)
        {
            int combinedValue = 0;
            foreach (var value in values)
            {
                combinedValue = value | combinedValue;
            }
            return Enum.Parse(enumType, combinedValue.ToString());
        }

        public IEnumerable<Tuple<int, string>> GetEnumList(Type enumType)
        {
            var values = Enum.GetValues(enumType);
            var list = new List<Tuple<int, string>>(values.Length);
            foreach (var value in values)
            {
                list.Add(Tuple.Create((int)value, Enum.GetName(enumType, value)));
            }
            return list;
        }

        public IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}
