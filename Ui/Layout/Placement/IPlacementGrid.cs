namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public interface IPlacementGrid : IPlacement
    {
        int Rows { get; set; }
        int Columns { get; set;}
    }
}