using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Util
{
    public class TagFormatter
    {
        public List<string> FormatTagsToList(string tags)
        {
            String[] strings = tags.Split(',');
            return strings.Cast<string>().ToList();
        }

        public string FormatTagsToString(IEnumerable<Tag> tags)
        {
            string result = "";
            foreach (var tag in tags)
            {
                result+=tag.TagString + ",";
            }

            if (result.Length > 1)
            {
                result.Substring(0, result.Length - 2);
            }
            return result;
        }
    }
}
