using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Base.Model;

namespace WebApi.Model
{
    public class LoginData<T> where T : UserBase
    {
        public string Token { get; set; }
        public T LoggedUser { get; set; }
    }
}
