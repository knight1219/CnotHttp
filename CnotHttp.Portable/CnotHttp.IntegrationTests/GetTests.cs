using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CnotHttp.IntegrationTests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public async Task TestGetGoodReturn()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get("https://jsonplaceholder.typicode.com/posts/1");
            Assert.IsNotNull(test);
            Assert.IsNotNull(test.ServerResponse);
        }

        [TestMethod]
        public async Task TestGetBadReturn()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get("https://jsonplaceholder.typicode.com/po/1");
            Assert.IsNotNull(test);
            Assert.IsNull(test.ServerResponse);
            Assert.IsNotNull(test.Error);
        }
    }
}
