
namespace FitzLane.Interfaces
{
    public interface IPlayer
    {
        string Name { get; }
        string Description { get; }
        
        void Update(EasyErgsocket.Erg givenParent = null);
        EasyErgsocket.Erg GetErg();
        void Reset();
    }
}
