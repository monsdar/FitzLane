
namespace FitzLaneManager.Interfaces
{
    public interface IPlayer
    {
        string Name { get; }
        string Description { get; }
        
        void Update(double timePassed, EasyErgsocket.Erg givenParent = null);
        EasyErgsocket.Erg GetErg();
        void Reset();
    }
}
