using System;
using System.Collections.Generic;
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

        [TestMethod]
        public async Task TestGetGoodObjectReturn()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get<TestClasses.Post>("https://jsonplaceholder.typicode.com/posts/1");
            Assert.IsNotNull(test);
            Assert.IsNotNull(test.ServerResponse);
            Assert.IsInstanceOfType(test.ServerResponse, typeof(TestClasses.Post));
        }

        [TestMethod]
        public async Task TestGetGoodListOfObjectsReturn()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get<List<TestClasses.Post>>("https://jsonplaceholder.typicode.com/posts");
            Assert.IsNotNull(test);
            Assert.IsNotNull(test.ServerResponse);
            Assert.IsInstanceOfType(test.ServerResponse, typeof(List<TestClasses.Post>));
        }


        [TestMethod]
        public async Task TestGetBadListOfObjectsReturn()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get<List<TestClasses.Post>>("https://jsonplaceholder.typicode.com/posts/1");
            Assert.IsNotNull(test);
            Assert.IsNull(test.ServerResponse);
            Assert.IsNotNull(test.Error);
        }

        [TestMethod]
        public async Task TestGetBadErrorMessage()
        {
            var test = await CnotHttp.Portable.CnotHttp.Get<List<TestClasses.Post>>("https://jsonplaceholder.typicode.com/posts/1");
            Assert.IsNotNull(test);
            Assert.IsNull(test.ServerResponse);
            Assert.IsNotNull(test.Error);
            Assert.AreEqual(test.Error.Message, "Unable to convert response into object");
        }

        
    }
}
