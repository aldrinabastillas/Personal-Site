namespace WebAppPortfolio.Interfaces
{
    /// <summary>
    /// Use dependency injection to unit test Server.MapPath
    /// </summary>
    public interface IPathProvider
    {
        string MapPath(string path);
    }
}
