﻿using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Models.TagModel;

namespace RealWorldApp.Commons.Intefaces
{
    public interface ITagService
    {
        Task<TagResponseModel> GetTags();
        Task<List<Tag>> CheckTags();
        Task<List<Tag>> AddTag(List<string> tags);
        Task RemoveTag(List<Tag> tags);
    }
}
