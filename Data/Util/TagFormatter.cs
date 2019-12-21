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
            List<string> result = new List<string>();
            foreach (var s in strings)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    result.Add(s);
                }
            }
            return result;
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

        public List<string> FormatTagsToListEdit(string tagsBefore, string tagsAfter)
        {
            List<string> result = new List<string>();
            if (tagsAfter.Contains(tagsBefore))
            {
                var indexTagsBefore = tagsAfter.IndexOf(tagsBefore);
                var tags = tagsAfter.Remove(indexTagsBefore, tagsBefore.Length);
                result = FormatTagsToList(tags);
            }
            return result;
        }
    }
}
