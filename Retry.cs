using System;

namespace SFDemo
{
    internal class Retry
    {
        public delegate void NoArgumentHandler();

        public static void RetryHandle(int retryTimes, TimeSpan interval, bool throwIfFail, NoArgumentHandler function)
        {
            if (function != null)
            {
                for (int i = 0; i < retryTimes; ++i)
                {
                    try
                    {
                        function();
                        break;
                    }
                    catch (Exception)
                    {
                        if (i == retryTimes - 1)
                        {
                            if (throwIfFail)
                            {
                                throw;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (interval != null)
                            {
                                System.Threading.Thread.Sleep(interval);
                            }
                        }
                    }
                }
            }
        }
    }
}
