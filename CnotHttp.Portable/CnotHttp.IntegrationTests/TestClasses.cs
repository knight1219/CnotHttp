using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnotHttp.IntegrationTests
{
    internal class TestClasses
    {

        //{
        //        "userId": 1,
        //        "id": 1,
        //        "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
        //        "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
        //}
        public class Post
        {
            public int userId { get; set; }

            public int id { get; set; }

            public string title { get; set; }

            public string body { get; set; }
        }
    }
}
