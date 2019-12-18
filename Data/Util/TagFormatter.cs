using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirice.Data.Util
{
    public class TagFormatter
    {
        public List<string> FormatTags(string tags)
        {
            String[] strings = tags.Split(',');
            return strings.Cast<string>().ToList();
        }
    }
}
