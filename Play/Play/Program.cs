using BacktraderLib;

namespace Play;


static class Program
{
	static void Main()
	{
		var prices = Prices.Load(@"E:\cache\survivorship-free-spy\data");
	}
}
