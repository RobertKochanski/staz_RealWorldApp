using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface ITagRepositorie
    {
        Task<List<Tag>> GetTags();
        Task<Tag> GetTag(string name);
        Task<Tag> AddTag(Tag tag);
    }
}
