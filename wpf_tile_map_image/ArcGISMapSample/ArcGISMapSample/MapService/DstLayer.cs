using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcGISMapSample.MapService
{
    public class DstLayer : ServiceImageTiledLayer
    {
        private List<LayerInfo> layerList = new List<LayerInfo>();

        public DstLayer()
               : base(CreateTileInfo(), new Envelope(-1 * 256 * 260 / 2, -1 * 256 * 260 / 2, 256 * 260 / 2, 256 * 260 / 2, SpatialReferences.WebMercator))
        {
        }

        private static TileInfo CreateTileInfo()
        {
            var levels = new LevelOfDetail[9];
            double resolution = 256 * 260 / 256;
            double scale = resolution * 96 * 39.37;
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new LevelOfDetail(i, resolution, scale);
                resolution /= 2;
                scale /= 2;
            }
            return new TileInfo(96, TileImageFormat.Png, levels, new MapPoint(-1 * 256 * 260 / 2, 256 * 260 / 2, SpatialReferences.WebMercator),
                 SpatialReferences.WebMercator, 256, 256);
        }

        protected override Task<Uri> GetTileUriAsync(int level, int row, int column, CancellationToken cancellationToken)
        {
            this.layerList.AddLayer(new LayerInfo(level + 1, column, row));
            // https://image03-sz.deepsight.cloud/images/cycle/411024C0320120100701/9/58/67.jpg
            string url = string.Format(@"https://image03-sz.deepsight.cloud/images/cycle/411622C1020120101003/{0}/{1}/{2}.jpg", level + 1, column, row);
            return Task.FromResult(new Uri(url, UriKind.Absolute));
        }
    }

    public class LayerInfo
    {
        public int Level { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

        public LayerInfo(int l, int c, int r)
        {
            this.Level = l;
            this.Column = c;
            this.Row = r;
        }
    }

    public static class LayerInfoExte
    {
        public static List<LayerInfo> AddLayer(this List<LayerInfo> layerList, LayerInfo cur)
        {
            if (layerList.FirstOrDefault(x => x.Level == cur.Level && x.Column == cur.Column && x.Row == cur.Row) == null)
            {
                layerList.Add(cur);
            }
            else
            {

            }

            return layerList;
        }
    }
}
