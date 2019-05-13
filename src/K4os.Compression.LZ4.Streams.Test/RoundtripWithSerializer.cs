using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace K4os.Compression.LZ4.Streams.Test
{
	public class RoundtripWithSerializer
	{
		public class TestClass
		{
			public string Test1 { get; set; }
		}

		[Fact]
		public void Issue22_DataContractSerializerCausesInvalidDataException()
		{
			var obj = new TestClass { Test1 = "1" };
			var serializer = new DataContractSerializer(typeof(TestClass));

			byte[] bytes;
			using (var ms = new MemoryStream())
			{
				using (var compressionStream = LZ4Stream.Encode(ms))
				{
					serializer.WriteObject(compressionStream, obj);
				}

				bytes = ms.ToArray();
			}

			using (var ms = new MemoryStream(bytes))
			{
				using (var decompressionStream = LZ4Stream.Decode(ms))
				{
					var o = serializer.ReadObject(decompressionStream) as TestClass;

					Assert.Equal(obj.Test1, o.Test1);
				}
			}
		}
	}
}
