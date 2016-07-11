
namespace LxfHandler
{
    public static class WinError
    {
        public const int S_OK = 0x0000;
        public const int S_FALSE = 0x0001;
        public const int E_FAIL = -2147467259;
        public const int E_INVALIDARG = -2147024809;
        public const int E_OUTOFMEMORY = -2147024882;
        public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
        public const int STRSAFE_E_INSUFFICIENT_BUFFER = -2147024774;

        public const uint SEVERITY_SUCCESS = 0;
        public const uint SEVERITY_ERROR = 1;
    }
}
