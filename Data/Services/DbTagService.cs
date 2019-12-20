using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirice.Data.Models;

namespace Cirice.Data.Services
{
    public class DbTagService
    {
        //todo refactor compositiontag service and tagservice in one
        private AppDbContext _dbContext;

        public DbTagService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Tag> GetTags()
        {
            return _dbContext.Set<Tag>();
        }

        public async Task AddTagsAsync(ICollection<Tag> tags)
        {
            await _dbContext.Tags.AddRangeAsync(tags);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Tag> FindByCompositionId(long compositionId)
        {
            var tagIds = _dbContext.CompositionTags.Where(ct => ct.CompositionId == compositionId).Select(t=>t.TagId);
            var tags = _dbContext.Tags.Where(t => tagIds.Contains(t.Id));

            return tags;
        }

        public void AddTagsIfNotExist(ICollection<string> tagString)
        {
            var tagsInDb = _dbContext.Tags.Where(t => tagString.Contains(t.TagString));
            var tagsStringInDb = new List<string>();
            foreach (Tag tag in tagsInDb)
            {
                tagsStringInDb.Add(tag.TagString);
            }
            var notExistingTagsString = tagString.Except(tagsStringInDb);
            List<Tag> notExistingTags=new List<Tag>();
            foreach (string ts in notExistingTagsString)
            {
                notExistingTags.Add(new Tag()
                {
                    TagString = ts
                });
            }
            _dbContext.Tags.AddRange(notExistingTags);
            _dbContext.SaveChanges();
        }

        public IQueryable<Tag> FindAllByTagStrings(ICollection<string> tagStrings)
        {
            var iquerybleTags = _dbContext.Tags.Where(t => tagStrings.Contains(t.TagString));
            return iquerybleTags;
        }

        public void AddCompositionTags(long compositionId, IQueryable<Tag> tags)
        {
            List<CompositionTag> list = new List<CompositionTag>();
            foreach (Tag tag in tags)
            {
                list.Add(new CompositionTag()
                {
                    CompositionId = compositionId,
                    TagId = tag.Id
                });
            }
            _dbContext.CompositionTags.AddRange(list);
            _dbContext.SaveChanges();
        }

        public void UpdateCompositionTags(long compositionId, IQueryable<Tag> tags)
        {
            List<CompositionTag> tagsAfter = new List<CompositionTag>();
            foreach (Tag tag in tags)
            {
                tagsAfter.Add(new CompositionTag()
                {
                    CompositionId = compositionId,
                    TagId = tag.Id
                });
            }
            var allTagsByCompositionBefore = _dbContext.CompositionTags.Where(t => t.CompositionId == compositionId);
            var tagsDeleted = allTagsByCompositionBefore.Except(tagsAfter);
            var tagsToAdd = tagsAfter.Except(allTagsByCompositionBefore);
            _dbContext.CompositionTags.RemoveRange(tagsDeleted);
            _dbContext.CompositionTags.AddRange(tagsToAdd);
            _dbContext.SaveChanges();
        }
    }
}
