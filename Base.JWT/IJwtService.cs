using System;

namespace Base.JWT
{
    public interface IJwtService
    {
        string GetToken(string userName);
    }
}
