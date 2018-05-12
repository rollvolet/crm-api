using System.Collections.Generic;
using System.Linq;

namespace Rollvolet.CRM.Domain.Models
{
    public class Tag
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static bool Equals(IEnumerable<Tag> tags, IEnumerable<Tag> others)
        {
            if (tags == null && others == null)
            {
                return true;
            }
            else if (tags == null || others == null)
            {
                return false; // only 1 of them is null
            }
            else if (tags.Count() != others.Count())
            {
                return false;
            }
            else
            {
                var ids = tags.Select(t => t.Id).OrderBy(t => t).ToList();
                var otherIds = tags.Select(t => t.Id).OrderBy(t => t).ToList();

                for (var i = 0; i < ids.Count; i++)
                {
                    if (ids.ElementAt(i) != otherIds.ElementAt(i))
                        return false;
                }

                return true;
            }
        }
    }
}