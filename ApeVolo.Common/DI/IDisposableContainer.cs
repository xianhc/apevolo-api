using System;

namespace ApeVolo.Common.DI
{
    public interface IDisposableContainer : IDisposable
    {
        void AddDisposableObj(IDisposable disposableObj);
    }
}
