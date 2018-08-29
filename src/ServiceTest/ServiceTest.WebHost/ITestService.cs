namespace ServiceTest.WebHost
{
	public interface ITestService
	{
		string GetDateTimeString();
		int AddProduct(Product product);
		Product GetProductById(int id);
	}
}