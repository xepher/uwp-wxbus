using System.Threading.Tasks;

namespace Framework.Tile
{
    public interface ISecondaryPinner
    {
        Task<bool> Pin(TileInfo tileInfo);

        Task<bool> Unpin(TileInfo tileInfo);

        bool IsPinned(TileInfo tiltileInfoeId);
    }
}
