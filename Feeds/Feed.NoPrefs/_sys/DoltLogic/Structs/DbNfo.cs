namespace Feed.NoPrefs._sys.DoltLogic.Structs;

// TODO public
public record DbNfo(string User, string Name, int Port)
{
	public string Key => $"{User}/{Name}";
	public string ConnectionString => $"Server=localhost;User ID=root;Database={Name}";
}