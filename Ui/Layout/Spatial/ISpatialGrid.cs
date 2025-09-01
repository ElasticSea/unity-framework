namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public interface ISpatialGrid : IUniformSpatialLayout
    {
        int Rows { get; set; }
        int Columns { get; set;}
    }
}