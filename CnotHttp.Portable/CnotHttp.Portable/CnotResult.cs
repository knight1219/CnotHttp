using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnotHttp.Portable
{

    public interface ICnotResult
    {

    }

    public interface ICnotResult<TData> : ICnotResult
    {
        TData ServerResponse { get; set; }

        Exception Error { get; set; }
    }



    public class CnotResult<T> : ICnotResult<T>
    {
        public T ServerResponse { get; set; }

        public Exception Error { get; set; }
    }
}
