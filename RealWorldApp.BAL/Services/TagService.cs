using AutoMapper;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.TagModel;

namespace RealWorldApp.BAL.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepositorie _tagRepositorie;
        private readonly IMapper _mapper;

        public TagService(ITagRepositorie tagRepositorie, IMapper mapper)
        {
            _tagRepositorie = tagRepositorie;
            _mapper = mapper;
        }

        public async Task<TagResponseModel> GetTags()
        {
            var tagList = await _tagRepositorie.GetTags();
            var tags = new List<string>();

            foreach (var tag in tagList)
            {
                tags.Add(tag.Name);
            }

            var response = new TagResponseModel() { Tags = tags };

            return response;
        }

        public async Task<List<Tag>> CheckTags()
        {
            var tagList = await _tagRepositorie.GetTags();

            return tagList;
        }

        public async Task<List<Tag>> AddTag(List<string> tags)
        {
            var response = new List<Tag>();

            if (tags != null)
            {
                foreach (var item in tags)
                {
                    var tag = await _tagRepositorie.GetTag(item);

                    if (tag == null)
                    {
                        await _tagRepositorie.AddTag(new Tag() { Name = item });
                    }

                    tag = await _tagRepositorie.GetTag(item);
                    response.Add(tag);
                }
            }
            return response;
        }

        public async Task RemoveTag(List<Tag> tags)
        {
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (tag.Articles.Count == 0)
                    {
                        _tagRepositorie.RemoveTag(tag);
                    }
                }
            }
            
        }
    }
}
