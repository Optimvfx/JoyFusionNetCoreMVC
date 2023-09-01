using System.Drawing;
using Common.Models;

namespace CCL.ControllersLogic.Config;

public class PostControllersLogicConfig
{
    public readonly WidthHeightRange WidthHeightImageRange;
    public readonly int PostsPerPage;

    public PostControllersLogicConfig(int postsPerPage, WidthHeightRange widthHeightImageRange)
    {
        if (postsPerPage <= 0)
            throw new AggregateException();
        
        WidthHeightImageRange = widthHeightImageRange;
        PostsPerPage = postsPerPage;
    }
}

public struct WidthHeightRange
{
    public readonly ValueRange<int> Width;
    public readonly ValueRange<int> Height;

    public WidthHeightRange(ValueRange<int> width, ValueRange<int> height)
    {
        if (width.Min <= 0 || height.Min <= 0)
            throw new ArgumentException();
        
        Width = width;
        Height = height;
    }

    public bool InRange(Image img)
    {
        return Width.InRange(img.Width) && Height.InRange(img.Height);
    }
}
