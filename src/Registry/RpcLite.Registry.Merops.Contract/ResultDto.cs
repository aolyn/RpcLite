namespace RpcLite.Registry.Merops.Contract
{
	public class ResultDto
	{
		public ServiceIdentifierDto Identifier { get; set; }
		public ServiceInfoDto[] ServiceInfos { get; set; }
	}
}
