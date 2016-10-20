namespace RpcLite.Registry.Merops.Contract
{
	public class ServiceResultDto
	{
		public ServiceIdentifierDto Identifier { get; set; }
		public ServiceInfoDto[] ServiceInfos { get; set; }
	}
}
