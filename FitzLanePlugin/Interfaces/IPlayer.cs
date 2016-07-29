
namespace FitzLanePlugin.Interfaces
{
    public interface IPlayer
    {
        string Name { get; }
        string Description { get; }
        string ParentId { get; set; }
        
        void Update(EasyErgsocket.Erg givenParent = null);
        EasyErgsocket.Erg GetErg();
        void Reset();
    }
}
